#nullable enable

using System.Collections.Generic;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Axe Run context parameter for string CSS selectors.
    /// </summary>
    public sealed class AxeRunSerialContext : AxeRunContext
    {
        /// <summary>
        /// List of items to include.
        /// If not specified it includes the entire document by default.
        /// The last string items in the array will be the CSS selector,
        /// the preceding items are specifying nested frames.
        /// </summary>
        public IList<IList<string>>? Include { get; }

        /// <summary>
        /// List of items to include.
        /// The last string items in the array will be the CSS selector,
        /// the preceding items are specifying nested frames.
        /// </summary>
        public IList<IList<string>>? Exclude { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AxeRunSerialContext(string value) : this(value, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AxeRunSerialContext(string? include = null, string? exclude = null)
        {
            if (include != null)
            {
                Include = new List<IList<string>>()
                {
                    new List<string>()
                    {
                        include
                    }
                };
            }

            if (exclude != null)
            {
                Exclude = new List<IList<string>>()
                {
                    new List<string>()
                    {
                        exclude
                    }
                };
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AxeRunSerialContext(IList<string>? include = null, IList<string>? exclude = null)
        {
            if (include != null)
            {
                Include = new List<IList<string>>()
                {
                    include
                };
            }

            if (exclude != null)
            {
                Exclude = new List<IList<string>>()
                {
                    exclude
                };
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AxeRunSerialContext(IList<IList<string>>? include = null, IList<IList<string>>? exclude = null)
        {
            if (include != null)
            {
                Include = include;
            }

            if (exclude != null)
            {
                Exclude = exclude;
            }
        }
    }
}
