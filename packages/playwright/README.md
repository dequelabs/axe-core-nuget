# Deque.AxeCore.Playwright

[![Deque.AxeCore.Playwright NuGet package](https://img.shields.io/nuget/v/Deque.AxeCore.Playwright)](https://www.nuget.org/packages/Deque.AxeCore.Selenium) 
[![NuGet package download counter](https://img.shields.io/nuget/dt/Deque.AxeCore.Playwright)](https://www.nuget.org/packages/Deque.AxeCore.Selenium/) 

Automated web accessibility testing with .NET, C#, and Playwright. Wraps the [axe-core](https://github.com/dequelabs/axe-core) accessibility scanning engine and the [Selenium.WebDriver](https://www.seleniumhq.org/) browser automation framework.

Compatible with .NET Standard 2.1.

## Getting Started

Install via NuGet:

```powershell
PM> Install-Package Deque.AxeCore.Playwright
# or, use the Visual Studio "Manage NuGet Packages" UI
```

Example usage:

```csharp
using System.Threading.Tasks;
using Microsoft.Playwright;
using Deque.AxeCore.Playwright;

class Program
{
    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://playwright.dev/dotnet");
        
        AxeResults axeResults = await page.RunAxe();

        // Assert.AreEqual(axeResults.Violations.Count, 0);
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

This method executes the axe run method, which will run rules against the current state of the page.

```cs

AxeResults axeResults = await page.RunAxe();

Console.WriteLine($"Axe ran against {axeResults.Url} on {axeResults.Timestamp}.");

Console.WriteLine($"Rules that failed:");
foreach(var violation in axeResults.Violations)
{
    Console.WriteLine($"Rule Id: {violation.Id} Impact: {violation.Impact} HelpUrl: {violation.HelpUrl}.");

    foreach(var node in violation.Nodes)
    {
        Console.WriteLine($"\tViolation found in Html: {node.Html}.");

        foreach(var target in node.Target)
        {
            Console.WriteLine($"\t\t{target}.");
        }
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

This method can be run on an element via the context parameter.
This allows the inclusion and exclusion of string selectors.
When exclude is only specified, include will default to the entire document.
Currently the node and node lists functionality are not supported.

``` cs

AxeRunContext runContext = new AxeRunSerialContext("#my-id"); // Only run on this id.

runContext = new AxeRunSerialContext(null, "#my-id"); // Run on everything but this id.

runContext = new AxeRunSerialContext("button", "#my-id"); // Run on every button that does not have this id.

runContext = new AxeRunSerialContext(new List<string>()
{
    new List<string>()
    {
        "#my-frame",
        "#my-id"
    }
}); // Run on the element with my-id Id and which is inside the frame with id my-frame.

AxeResults axeResults = await page.RunAxe(runContext);

```

The run method can also be run on a Playwright Locator.
This does not support context parameter.

``` cs
ILocator locator = page.Locator("text=Sign In");

AxeResults axeResults = await locator.RunAxe();
```

All these run methods support an AxeRunOptions parameter.
This is roughly the equivalent of [axe options](https://www.deque.com/axe/core-documentation/api-documentation/#options-parameter) .

```cs

AxeRunOptions options = new AxeRunOptions(
    // Run only tags that are wcag2aa.
    runOnly: new AxeRunOnly(AxeRunOnlyType.Tag, new List<string> { "wcag2aa" }),

    // Specify rules.
    rules: new Dictionary<string, AxeRuleObjectValue>()
    {
        // Don't run color-contrast.
        {"color-contrast", new AxeRuleObjectValue(false)}
    },

    // Limit result types to Violations.
    resultTypes: new List<AxeResultGroup>()
    {
        AxeResultGroup.Violations
    },

    // Don't return css selectors in results.
    selectors: false,

    // Return CSS selector for elements, with all the element's ancestors.
    ancestry: true,

    // Don't return xpath selectors for elements.
    xpath: false,

    // Don't run axe on iframes inside the document.
    iframes: false
);

AxeResults axeResults = await page.RunAxe(options);
axeResults = await page.RunAxe(context, options);
axeResults = await locator.RunAxe(options);


```

It is also possible to create a html report of a run.

```cs

AxeHtmlReportOptions reportOptions = new(reportDir: "C:\myReport");
AxeResults axeResults = await page.RunAxe(reportOptions: reportOptions);

```

## Contributing

Refer to the general [axe-core-nuget CONTRIBUTING.md](../../CONTRIBUTING.md).

## License

This package is distributed under the terms of the [MIT License](../../LICENSE-Deque.AxeCore.Playwright.txt).

However, note that it has a dependency on the ([MPL licensed](../../LICENSE-Deque.AxeCore.Commons.txt)) `Deque.AxeCore.Commons` NuGet package.

## Acknowledgements

This package builds on past work from the [PlaywrightAxeDotnet](https://github.com/IsaacWalker/PlaywrightAxeDotnet) project (see [NOTICE.txt](../../NOTICE.txt)). We @IsaacWalker for his work on that project.
