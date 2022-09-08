#nullable enable

using System.Collections.Generic;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Options for configuring a single Axe run.
    /// </summary>
    public sealed class AxeRunOptions
    {
        /// <summary>
        /// Limit which rules are executed, based on names or tags.
        /// </summary>
        public AxeRunOnly? RunOnly { get; }

        /// <summary>
        /// Enable or disable rules.
        /// </summary>
        public IDictionary<string, AxeRuleObjectValue>? Rules { get; }

        /// <summary>
        /// Limit which result types are processed and aggregated.
        /// </summary>
        public IList<AxeResultGroup>? ResultTypes { get; }

        /// <summary>
        /// Return CSS selector for elements, optimised for readability.
        /// </summary>
        public bool? Selectors { get; }

        /// <summary>
        /// Return CSS selector for elements, with all the element's ancestors.
        /// </summary>
        public bool? Ancestry { get; }

        /// <summary>
        /// Return xpath selectors for elements.
        /// </summary>
        public bool? Xpath { get; }

        /// <summary>
        /// Use absolute paths when creating element selectors.
        /// </summary>
        public bool? AbsolutePaths { get; }

        /// <summary>
        /// Tell axe to run inside iframes.
        /// </summary>
        public bool? Iframes { get; }

        /// <summary>
        /// Return element references in addition to the target.
        /// </summary>
        public bool? ElementRef { get; }

        /// <summary>
        /// How long (in milliseconds) axe waits for a response from embedded frames before timing out.
        /// </summary>
        public int? FrameWaitTime { get; }

        /// <summary>
        /// Log rule performance metrics to the console.
        /// </summary>
        public bool? PerformanceTimer { get; }

        /// <summary>
        /// Time before axe-core considers a frame unresponsive.
        /// </summary>
        public int? PingWaitTime { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AxeRunOptions(
            AxeRunOnly? runOnly = null,
            IDictionary<string, AxeRuleObjectValue>? rules = null,
            IList<AxeResultGroup>? resultTypes = null,
            bool? selectors = null,
            bool? ancestry = null,
            bool? xpath = null,
            bool? absolutePaths = null,
            bool? iframes = null,
            bool? elementRef = null,
            int? frameWaitTime = null,
            bool? performanceTimer = null,
            int? pingWaitTime = null
            )
        {
            RunOnly = runOnly;
            Rules = rules;
            ResultTypes = resultTypes;
            Selectors = selectors;
            Ancestry = ancestry;
            Xpath = xpath;
            AbsolutePaths = absolutePaths;
            Iframes = iframes;
            ElementRef = elementRef;
            FrameWaitTime = frameWaitTime;
            PerformanceTimer = performanceTimer;
            PingWaitTime = pingWaitTime;
        }
    }
}
