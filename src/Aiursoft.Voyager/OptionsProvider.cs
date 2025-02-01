using System.CommandLine;

namespace Aiursoft.Voyager;

public class OptionsProvider
{
    public static readonly Option<string> PathOption = new(
        name: "--path",
        getDefaultValue: () => ".",
        description: "The path to the project.")
    {
        IsRequired = false,
    };

    public static readonly Option<string> TemplateOption = new(
        aliases: ["--template-short-name", "-t"],
        description: "The short name of the template to use. Run `voyager list` to see all available templates.")
    {
        IsRequired = true,
    };
    
    public static readonly Option<string> TemplatesEndpoint = new(
        aliases: ["--templates-endpoint", "-p"],
        getDefaultValue: () => "https://gitlab.aiursoft.cn/aiursoft/voyager/-/raw/master/templates.json",
        description: "The endpoint to fetch templates from.")
    {
        IsRequired = false,
    };
    
    public static readonly Option<string> NewProjectNameOption = new(
        aliases: ["--name", "-n"],
        getDefaultValue: () => new DirectoryInfo(Directory.GetCurrentDirectory()).Name,
        description: "The name of the new project.")
    {
        IsRequired = false,
    };
}