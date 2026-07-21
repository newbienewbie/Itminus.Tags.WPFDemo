using Itminus.Tags;
using Itminus.Tags.McpServer;
using Itminus.Tags.Rx;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;

namespace WPFDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IDisposable
{
    private CompositeDisposable _disposables;
    private ITagsProjectCtrl? _ctrl;

    public MainWindow()
    {
        InitializeComponent();
        this._disposables = new CompositeDisposable();

        var app = App.Current as App ?? throw new InvalidCastException("App.Current is not of type App");
        this._ctrl = app.Ctrl;
        var tags= app.Ctrl!.Project!.Tags;
        SubscribeTags(tags);
    }

    private void SubscribeTags(ITagGrp tags)
    {
        var req = tags.SelectTag("IoBox/通用状态/PLC/心跳请求");
        var ack = tags.SelectTag("IoBox/通用状态/MST/心跳响应");


        req.Watch()
            .ObserveOn(DispatcherScheduler.Current)
            .Subscribe(evt =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.txtReq.Text = evt.EventArgs.NewValue?.ToString();
                });
            })
            .DisposeWith(_disposables);

        ack.Watch()
            .ObserveOn(DispatcherScheduler.Current)
            .Subscribe(evt =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.txtAck.Text = evt.EventArgs.NewValue?.ToString();
                });
            })
            .DisposeWith(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    private async void btnToggle_Click(object sender, RoutedEventArgs e)
    {
        var proj = this._ctrl?.Project;
        if(proj is null)
        {
            return;
        }
        var enqueue = proj.WriteIntent(
            entry:"IoBox", 
            intent: (grp, ct) => {
                var tag = proj.Tags.SelectTag("IoBox/通用状态/PLC/心跳请求");
                var old = tag.GetTagValue<bool>();
                tag.Value = !old;
                return ValueTask.CompletedTask;
            }, 
            out var task
        );
        await task;
    }
}