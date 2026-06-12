namespace Deucarian.Logging
{
    /// <summary>
    /// Global runtime settings used by the Deucarian logging dispatcher and default console sink.
    /// </summary>
    public static class DeucarianLogSettings
    {
        /// <summary>
        /// Gets or sets whether logging is globally enabled.
        /// </summary>
        public static bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the minimum severity that will be dispatched.
        /// </summary>
        public static DeucarianLogLevel MinimumLevel { get; set; }

        /// <summary>
        /// Gets or sets whether Unity console messages include UTC timestamps.
        /// </summary>
        public static bool IncludeTimestamp { get; set; }

        /// <summary>
        /// Gets or sets whether Unity console messages include the current Unity frame.
        /// </summary>
        public static bool IncludeFrame { get; set; }

        /// <summary>
        /// Gets or sets the prefix used in Unity console category labels.
        /// </summary>
        public static string Prefix { get; set; }

        static DeucarianLogSettings()
        {
            ResetToDefaults();
        }

        /// <summary>
        /// Restores sensible defaults for the current build target.
        /// </summary>
        public static void ResetToDefaults()
        {
            Enabled = true;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            MinimumLevel = DeucarianLogLevel.Debug;
#else
            MinimumLevel = DeucarianLogLevel.Warning;
#endif
            IncludeTimestamp = false;
            IncludeFrame = false;
            Prefix = "Deucarian";
        }
    }
}
