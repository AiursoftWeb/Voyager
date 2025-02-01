using System.Text.RegularExpressions;
using Aiursoft.CSTools.Tools;
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
        var templates = await httpClient.Get<Template>(endPoint);
        var template = templates.Projects.FirstOrDefault(t =>
            string.Equals(t.ShortName, name, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(t.FullName, name, StringComparison.OrdinalIgnoreCase));
        if (template is null)
        {
            throw new InvalidOperationException(
                $"Template '{name}' not found. Please run 'voyager list' to see all available templates.");
        }

        logger.LogInformation("Creating project {newProjectName} at {path} from template {name}", newProjectName, path,
            name);

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
        var parts = newProjectName.Split('.');
        var orgName = parts.Length > 1 ? string.Join('.', parts.Take(parts.Length - 1)) : newProjectName;
        var projName = parts.Last();

        // Ensure the two names only contains alphanumeric characters, numbers, underscores, dashes, and dots.
        if (!IsValidOrgOrProjName(orgName) || !IsValidOrgOrProjName(projName))
        {
            throw new InvalidOperationException(
                "The organization name and project name can only contain alphanumeric characters, numbers, underscores, dashes, and dots.");
        }

        logger.LogTrace("Organization name: {orgName}, Project name: {projName}", orgName, projName);

        // Replace the template.ProjectOrg to orgName and template.ProjectName to projName
        await ReplaceEveryString(path, template.ProjectOrg, orgName, templates.Rules);
        await ReplaceEveryString(path, template.ProjectName, projName, templates.Rules);
    }

    public async Task ListTemplates(string endPoint)
    {
        logger.LogTrace("Listing templates from {endPoint}", endPoint);
        var templates = await httpClient.Get<Template>(endPoint);
        foreach (var template in templates.Projects)
        {
            Console.WriteLine($"Template '{template.ShortName}' from {template.ProjectOrg}/{template.ProjectName}:");
            Console.WriteLine($"  - Full name: {template.FullName}\n");
            logger.LogTrace("Git repo clone url: {gitRepoCloneUrl}", template.GitRepoCloneUrl);
        }
    }

    private async Task ReplaceEveryString(string path, string source, string target, IReadOnlyCollection<Rule> rules)
    {
        await ReplaceInAllFiles(path, source, target, rules);
        await ReplaceFolderNames(path, source, target);
        await ReplaceAllFileNames(path, source, target);
    }

    private async Task ReplaceInAllFiles(string path, string source, string target, IReadOnlyCollection<Rule> rules)
    {
        var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            logger.LogTrace("Processing file {file} for replacement from {source} to {target}.", file, source, target);
            var fileExtension = Path.GetExtension(file);
            var applicableRules = rules
                .Where(rule => string.Equals(rule.Extension, fileExtension, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (applicableRules.Count != 0)
            {
                var lines = await File.ReadAllLinesAsync(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var skipReplacement = applicableRules.Any(rule => lines[i].Contains(rule.DontReplaceWhenLineContains));
                    if (!skipReplacement)
                    {
                        lines[i] = lines[i].ReplaceWithUpperLowerRespect(source, target);
                    }
                }
                await File.WriteAllLinesAsync(file, lines);
            }
            else
            {
                // 如果文件扩展名未命中任何规则，则整体替换
                var content = await File.ReadAllTextAsync(file);
                content = content.ReplaceWithUpperLowerRespect(source, target);
                await File.WriteAllTextAsync(file, content);
            }
        }
    }


    private Task ReplaceFolderNames(string root, string source, string target)
    {
        // Rename all subdirectories under root.
        RenameSubdirectoriesRecursively(root, source, target);
        return Task.CompletedTask;
    }

    private void RenameSubdirectoriesRecursively(string currentDir, string source, string target)
    {
        // Get a snapshot of immediate child directories.
        var subDirs = Directory.GetDirectories(currentDir).ToList();
        foreach (var subDir in subDirs)
        {
            // Recurse first so that deeper directories are renamed before their parents.
            RenameSubdirectoriesRecursively(subDir, source, target);

            // Rename this directory if its name contains the source substring.
            var dirName = Path.GetFileName(subDir);
            if (dirName.Contains(source))
            {
                var newDirName = dirName.Replace(source, target);
                var parentDir = Path.GetDirectoryName(subDir);
                if (string.IsNullOrEmpty(parentDir))
                {
                    continue; // Should not happen unless we're at the root.
                }
                var newDirPath = Path.Combine(parentDir, newDirName);
                if (!Directory.Exists(newDirPath))
                {
                    logger.LogTrace("Renaming folder from {OldDir} to {NewDir}", subDir, newDirPath);
                    Directory.Move(subDir, newDirPath);
                }
                else
                {
                    logger.LogWarning("Target directory {NewDir} already exists. Skipping renaming of {OldDir}", newDirPath, subDir);
                }
            }
        }
    }

    private Task ReplaceAllFileNames(string path, string source, string target)
    {
        var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var newFile = file.Replace(source, target);
            logger.LogTrace("Renaming file {file} to {newFile}", file, newFile);
            File.Move(file, newFile);
        }

        return Task.CompletedTask;
    }

    private bool IsValidOrgOrProjName(string name)
    {
        return Regex.IsMatch(name, @"^[a-zA-Z0-9_.-]+$");
    }
}
