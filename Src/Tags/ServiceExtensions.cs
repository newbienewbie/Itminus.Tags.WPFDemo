using Itminus.Tags;
using Itminus.Tags.S7;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo.Tags;

internal static class ServiceExtensions
{
    public static IServiceCollection AddWpfDemoTags(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddTagsProjectServices(builder =>
        {
            builder.AddS7Support();
        });
        services.AddSingleton<TagsProjectCtrl>();
        return services;
    }


    public static ITagsProject MakeProject(this IServiceProvider sp, string? dir = null)
    {
        var factory = sp.GetRequiredService<ITagsProjectFactory>();

        if (string.IsNullOrEmpty(dir))
        {
            var loc = Assembly.GetExecutingAssembly().Location;
            dir = Path.GetDirectoryName(loc);
        }
        if (string.IsNullOrEmpty(dir))
        {
            dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        var proj = factory.Create(dir!, root: null);
        return proj;
    }
}
