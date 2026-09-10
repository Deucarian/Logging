using System;
using Deucarian.Editor;
using UnityEngine.UIElements;

namespace Deucarian.Logging.Editor
{
    internal sealed class DeucarianLoggingSettingsView : IDisposable
    {
        private readonly DeucarianEditorWorkspaceForm form;
        private readonly DeucarianLogLevel[] levels =
            (DeucarianLogLevel[])Enum.GetValues(typeof(DeucarianLogLevel));
        private bool disposed;
        internal VisualElement Root { get; }

        internal DeucarianLoggingSettingsView(VisualElement parent)
        {
            Root = DeucarianEditorWorkspaceControls.Scroll("logging-settings-form");
            Root.AddToClassList("dw-settings-page");
            parent.Add(Root);
            form = new DeucarianEditorWorkspaceForm(Root);

            var output = form.Section("Output");
            output.Toggle("logging-enabled", "Enable logging",
                () => DeucarianLoggingEditorSettings.Enabled, value => Apply(enabled: value));
            output.Choice("logging-minimum-level", "Minimum level", Array.ConvertAll(levels, level => level.ToString()),
                () => Array.IndexOf(levels, DeucarianLoggingEditorSettings.MinimumLevel),
                index => Apply(level: levels[index]));
            output.Note(() => !DeucarianLoggingEditorSettings.Enabled ? "Logging is turned off." :
                DeucarianLoggingEditorSettings.MinimumLevel == DeucarianLogLevel.None ? "All message levels are filtered out." :
                "Messages at this level and above are shown.");

            var formatting = form.Section("Formatting");
            formatting.Toggle("logging-timestamp", "Include timestamp",
                () => DeucarianLoggingEditorSettings.IncludeTimestamp, value => Apply(timestamp: value));
            formatting.Toggle("logging-frame", "Include frame number",
                () => DeucarianLoggingEditorSettings.IncludeFrame, value => Apply(frame: value));
            formatting.Text("logging-prefix", "Category prefix",
                () => DeucarianLoggingEditorSettings.Prefix, value => Apply(prefix: value)).tooltip =
                "Used by log sinks that include a category prefix.";

            form.Action("logging-reset", DeucarianEditorSettingsActions.ResetToDefaultsLabel, Reset);
            form.Note(() => "Saved automatically · Local editor preferences");
            Root.RegisterCallback<AttachToPanelEvent>(OnAttached);
            Root.RegisterCallback<FocusInEvent>(OnFocus);
        }

        internal void Refresh()
        {
            if (!disposed) form.Refresh();
        }

        private void Apply(bool? enabled = null, DeucarianLogLevel? level = null,
            bool? timestamp = null, bool? frame = null, string prefix = null)
        {
            if (disposed) return;
            DeucarianLoggingEditorSettings.SetValues(
                enabled ?? DeucarianLoggingEditorSettings.Enabled,
                level ?? DeucarianLoggingEditorSettings.MinimumLevel,
                timestamp ?? DeucarianLoggingEditorSettings.IncludeTimestamp,
                frame ?? DeucarianLoggingEditorSettings.IncludeFrame,
                prefix ?? DeucarianLoggingEditorSettings.Prefix);
            Refresh();
        }

        private void Reset()
        {
            if (disposed) return;
            DeucarianLoggingEditorSettings.ResetToDefaults();
            Refresh();
        }

        private void OnAttached(AttachToPanelEvent evt) => Refresh();
        private void OnFocus(FocusInEvent evt) => Refresh();

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            Root.UnregisterCallback<AttachToPanelEvent>(OnAttached);
            Root.UnregisterCallback<FocusInEvent>(OnFocus);
            Root.RemoveFromHierarchy();
        }
    }
}
