#nullable enable

using System.Collections.Generic;

namespace Playwright.Axe
{
    /// <summary>
    /// Information about other nodes related to a check.
    /// </summary>
    public sealed class AxeRelatedNode
    {
        /// <summary>
        /// List of selectors for the related node.
        /// </summary>
        public IList<string> Targets { get; } = new List<string>();

        /// <summary>
        /// HTML source of the related node.
        /// </summary>
        public string Html { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxeRelatedNode(IList<string> targets, string html)
        {
            Targets = targets;
            Html = html;
        }
    }
}
