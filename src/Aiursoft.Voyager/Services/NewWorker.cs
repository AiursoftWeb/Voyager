using Aiursoft.Voyager.Models;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Voyager.Services;

public class NewWorker(
    ILogger<NewWorker> logger,
    VoyagerHttpClient httpClient)
{
    public async Task CreateProject(string name, string endPoint, string newProjectName)
    {
        logger.LogTrace("Listing templates from {endPoint}", endPoint);
        var templates = await httpClient.Get<List<Template>>(endPoint);
        var template = templates.FirstOrDefault(t => 
            string.Equals(t.ShortName, name, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(t.FullName, name, StringComparison.OrdinalIgnoreCase));
        if (template is null)
        {
            throw new InvalidOperationException($"Template '{name}' not found. Please run 'voyager list' to see all available templates.");
        }
        logger.LogInformation("Creating project {newProjectName} from template {name}", newProjectName, name);
    }

    public async Task ListTemplates(string endPoint)
    {
        logger.LogTrace("Listing templates from {endPoint}", endPoint);
        var templates = await httpClient.Get<List<Template>>(endPoint);
        foreach (var template in templates)
        {
            Console.WriteLine($"Template '{template.ShortName}' from {template.ProjectOrg}/{template.ProjectName}:");
            Console.WriteLine($"  - Full name: {template.FullName}\n");
            logger.LogTrace("Git repo clone url: {gitRepoCloneUrl}", template.GitRepoCloneUrl);
        }
    }
}