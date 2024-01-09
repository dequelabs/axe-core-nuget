using System.Collections.Generic;
using System;
using System.Linq;
using NUnit.Framework;
using Deque.AxeCore.Commons;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class IncludeExcludeTests : TestBase
    {
        private List<string> getPassTargets(AxeResult axeResults)
        {
            return axeResults.Passes.SelectMany(p => p.Nodes.Select(n => n.Target.ToString())).ToList();
        }
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldHonorInclude(string browser)
        {
            InitDriver(browser);
            GoToResource("context.html");

            var axe = new AxeBuilder(WebDriver)
                .Include(".include");
            var include1Res = axe.Analyze();

            var axe2 = new AxeBuilder(WebDriver)
                .Include(".include")
                .Include(".include2");
            var include2Res = axe2.Analyze();

            var targets2 = getPassTargets(include2Res);
            var targets1 = getPassTargets(include1Res);
            Assert.IsNotNull(targets1.Find(t => t.Contains(".include")));
            Assert.IsNotNull(targets2.Find(t => t.Contains(".include")));
            Assert.IsNotNull(targets2.Find(t => t.Contains(".include2")));
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldHonorExclude(string browser)
        {
            InitDriver(browser);
            GoToResource("context.html");

            var axe = new AxeBuilder(WebDriver)
                .Exclude(".exclude");
            var exclude1Res = axe.Analyze();

            var axe2 = new AxeBuilder(WebDriver)
                .Exclude(".exclude")
                .Exclude(".exclude2");
            var exclude2Res = axe2.Analyze();

            var targets2 = getPassTargets(exclude2Res);
            var targets1 = getPassTargets(exclude1Res);
            Assert.IsNull(targets1.Find(t => t.Equals(".exclude")));
            Assert.IsNull(targets2.Find(t => t.Equals(".exclude")));
            Assert.IsNull(targets2.Find(t => t.Equals(".exclude2")));
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldHonorIncludeExclude(string browser)
        {
            InitDriver(browser);
            GoToResource("context.html");

            var axe = new AxeBuilder(WebDriver)
                .Include(".include")
                .Include(".include2")
                .Exclude(".exclude")
                .Exclude(".exclude2");
            var res = axe.Analyze();
            var targets = getPassTargets(res);

            Assert.IsNotNull(targets.Find(t => t.Equals(".include")));
            Assert.IsNotNull(targets.Find(t => t.Equals(".include2")));
            Assert.IsNull(targets.Find(t => t.Equals(".exclude")));
            Assert.IsNull(targets.Find(t => t.Equals(".exclude2")));
        }
    }
}
