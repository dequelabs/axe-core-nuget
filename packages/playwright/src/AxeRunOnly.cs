#nullable enable

using System.Collections.Generic;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Limits which rules are executed, based on names or tags.
    /// </summary>
    public sealed class AxeRunOnly
    {
        /// <summary>
        /// The type of value to restrict rules executed on.
        /// </summary>
        public AxeRunOnlyType Type { get; }

        /// <summary>
        /// The values to restrict rules on.
        /// </summary>
        public IList<string> Values { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxeRunOnly(AxeRunOnlyType type, IList<string> values)
        {
            Type = type;
            Values = values;
        }
    }
}
