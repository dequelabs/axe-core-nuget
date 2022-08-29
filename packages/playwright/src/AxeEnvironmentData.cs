#nullable enable

using System;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Data related to the environment that Axe executed in.
    /// </summary>
    public class AxeEnvironmentData
    {
        /// <summary>
        /// The application and version that ran the audit.
        /// </summary>
        public AxeTestEngine TestEngine { get; }

        /// <summary>
        /// The runner that ran the audit.
        /// </summary>
        public AxeTestRunner TestRunner { get; }

        /// <summary>
        /// Information about the current browser or node application that ran the audit.
        /// </summary>
        public AxeTestEnvironment TestEnvironment { get; }

        /// <summary>
        /// The URL of the page that was tested.
        /// </summary>
        public Uri Url { get; }

        /// <summary>
        /// The date and time that analysis was completed.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AxeEnvironmentData(
            AxeTestEngine testEngine,
            AxeTestRunner testRunner,
            AxeTestEnvironment testEnvironment,
            Uri url,
            DateTime timestamp
            )
        {
            TestEngine = testEngine;
            TestRunner = testRunner;
            TestEnvironment = testEnvironment;
            Url = url;
            Timestamp = timestamp;
        }
    }
}
