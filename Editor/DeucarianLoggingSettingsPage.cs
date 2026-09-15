using Deucarian.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Deucarian.Logging.Editor
{
    internal static class DeucarianLoggingSettingsPage
    {
        internal static IDeucarianEditorPage Create()
        {
            var root = new VisualElement();
            var workspace = new DeucarianEditorWorkspace(root, Application.productName);
            workspace.Title.text = "Logging";
            workspace.Subtitle.text = "Keep the Console useful.";
            DeucarianEditorWorkspaceNavigation.Populate(workspace, DeucarianToolIds.LoggingSettings);
            DeucarianEditorWorkspaceControls.Show(workspace.Tabs, false);
            DeucarianEditorWorkspaceControls.Show(workspace.Scope, false);
            var view = new DeucarianLoggingSettingsView(workspace.Content);
            return new DeucarianEditorPage(root, activate: _ => view.Refresh(),
                dispose: () => { view.Dispose(); workspace.Dispose(); });
        }
    }
}
