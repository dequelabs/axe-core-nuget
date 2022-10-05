namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// A tested rule element.
    /// </summary>
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

        /// <summary>
        /// Snippet of HTML of the Element.
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// How serious the violation is.
        /// </summary>
        public string Impact { get; set; }

        /// <summary>
        /// List of checks that were made where at least one must have passed.
        /// </summary>
        public AxeResultCheck[] Any { get; set; }

        /// <summary>
        /// List of checks that were made where all must have passed.
        /// </summary>
        public AxeResultCheck[] All { get; set; }

        /// <summary>
        /// List of checks that were made where all must have not passed.
        /// </summary>
        public AxeResultCheck[] None { get; set; }
    }
}
