namespace Deque.AxeCore.Commons
{
    public class AxeResultRelatedNode
    {
        public string Html { get; set; }

        /// <summary>
        /// Represents a CSS selector (or, for elements inside iframes/shadow DOMs, a chain of CSS selectors) which uniquely
        /// identifies the node in question on the page.
        /// </summary>
        public AxeSelector Target { get; set; }
    }
}
