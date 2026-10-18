using System.Net;
using Aiursoft.CommandFramework;
using Aiursoft.CommandFramework.Models;
using Aiursoft.CSTools.Services;
using Aiursoft.CSTools.Tools;
using Aiursoft.Voyager.Handlers;

namespace Aiursoft.Voyager.Tests;

[TestClass]
public class IntegrationTests
{
    private static HttpListener? _templatesServer;
    private static Task? _templatesRequests;
    private static string _templatesEndpoint = string.Empty;

    [ClassInitialize]
    public static void StartTemplatesServer(TestContext context)
    {
        var content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "templates.json"));
        // Bind directly and retry collisions without reserving a separate socket.
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var listener = new HttpListener();
            var prefix = $"http://127.0.0.1:{Random.Shared.Next(20000, 60000)}/";
            listener.Prefixes.Add(prefix);
            try
            {
                listener.Start();
                _templatesServer = listener;
                _templatesEndpoint = prefix + "templates.json";
                break;
            }
            catch (HttpListenerException) when (attempt < 9)
            {
                listener.Close();
            }
        }

        _templatesRequests = Task.Run(async () =>
        {
            try
            {
                while (_templatesServer!.IsListening)
                {
                    var request = await _templatesServer.GetContextAsync();
                    request.Response.ContentType = "application/json";
                    request.Response.ContentLength64 = content.Length;
                    await request.Response.OutputStream.WriteAsync(content);
                    request.Response.Close();
                }
            }
            catch (Exception ex) when (!_templatesServer!.IsListening &&
                                       ex is HttpListenerException or ObjectDisposedException)
            {
                // Closing the listener ends the pending request during cleanup.
            }
        });
    }

    [ClassCleanup]
    public static async Task StopTemplatesServer()
    {
        _templatesServer?.Close();
        if (_templatesRequests != null)
        {
            await _templatesRequests;
        }
    }

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
        var result = await _program.TestRunAsync(["list", "--templates-endpoint", _templatesEndpoint], defaultOption: OptionsProvider.TemplateOption);
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
            "--templates-endpoint", _templatesEndpoint,
            "--path", tempFolder,
            "--template-short-name", projectTemplateName,
            "--name", "Contoso.WebProject",
            "-v"
        ]);

        // Assert
        if (result.ProgramReturn != 0)
        {
            Console.WriteLine(result.StdErr);
            Console.WriteLine(result.StdOut);
        }
        Assert.AreEqual(0, result.ProgramReturn);

        // Clean
        FolderDeleter.DeleteByForce(tempFolder);
    }

    [TestMethod]
    public async Task TestParentPathConflict()
    {
        // Prepare
        // The 'class-library' template uses 'Aiursoft' as ProjectOrg.
        // We create a path that contains 'Aiursoft' in the parent directory.
        var tempRoot = Path.Combine(Path.GetTempPath(), $"Aiursoft-Conflict-Test-{Guid.NewGuid()}");
        var projectPath = Path.Combine(tempRoot, "MyProject");
        Directory.CreateDirectory(projectPath);

        // Run
        var result = await _program.TestRunAsync([
            "new",
            "--templates-endpoint", _templatesEndpoint,
            "--path", projectPath,
            "--template-short-name", "class-library",
            "--name", "Contoso.WebProject",
            "-v"
        ]);

        // Assert
        if (result.ProgramReturn != 0)
        {
            Console.WriteLine(result.StdErr);
            Console.WriteLine(result.StdOut);
        }
        Assert.AreEqual(0, result.ProgramReturn);

        // Clean
        FolderDeleter.DeleteByForce(tempRoot);
    }

    [TestMethod]
    [DataRow("class-library")]
    [DataRow("dotnet-cli-tool-simple")]
    [DataRow("dotnet-cli-tool-configuration")]
    [DataRow("dotnet-cli-tool-service")]
    [DataRow("web-app-simple")]
    [DataRow("web-app-all-in-one")]
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
            "--templates-endpoint", _templatesEndpoint,
            "--path", tempFolder,
            "--template-short-name", projectTemplateName,
            "--name", "Contoso.WebProject",
            "-v"
        ]);

        // Assert
        if (result.ProgramReturn != 0)
        {
            Console.WriteLine(result.StdErr);
            Console.WriteLine(result.StdOut);
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
