#nullable enable

using System;
using System.Collections.Generic;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Axe Rule Metadata
    /// </summary>
    public sealed class AxeRuleMetadata 
    {
        /// <summary>
        /// The Rule Id e.g. "color-contrast"
        /// </summary>
        public string RuleId { get; }

        /// <summary>
        /// Text string that describes what the rule does.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Help text that describes the test that was performed.
        /// </summary>
        public string Help { get; }

        /// <summary>
        /// URL that provides more information about the specifics of the violation. Links to a page on the Deque University site.
        /// </summary>
        public Uri HelpUrl { get; }

        /// <summary>
        /// Array of tags that this rule is assigned.
        /// </summary>
        public IList<string> Tags { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxeRuleMetadata(
            string ruleId,
            string description,
            string help,
            Uri helpUrl,
            IList<string> tags)
        {
            RuleId = ruleId;
            Description = description;
            Help = help;
            HelpUrl = helpUrl;
            Tags = tags;
        }
    }
}
