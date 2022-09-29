namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Information about the current browser or node application that ran the audit.
    /// </summary>
    public class AxeTestEnvironment
    {
        /// <summary>
        /// User Agent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Window Width.
        /// </summary>
        public int WindowWidth { get; set; }

        /// <summary>
        /// Window Height.
        /// </summary>
        public int WindowHeight { get; set; }

        /// <summary>
        /// Orientation Type.
        /// </summary>
        public string OrientationType { get; set; }

        /// <summary>
        /// Orientation Angle.
        /// </summary>
        public double? OrientationAngle { get; set; }
    }
}
