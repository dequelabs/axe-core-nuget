#nullable enable

namespace Playwright.Axe
{
    /// <summary>
    /// The application and version that ran the audit.
    /// </summary>
    public sealed class AxeTestEngine
    {
        /// <summary>
        /// The Name of the application.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The Version of the application.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Axe Test Engine Constructor
        /// </summary>
        public AxeTestEngine(
            string name,
            string version)
        {
            Name = name;
            Version = version;
        }
    }
}
