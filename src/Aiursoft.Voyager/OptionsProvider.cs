using System.CommandLine;

namespace Aiursoft.Voyager;

public class OptionsProvider
{
    public static readonly Option<string> PathOption = new(
        name: "--path")
    {
        DefaultValueFactory = _ => ".",
        Description = "The path to the project.",
        Required = false,
    };

    public static readonly Option<string> TemplateOption = new(
        name: "--template-short-name",
        aliases: ["-t"])
    {
        Description = "The short name of the template to use. Run `voyager list` to see all available templates.",
        Required = true,
    };

    public static readonly Option<string> TemplatesEndpoint = new(
        name: "--templates-endpoint",
        aliases: ["-p"])
    {
        DefaultValueFactory = _ => "https://gitlab.aiursoft.com/aiursoft/voyager/-/raw/master/templates.json",
        Description = "The endpoint to fetch templates from.",
        Required = false,
    };

    public static readonly Option<string> NewProjectNameOption = new(
        name: "--name",
        aliases: ["-n"])
    {
        DefaultValueFactory = _ =>
        {
            var name = new DirectoryInfo(Directory.GetCurrentDirectory()).Name;
            Console.WriteLine($"Didn't pass the argument '-n', using the current directory name '{name}' as the project name.");
            return name;
        },
        Description = "The name of the new project.",
        Required = false,
    };
}
