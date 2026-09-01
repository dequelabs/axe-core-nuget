using Deque.AxeCore.Commons;

namespace Deque.AxeCore.Selenium
{
    public class AxeBuilderOptions
    {
        public IAxeScriptProvider ScriptProvider { get; set; }

        /// <summary>
        /// Write <see cref="AxeResultNode.Target"/>, <see cref="AxeResultNode.XPath"/> and <see cref="AxeResultNode.Ancestry"/>
        /// as arrays in all cases when serializing the results, matching the shape axe-core itself emits. Without this, a
        /// selector which involves no iframes or shadow DOMs is written as a bare string. Becomes the default in v5.
        /// </summary>
        public bool ArraySelectors { get; set; }
    }
}
