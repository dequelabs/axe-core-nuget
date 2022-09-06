#nullable enable

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Rule Object Value.
    /// </summary>
    public sealed class AxeRuleObjectValue
    {
        /// <summary>
        /// Whether the rule is enabled or not.
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxeRuleObjectValue(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
