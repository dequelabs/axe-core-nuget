using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Represents the results from one specific axe rule across all the nodes from an axe scan.
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
        /// List of all elements the Rule evaluated to the same result for.
        /// </summary>
        public AxeResultNode[] Nodes { get; set; }

        // Mirrors the parent AxeResult, so an item and the result it came from serialize selectors the same way.
        internal bool ArraySelectors { get; set; }

        [OnSerializing]
        internal void OnSerializing(StreamingContext context) => SetSelectorsArraySelectors(ArraySelectors);

        [OnSerialized]
        internal void OnSerialized(StreamingContext context) => SetSelectorsArraySelectors(false);

        private void SetSelectorsArraySelectors(bool arraySelectors)
        {
            foreach (AxeSelector selector in Selectors().Where(selector => selector != null))
            {
                selector.ArraySelectors = arraySelectors;
            }
        }

        private IEnumerable<AxeSelector> Selectors()
        {
            foreach (AxeResultNode node in (Nodes ?? Array.Empty<AxeResultNode>()).Where(node => node != null))
            {
                yield return node.Target;
                yield return node.XPath;
                yield return node.Ancestry;

                IEnumerable<AxeResultCheck> checks = (node.Any ?? Array.Empty<AxeResultCheck>())
                    .Concat(node.All ?? Array.Empty<AxeResultCheck>())
                    .Concat(node.None ?? Array.Empty<AxeResultCheck>())
                    .Where(check => check != null);
                foreach (AxeResultCheck check in checks)
                {
                    foreach (AxeResultRelatedNode relatedNode in check.RelatedNodes ?? Array.Empty<AxeResultRelatedNode>())
                    {
                        yield return relatedNode?.Target;
                    }
                }
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, AxeJsonSerializerSettings.WithFormatting(Formatting.Indented, ArraySelectors));
        }
    }
}
