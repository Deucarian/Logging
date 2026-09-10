using System.Collections.Generic;
using Deucarian.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Deucarian.Logging.Editor
{
    /// <summary>Unity Project Settings adapter for the shared Logging form.</summary>
    public sealed class DeucarianLoggingSettingsProvider : SettingsProvider
    {
        /// <summary>Settings path shown in Unity Project Settings.</summary>
        public const string SettingsPath = "Project/Deucarian/Logging";
        private DeucarianEditorWorkbench workbench;
        private DeucarianLoggingSettingsView view;
        private VisualElement host;

        /// <summary>Creates the editor settings provider.</summary>
        public DeucarianLoggingSettingsProvider(string path, SettingsScope scope) : base(path, scope)
        {
            keywords = new HashSet<string> {
                "Deucarian", "Logging", "DLog", "Minimum", "Timestamp", "Frame", "Prefix"
            };
        }

        /// <summary>Creates the Unity Project Settings entry.</summary>
        [SettingsProvider]
        public static SettingsProvider CreateProvider() =>
            new DeucarianLoggingSettingsProvider(SettingsPath, SettingsScope.Project);

        /// <inheritdoc />
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            OnDeactivate();
            base.OnActivate(searchContext, rootElement);
            host = new VisualElement();
            rootElement.Add(host);
            host.AddToClassList("deucarian-workspace-host");
            DeucarianEditorUIResources.TryAddStyleSheet(host, DeucarianEditorWorkspace.StyleSheetPath);
            workbench = DeucarianEditorWorkbench.Create(host, new DeucarianEditorWorkbenchOptions {
                IncludeToolbar = false
            });
            workbench.ShellContent.AddToClassList("deucarian-workspace");
            workbench.Content.AddToClassList("dw-page");
            view = new DeucarianLoggingSettingsView(workbench.Content);
        }

        /// <inheritdoc />
        public override void OnDeactivate()
        {
            view?.Dispose();
            view = null;
            workbench?.Dispose();
            workbench = null;
            host?.RemoveFromHierarchy();
            host = null;
            base.OnDeactivate();
        }
    }
}
