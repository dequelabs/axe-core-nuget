namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Information about other nodes related to a check.
    /// </summary>
    public class AxeResultRelatedNode
    {
        /// <summary>
        /// HTML source of the related node.
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// Represents a CSS selector (or, for elements inside iframes/shadow DOMs, a chain of CSS selectors) which uniquely
        /// identifies the related node in question on the page.
        /// </summary>
        public AxeSelector Target { get; set; }
    }
}
