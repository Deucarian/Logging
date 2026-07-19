using System;
using UnityEditor;

namespace Deucarian.Logging.Editor
{
    /// <summary>
    /// Unity editor menu items for Deucarian logging.
    /// </summary>
    public static class DeucarianLoggingMenu
    {
        private const string MenuRoot = "Tools/Deucarian/Infrastructure/Logging/";
        private static readonly DLog EditorLog = DLog.For("Logging.Editor");

        /// <summary>
        /// Opens the Deucarian logging page in Project Settings.
        /// </summary>
        [MenuItem(MenuRoot + "Open Logging Settings")]
        public static void OpenLoggingSettings()
        {
            SettingsService.OpenProjectSettings(DeucarianLoggingSettingsProvider.SettingsPath);
        }

        /// <summary>
        /// Restores editor logging settings to their defaults.
        /// </summary>
        [MenuItem(MenuRoot + "Reset Logging Settings")]
        public static void ResetLoggingSettings()
        {
            DeucarianLoggingEditorSettings.ResetToDefaults();
            EditorLog.Info("[Deucarian.Logging] Logging settings reset.");
        }

        /// <summary>
        /// Emits one example message per log level through the Deucarian logger.
        /// </summary>
        [MenuItem(MenuRoot + "Test Log Messages")]
        public static void TestLogMessages()
        {
            bool previousEnabled = DeucarianLogSettings.Enabled;
            DeucarianLogLevel previousMinimumLevel = DeucarianLogSettings.MinimumLevel;

            try
            {
                DeucarianLogSettings.Enabled = true;
                DeucarianLogSettings.MinimumLevel = DeucarianLogLevel.Trace;

                DLog log = DLog.For("Editor.Test");
                log.Trace("Trace test message.");
                log.Debug("Debug test message.");
                log.Info("Info test message.");
                log.Warning("Warning test message.");
                log.Error("Error test message.");

                var exception = new InvalidOperationException("Exception test message.");
                log.Exception(exception, "Exception test message.");
            }
            finally
            {
                DeucarianLogSettings.Enabled = previousEnabled;
                DeucarianLogSettings.MinimumLevel = previousMinimumLevel;
            }
        }
    }
}
