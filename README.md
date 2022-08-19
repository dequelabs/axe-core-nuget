# axe-core-nuget

[![Join our Slack chat](https://img.shields.io/badge/slack-chat-purple.svg?logo=slack)](https://accessibility.deque.com/axe-community)

This repository contains 3 NuGet packages, which can be used for automated accessibility testing powered by [axe core][axe-core].

The packages are listed below:

- [`Deque.AxeCore.Playwright`](./packages/playwright/README.md)
- [`Deque.AxeCore.Selenium`](./packages/selenium/README.md)
- [`Deque.AxeCore.Commons`](./packages/commons/README.md)

## Philosophy

We believe that automated testing has an important role to play in achieving digital equality and that in order to do that, it must achieve mainstream adoption by professional web developers. That means that the tests must inspire trust, must be fast, must work everywhere and must be available everywhere.

## Manifesto

1. Automated accessibility testing rules must have a zero false positive rate
2. Automated accessibility testing rules must be lightweight and fast
3. Automated accessibility testing rules must work in all modern browsers
4. Automated accessibility testing rules must, themselves, be tested automatically

[axe-core]: https://github.com/dequelabs/axe-core

## License

* The `Deque.AxeCore.Playwright` NuGet package and its source code under the [`packages/playwright/` directory](./packages/playwright) are distributed under the terms of the [MIT License](./LICENSE-Deque.AxeCore.Playwright.txt).
* The `Deque.AxeCore.Selenium` NuGet package and its source code under the [`packages/selenium/` directory](./packages/selenium) are distributed under the terms of the [MIT License](./LICENSE-Deque.AxeCore.Selenium.txt).
* The `Deque.AxeCore.Commons` NuGet package, its source code under [`packages/commons/` directory](./packages/commons), its embedded copy of [`axe-core`](https://github.com/dequelabs/axe-core), and all other source code in this repository outside of the `packages/` directory are distributed under the terms of the [Mozilla Public License, version 2.0](./LICENSE-Deque.AxeCore.Commons.txt).

Note that the (MIT licensed) `Deque.AxeCore.Playwright` and `Deque.AxeCore.Selenium` NuGet packages each have a dependency on the (MPL licensed) `Deque.AxeCore.Commons` NuGet package.

## Acknowledgments

* `Deque.AxeCore.Playwright` is a fork of [IsaacWalker/PlaywrightAxeDotnet](https://github.com/IsaacWalker/PlaywrightAxeDotnet). We thank @IsaacWalker for his work and for agreeing to re-home the library as an officially supported integration package.
* `Deque.AxeCore.Selenium` is a fork of [TroyWalshProf/SeleniumAxeDotnet](https://github.com/TroyWalshProf/SeleniumAxeDotnet/graphs/contributors), which in turn is a fork of [javnov/axe-selenium-csharp](https://github.com/javnov/axe-selenium-csharp/graphs/contributors). We thank @TroyWalshProf, @javnov, and all the other contributors to those projects for their work. We especially thank @TroyWalshProf for agreeing to re-home the library as an officially supported integration package.
