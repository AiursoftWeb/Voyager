namespace Aiursoft.Voyager.Models;

public class ProjectTemplate
{
    public required string ShortName { get; init; }
    public required string FullName { get; init; }
    public required string GitRepoCloneUrl { get; init; }
    public required string Branch { get; init; }
    public required string ProjectOrg { get; init; }
    public required string ProjectName { get; init; }
}

public class Rule
{
    public required string Extension { get; init; }
    public required string DontReplaceWhenLineContains { get; init; }
}

public class Template
{
    public required IReadOnlyCollection<ProjectTemplate> Projects { get; init; } = new List<ProjectTemplate>();
    public required IReadOnlyCollection<Rule> Rules { get; init; } = new List<Rule>();
}