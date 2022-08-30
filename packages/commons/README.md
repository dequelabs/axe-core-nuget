# Deque.AxeCore.Commons

Provides a .NET wrapper around [axe-core](https://github.com/dequelabs/axe-core), including a bundled copy of `axe-core` and .NET typings for its options and results.

## Getting Started

_Note: this package is still under development; these instructions won't work until we perform an initial NuGet release_

Install a [.NET SDK](https://dotnet.microsoft.com/download) if you haven't already.

Install `Deque.AxeCore.Commons` and its dependencies:

```console
dotnet add package Deque.AxeCore.Commons
```

## Usage

This package exists primarily to help .NET tool developers integrate `axe-core` with different tools and test frameworks. If you are just trying to write test cases using `axe-core`, you probably want to use one of these instead:

- [Deque.AxeCore.Playwright](../playwright/README.md) in combination with [Playwright for .NET](https://playwright.dev/dotnet/)
- [Deque.AxeCore.Selenium](../selenium/README.md) in combination with [Selenium](https://www.selenium.dev/)'s [C# Selenium.WebDriver package](https://www.nuget.org/packages/Selenium.WebDriver)

### `AxeResult`

`AxeResult` represents the [axe-core Results Object](https://www.deque.com/axe/core-documentation/api-documentation/#results-object).

### `AxeRunContext`

`AxeRunContext` represents the [axe-core Context Parameter](https://www.deque.com/axe/core-documentation/api-documentation/#context-parameter).

### `AxeRunOptions`

`AxeRunOptions` represents the [axe-core Options Parameter](https://www.deque.com/axe/core-documentation/api-documentation/#options-parameter).

## License

This package, including its embedded copy of [axe-core][axe-core], is distributed under the terms of the [Mozilla Public License, version 2.0](../../LICENSE-Deque.AxeCore.Commons.txt).

## Acknowledgements

This package builds on past work from the [SeleniumAxeDotnet](https://github.com/TroyWalshProf/SeleniumAxeDotnet) and [PlaywrightAxeDotnet](https://github.com/IsaacWalker/PlaywrightAxeDotnet) projects (see [NOTICE.txt](../../NOTICE.txt)). We thank all of those projects' contributors for their work.
