using System.Collections.Generic;
using Deucarian.Editor;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Logging.Editor
{
    /// <summary>
    /// Project Settings page for editing Deucarian logging defaults in the Unity Editor.
    /// </summary>
    public sealed class DeucarianLoggingSettingsProvider : SettingsProvider
    {
        /// <summary>
        /// Settings path shown in Unity Project Settings.
        /// </summary>
        public const string SettingsPath = "Project/Deucarian/Logging";

        /// <summary>
        /// Creates a settings provider instance.
        /// </summary>
        /// <param name="path">Settings path.</param>
        /// <param name="scope">Settings scope.</param>
        public DeucarianLoggingSettingsProvider(string path, SettingsScope scope)
            : base(path, scope)
        {
            keywords = new HashSet<string>
            {
                "Deucarian",
                "Logging",
                "DLog",
                "Minimum",
                "Timestamp",
                "Frame",
                "Prefix"
            };
        }

        /// <summary>
        /// Creates the Unity settings provider for Project Settings.
        /// </summary>
        /// <returns>The settings provider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new DeucarianLoggingSettingsProvider(SettingsPath, SettingsScope.Project);
        }

        /// <inheritdoc />
        public override void OnGUI(string searchContext)
        {
            using (DeucarianEditorWorkbenchPanelScope page =
                   DeucarianEditorWorkbenchGUI.BeginSettingsPage(GUILayout.ExpandHeight(true)))
            {
                // Package headers are intentionally disabled for now. Keep this call
                // ready for a future UI pass without removing the shared implementation.
                // DeucarianEditorChrome.DrawPackageHeader(
                //     "logging",
                //     "Deucarian Logging",
                //     "Configure editor defaults for the runtime logging dispatcher.");

                DeucarianEditorChrome.DrawSectionHeader("Runtime Settings");
                DeucarianEditorChrome.BeginSection();

                EditorGUI.BeginChangeCheck();
                Rect enabledRect = DeucarianEditorWorkbenchGUI.DrawLabeledField(
                    "Enabled",
                    "Enable or disable the runtime logging dispatcher.");
                bool enabled = EditorGUI.Toggle(
                    enabledRect,
                    DeucarianLoggingEditorSettings.Enabled);
                Rect levelRect = DeucarianEditorWorkbenchGUI.DrawLabeledField(
                    "Minimum Level",
                    "Messages below this severity are ignored.");
                DeucarianLogLevel minimumLevel = (DeucarianLogLevel)EditorGUI.EnumPopup(
                    levelRect,
                    DeucarianLoggingEditorSettings.MinimumLevel);
                Rect timestampRect = DeucarianEditorWorkbenchGUI.DrawLabeledField(
                    "Include Timestamp",
                    "Prefix log entries with a timestamp.");
                bool includeTimestamp = EditorGUI.Toggle(
                    timestampRect,
                    DeucarianLoggingEditorSettings.IncludeTimestamp);
                Rect frameRect = DeucarianEditorWorkbenchGUI.DrawLabeledField(
                    "Include Frame",
                    "Include the Unity frame number in log entries.");
                bool includeFrame = EditorGUI.Toggle(
                    frameRect,
                    DeucarianLoggingEditorSettings.IncludeFrame);
                Rect prefixRect = DeucarianEditorWorkbenchGUI.DrawLabeledField(
                    "Prefix",
                    "Text prepended to Deucarian log entries.");
                string prefix = EditorGUI.TextField(
                    prefixRect,
                    DeucarianLoggingEditorSettings.Prefix);

                if (EditorGUI.EndChangeCheck())
                {
                    DeucarianLoggingEditorSettings.SetValues(
                        enabled,
                        minimumLevel,
                        includeTimestamp,
                        includeFrame,
                        prefix);
                }

                EditorGUILayout.Space(6f);
                DeucarianEditorSettingsActions.DrawResetToDefaultsButton(
                    DeucarianLoggingEditorSettings.ResetToDefaults,
                    "Restore the package logging defaults.");

                DeucarianEditorChrome.EndSection();
                DeucarianEditorChrome.DrawFooterVersion("com.deucarian.logging", "1.0.1");
            }
        }
    }
}
