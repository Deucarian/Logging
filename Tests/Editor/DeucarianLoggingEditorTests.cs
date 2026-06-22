using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;

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
