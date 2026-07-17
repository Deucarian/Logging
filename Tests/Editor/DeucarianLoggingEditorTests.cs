using System.Collections.Generic;
using System.IO;
using Deucarian.Editor;
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
        public void SettingsProviderUsesSharedThemedPageAndCompactControls()
        {
            const string sourcePath =
                "Packages/com.deucarian.logging/Editor/DeucarianLoggingSettingsProvider.cs";
            UnityEditor.PackageManager.PackageInfo package =
                UnityEditor.PackageManager.PackageInfo.FindForAssetPath(sourcePath);
            Assert.NotNull(package);
            string source = File.ReadAllText(Path.Combine(
                package.resolvedPath,
                "Editor/DeucarianLoggingSettingsProvider.cs"));

            StringAssert.Contains("BeginSettingsPage", source);
            StringAssert.Contains("DrawLabeledField", source);
            StringAssert.Contains("// DeucarianEditorChrome.DrawPackageHeader", source);
            StringAssert.Contains("DrawResetToDefaultsButton", source);
            StringAssert.DoesNotContain(
                "                DeucarianEditorChrome.DrawPackageHeader(",
                source);
            StringAssert.DoesNotContain("DrawCompactIconAction", source);
            StringAssert.DoesNotContain("Reset to Defaults", source);
            StringAssert.DoesNotContain("24f", source);
            StringAssert.DoesNotContain("GUILayout.Button(resetContent)", source);
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
