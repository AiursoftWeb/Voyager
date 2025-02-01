using Microsoft.Extensions.Logging;

namespace Aiursoft.Voyager.Services;

public class NewWorker(ILogger<NewWorker> logger)
{
    public async Task CreateProject(string name, string endPoint, string newProjectName)
    {
        logger.LogTrace("Creating project {name} from {endPoint} to {newProjectName}", name, endPoint, newProjectName);
    }

    public async Task ListTemplates(string endPoint)
    {
        await Task.CompletedTask;
    }
}