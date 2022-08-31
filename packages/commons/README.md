# Deque.AxeCore.Commons

Provides a .NET wrapper around [axe-core](https://github.com/dequelabs/axe-core), including a bundled copy of `axe-core` and .NET typings for its options and results.

## Getting Started

*Note: this package is still under development; these instructions won't work until we perform an initial NuGet release*

Install a [.NET SDK](https://dotnet.microsoft.com/download) if you haven't already.

Install `Deque.AxeCore.Commons` and its dependencies:

```console
dotnet add package Deque.AxeCore.Commons
```

## Usage

This package exists primarily to help .NET tool developers integrate `axe-core` with different tools and test frameworks. If you are just trying to write test cases using `axe-core`, you probably want to use one of these instead:

* [Deque.AxeCore.Playwright](../playwright/README.md) in combination with [Playwright for .NET](https://playwright.dev/dotnet/)
* [Deque.AxeCore.Selenium](../selenium/README.md) in combination with [Selenium](https://www.selenium.dev/)'s [C# Selenium.WebDriver package](https://www.nuget.org/packages/Selenium.WebDriver)

## Axe script providers

The `IAxeScriptProvider` interface is suitable for use as an option in an API for running an `axe-core` scan of a page. It specifies a single method, `GetScript()`, which returns a string containing JavaScript code suitable for injecting into a running page.

This library provides two `IAxeScriptProvider` implementations:

### 1. `BundledAxeScriptProvider`

`BundledAxeScriptProvider` provides a bundled copy of [`axe-core`](https://github.com/dequelabs/axe-core) which is included with the library. The included version will match the major and minor version of the library, but may not match the patch version.

This script provider is suitable as a default option for users that don't wish to explicitly override the version of `axe-core` to use.
```csharp
IAxeScriptProvider axeScriptProvider = new BundledAxeScriptProvider();
axeScriptProvider.GetScript(); // a string containing the contents of the bundled copy of axe.min.js
```

### 2. `FileAxeScriptProvider`

`FileAxeScriptProvider` is an alternate option for users that wish to provide their own separate implementation of `axe-core`, usually for the purposes of pinning to a specific `axe-core` version independently of this library's version.

```csharp
new FileAxeScriptProvider("./path/to/axe.min.js");
axeScriptProvider.GetScript(); // synchronously reads the contents of file ./path/to/axe.min.js
```


## License

This package, including its embedded copy of [axe-core][axe-core], is distributed under the terms of the [Mozilla Public License, version 2.0](../../LICENSE-Deque.AxeCore.Commons.txt).

## Acknowledgements

This package builds on past work from the [SeleniumAxeDotnet](https://github.com/TroyWalshProf/SeleniumAxeDotnet) and [PlaywrightAxeDotnet](https://github.com/IsaacWalker/PlaywrightAxeDotnet) projects (see [NOTICE.txt](../../NOTICE.txt)). We thank all of those projects' contributors for their work.