# axe-core-nuget

[![Join our Slack chat](https://img.shields.io/badge/slack-chat-purple.svg?logo=slack)](https://accessibility.deque.com/axe-community)

This repository contains 3 NuGet packages, which can be used for automated accessibility testing powered by [axe core][axe-core].

The packages are listed below:

- [`Playwright.Axe`](./packages/playwright/README.md)
- [`Selenium.Axe`](./packages/selenium/README.md)
- [`Axe.Core`](./packages/core/README.md)

## Philosophy

We believe that automated testing has an important role to play in achieving digital equality and that in order to do that, it must achieve mainstream adoption by professional web developers. That means that the tests must inspire trust, must be fast, must work everywhere and must be available everywhere.

## Manifesto

1. Automated accessibility testing rules must have a zero false positive rate
2. Automated accessibility testing rules must be lightweight and fast
3. Automated accessibility testing rules must work in all modern browsers
4. Automated accessibility testing rules must, themselves, be tested automatically

[axe-core]: https://github.com/dequelabs/axe-core

## License

* The `Playwright.Axe` NuGet package and its source code under the [`packages/playwright/` directory](./packages/playwright) are distributed under the terms of the [MIT License](./LICENSE-Playwright.Axe.txt).
* The `Selenium.Axe` NuGet package and its source code under the [`packages/selenium/` directory](./packages/selenium) are distributed under the terms of the [MIT License](./LICENSE-Selenium.Axe.txt).
* `axe-core`, the `Axe.Core` NuGet package, and all source code in this repository outside of the `packages/playwright/` and `packages/selenium/` directories are distributed under the terms of the [Mozilla Public License, version 2.0](./LICENSE-Axe.Core.txt).

Note that the `Playwright.Axe` and `Selenium.Axe` NuGet packages each have a dependency on the `Axe.Core` NuGet package.

## Acknowledgments

* Selenium.Axe was originally authored by @TroyWalshProf, @javnov, and a variety of other contributors in [TroyWalshProf/SeleniumAxeDotnet](https://github.com/TroyWalshProf/SeleniumAxeDotnet/graphs/contributors) and [javnov/axe-selenium-csharp](https://github.com/javnov/axe-selenium-csharp/graphs/contributors).
* Playwright.Axe was originally authored by @IsaacWalker in [IsaacWalker/PlaywrightAxeDotnet](https://github.com/IsaacWalker/PlaywrightAxeDotnet).
