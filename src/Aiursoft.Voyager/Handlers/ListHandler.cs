using System.CommandLine;
using System.CommandLine.Invocation;
using Aiursoft.CommandFramework.Framework;
using Aiursoft.CommandFramework.Models;
using Aiursoft.CommandFramework.Services;
using Aiursoft.Voyager.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Voyager.Handlers;

public class ListHandler : ExecutableCommandHandlerBuilder
{
    protected override string Name => "list";

    protected override string Description => "List all available templates.";

    protected override Option[] GetCommandOptions() =>
    [
        OptionsProvider.TemplatesEndpoint,
        CommonOptionsProvider.VerboseOption
    ];
    
    protected override async Task Execute(InvocationContext context)
    {
        var endPoint = context.ParseResult.GetValueForOption(OptionsProvider.TemplatesEndpoint)!;
        var verbose = context.ParseResult.GetValueForOption(CommonOptionsProvider.VerboseOption);

        var host = ServiceBuilder
            .CreateCommandHostBuilder<Startup>(verbose)
            .Build();

        await host.StartAsync();

        var newWorker = host.Services.GetRequiredService<NewWorker>();
        await newWorker.ListTemplates(endPoint);
    }
}