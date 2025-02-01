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
        OptionsProvider.TemplateOption,
        OptionsProvider.TemplatesEndpoint,
        OptionsProvider.NewProjectNameOption,
        CommonOptionsProvider.VerboseOption
    ];
    
    protected override async Task Execute(InvocationContext context)
    {
        var name = context.ParseResult.GetValueForOption(OptionsProvider.TemplateOption)!;
        var endPoint = context.ParseResult.GetValueForOption(OptionsProvider.TemplatesEndpoint)!;
        var newProjectName = context.ParseResult.GetValueForOption(OptionsProvider.NewProjectNameOption)!;
        var verbose = context.ParseResult.GetValueForOption(CommonOptionsProvider.VerboseOption);

        var host = ServiceBuilder
            .CreateCommandHostBuilder<Startup>(verbose)
            .Build();

        await host.StartAsync();

        var newWorker = host.Services.GetRequiredService<NewWorker>();
        await newWorker.CreateProject(name, endPoint, newProjectName);
    }
}