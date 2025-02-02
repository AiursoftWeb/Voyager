using Aiursoft.CommandFramework;
using Aiursoft.CommandFramework.Models;
using Aiursoft.CSTools.Services;
using Aiursoft.CSTools.Tools;
using Aiursoft.Voyager.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.Voyager.Tests;

[TestClass]
public class IntegrationTests
{
    private readonly CommandApp _program = new NestedCommandApp()
        .WithFeature(new NewHandler())
        .WithFeature(new ListHandler())
        .WithGlobalOptions(CommonOptionsProvider.VerboseOption)
        .WithGlobalOptions(OptionsProvider.TemplatesEndpoint);

    [TestMethod]
    public async Task InvokeHelp()
    {
        var result = await _program.TestRunAsync(["--help"], defaultOption: OptionsProvider.TemplateOption);
        Assert.AreEqual(0, result.ProgramReturn);
    }

    [TestMethod]
    public async Task InvokeVersion()
    {
        var result = await _program.TestRunAsync(["--version"], defaultOption: OptionsProvider.TemplateOption);
        Assert.AreEqual(0, result.ProgramReturn);
    }

    [TestMethod]
    public async Task InvokeUnknown()
    {
        var result = await _program.TestRunAsync(["--wtf"], defaultOption: OptionsProvider.TemplateOption);
        Assert.AreEqual(1, result.ProgramReturn);
    }

    [TestMethod]
    public async Task InvokeWithoutArg()
    {
        var result = await _program.TestRunAsync([], defaultOption: OptionsProvider.TemplateOption);
        Assert.AreEqual(1, result.ProgramReturn);
    }
    
    [TestMethod]
    public async Task InvokeTemplatesList()
    {
        var result = await _program.TestRunAsync(["list"], defaultOption: OptionsProvider.TemplateOption);
        Assert.AreEqual(0, result.ProgramReturn);
    }

    [TestMethod]
    [DataRow("class-library")]
    [DataRow("web-app-database-crud")]
    public async Task InvokeProjectCreation(string projectTemplateName)
    {
        // Prepare
        var tempFolder = Path.Combine(Path.GetTempPath(), $"Parser-UT-{Guid.NewGuid()}");
        if (!Directory.Exists(tempFolder))
        {
            Directory.CreateDirectory(tempFolder);
        }
        // Run
        var result = await _program.TestRunAsync([
            "new",
            "--path", tempFolder,
            "--template-short-name", projectTemplateName,
            "--name", "Contoso.WebProject",
            "-v"
        ]);
        
        // Assert
        if (result.ProgramReturn != 0)
        {
            Console.WriteLine(result.Error);
            Console.WriteLine(result.Output);
        }
        Assert.AreEqual(0, result.ProgramReturn);
        
        // Clean
        FolderDeleter.DeleteByForce(tempFolder);
    }
    
    [TestMethod]
    [DataRow("class-library")]
    [DataRow("dotnet-cli-tool-simple")]
    [DataRow("dotnet-cli-tool-configuration")]
    [DataRow("dotnet-cli-tool-service")]
    [DataRow("web-app-simple")]
    [DataRow("web-app-database-crud")]
    [DataRow("web-app-storage")]
    [DataRow("web-app-client-sdk")]
    [Ignore] // Ignore this because on the CI, the dotnet build command will fail.
    public async Task InvokeProjectCreationAndBuild(string projectTemplateName)
    {
        // Prepare
        var tempFolder = Path.Combine(Path.GetTempPath(), $"Parser-UT-{Guid.NewGuid()}");
        if (!Directory.Exists(tempFolder))
        {
            Directory.CreateDirectory(tempFolder);
        }
        // Run
        var result = await _program.TestRunAsync([
            "new",
            "--path", tempFolder,
            "--template-short-name", projectTemplateName,
            "--name", "Contoso.WebProject",
            "-v"
        ]);
        
        // Assert
        if (result.ProgramReturn != 0)
        {
            Console.WriteLine(result.Error);
            Console.WriteLine(result.Output);
        }
        Assert.AreEqual(0, result.ProgramReturn);
        
        // Build
        var commandRunner = new CommandService();
        var buildResult = await commandRunner.RunCommandAsync(
            bin: "dotnet",
            arg: "build",
            path: tempFolder,
            timeout: TimeSpan.FromMinutes(5));
        if (buildResult.code != 0)
        {
            Console.WriteLine(buildResult.error);
            Console.WriteLine(buildResult.output);
        }
        Assert.AreEqual(0, buildResult.code);
        
        // Clean
        FolderDeleter.DeleteByForce(tempFolder);
    }
}