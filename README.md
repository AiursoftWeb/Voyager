# Voyager

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://gitlab.aiursoft.cn/aiursoft/Voyager/-/blob/master/LICENSE)
[![Pipeline stat](https://gitlab.aiursoft.cn/aiursoft/Voyager/badges/master/pipeline.svg)](https://gitlab.aiursoft.cn/aiursoft/Voyager/-/pipelines)
[![Test Coverage](https://gitlab.aiursoft.cn/aiursoft/Voyager/badges/master/coverage.svg)](https://gitlab.aiursoft.cn/aiursoft/Voyager/-/pipelines)
[![NuGet version](https://img.shields.io/nuget/v/Aiursoft.Voyager.svg)](https://www.nuget.org/packages/Aiursoft.Voyager/)
[![ManHours](https://manhours.aiursoft.cn/r/gitlab.aiursoft.cn/aiursoft/Voyager.svg)](https://gitlab.aiursoft.cn/aiursoft/Voyager/-/commits/master?ref_type=heads)

A cli tool helps you create new projects with a template.

## Install

Requirements:

1. [.NET 9 SDK](http://dot.net/)

Run the following command to install this tool:

```bash
dotnet tool install --global Aiursoft.Voyager
```

## Usage

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
  -p, --templates-endpoint <templates-endpoint>               The endpoint to fetch templates from. [default: https://gitlab.aiursoft.cn/aiursoft/voyager/-/raw/master/templates.json]
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

## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you with push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your workflow cruft out of sight.

We're also interested in your feedback on the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.
