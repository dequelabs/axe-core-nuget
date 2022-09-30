namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Axe results object item
    /// </summary>
    public class AxeResultItem
    {
        /// <summary>
        /// Unique Identifier for the rule.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Text string that describes what the rule does.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Help text that describes the test that was performed.
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// URL that provides more information about the specifics of the violation. Links to a page on the Deque University site.
        /// </summary>
        public string HelpUrl { get; set; }

        /// <summary>
        /// How serious the violation is.
        /// </summary>
        public string Impact { get; set; }

        /// <summary>
        /// Array of tags that this rule is assigned.
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// List of all elements the Rule tested.
        /// </summary>
        public AxeResultNode[] Nodes { get; set; }
    }
}
