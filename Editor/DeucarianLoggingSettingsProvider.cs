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
            DeucarianEditorChrome.DrawPackageHeader(
                "logging",
                "Deucarian Logging",
                "Configure editor defaults for the runtime logging dispatcher.");

            DeucarianEditorChrome.DrawSectionHeader("Runtime Settings");
            DeucarianEditorChrome.BeginSection();

            EditorGUI.BeginChangeCheck();
            bool enabled = EditorGUILayout.Toggle("Enabled", DeucarianLoggingEditorSettings.Enabled);
            DeucarianLogLevel minimumLevel = (DeucarianLogLevel)EditorGUILayout.EnumPopup(
                "Minimum Level",
                DeucarianLoggingEditorSettings.MinimumLevel);
            bool includeTimestamp = EditorGUILayout.Toggle(
                "Include Timestamp",
                DeucarianLoggingEditorSettings.IncludeTimestamp);
            bool includeFrame = EditorGUILayout.Toggle(
                "Include Frame",
                DeucarianLoggingEditorSettings.IncludeFrame);
            string prefix = EditorGUILayout.TextField("Prefix", DeucarianLoggingEditorSettings.Prefix);

            if (EditorGUI.EndChangeCheck())
            {
                DeucarianLoggingEditorSettings.SetValues(
                    enabled,
                    minimumLevel,
                    includeTimestamp,
                    includeFrame,
                    prefix);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset to Defaults"))
            {
                DeucarianLoggingEditorSettings.ResetToDefaults();
            }

            DeucarianEditorChrome.EndSection();
            DeucarianEditorChrome.DrawFooterVersion("com.deucarian.logging", "1.0.2");
        }
    }
}
