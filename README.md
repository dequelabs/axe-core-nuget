# Playwright Axe for .NET

Combines [Microsoft Playwright](https://playwright.dev/dotnet/) with [Deque's Axe](https://www.deque.com/axe/core-documentation/api-documentation/#section-1-introduction) for automated accessibility testing in C#/.NET.
Works by injecting and wrapping the axe-core library via the [JavaScript Evaluation API](https://playwright.dev/dotnet/docs/evaluating).

Approach and setup heavily inspired by similar projects, particular thanks to [SeleniumAxeDotnet](https://github.com/TroyWalshProf/SeleniumAxeDotnet).

## Quickstart

```cs
using System.Threading.Tasks;
using Microsoft.Playwright;
using Playwright.Axe;

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


