using Deque.AxeCore.Commons;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Deque.AxeCore.Selenium
{
    internal static class WebDriverInjectorExtensions
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Include
        };

        /// <summary>
        ///  Yeilds contexts such that every yield will be in a different frame context.
        ///  To be used to take actions in every frame on a page.
        /// </summary>
        /// <param name="driver">An initialized WebDriver</param>
        internal static IEnumerable<int> FrameContexts(this IWebDriver driver) {
            IList<IWebElement> parents = new List<IWebElement>();
            foreach (var x in FrameContexts(driver, parents)) {
                yield return x;
            }

            driver.SwitchTo().DefaultContent();
        }
        private static IEnumerable<int> FrameContexts(this IWebDriver driver, IList<IWebElement> parents)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            IList<IWebElement> frames = driver.FindElements(By.TagName("iframe"));

            foreach (var frame in frames)
            {
                driver.SwitchTo().DefaultContent();

                if (parents != null)
                {
                    foreach (IWebElement parent in parents)
                    {
                        driver.SwitchTo().Frame(parent);
                    }
                }

                driver.SwitchTo().Frame(frame);
                yield return 0;

                IList<IWebElement> localParents = parents.ToList();
                localParents.Add(frame);

                foreach (var x in FrameContexts(driver, localParents)) {
                    yield return x;
                }
            }
        }

        internal static object ExecuteScript(this IWebDriver driver, string script, params object[] args)
        {
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor) driver;
            return jsExecutor.ExecuteScript(script, args);
        }

        internal static object ExecuteAsyncScript(this IWebDriver driver, string script, params object[] args)
        {
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor) driver;
            return jsExecutor.ExecuteAsyncScript(script, args);
        }
    }
}
