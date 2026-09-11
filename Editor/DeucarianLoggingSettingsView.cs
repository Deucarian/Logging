using System;
using Deucarian.Editor;
using UnityEngine.UIElements;

namespace Deucarian.Logging.Editor
{
    internal sealed class DeucarianLoggingSettingsView : IDisposable
    {
        private readonly DeucarianEditorWorkspaceForm form;
        private readonly DeucarianEditorFeatureSection output;
        private readonly Label example;
        private readonly DeucarianLogLevel[] levels =
            (DeucarianLogLevel[])Enum.GetValues(typeof(DeucarianLogLevel));
        private bool disposed;
        internal VisualElement Root { get; }

        internal DeucarianLoggingSettingsView(VisualElement parent)
        {
            Root = DeucarianEditorWorkspaceControls.Scroll("logging-settings-form");
            Root.AddToClassList("dw-feature-page");
            parent.Add(Root);
            output = new DeucarianEditorFeatureSection("runtime-logging", "Runtime logging",
                "Choose what appears in the Console.", DeucarianEditorIconIds.Document,
                value => Apply(enabled: value));
            output.Switch.name = "logging-enabled";
            output.Root.AddToClassList("dw-feature-unseparated");
            output.Actions.AddToClassList("dw-feature-actions-leading");
            Root.Add(output.Root);
            form = new DeucarianEditorWorkspaceForm(output.Details);
            form.Choice("logging-minimum-level", "Minimum level", Array.ConvertAll(levels, level => level.ToString()),
                () => Array.IndexOf(levels, DeucarianLoggingEditorSettings.MinimumLevel),
                index => Apply(level: levels[index]));
            form.Text("logging-prefix", "Prefix",
                () => DeucarianLoggingEditorSettings.Prefix, value => Apply(prefix: value)).tooltip =
                "Category prefix for sinks configured to include it. Unity Console uses the category by default.";
            output.Details.Add(DeucarianEditorWorkspaceControls.Divider());
            form.Toggle("logging-timestamp", "Timestamp",
                () => DeucarianLoggingEditorSettings.IncludeTimestamp, value => Apply(timestamp: value)).parent.AddToClassList("dw-switch-trailing");
            form.Toggle("logging-frame", "Frame number",
                () => DeucarianLoggingEditorSettings.IncludeFrame, value => Apply(frame: value)).parent.AddToClassList("dw-switch-trailing");
            output.Details.Add(DeucarianEditorWorkspaceControls.Divider());
            output.Details.Add(DeucarianEditorWorkspaceControls.Label("Example output", "dw-muted"));
            example = DeucarianEditorWorkspaceControls.Label(string.Empty, "dw-code-example");
            example.name = "logging-example";
            example.tooltip = "Illustrative Console output. This does not emit a log message.";
            output.Details.Add(example);
            var reset = DeucarianEditorWorkspaceControls.IconButton("Reset to defaults", DeucarianEditorIconIds.Reset, Reset);
            reset.name = "logging-reset";
            output.Actions.Add(reset);
            Root.RegisterCallback<AttachToPanelEvent>(OnAttached);
            Root.RegisterCallback<FocusInEvent>(OnFocus);
        }

        internal void Refresh()
        {
            if (disposed) return;
            form.Refresh();
            output.SetState(DeucarianLoggingEditorSettings.Enabled);
            example.text = DeucarianLoggingEditorSettings.MinimumLevel == DeucarianLogLevel.None
                ? "All message levels are filtered out."
                : (DeucarianLoggingEditorSettings.IncludeTimestamp ? "[2026-01-01 12:00:00.000 UTC] " : "")
                  + (DeucarianLoggingEditorSettings.IncludeFrame ? "[Frame 120] " : "")
                  + "[Example] Your message appears here.";
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
