#nullable enable

using System;
using System.Collections.Generic;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Axe Results.
    /// </summary>
    public sealed class AxeResults : AxeEnvironmentData
    {
        /// <summary>
        /// These results indicate what elements passed the rules.
        /// </summary>
        public IList<AxeResult> Passes { get; }

        /// <summary>
        /// These results indicate what elements failed the rules.
        /// </summary>
        public IList<AxeResult> Violations { get; }

        /// <summary>
        /// These results indicate which rules did not run because no matching content was found on the page. 
        /// For example, with no video, those rules won't run.
        /// </summary>
        public IList<AxeResult> Incomplete { get; }

        /// <summary>
        /// These results were aborted and require further testing. 
        /// This can happen either because of technical restrictions to what the rule can test, or because a javascript error occurred.
        /// </summary>
        public IList<AxeResult> Inapplicable { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxeResults(
            AxeTestEngine testEngine,
            AxeTestRunner testRunner,
            AxeTestEnvironment testEnvironment,
            Uri url,
            DateTime timestamp,
            IList<AxeResult>? passes,
            IList<AxeResult>? violations,
            IList<AxeResult>? incomplete,
            IList<AxeResult>? inapplicable)
            : base(
              testEngine,
              testRunner,
              testEnvironment,
              url,
              timestamp)
        {
            Passes = passes ?? new List<AxeResult>();
            Violations = violations ?? new List<AxeResult>();
            Incomplete = incomplete ?? new List<AxeResult>();
            Inapplicable = inapplicable ?? new List<AxeResult>();
        }
    }
}
