using System.CommandLine;
using System.CommandLine.Invocation;
using Aiursoft.CommandFramework.Framework;
using Aiursoft.CommandFramework.Models;
using Aiursoft.CommandFramework.Services;
using Aiursoft.Voyager.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Voyager.Handlers;


public class NewHandler : ExecutableCommandHandlerBuilder
{
    protected override string Name => "new";

    protected override string Description => "Create a new project based on a template.";

    protected override Option[] GetCommandOptions() =>
    [
        OptionsProvider.PathOption,
        OptionsProvider.TemplateOption,
        OptionsProvider.TemplatesEndpoint,
        OptionsProvider.NewProjectNameOption,
        CommonOptionsProvider.VerboseOption
    ];
    
    protected override async Task Execute(ParseResult context)
    {
        var path = context.GetValue(OptionsProvider.PathOption)!;
        var name = context.GetValue(OptionsProvider.TemplateOption)!;
        var endPoint = context.GetValue(OptionsProvider.TemplatesEndpoint)!;
        var newProjectName = context.GetValue(OptionsProvider.NewProjectNameOption)!;
        var verbose = context.GetValue(CommonOptionsProvider.VerboseOption);
        
        var contentRoot = Path.GetFullPath(path);
        var host = ServiceBuilder
            .CreateCommandHostBuilder<Startup>(verbose)
            .Build();

        var newWorker = host
            .Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<NewWorker>();
        await newWorker.CreateProject(contentRoot, name, endPoint, newProjectName);
    }
}