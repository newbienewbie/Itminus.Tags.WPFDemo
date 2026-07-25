using Itminus.Tags;
using Itminus.Tags.R3;
using R3;
using System.Windows;
using System.Windows.Media;

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
            .ObserveOnCurrentDispatcher()
            .Subscribe(evt =>
            {
                var newvalue = evt.NewValue;
                this.txtReq.Text = newvalue?.ToString();
                this.txtReq.Foreground= newvalue is true ? Brushes.Green : Brushes.Black;
            })
            .AddTo(_disposables);

        ack.Watch()
            .ObserveOnCurrentDispatcher()
            .Subscribe(evt =>
            {
                var newvalue = evt.NewValue;
                this.txtAck.Text = newvalue?.ToString();
                this.txtAck.Foreground = newvalue is true ? Brushes.Green : Brushes.Black;
            })
            .AddTo(_disposables);
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