# Contributing

## Contributor License Agreement

In order to contribute, you must accept the [contributor license agreement](https://cla-assistant.io/dequelabs/axe-core-nuget), indicating that you consent to our use of your contribution. 

## Contribution Guidelines

For code contributions, we recommend reviewing the following docs from the axe-core repo:
1. [Code submission guidelines](https://github.com/dequelabs/axe-core/blob/develop/doc/code-submission-guidelines.md) 
2. [CONTRIBUTING.md](https://github.com/dequelabs/axe-core/blob/develop/CONTRIBUTING.md) 

This documentation covers a variety of important information you'll need to review before making code changes, including our commitment to code quality, preferred commit structure, and relevant pull request formatting and processes. 

### Code Quality

Although we do not have official code style guidelines, we can and will request you to make changes if we feel the changes are warranted. You can take clues from the existing code base to see what we consider to be reasonable code quality. Please be prepared to make changes that we ask of you even if you might not agree with the request(s).

Please respect the coding style of the files you are changing and adhere to that.

The files in this project are formatted by using the `dotnet format` command

### Testing

We expect all code to be 100% covered by tests. We don't have or want code coverage metrics but we will review tests and suggest changes when we think the test(s) do(es) not adequately exercise the code/code changes.

### Style

This project uses the default `dotnet format` settings for formatting C# files. PR builds include an automated formatting check.

## Getting Started

### Prerequisites

You'll need the following installed:

* [.NET SDK 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* [.NET Core 3.1 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/3.1)
* [.NET Framework 4.7.1 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net471)
* [PowerShell 7+](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell)
* [Node.js (latest LTS version)](https://nodejs.org)
* [Google Chrome (latest stable version)](https://www.google.com/chrome/downloads)
* [Mozilla Firefox (latest stable version)](https://www.mozilla.org/firefox)

### Building

* To build all projects, run `dotnet build` from the root of the repo
* To build one project, run `dotnet build` from that project's folder (eg, `packages/commons/src`)

### Testing

* To run all tests, run `dotnet test` from the root of the repo
* To run one test project, run `dotnet test` from that test project's folder (eg, `packages/commons/test`)
* To run one individual test case, run `dotnet test --filter "Name=TestCaseMethodName"`
  * `dotnet test --list-tests` will list the available test cases
