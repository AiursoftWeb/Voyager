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
        await Task.CompletedTask;
    }

    public async Task ListTemplates(string endPoint)
    {
        var templates = await httpClient.Get<List<Template>>(endPoint);
        foreach (var template in templates)
        {
            logger.LogInformation("Template {shortName} from {projectOrg}/{projectName}", template.ShortName, template.ProjectOrg, template.ProjectName);
            logger.LogTrace("Git repo clone url: {gitRepoCloneUrl}", template.GitRepoCloneUrl);
        }
    }
}