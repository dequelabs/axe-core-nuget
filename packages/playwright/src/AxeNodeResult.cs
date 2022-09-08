#nullable enable

using System.Collections.Generic;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// A tested rule element.
    /// </summary>
    public sealed class AxeNodeResult
    {
        /// <summary>
        /// Snippet of HTML of the Element.
        /// </summary>
        public string Html { get; }

        /// <summary>
        /// How serious the violation is.
        /// </summary>
        public AxeImpactValue? Impact { get; }

        /// <summary>
        /// Target identifier of this result node.
        /// </summary>
        public IList<string> Target { get; }

        /// <summary>
        /// Xpath selectors for elements
        /// </summary>
        public IList<string>? XPath { get; }

        /// <summary>
        /// Elements ancestry.
        /// </summary>
        public IList<string>? Ancestry { get; }

        /// <summary>
        /// List of checks that were made where at least one must have passed.
        /// </summary>
        public IList<AxeCheckResult> Any { get; } = new List<AxeCheckResult>();

        /// <summary>
        /// List of checks that were made where all must have passed.
        /// </summary>
        public IList<AxeCheckResult> All { get; } = new List<AxeCheckResult>();

        /// <summary>
        /// List of checks that were made where all must have not passed.
        /// </summary>
        public IList<AxeCheckResult> None { get; } = new List<AxeCheckResult>();

        /// <summary>
        /// Failure summary.
        /// </summary>
        public string? FailureSummary { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxeNodeResult(
            string html,
            AxeImpactValue? impact,
            IList<string>? target,
            IList<string>? xpath,
            IList<string>? ancestry,
            IList<AxeCheckResult> any,
            IList<AxeCheckResult> all,
            IList<AxeCheckResult> none,
            string? failureSummary
            )
        {
            Html = html;
            Impact = impact;
            Target = target ?? new List<string>();
            XPath = xpath;
            Ancestry = ancestry;
            Any = any;
            All = all;
            None = none;
            FailureSummary = failureSummary;
        }
    }
}
