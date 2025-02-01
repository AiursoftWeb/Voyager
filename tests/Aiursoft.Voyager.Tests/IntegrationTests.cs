using Aiursoft.CommandFramework;
using Aiursoft.Voyager.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.Voyager.Tests;

[TestClass]
public class IntegrationTests
{
    private readonly SingleCommandApp<NewHandler> _program = new SingleCommandApp<NewHandler>()
        .WithDefaultOption(OptionsProvider.TemplateOption);

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
}