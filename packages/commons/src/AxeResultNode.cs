namespace Deque.AxeCore.Commons
{
    public class AxeResultNode
    {
        /// <summary>
        /// Represents a CSS selector (or, for elements inside iframes/shadow DOMs, a chain of CSS selectors) which uniquely
        /// identifies the node in question on the page.
        /// </summary>
        public AxeSelector Target { get; set; }

        /// <summary>
        /// Only present if the XPath flag was passed in <see cref="AxeRunOptions"/>. This <see cref="AxeSelector"/> is built up
        /// of XPath selectors rather than CSS selectors, but otherwise follows the same structure as a typical AxeSelector.
        /// </summary>
        public AxeSelector XPath { get; set; }

        /// <summary>
        /// Only present if the Ancestry flag was passed in <see cref="AxeRunOptions"/>. Represents a CSS selector (or, for
        /// elements inside iframes/shadow DOMs, a chain of CSS selectors) which uniquely identifies the node in question on
        /// the page. Includes all the element's ancestors, usually more verbose than <see cref="Target"/>.
        /// </summary>
        public AxeSelector Ancestry { get; set; }
        public string Html { get; set; }
        public string Impact { get; set; }
        public AxeResultCheck[] Any { get; set; }
        public AxeResultCheck[] All { get; set; }
        public AxeResultCheck[] None { get; set; }
    }
}
