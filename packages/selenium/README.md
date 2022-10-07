# Deque.AxeCore.Selenium

[![Deque.AxeCore.Selenium NuGet package](https://img.shields.io/nuget/v/Deque.AxeCore.Selenium)](https://www.nuget.org/packages/Deque.AxeCore.Selenium) 
[![NuGet package download counter](https://img.shields.io/nuget/dt/Deque.AxeCore.Selenium)](https://www.nuget.org/packages/Deque.AxeCore.Selenium/) 

Automated web accessibility testing with .NET, C#, and Selenium. Wraps the [axe-core](https://github.com/dequelabs/axe-core) accessibility scanning engine and the [Selenium.WebDriver](https://www.seleniumhq.org/) browser automation framework.

Compatible with .NET Standard 2.0+, .NET Framework 4.7.1+, and .NET Core 2.0+.

## Getting Started

Install via NuGet:

```powershell
PM> Install-Package Deque.AxeCore.Selenium
# or, use the Visual Studio "Manage NuGet Packages" UI
```

Import this namespace:

```csharp
using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
```

To run an axe accessibility scan of a web page with the default configuration, create a new `AxeBuilder` using your Selenium `IWebDriver` object and call `Analyze`:

```csharp
IWebDriver webDriver = new ChromeDriver();
AxeResult axeResult = new AxeBuilder(webDriver).Analyze();
```

To cause a test to pass or fail based on the scan, use the `Violations` property of the `AxeResult`:

```csharp
// We recommend FluentAssertions to get great error messages out of the box
using FluentAssertions;

axeResult.Violations.Should().BeEmpty();
```

To configure different scan options, use the chainable methods of the `AxeBuilder` ([reference docs](#axebuilder-reference)):

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Exclude(".css-class-of-element-with-known-failures")
    .WithTags("wcag2a")
    .Analyze();
```

For a complete working sample project that uses this library, see [the C# sample in microsoft/axe-pipelines-samples](https://github.com/microsoft/axe-pipelines-samples/tree/master/csharp-selenium-webdriver-sample).

## AxeBuilder Reference

### `AxeBuilder.Analyze()`

```csharp
AxeResult axeResult = new AxeBuilder(webDriver).Analyze();
```

Runs an axe accessibility scan of the entire page using all previously chained options and returns an `AxeResult` representing the scan results.

### `AxeBuilder.Analyze(IWebElement element)`

```csharp
IWebElement elementToTest = webDriver.FindElement(By.Id("nav-bar"));
AxeResult axeResult = new AxeBuilder(webDriver)
    .Analyze(elementToTest);
```

Runs an axe accessibility scan scoped using all previously chained options to the given Selenium `IWebElement`. Returns an `AxeResult` representing the scan results.

Not compatible with `AxeBuilder.Include` or `AxeBuilder.Exclude`; the element passed to `Analyze` will take precedence and the `Include`/`Exclude` calls will be ignored.

### `AxeBuilder.Include(string cssSelector)`

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Include(".class-of-element-under-test")
    .Analyze();
```

Scopes future `Analyze()` calls to include *only* the element(s) matching the given CSS selector.

`Include` may be chained multiple times to include multiple selectors in a scan.

`Include` may be combined with `Exclude` to scan a tree of elements but omit some children of that tree. For example:

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Include("#element-under-test")
    .Exclude("#element-under-test div.child-class-with-known-issues")
    .Analyze();
```

`Include` is not compatible with `Analyze(IWebElement)` - the `Analyze` argument will take precedence and `Include` will be ignored.

This overload of `Include` only supports CSS selectors which refer to elements which are in the topmost frame of the page and not contained within a shadow DOM. To specify a selector in an iframe or a shadow DOM, see the overload that accepts an `AxeSelector`.

### `AxeBuilder.Include(AxeSelector axeSelector)`

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Include(new AxeSelector("#element-inside-iframe", new List<string> { "#containing-iframe-element" }))
    .Analyze();
```

Scopes future `Analyze()` calls to include *only* the element(s) matching the given `AxeSelector`.

`Include` may be chained multiple times to include multiple selectors in a scan.

`Include` may be combined with `Exclude` to scan a tree of elements but omit some children of that tree. For example:

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Include(new AxeSelector("#element-inside-iframe", new List<string> { "#containing-iframe-element" }))
    .Exclude(new AxeSelector("#element-inside-iframe div.child-class-with-known-issues", new List<string> { "#containing-iframe-element" }))
    .Analyze();
```

This overload of `Include` supports complex AxeSelectors which specify elements inside iframes and/or shadow DOMs:

```csharp
AxeSelector elementInNestedIframes = new AxeSelector("#element-in-nested-iframe", new List<string> { "#topmost-iframe", "#nested-iframe" });
AxeSelector elementInShadowDom = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> { new List<string> { "#shadow-root-in-topmost-frame", "#element-in-shadow-dom" }});
AxeSelector elementInComplexFrameShadowLayout = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
    new List<string> { "#shadow-root-in-topmost-frame", "#nested-shadow-root", "#iframe-in-nested-shadow-dom" },
    new List<string> { "#shadow-root-in-iframe", "#deeply-nested-target-element" }
});
```

### `AxeBuilder.Exclude(string cssSelector)`

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Exclude(".class-of-element-with-known-issues")
    .Analyze();
```

Scopes future `Analyze()` calls to exclude the element(s) matching the given CSS selector.

`Exclude` may be chained multiple times to exclude multiple selectors in a scan.

`Exclude` may be combined with `Include` to scan a tree of elements but omit some children of that tree. For example:

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Include("#element-under-test")
    .Exclude("#element-under-test div.child-class-with-known-issues")
    .Analyze();
```

`Exclude` is not compatible with `Analyze(IWebElement)` - the `Analyze` argument will take precedence and `Exclude` will be ignored.

This overload of `Include` only supports CSS selectors which refer to elements which are in the topmost frame of the page and not contained within a shadow DOM. To specify a selector in an iframe or a shadow DOM, see the overload that accepts an `AxeSelector`.

### `AxeBuilder.Exclude(AxeSelector axeSelector)`

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Exclude(new AxeSelector("#element-inside-iframe-with-known-issues", new List<string> { "#containing-iframe-element" }))
    .Analyze();
```

Scopes future `Analyze()` calls to exclude the element(s) matching the given CSS selector.

`Exclude` may be chained multiple times to exclude multiple selectors in a scan.

`Exclude` may be combined with `Include` to scan a tree of elements but omit some children of that tree. For example:

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Include(new AxeSelector("#element-inside-iframe", new List<string> { "#containing-iframe-element" }))
    .Exclude(new AxeSelector("#element-inside-iframe div.child-class-with-known-issues", new List<string> { "#containing-iframe-element" }))
    .Analyze();
```

`Exclude` is not compatible with `Analyze(IWebElement)` - the `Analyze` argument will take precedence and `Exclude` will be ignored.

This overload of `Exclude` supports complex AxeSelectors which specify elements inside iframes and/or shadow DOMs:

```csharp
AxeSelector elementInNestedIframes = new AxeSelector("#element-in-nested-iframe", new List<string> { "#topmost-iframe", "#nested-iframe" });
AxeSelector elementInShadowDom = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> { new List<string> { "#shadow-root-in-topmost-frame", "#element-in-shadow-dom" }});
AxeSelector elementInComplexFrameShadowLayout = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
    new List<string> { "#shadow-root-in-topmost-frame", "#nested-shadow-root", "#iframe-in-nested-shadow-dom" },
    new List<string> { "#shadow-root-in-iframe", "#deeply-nested-target-element" }
});
```

### `AxeBuilder.WithRules(params string[] axeRuleIds)`

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .WithRules("color-contrast", "duplicate-id")
    .Analyze();
```

Causes future calls to `Analyze` to only run the specified axe rules.

For a list of the available axe rules, see https://dequeuniversity.com/rules/axe/3.3. The "Rule ID" at the top of each individual rule's page is the ID you would want to pass to this method.

`WithRules` is not compatible with `WithTags` or `DisableRules`; whichever one you specify last will take precedence.

`WithRules` is not compatible with the deprecated raw `Options` property.

### `AxeBuilder.DisableRules(params string[] axeRuleIds)`

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .DisableRules("color-contrast", "duplicate-id")
    .Analyze();
```

Causes future calls to `Analyze` to omit the specified axe rules.

For a list of the available axe rules, see https://dequeuniversity.com/rules/axe/3.3. The "Rule ID" at the top of each individual rule's page is the ID you would want to pass to this method.

`DisableRules` is compatible with `WithTags`; you can use this to run all-but-some of the rules for a given set of tags. For example, to run all WCAG 2.0 A rules except for `color-contrast`:

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .WithTags("wcag2a")
    .DisableRules("color-contrast")
    .Analyze();
```

`DisableRules` is not compatible with `WithRules`; whichever one you specify second will take precedence.

`DisableRules` is not compatible with the deprecated raw `Options` property.

### `AxeBuilder.WithTags(params string[] axeRuleTags)`

Causes future calls to `Analyze` to only run axe rules that match at least one of the specified tags.

A "tag" is a string that may be associated with a given axe rule. See the [axe-core API documentation](https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter) for a complete list of available tags.

`WithTags` is compatible with `DisableRules`; you can use this to run all-but-some of the rules for a given set of tags. For example, to run all WCAG 2.0 A rules except for `color-contrast`:

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .WithTags("wcag2a")
    .DisableRules("color-contrast")
    .Analyze();
```

`WithTags` is not compatible with `WithRules`; whichever one you specify second will take precedence.

`WithTags` is not compatible with the deprecated raw `Options` property.

### `AxeBuilder.WithOptions(AxeRunOptions options)`

*Note: in most cases, the simpler `WithRules`, `WithTags`, and `DisableRules` can be used instead.*

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .WithOptions(new AxeRunOptions()
    {
        RunOnly = new RunOnlyOptions
        {
            Type = "rule",
            Values = new List<string> { "duplicate-id", "color-contrast" }
        },
        RestoreScroll = true
    })
    .Analyze();
```

Causes future calls to `Analyze` to use the specified options when calling `axe.run` in axe-core. See [the axe-core API documentation](https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter) for descriptions of the different properties of `AxeRunOptions`.

`WithOptions` will override any values previously set by `WithRules`, `WithTags`, and `DisableRules`.

`WithOptions` is not compatible with the deprecated raw `Options` property.

#### Skip iFrames
If you don't want to run Axe on iFrames you can tell Axe skip with AxeRunOptions.

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .WithOptions(new AxeRunOptions()
    {
        Iframes = false
    })
    .Analyze();
```
Prevents Axe core from getting injected into page iFrames.

Causes future calls to `Analyze` to use the specified options when calling `axe.run` in axe-core. See [the axe-core API documentation](https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter) for descriptions of the different properties of `AxeRunOptions`.

### `AxeBuilder.WithOutputFile(string filePath)`

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .WithOutputFile(@"./path/to/axe-results.json")
    .Analyze();
```

Causes future calls to `Analyze` to export their results to a JSON file, *in addition* to being returned as an `AxeResult` object as usual.

The output format is exactly the same as axe-core would have produced natively, and is compatible with other tools that read axe result JSON, like [axe-sarif-converter](https://github.com/microsoft/axe-sarif-converter).

### `AxeBuilder.AxeBuilder(webDriver, axeBuilderOptions)`

This constructor overload enables certain advanced options not required by most `Deque.AxeCore.Selenium` users. Currently, its only use is to allow you to use a custom `axe-core` implementation, rather than the one that is packaged with this library.

```csharp
AxeBuilderOptions axeBuilderOptions = new AxeBuilderOptions
{
    ScriptProvider = new FileAxeScriptProvider(".\\axe.min.js")
};
AxeResult axeResult = new AxeBuilder(webDriver, axeBuilderOptions).Analyze();
```

## Working with AxeResult objects

In most cases, you would run an axe scan from within a test method in a suite of end to end tests, and you would want to use a test assertion to verify that there are no unexpected accessibility violations in a page or component.

We strongly recommend [FluentAssertions](https://fluentassertions.com/), a NuGet package that does a good job of producing actionable error messages based on the `AxeResult` output from a scan. That said, if you prefer a different assertion library, you can still use this library; it does not require any particular test or assertion framework.

If you start with no accessibility issues in your page, you can stay clean by validating that the Violations list is empty:

```csharp
IWebDriver webDriver = new ChromeDriver();
AxeResult results = new AxeBuilder(webDriver).Analyze();

results.Violations.Should().BeEmpty();
```

If you already have some accessibility issues & you want to make sure that you do not introduce any more new issues till you get to a clean state, you can use `Exclude` to remove problematic elements from a broad scan, and a combination of `Include` and `DisableRules` to perform a more scoped scan of the element with known issues:

```csharp
// Suppose #element-with-contrast-issue has known violations of the color-contrast rule.

// You could scan that element with the color-contrast rule disabled...
new AxeBuilder(webDriver)
    .Include("#element-with-contrast-issue")
    .DisableRules("color-contrast")
    .Analyze()
    .Violations.Should().BeEmpty();

// ...and then also scan the rest of the page with all rules enabled.
new AxeBuilder(webDriver)
    .Exclude("#element-with-contrast-issue")
    .Analyze()
    .Violations.Should().BeEmpty();
```

## Migrating from `Selenium.Axe` ([SeleniumAxeDotnet](https://github.com/TroyWalshProf/SeleniumAxeDotnet))

This project acts as a drop-in replacement for most of the functionality from the `Selenium.Axe` NuGet package ([SeleniumAxeDotnet](https://github.com/TroyWalshProf/SeleniumAxeDotnet)). To migrate:

1. Update your test `.csproj` file's `<PackageReference>` for `Selenium.Axe` to `Deque.AxeCore.Selenium`
1. Add a new `<PackageReference>` to `Deque.AxeCore.Commons` at the same version number as `Deque.AxeCore.Selenium`
1. Update all `using Selenium.Axe;` statements in your tests to `using Deque.AxeCore.Selenium;` and/or `using Deque.AxeCore.Commons;`
1. The `.Target` and `.XPath` properties of `AxeResultNode` or `AxeResultRelatedNode` are now strongly-typed `AxeSelector` objects. This will not impact most `Selenium.Axe` users, but if your tests refer to those properties explicitly, you will probably want to do so via their `ToString()` representations.
1. `AxeRunOptions.FrameWaitTimeInMilliseconds` was renamed to `AxeRunOptions.FrameWaitTime` to match the equivalent `axe-core` API. The usage is unchanged; it still represents a value in milliseconds.
1. The already-deprecated `AxeBuilder.Options` property was removed; replace it with `WithOptions`, `WithRules`, `WithTags`, and/or `DisableRules`.
1. The `AxeBuilder.Include` and `AxeBuilder.Exclude` overloads which accept more than one parameter have changed:
    * If you were using it to refer to an element inside a nested frame, replace it with the `Include`/`Exclude` overloads which accept an `AxeSelector`:
        ```cs
        //old
        new AxeBuilder(webDriver).Include("#some-iframe", "#element-in-nested-frame").Analyze();

        // new
        new AxeBuilder(webDriver).Include(new AxeBuilder("#element-in-nested-frame", new List<string> { "#some-iframe" })).Analyze();
        ```
    * If you were using it to refer to multiple elements in the main frame of the page under test, replace it with multiple `Include`/`Exclude` calls (one per element):
        ```cs
        // old
        // This wasn't doing what you meant it to and may have been hiding issues; this
        // would have looked for #bar elements inside an iframe with selector #foo, *not*
        // for sibling #foo and #bar elements in the main frame of a page.
        new AxeBuilder(webDriver).Include("#foo", "#bar").Analyze();

        // new
        new AxeBuilder(webDriver).Include("#foo").Include("#bar").Analyze();
        ```

This project does *not* include a replacement for `Selenium.Axe`'s built-in HTML report functionality. We expect it to be split out into a separate standalone library (usable from either this package or `Deque.AxeCore.Playwright`) at a later date, but there is currently no direct replacement.

## Contributing

Refer to the general [axe-core-nuget CONTRIBUTING.md](../../CONTRIBUTING.md).

## License

This package is distributed under the terms of the [MIT License](../../LICENSE-Deque.AxeCore.Selenium.txt).

However, note that it has a dependency on the ([MPL licensed](../../LICENSE-Deque.AxeCore.Commons.txt)) `Deque.AxeCore.Commons` NuGet package.

## Acknowledgements

This package builds on past work from the [SeleniumAxeDotnet](https://github.com/TroyWalshProf/SeleniumAxeDotnet) project (see [NOTICE.txt](../../NOTICE.txt)). We thank all of its contributors for their work.
