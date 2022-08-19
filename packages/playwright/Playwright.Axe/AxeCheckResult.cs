#nullable enable

using System.Collections.Generic;

namespace Playwright.Axe
{
    /// <summary>
    /// A Check Result.
    /// </summary>
    public sealed class AxeCheckResult
    {
        /// <summary>
        /// Unique Identifier for this check.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// How serious this particular.
        /// </summary>
        public string Impact { get; }

        /// <summary>
        /// Description of why this check passed or failed.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Additional information that is specific to the type of Check which is optional.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// List of information about other nodes that are related to this check.
        /// </summary>
        public IList<AxeRelatedNode>? RelatedNodes { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxeCheckResult(
            string id,
            string impact,
            string message,
            object data,
            IList<AxeRelatedNode>? relatedNodes)
        {
            Id = id;
            Impact = impact;
            Message = message;
            Data = data;
            RelatedNodes = relatedNodes;
        }
    }
}
