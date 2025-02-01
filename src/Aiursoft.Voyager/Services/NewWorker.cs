using Aiursoft.Voyager.Models;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Voyager.Services;

public class NewWorker(
    ILogger<NewWorker> logger,
    VoyagerHttpClient httpClient)
{
    public async Task CreateProject(string name, string endPoint, string newProjectName)
    {
        logger.LogTrace("Creating project {name} from {endPoint} to {newProjectName}", name, endPoint, newProjectName);
        logger.LogTrace("Listing templates from {endPoint}", endPoint);
        await Task.CompletedTask;
    }

    public async Task ListTemplates(string endPoint)
    {
        logger.LogTrace("Listing templates from {endPoint}", endPoint);
        var templates = await httpClient.Get<List<Template>>(endPoint);
        foreach (var template in templates)
        {
            Console.WriteLine($"Template '{template.ShortName}' from {template.ProjectOrg}/{template.ProjectName}:");
            Console.WriteLine($"  - Full name: {template.FullName}");
            logger.LogTrace("Git repo clone url: {gitRepoCloneUrl}", template.GitRepoCloneUrl);
        }
    }
}