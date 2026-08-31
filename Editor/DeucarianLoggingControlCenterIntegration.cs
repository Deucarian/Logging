using System.Collections.Generic;
using Deucarian.Editor;
using UnityEditor;

namespace Deucarian.Logging.Editor
{
    [InitializeOnLoad]
    internal static class DeucarianLoggingControlCenterIntegration
    {
        private const string PackageId = "com.deucarian.logging";

        static DeucarianLoggingControlCenterIntegration()
        {
            DeucarianToolRegistry.Register(new DeucarianToolDescriptor(
                DeucarianToolIds.LoggingSettings,
                "Logging Settings",
                "Configure local Deucarian logging filters and formatting.",
                DeucarianControlCenterArea.Developer,
                DeucarianLoggingMenu.OpenLoggingSettings,
                PackageId,
                "console.infoicon",
                new[] { "logging", "console", "categories", "levels" },
                30));
            DeucarianControlCenterRegistry.RegisterCardProvider(
                new DeucarianLoggingCardProvider());
        }
    }

    internal sealed class DeucarianLoggingCardProvider :
        IDeucarianControlCenterCardProvider
    {
        public string Id => "com.deucarian.logging.status";

        public IEnumerable<DeucarianControlCenterCard> Capture(
            DeucarianControlCenterContext context)
        {
            yield return CreateCard(
                DeucarianLoggingEditorSettings.Enabled,
                DeucarianLoggingEditorSettings.MinimumLevel);
        }

        internal static DeucarianControlCenterCard CreateCard(
            bool enabled,
            DeucarianLogLevel minimumLevel)
        {
            return new DeucarianControlCenterCard(
                "logging.settings",
                DeucarianControlCenterArea.Developer,
                "Logging",
                "Local logging preferences and deliberate maintenance actions.",
                "com.deucarian.logging",
                DeucarianControlCenterStatus.Info,
                enabled ? "Enabled" : "Disabled",
                30,
                new[]
                {
                    "Minimum level: " + minimumLevel + ".",
                    "Log content is not included in this summary."
                },
                new[]
                {
                    new DeucarianControlCenterAction(
                        "logging.open-settings",
                        "Open Logging Settings",
                        DeucarianLoggingMenu.OpenLoggingSettings),
                    new DeucarianControlCenterAction(
                        "logging.reset-settings",
                        "Reset Logging Settings",
                        DeucarianLoggingMenu.ResetLoggingSettings,
                        "Restore local logging preferences to package defaults.",
                        requiresConfirmation: true),
                    new DeucarianControlCenterAction(
                        "logging.test-messages",
                        "Emit Test Log Messages",
                        DeucarianLoggingMenu.TestLogMessages,
                        "Write one local test entry at each level.",
                        requiresConfirmation: true)
                },
                new[] { "logging", "console", "level", "settings", "test" });
        }
    }
}
