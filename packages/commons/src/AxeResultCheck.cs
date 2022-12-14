namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Represents the results from one specific axe check on one specific node. The results of an axe rule may be based on the results of multiple checks.
    /// </summary>
    public class AxeResultCheck
    {
        /// <summary>
        /// Additional information that is specific to the type of Check which is optional.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Unique Identifier for this check.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// How serious this particular check is.
        /// </summary>
        public string Impact { get; set; }

        /// <summary>
        /// Description of why this check passed or failed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Array of information about other nodes that are related to this check.
        /// </summary>
        public AxeResultRelatedNode[] RelatedNodes { get; set; }
    }
}
