# Aiursoft Voyager

<a name="voyager"></a>
[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/AiursoftWeb/voyager/blob/master/LICENSE)
[![Pipeline stat](https://badge-proxy.aiursoft.com/aiursoft/Voyager/badges/master/pipeline.svg)](https://gitlab.aiursoft.com/aiursoft/Voyager/-/pipelines)
[![Test Coverage](https://badge-proxy.aiursoft.com/aiursoft/Voyager/badges/master/coverage.svg)](https://gitlab.aiursoft.com/aiursoft/Voyager/-/pipelines)
[![S360 compliance](https://s360.aiursoft.com/badge.svg?project=aiursoft%2Fvoyager)](https://s360.aiursoft.com/SupplyChain?tab=projects&project=aiursoft%2Fvoyager&scope=baseline&ecosystem=all)
[![NuGet version](https://img.shields.io/nuget/v/Aiursoft.Voyager.svg)](https://www.nuget.org/packages/Aiursoft.Voyager/)
[![Man hours](https://manhours.aiursoft.com/r/github.com/aiursoftweb/Voyager.svg)](https://manhours.aiursoft.com/r/github.com/aiursoftweb/Voyager.html)

A cli tool helps you create new projects with a template.

## Overview

### What is Voyager?

Create a new project from a named template, or list available templates from the configured catalog.

## Installation

Install the CLI with the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0):

```bash
dotnet tool install --global Aiursoft.Voyager
```

Update an existing installation:

```bash
dotnet tool update --global Aiursoft.Voyager
```

Remove the CLI:

```bash
dotnet tool uninstall --global Aiursoft.Voyager
```

## Usage

Run the command to see available options:

```bash
voyager --help
```

After getting the binary, run it directly in the terminal.

```bash
anduin@anduin-lunar:~/Temp$ ~/.dotnet/tools/voyager new

Description:
  Create a new project based on a template.

Usage:
  voyager new [options]

Options:
  --path <path>                                               The path to the project. [default: .]
  -t, --template-short-name <template-short-name> (REQUIRED)  The short name of the template to use. Run `voyager list` to see all available templates.
  -p, --templates-endpoint <templates-endpoint>               The endpoint to fetch templates from. [default: https://github.com/AiursoftWeb/Voyager/raw/refs/heads/master/templates.json]
  -n, --name <name>                                           The name of the new project. [default: Temp]
  -v, --verbose                                               Show detailed log
  -?, -h, --help                                              Show help and usage information
```

To create a new project, run the following command:

```bash
anduin@anduin-lunar:~/Temp$ ~/.dotnet/tools/voyager new -t web-app-simple
```

To list all available templates, run the following command:

```bash
anduin@anduin-lunar:~/Temp$ ~/.dotnet/tools/voyager list
Template 'class-library' from Aiursoft/GitRunner:
  - Full name: Class Library, with unit test project

Template 'dotnet-cli-tool-simple' from Aiursoft/Httping:
  - Full name: .NET CLI global CLI application, single command.

Template 'dotnet-cli-tool-configuration' from Anduin/HappyRecorder:
  - Full name: .NET CLI global CLI application, multiple commands, with configuration system

Template 'dotnet-cli-tool-service' from Aiursoft/Static:
  - Full name: .NET CLI global CLI application, running with host service

Template 'web-app-simple' from Aiursoft/Tracer:
  - Full name: ASP.NET Core Web Application, with front-end packages and simple back-end

Template 'web-app-database-crud' from Anduin/FlyClass:
  - Full name: ASP.NET Core Web Application, with database and CRUD operations

Template 'web-app-storage' from Aiursoft/AiurDrive:
  - Full name: ASP.NET Core Web Application, with user uploading and stroage service

Template 'web-app-client-sdk' from Aiursoft/StatHub:
  - Full name: ASP.NET Core Web Application, with client side application and SDK

```

## Configuration

Configuration is supplied through command-line options. `voyager new` requires `--template-short-name` (`-t`); use `voyager list` to discover the current catalog. `--path` selects the destination (default: current directory), `--name` sets the new project name, and `--templates-endpoint` (`-p`) selects the template catalog. Use `voyager new --help` for current defaults.

The catalog and template repositories can change independently of the CLI; the list in Usage is an illustrative output snapshot. Read the current catalog rather than assuming its names or repository mappings are fixed.

## Development

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

From the repository root, restore and build:

```bash
dotnet restore Aiursoft.Voyager.sln
dotnet build Aiursoft.Voyager.sln --no-restore
```

<a name="run-locally"></a>
<a name="local-run"></a>
Run the tool from source, with the prerequisites and configuration above:

```bash
dotnet run --project src/Aiursoft.Voyager/Aiursoft.Voyager.csproj -- --help
```

<a name="run-in-microsoft-visual-studio"></a>
In Microsoft Visual Studio, open `Aiursoft.Voyager.sln`, select `Aiursoft.Voyager` as the startup project and press `F5`. Configure the arguments and environment for the task you want to run.

Run the existing tests:

```bash
dotnet test Aiursoft.Voyager.sln --no-restore
```


## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you have push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your workflow cruft out of sight.

We're also interested in your feedback on the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.

## License

Licensed under the [MIT License](https://github.com/AiursoftWeb/voyager/blob/master/LICENSE).
