# Deque.AxeCore.Playwright

[![Deque.AxeCore.Playwright NuGet package](https://img.shields.io/nuget/v/Deque.AxeCore.Playwright)](https://www.nuget.org/packages/Deque.AxeCore.Playwright)
[![NuGet package download counter](https://img.shields.io/nuget/dt/Deque.AxeCore.Playwright)](https://www.nuget.org/packages/Deque.AxeCore.Playwright/)

Automated web accessibility testing with .NET, C#, and Playwright. Wraps the [axe-core](https://github.com/dequelabs/axe-core) accessibility scanning engine and the [Playwright](https://playwright.dev/dotnet/) browser automation framework.

Compatible with .NET Standard 2.1.

## Getting Started

Install via NuGet:

```powershell
PM> Install-Package Deque.AxeCore.Playwright
# or, use the Visual Studio "Manage NuGet Packages" UI
```

Ensure you have Playwright browsers installed. 
For reference, see https://playwright.dev/dotnet/docs/browsers

Example usage with Playwright's NUnit integration:

```cs
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Playwright.NUnit;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;

[TestFixture]
class MyPlaywrightTests : PageTest
{
    [Test]
    public async Task CheckAxeClean()
    {
        const string expectedViolationId = "color-contrast";

        await Page!.GotoAsync("https://playwright.dev/dotnet");

        AxeResult axeResults = await Page!.RunAxe();
        Assert.That(axeResults.Violations, Is.Null.Or.Empty);
    }
}
```

## API Reference

This library essentially wraps the axe-core library.
Currently the two supported functions are for rule retrieval and for running.

### GetAxeRules

This method retrieves metadata about the rules that axe is capable of running.

```cs
IList<AxeRuleMetadata> axeRules = await page.GetAxeRules();
foreach(var rule in axeRules)
{
    Console.WriteLine($"Rule name: {rule.RuleId} Help: {rule.Help} HelpUrl: {rule.HelpUrl}");
    Console.WriteLine($"Tags: {string.Join(", ", rule.Tags)}");
}
```

It is also possible to run this only selecting for rules with particular tags.
Tags are considered in a disjunctive fashion.

```cs
// Only take rules which are wcag2aa or wcag2a.
IList<string> tags = new List<string>() { "wcag2aa", "wcag2a"}

IList<AxeRuleMetadata> axeRules = await page.GetAxeRules(tags);
foreach(var rule in axeRules)
{
    Console.WriteLine($"Rule name: {rule.RuleId} Help: {rule.Help} HelpUrl: {rule.HelpUrl}");
    Console.WriteLine($"Tags: {string.Join(", ", rule.Tags)}");
}
```

### RunAxe

This method executes the [axe.run](https://www.deque.com/axe/core-documentation/api-documentation/#api-name-axerun) API, which will run rules against the current state of the page.

```cs
AxeResult axeResults = await page.RunAxe();

Console.WriteLine($"Axe ran against {axeResults.Url} on {axeResults.Timestamp}.");

Console.WriteLine($"Rules that failed:");
foreach(var violation in axeResults.Violations)
{
    Console.WriteLine($"Rule Id: {violation.Id} Impact: {violation.Impact} HelpUrl: {violation.HelpUrl}.");

    foreach(var node in violation.Nodes)
    {
        Console.WriteLine($"\tViolation found at: {node.Target}");
        Console.WriteLine($"\t...with HTML: {node.Html}");
    }
}

Console.WriteLine($"Rules that passed successfully:");
foreach(var pass in axeResults.Passes)
{
    Console.WriteLine($"Rule Id: {pass.Id} Impact: {pass.Impact} HelpUrl: {pass.HelpUrl}.");
}

Console.WriteLine($"Rules that did not fully run:");
foreach(var incomplete in axeResults.Incomplete)
{
    Console.WriteLine($"Rule Id: {incomplete.Id}.");
}

Console.WriteLine($"Rules that were not applicable:");
foreach(var inapplicable in axeResults.Inapplicable)
{
    Console.WriteLine($"Rule Id: {inapplicable.Id}.");
}
```

`RunAxe` is supported on both Playwright `Page`s and Playwright `Locator`s. `Locator`s are the easiest way to run axe against only part of a page:

```cs
ILocator locator = page.Locator("text=Sign In");

AxeResult axeResults = await locator.RunAxe();
```

Alternately, when used on a `Page`, `RunAxe` supports an `AxeRunContext` parameter which supports more complex rules for deciding which parts of a page to scan (or exclude from a scan). This corresponds to the [axe.run Context parameter](https://www.deque.com/axe/core-documentation/api-documentation/#context-parameter):

```cs
AxeRunContext runContext = new AxeRunContext()
{
    // Only run on this #my-id.
    Include = new List<AxeSelector>() { new AxeSelector("#my-id") }
};

runContext = new AxeRunContext()
{
    // Run on everything except #my-id.
    Exclude = new List<AxeSelector>() { new AxeSelector("#my-id") }
};

runContext = new AxeRunContext()
{
    // Run on every button except #excluded-id.
    Include = new List<AxeSelector>() { new AxeSelector("button") },
    Exclude = new List<AxeSelector>() { new AxeSelector("#my-id") }
};

runContext = new AxeRunContext()
{
    // Run on everything, except for #my-id within the iframe #my-frame.
    Exclude = new List<AxeSelector>()
    {
        new AxeSelector("#my-id", new List<string>() { "#my-frame" })
    }
};

AxeResult axeResults = await page.RunAxe(runContext);
```

All `RunAxe` methods also support an `AxeRunOptions`` parameter.
This corresponds to the [axe.run Options parameter](https://www.deque.com/axe/core-documentation/api-documentation/#options-parameter).

```cs
AxeRunOptions options = new AxeRunOptions()
{
    // Run only tags that are wcag2aa.
    RunOnly = new RunOnlyOptions { Type = "tag", Values = new List<string> { "wcag2aa" } },

    // Specify rules.
    Rules = new Dictionary<string, RuleOptions>()
    {
        // Don't run color-contrast.
        { "color-contrast", new RuleOptions() { Enabled = false } }
    },

    // Limit result types to Violations.
    ResultTypes = new HashSet<ResultType>()
    {
        ResultType.Violations
    },

    // Don't return css selectors in results.
    Selectors = false,

    // Return CSS selector for elements, with all the element's ancestors.
    Ancestry = true,

    // Don't return xpath selectors for elements.
    XPath = false,

    // Don't run axe on iframes inside the document.
    Iframes = false
};

AxeResult axeResults = await page.RunAxe(options);
axeResults = await page.RunAxe(context, options);
axeResults = await locator.RunAxe(options);
```

### `RunAxeLegacy`

Set the frame testing method to "legacy mode". In this mode, axe will not open a blank page in which to aggregate its results. This can be used in an environment where opening a blank page causes issues.

With legacy mode turned on, axe will fall back to its test solution prior to the 4.3 release, but with cross-origin frame testing disabled. The frame-tested rule will report which frames were untested.

**Important**: Use of `.RunAxeLegacy()` is a last resort. If you find there is no other solution, please [report this as an issue](https://github.com/dequelabs/axe-core-nuget/issues/). It will be removed in a future release.

```csharp
AxeResult axeResults = await page.RunAxeLegacy();
```

## Migrating from `Playwright.Axe` ([PlaywrightAxeDotnet](https://github.com/IsaacWalker/PlaywrightAxeDotnet))

This project acts as a drop-in replacement for most of the functionality from `Playwright.Axe`. ([PlaywrightAxeDotnet](https://github.com/IsaacWalker/PlaywrightAxeDotnet)). To migrate:

1. Update your test `.csproj` file's `<PackageReference>` for `Playwright.Axe` to `Deque.AxeCore.Playwright`
1. Add a new `<PackageReference>` to `Deque.AxeCore.Commons` at the same version number as `Deque.AxeCore.Playwright`
1. Update all `using Playwright.Axe;` statements in your tests to `using Deque.AxeCore.Playwright;` and/or `using Deque.AxeCore.Commons;`

In a move to standardize, we migrated this package away from Playwright specific typings, instead opting to use the typings from the `Deque.AxeCore.Commons` package instead. The result is several minor breaking changes that may require updates to your code:

### Replacements/Renamings

1. `AxeResults` has now been renamed to `AxeResult`; the previously used `AxeResult` for this package will now be `AxeResultItem`.
1. `AxeNodeResult` has been replaced with `AxeResultNode`
1. `AxeCheckResult` has been replaced with `AxeResultCheck`
1. `AxeResultGroup` has been replaced with `ResultType`
1. `AxeRuleObjectValue` has been replaced with `RuleOptions`
1. `AxeRunOnly` has been replaced with `RunOnlyOptions`
1. `AxeRelatedNode` has been replaced with `AxeResultRelatedNode`. With this type, the Targets property is changed from a `IList<string>` to a `List<AxeResultTarget>`; users should expect to modify usages of the Targets property to include a `.ToString()` method call.
1. `AxeRunSerialContext` has been replaced by `AxeRunContext` and `AxeSelector` types. Here are some examples of how to use the new types:
 
```cs
// Finding a single element using the Playwright Locator API.
ILocator locator = page.GetByRole("menu").RunAxe();

// Iincluding/excluding elements in the main frame.
new AxeRunContext()
    {
        Include = new List<AxeSelector> { new AxeSelector("#foo") },
        Exclude = new List<AxeSelector> {},
    };

// Including/excluding an element in a child frame.
new AxeRunContext()
    {
        Include = new List<AxeSelector> { new AxeSelector("#element-in-child-frame", new List<string> { "#iframe-in-main-frame" })},
        Exclude = new List<AxeSelector> {},
    };
```

### Type Modifications

1. The Timestamp type in `AxeResult` changed from System.DateTime to System.DateTimeOffset
1. The url type in `AxeResult` changed from Uri to string
1. The `OrientationAngle` type in `AxeTestEnvironment` changed from int to double
1. The `HelpUrl` in `AxeResultItem` changed from Uri to string

### Removals

1. Removed `AxeEnvironmentData` interface and using the existing environment data info in `Deque.AxeCore.Commons.AxeResult`
1. Removed `AxeImpactValue` and `AxeRunOnlyType` enums in favor of using `string` in the Commons typings (`AxeResultItem.cs` and `AxeRunOptions.cs`, respectively)
1. `FailureSummary` was removed from `AxeResultNode` (formerly `AxeNodeResult`)
1. `ElementRef` and `PerformanceTimer` were removed from `AxeRunOptions`
1. `AxeRunOptions` and `RunOnlyOptions` now use named object-initializer syntax instead of constructor parameters.

## Contributing

Refer to the general [axe-core-nuget CONTRIBUTING.md](../../CONTRIBUTING.md).

## License

This package is distributed under the terms of the [MIT License](../../LICENSE-Deque.AxeCore.Playwright.txt).

However, note that it has a dependency on the ([MPL licensed](../../LICENSE-Deque.AxeCore.Commons.txt)) `Deque.AxeCore.Commons` NuGet package.

## Acknowledgements

This package builds on past work from the [PlaywrightAxeDotnet](https://github.com/IsaacWalker/PlaywrightAxeDotnet) project (see [NOTICE.txt](../../NOTICE.txt)). We @IsaacWalker for his work on that project.
