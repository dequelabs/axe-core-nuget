#nullable enable

namespace Playwright.Axe
{
    /// <summary>
    /// Axe Test Runner
    /// </summary>
    public sealed class AxeTestRunner
    {
        /// <summary>
        /// Test Runner Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Axe Test Runner Constructor
        /// </summary>
        public AxeTestRunner(string name)
        {
            Name = name;
        }
    }
}
