using UnityEditor;

namespace Deucarian.Logging.Editor
{
    /// <summary>
    /// Editor-backed settings bridge for Deucarian logging.
    /// </summary>
    [InitializeOnLoad]
    public static class DeucarianLoggingEditorSettings
    {
        /// <summary>
        /// Default enabled state used by the editor settings page.
        /// </summary>
        public const bool DefaultEnabled = true;

        /// <summary>
        /// Default minimum log level used in the Unity Editor.
        /// </summary>
        public const DeucarianLogLevel DefaultMinimumLevel = DeucarianLogLevel.Debug;

        /// <summary>
        /// Default timestamp visibility used by the editor settings page.
        /// </summary>
        public const bool DefaultIncludeTimestamp = false;

        /// <summary>
        /// Default frame visibility used by the editor settings page.
        /// </summary>
        public const bool DefaultIncludeFrame = false;

        /// <summary>
        /// Default category prefix used by the editor settings page.
        /// </summary>
        public const string DefaultPrefix = "Deucarian";

        private const string KeyPrefix = "Deucarian.Logging.";
        private const string EnabledKey = KeyPrefix + "Enabled";
        private const string MinimumLevelKey = KeyPrefix + "MinimumLevel";
        private const string IncludeTimestampKey = KeyPrefix + "IncludeTimestamp";
        private const string IncludeFrameKey = KeyPrefix + "IncludeFrame";
        private const string PrefixKey = KeyPrefix + "Prefix";

        static DeucarianLoggingEditorSettings()
        {
            ApplyToRuntime();
        }

        /// <summary>
        /// Gets the editor-persisted enabled state.
        /// </summary>
        public static bool Enabled => EditorPrefs.GetBool(EnabledKey, DefaultEnabled);

        /// <summary>
        /// Gets the editor-persisted minimum log level.
        /// </summary>
        public static DeucarianLogLevel MinimumLevel =>
            (DeucarianLogLevel)EditorPrefs.GetInt(MinimumLevelKey, (int)DefaultMinimumLevel);

        /// <summary>
        /// Gets whether timestamps are included in editor console output.
        /// </summary>
        public static bool IncludeTimestamp => EditorPrefs.GetBool(IncludeTimestampKey, DefaultIncludeTimestamp);

        /// <summary>
        /// Gets whether frame numbers are included in editor console output.
        /// </summary>
        public static bool IncludeFrame => EditorPrefs.GetBool(IncludeFrameKey, DefaultIncludeFrame);

        /// <summary>
        /// Gets the editor-persisted category prefix.
        /// </summary>
        public static string Prefix => EditorPrefs.GetString(PrefixKey, DefaultPrefix);

        /// <summary>
        /// Persists editor settings and applies them to runtime static settings.
        /// </summary>
        public static void SetValues(
            bool enabled,
            DeucarianLogLevel minimumLevel,
            bool includeTimestamp,
            bool includeFrame,
            string prefix)
        {
            EditorPrefs.SetBool(EnabledKey, enabled);
            EditorPrefs.SetInt(MinimumLevelKey, (int)minimumLevel);
            EditorPrefs.SetBool(IncludeTimestampKey, includeTimestamp);
            EditorPrefs.SetBool(IncludeFrameKey, includeFrame);
            EditorPrefs.SetString(PrefixKey, prefix ?? string.Empty);
            ApplyToRuntime();
        }

        /// <summary>
        /// Restores editor logging settings to their defaults.
        /// </summary>
        public static void ResetToDefaults()
        {
            SetValues(
                DefaultEnabled,
                DefaultMinimumLevel,
                DefaultIncludeTimestamp,
                DefaultIncludeFrame,
                DefaultPrefix);
        }

        /// <summary>
        /// Applies editor-persisted settings to the runtime static settings.
        /// </summary>
        public static void ApplyToRuntime()
        {
            DeucarianLogSettings.Enabled = Enabled;
            DeucarianLogSettings.MinimumLevel = MinimumLevel;
            DeucarianLogSettings.IncludeTimestamp = IncludeTimestamp;
            DeucarianLogSettings.IncludeFrame = IncludeFrame;
            DeucarianLogSettings.Prefix = Prefix;
        }
    }
}
