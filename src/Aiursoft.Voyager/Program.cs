using Aiursoft.CommandFramework;
using Aiursoft.CommandFramework.Models;
using Aiursoft.Voyager;
using Aiursoft.Voyager.Handlers;

return await new NestedCommandApp()
    .WithFeature(new NewHandler())
    .WithFeature(new ListHandler())
    .WithGlobalOptions(CommonOptionsProvider.VerboseOption)
    .WithGlobalOptions(OptionsProvider.TemplatesEndpoint)
    .RunAsync(args);

