using System.CommandLine;
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

    protected override async Task Execute(ParseResult context)
    {
        var endPoint = context.GetValue(OptionsProvider.TemplatesEndpoint)!;
        var verbose = context.GetValue(CommonOptionsProvider.VerboseOption);

        var host = ServiceBuilder
            .CreateCommandHostBuilder<Startup>(verbose)
            .Build();

        var newWorker = host
            .Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<NewWorker>();
        await newWorker.ListTemplates(endPoint);
    }
}
