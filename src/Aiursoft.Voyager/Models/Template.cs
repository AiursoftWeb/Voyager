namespace Aiursoft.Voyager.Models;

public class Template
{
    public required string ShortName { get; init; }
    public required string FullName { get; init; }
    public required string GitRepoCloneUrl { get; init; }
    public required string Branch { get; init; }
    public required string ProjectOrg { get; init; }
    public required string ProjectName { get; init; }
}