using Aiursoft.Canon;
using Aiursoft.CommandFramework.Abstracts;
using Aiursoft.Voyager.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Voyager;

public class Startup : IStartUp
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTaskCanon();
        services.AddHttpClient();
        services.AddScoped<VoyagerHttpClient>();
        services.AddScoped<NewWorker>();
    }
}