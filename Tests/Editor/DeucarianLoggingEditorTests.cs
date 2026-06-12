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
