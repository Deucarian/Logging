using System.Collections.Generic;
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
            EditorGUILayout.LabelField("Runtime Settings", EditorStyles.boldLabel);

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
        }
    }
}
