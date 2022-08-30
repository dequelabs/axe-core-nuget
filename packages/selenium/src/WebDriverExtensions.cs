﻿using OpenQA.Selenium;
using System;
using Deque.AxeCore.Commons;

namespace Deque.AxeCore.Selenium
{
    public static class WebDriverExtensions
    {
        public static AxeResult Analyze(this IWebDriver webDriver)
        {
            if (webDriver == null)
                throw new ArgumentNullException(nameof(webDriver));

            AxeBuilder axeBuilder = new AxeBuilder(webDriver);
            return axeBuilder.Analyze();
        }

        public static AxeResult Analyze(this IWebDriver webDriver, AxeBuilderOptions axeBuilderOptions)
        {
            if (webDriver == null)
                throw new ArgumentNullException(nameof(webDriver));

            AxeBuilder axeBuilder = new AxeBuilder(webDriver, axeBuilderOptions);
            return axeBuilder.Analyze();
        }

        public static AxeResult Analyze(this IWebDriver webDriver, IWebElement context)
        {
            if (webDriver == null)
                throw new ArgumentNullException(nameof(webDriver));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            AxeBuilder axeBuilder = new AxeBuilder(webDriver);
            return axeBuilder.Analyze(context);
        }

        public static AxeResult Analyze(this IWebDriver webDriver, IWebElement context, AxeBuilderOptions axeBuilderOptions)
        {
            if (webDriver == null)
                throw new ArgumentNullException(nameof(webDriver));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            AxeBuilder axeBuilder = new AxeBuilder(webDriver, axeBuilderOptions);
            return axeBuilder.Analyze(context);
        }
    }
}
