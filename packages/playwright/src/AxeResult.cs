#nullable enable

using System;
using System.Collections.Generic;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Axe Result.
    /// </summary>
    public sealed class AxeResult
    {
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
        /// Unique Identifier for the rule.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// How serious the violation is.
        /// </summary>
        public AxeImpactValue? Impact { get; }

        /// <summary>
        /// Array of tags that this rule is assigned.
        /// </summary>
        public IList<string> Tags { get; }

        /// <summary>
        /// List of all elements the Rule tested.
        /// </summary>
        public IList<AxeNodeResult> Nodes { get; }

        /// <inheritdoc />
        public override string ToString() => Id;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxeResult(
            string description,
            string help,
            Uri helpUrl,
            string id,
            AxeImpactValue? impact,
            IList<string>? tags,
            IList<AxeNodeResult>? nodes)
        {
            Description = description;
            Help = help;
            HelpUrl = helpUrl;
            Id = id;
            Impact = impact;
            Tags = tags ?? new List<string>();
            Nodes = nodes ?? new List<AxeNodeResult>();
        }
    }
}
