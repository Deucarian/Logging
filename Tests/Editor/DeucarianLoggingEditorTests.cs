using System.Collections.Generic;
using System.Collections;
using Deucarian.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Deucarian.Logging.Editor.Tests
{
    public sealed class DeucarianLoggingEditorTests
    {
        private SettingsSnapshot snapshot;

        [SetUp]
        public void SetUp()
        {
            snapshot = SettingsSnapshot.Capture();
        }

        [TearDown]
        public void TearDown()
        {
            snapshot.Restore();
            DeucarianLog.ResetSinksToDefault();
        }

        [Test]
        public void EditorUtilityExposesPackageName()
        {
            Assert.AreEqual("com.deucarian.logging", DeucarianLoggingEditorUtility.PackageName);
        }

        [Test]
        public void SettingsResetSetsExpectedDefaults()
        {
            DeucarianLoggingEditorSettings.SetValues(
                false,
                DeucarianLogLevel.Error,
                true,
                true,
                "Custom");

            DeucarianLoggingEditorSettings.ResetToDefaults();

            Assert.IsTrue(DeucarianLoggingEditorSettings.Enabled);
            Assert.AreEqual(DeucarianLogLevel.Debug, DeucarianLoggingEditorSettings.MinimumLevel);
            Assert.IsFalse(DeucarianLoggingEditorSettings.IncludeTimestamp);
            Assert.IsFalse(DeucarianLoggingEditorSettings.IncludeFrame);
            Assert.AreEqual("Deucarian", DeucarianLoggingEditorSettings.Prefix);
            Assert.IsTrue(DeucarianLogSettings.Enabled);
            Assert.AreEqual(DeucarianLogLevel.Debug, DeucarianLogSettings.MinimumLevel);
        }

        [Test]
        public void SettingsProviderCanBeCreated()
        {
            SettingsProvider provider = null;

            Assert.DoesNotThrow(() => provider = DeucarianLoggingSettingsProvider.CreateProvider());
            Assert.IsNotNull(provider);
            Assert.AreEqual(DeucarianLoggingSettingsProvider.SettingsPath, provider.settingsPath);
        }

        [Test]
        public void LoggingAndResetIconsComeFromSharedEditorShell()
        {
            Assert.NotNull(DeucarianEditorIcons.GetPackageIcon("logging"));
            Assert.NotNull(DeucarianEditorIcons.GetIcon(DeucarianEditorIconIds.Reset));
            Assert.AreEqual(
                "Reset to Defaults",
                DeucarianEditorIcons.GetIconContent(
                    DeucarianEditorIconIds.Reset,
                    "Reset to Defaults").text);
        }

        [Test]
        public void SettingsProviderAndControlCenterUseTheSameSharedFormWithoutLegacyPanels()
        {
            var root = new VisualElement();
            var provider = DeucarianLoggingSettingsProvider.CreateProvider();
            try
            {
                provider.OnActivate(string.Empty, root);
                Assert.AreEqual(1, root.Query<ScrollView>("logging-settings-form").ToList().Count);
                Assert.AreEqual(0, root.Query<IMGUIContainer>().ToList().Count);
                Assert.IsNull(root.Q("workspace-sidebar"), "Project Settings already supplies its own navigation.");
                Assert.NotNull(root.Q<Toggle>("logging-enabled"));
                Assert.NotNull(root.Q<PopupField<string>>("logging-minimum-level"));
                Assert.NotNull(root.Q<TextField>("logging-prefix"));
                provider.OnActivate(string.Empty, root);
                Assert.AreEqual(1, root.Query<ScrollView>("logging-settings-form").ToList().Count);
            }
            finally { provider.OnDeactivate(); }
            Assert.AreEqual(0, root.childCount);
            Assert.DoesNotThrow(provider.OnDeactivate);

            using (var page = DeucarianLoggingSettingsPage.Create())
            {
                Assert.NotNull(page.Root.Q("workspace-navigation"));
                Assert.NotNull(page.Root.Q("logging-settings-form"));
                Assert.AreEqual(0, page.Root.Query<IMGUIContainer>().ToList().Count);
                Assert.NotNull(page.Root.Q<SliderInt>("workspace-scale-slider"));
            }
        }

        [UnityTest]
        public IEnumerator FormEditsPersistImmediatelyAndPreserveUneditedSettings()
        {
            DeucarianLoggingEditorSettings.SetValues(true, DeucarianLogLevel.Debug, true, true, "Original");
            var window = UnityEngine.ScriptableObject.CreateInstance<LoggingFormTestWindow>();
            window.Show();
            try
            {
                using (var page = DeucarianLoggingSettingsPage.Create())
                {
                    window.rootVisualElement.Add(page.Root);
                    yield return null;
                    page.Root.Q<Toggle>("logging-enabled").value = false;
                    Assert.IsFalse(DeucarianLoggingEditorSettings.Enabled);
                    Assert.IsFalse(DeucarianLogSettings.Enabled);
                    Assert.IsTrue(DeucarianLoggingEditorSettings.IncludeTimestamp);
                    Assert.AreEqual("Original", DeucarianLoggingEditorSettings.Prefix);

                    var level = page.Root.Q<PopupField<string>>("logging-minimum-level");
                    foreach (DeucarianLogLevel expected in System.Enum.GetValues(typeof(DeucarianLogLevel)))
                    {
                        level.value = expected.ToString();
                        Assert.AreEqual(expected, DeucarianLoggingEditorSettings.MinimumLevel);
                        Assert.AreEqual(expected, DeucarianLogSettings.MinimumLevel);
                    }
                    page.Root.Q<Toggle>("logging-timestamp").value = false;
                    page.Root.Q<Toggle>("logging-frame").value = false;
                    page.Root.Q<TextField>("logging-prefix").value = string.Empty;
                    Assert.IsFalse(DeucarianLogSettings.IncludeTimestamp);
                    Assert.IsFalse(DeucarianLogSettings.IncludeFrame);
                    Assert.AreEqual(string.Empty, DeucarianLogSettings.Prefix);
                }
            }
            finally { window.Close(); }
        }

        [Test]
        public void RevisitingPageRefreshesExternalChangesWithoutRebuildingFields()
        {
            using (var page = DeucarianLoggingSettingsPage.Create())
            {
                var prefix = page.Root.Q<TextField>("logging-prefix");
                DeucarianLoggingEditorSettings.SetValues(false, DeucarianLogLevel.None, true, true, "External");
                page.Deactivate();
                page.Activate(null);
                Assert.AreSame(prefix, page.Root.Q<TextField>("logging-prefix"));
                Assert.AreEqual("External", prefix.value);
                Assert.IsFalse(page.Root.Q<Toggle>("logging-enabled").value);
                Assert.AreEqual("None", page.Root.Q<PopupField<string>>("logging-minimum-level").value);
                Assert.IsTrue(page.Root.Q<Toggle>("logging-frame").value);
            }
        }

        [UnityTest]
        public IEnumerator DisposedFormCannotWritePreferencesThroughRetainedFields()
        {
            var window = UnityEngine.ScriptableObject.CreateInstance<LoggingFormTestWindow>();
            window.Show();
            try
            {
                var root = window.rootVisualElement;
                using (var view = new DeucarianLoggingSettingsView(root))
                {
                    var prefix = root.Q<TextField>("logging-prefix");
                    yield return null;
                    string previous = DeucarianLoggingEditorSettings.Prefix;
                    view.Dispose();
                    view.Dispose();
                    Assert.AreEqual(0, root.childCount);
                    root.Add(prefix);
                    prefix.value = "Must not persist";
                    Assert.AreEqual(previous, DeucarianLoggingEditorSettings.Prefix);
                }
            }
            finally { window.Close(); }
        }

        [UnityTest]
        public IEnumerator ProjectSettingsFormFillsItsHostAndSavesEdits()
        {
            DeucarianLoggingEditorSettings.SetValues(true, DeucarianLogLevel.Warning, true, true, "Initial settings");
            var window = UnityEngine.ScriptableObject.CreateInstance<LoggingFormTestWindow>();
            var provider = DeucarianLoggingSettingsProvider.CreateProvider();
            window.Show();
            try
            {
                window.position = new UnityEngine.Rect(20, 20, 820, 650);
                provider.OnActivate(string.Empty, window.rootVisualElement);
                Assert.IsTrue(window.rootVisualElement.Q<Toggle>("logging-enabled").value,
                    "A provider attached to an existing panel must initialize without waiting for focus.");
                Assert.AreEqual("Warning", window.rootVisualElement.Q<PopupField<string>>("logging-minimum-level").value);
                Assert.AreEqual("Initial settings", window.rootVisualElement.Q<TextField>("logging-prefix").value);
                StringAssert.Contains("Your message appears here", window.rootVisualElement.Q<Label>("logging-example").text);
                for (int frame = 0; frame < 5; frame++) yield return null;
                var form = window.rootVisualElement.Q<ScrollView>("logging-settings-form");
                Assert.Greater(form.resolvedStyle.height, 100);
                var prefix = form.Q<TextField>("logging-prefix");
                Assert.GreaterOrEqual(prefix.resolvedStyle.height, 30);
                Assert.LessOrEqual(prefix.worldBound.xMax, form.worldBound.xMax + 1);
                prefix.value = "Project settings edit";
                Assert.AreEqual("Project settings edit", DeucarianLoggingEditorSettings.Prefix);
            }
            finally { provider.OnDeactivate(); window.Close(); }
        }

        [UnityTest]
        public IEnumerator SharedFormStaysReadableAtDifferentSizesAndResetUsesTheVisibleButton()
        {
            var window = UnityEngine.ScriptableObject.CreateInstance<LoggingFormTestWindow>();
            int previousScale = DeucarianEditorAppearance.WorkspaceScalePercent;
            window.Show();
            try
            {
                using (var page = DeucarianLoggingSettingsPage.Create())
                {
                    window.rootVisualElement.Add(page.Root);
                    foreach (int scale in new[] { 75, 100, 150 })
                    {
                        DeucarianEditorAppearance.WorkspaceScalePercent = scale;
                        foreach (var size in new[] { new UnityEngine.Vector2(1480, 750), new UnityEngine.Vector2(820, 650) })
                        {
                            window.position = new UnityEngine.Rect(20, 20, size.x, size.y);
                            for (int frame = 0; frame < 5; frame++) yield return null;
                            var form = page.Root.Q<ScrollView>("logging-settings-form");
                            Assert.Greater(form.resolvedStyle.height, 100);
                            foreach (string id in new[] { "logging-enabled", "logging-minimum-level", "logging-timestamp", "logging-frame", "logging-prefix" })
                            {
                                var field = form.Q(id);
                                Assert.GreaterOrEqual(field.resolvedStyle.height, 30, id);
                                Assert.GreaterOrEqual(field.worldBound.xMin, form.worldBound.xMin - 1, id);
                                Assert.LessOrEqual(field.worldBound.xMax, form.worldBound.xMax + 1, id);
                            }
                            Assert.LessOrEqual(form.Q("logging-prefix").parent.parent.worldBound.xMax,
                                form.contentViewport.worldBound.xMax + 1);
                        }
                    }
                    DeucarianLoggingEditorSettings.SetValues(false, DeucarianLogLevel.Error, true, true, "Reset me");
                    page.Activate(null);
                    var reset = page.Root.Q<Button>("logging-reset");
                    reset.Focus();
                    yield return null;
                    using (var evt = NavigationSubmitEvent.GetPooled()) { evt.target = reset; reset.SendEvent(evt); }
                    Assert.IsTrue(DeucarianLoggingEditorSettings.Enabled);
                    Assert.AreEqual("Deucarian", page.Root.Q<TextField>("logging-prefix").value);
                    Assert.AreEqual("Debug", page.Root.Q<PopupField<string>>("logging-minimum-level").value);
                    Assert.IsFalse(page.Root.Q<Toggle>("logging-timestamp").value);
                }
            }
            finally { window.Close(); DeucarianEditorAppearance.WorkspaceScalePercent = previousScale; }
        }

        public sealed class LoggingFormTestWindow : EditorWindow { }

        [Test]
        public void MenuResetRestoresExpectedDefaults()
        {
            DeucarianLog.ClearSinks();
            var sink = new CapturingSink();
            DeucarianLog.RegisterSink(sink);

            DeucarianLoggingEditorSettings.SetValues(
                false,
                DeucarianLogLevel.Error,
                true,
                true,
                "Custom");

            DeucarianLoggingMenu.ResetLoggingSettings();

            Assert.IsTrue(DeucarianLoggingEditorSettings.Enabled);
            Assert.AreEqual(DeucarianLogLevel.Debug, DeucarianLoggingEditorSettings.MinimumLevel);
            Assert.IsFalse(DeucarianLoggingEditorSettings.IncludeTimestamp);
            Assert.IsFalse(DeucarianLoggingEditorSettings.IncludeFrame);
            Assert.AreEqual("Deucarian", DeucarianLoggingEditorSettings.Prefix);
            Assert.AreEqual(1, sink.Entries.Count);
            Assert.AreEqual(DeucarianLogLevel.Info, sink.Entries[0].Level);
            Assert.AreEqual("Logging.Editor", sink.Entries[0].Category);
            Assert.AreEqual("[Deucarian.Logging] Logging settings reset.", sink.Entries[0].Message);
        }

        [Test]
        public void ControlCenterCard_ExposesSanitizedSettingsAndGuardedActions()
        {
            DeucarianControlCenterCard card =
                DeucarianLoggingCardProvider.CreateCard(true, DeucarianLogLevel.Warning);

            Assert.AreEqual("logging.settings", card.Id);
            Assert.AreEqual(DeucarianControlCenterArea.Developer, card.Area);
            Assert.AreEqual("Enabled", card.StatusText);
            Assert.IsTrue(card.Actions[1].RequiresConfirmation);
            Assert.IsTrue(card.Actions[2].RequiresConfirmation);
        }

        private sealed class CapturingSink : IDeucarianLogSink
        {
            private readonly List<DeucarianLogEntry> entries = new List<DeucarianLogEntry>();

            public IReadOnlyList<DeucarianLogEntry> Entries
            {
                get { return entries; }
            }

            public void Log(in DeucarianLogEntry entry)
            {
                entries.Add(entry);
            }
        }

        private readonly struct SettingsSnapshot
        {
            private readonly bool enabled;
            private readonly DeucarianLogLevel minimumLevel;
            private readonly bool includeTimestamp;
            private readonly bool includeFrame;
            private readonly string prefix;

            private SettingsSnapshot(
                bool enabled,
                DeucarianLogLevel minimumLevel,
                bool includeTimestamp,
                bool includeFrame,
                string prefix)
            {
                this.enabled = enabled;
                this.minimumLevel = minimumLevel;
                this.includeTimestamp = includeTimestamp;
                this.includeFrame = includeFrame;
                this.prefix = prefix;
            }

            public static SettingsSnapshot Capture()
            {
                return new SettingsSnapshot(
                    DeucarianLoggingEditorSettings.Enabled,
                    DeucarianLoggingEditorSettings.MinimumLevel,
                    DeucarianLoggingEditorSettings.IncludeTimestamp,
                    DeucarianLoggingEditorSettings.IncludeFrame,
                    DeucarianLoggingEditorSettings.Prefix);
            }

            public void Restore()
            {
                DeucarianLoggingEditorSettings.SetValues(
                    enabled,
                    minimumLevel,
                    includeTimestamp,
                    includeFrame,
                    prefix);
            }
        }
    }
}
