#nullable enable

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Information about the current browser or node application that ran the audit.
    /// </summary>
    public sealed class AxeTestEnvironment
    {
        /// <summary>
        /// User Agent
        /// </summary>
        public string UserAgent { get; }

        /// <summary>
        /// Window Width.
        /// </summary>
        public int WindowWidth { get; }

        /// <summary>
        /// Window Height.
        /// </summary>
        public int WindowHeight { get; }

        /// <summary>
        /// Orientation Angle.
        /// </summary>
        public int OrientationAngle { get; }

        /// <summary>
        /// Orientation Type.
        /// </summary>
        public string OrientationType { get; }

        /// <summary>
        /// Axe Test Environment Constructor.
        /// </summary>
        public AxeTestEnvironment(
            string userAgent,
            int windowWidth,
            int windowHeight,
            int orientationAngle,
            string orientationType)
        {
            UserAgent = userAgent;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            OrientationAngle = orientationAngle;
            OrientationType = orientationType;
        }
    }
}
