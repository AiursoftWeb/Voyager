using Aiursoft.GitRunner;
using Aiursoft.GitRunner.Models;
using Aiursoft.Voyager.Models;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Voyager.Services;

public class NewWorker(
    WorkspaceManager workspaceManager,
    ILogger<NewWorker> logger,
    VoyagerHttpClient httpClient)
{
    public async Task CreateProject(string path, string name, string endPoint, string newProjectName)
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
        logger.LogInformation("Creating project {newProjectName} at {path} from template {name}", newProjectName, path, name);
        
        // TODO: Add a -Force option to skip the confirmation
        // Ensure no file under the path
        if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
        {
            throw new InvalidOperationException($"The path '{path}' is not empty. Please specify an empty directory.");
        }
        
        // Create the directory if not exists
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        // Clone the repo with depth 1.
        await workspaceManager.Clone(
            path: path,
            branch: template.Branch,
            endPoint: template.GitRepoCloneUrl,
            CloneMode.Depth1);
        
        // Delete the .git folder
        var gitFolder = Path.Combine(path, ".git");
        if (Directory.Exists(gitFolder))
        {
            logger.LogTrace("Deleting .git folder at {gitFolder}", gitFolder);
            Directory.Delete(gitFolder, true);
        }
        else
        {
            logger.LogWarning("The .git folder not found at {path}", path);
        }
        
        // Get new Organization and Project name
        // If newProjectName contains ".", then the last part should be the project name. Example: Microsoft.M365.Excel, Microsoft.M365 is the organization name, Excel is the project name.
        // If newProjectName doesn't contain ".", then the whole name should be the project name and the organization name. Example, input: Temp. Then organization name and project name are both Temp.
        var parts = newProjectName.Split('.');
        var orgName = parts.Length > 1 ? string.Join('.', parts.Take(parts.Length - 1)) : newProjectName;
        var projName = parts.Last();
        logger.LogTrace("Organization name: {orgName}, Project name: {projName}", orgName, projName);
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