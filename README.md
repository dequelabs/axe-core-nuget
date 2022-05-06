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




