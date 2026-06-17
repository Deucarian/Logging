using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace Deucarian.Logging.Tests
{
    public sealed class DLogTests
    {
        private CapturingSink sink;

        [SetUp]
        public void SetUp()
        {
            DeucarianLogSettings.ResetToDefaults();
            DeucarianLog.ClearSinks();
            sink = new CapturingSink();
            DeucarianLog.RegisterSink(sink);
        }

        [TearDown]
        public void TearDown()
        {
            DeucarianLogSettings.ResetToDefaults();
            DeucarianLog.ResetSinksToDefault();
        }

        [Test]
        public void ForSanitizesCategories()
        {
            Assert.AreEqual("General", DLog.For(null).Category);
            Assert.AreEqual("General", DLog.For(" ").Category);
            Assert.AreEqual("Inventory", DLog.For(" Inventory ").Category);
            Assert.AreEqual("Inventory", DLog.For("Deucarian.Inventory").Category);
        }

        [Test]
        public void CategoryConstantsArePlainStrings()
        {
            Assert.AreEqual(typeof(string), DeucarianLogCategories.PackageInstaller.GetType());
            Assert.AreEqual("PackageInstaller", DeucarianLogCategories.PackageInstaller);
            Assert.AreEqual("ObjectLoading", DeucarianLogCategories.ObjectLoading);
            Assert.AreEqual("Theming", DeucarianLogCategories.Theming);
            Assert.AreEqual("Selection", DeucarianLogCategories.Selection);
            Assert.AreEqual("Session", DeucarianLogCategories.Session);
            Assert.AreEqual("ApiHelper", DeucarianLogCategories.ApiHelper);
        }

        [Test]
        public void UnityConsoleCategoryLabelOmitsPrefixByDefault()
        {
            Assert.IsFalse(DeucarianLogSettings.IncludePrefixInUnityConsole);

            Assert.AreEqual("ReportViewer.Dev", FormatUnityConsoleCategoryLabel("ReportViewer.Dev"));
            Assert.AreEqual("ReportViewer.Dev", FormatUnityConsoleCategoryLabel("Deucarian.ReportViewer.Dev"));
        }

        [Test]
        public void UnityConsoleCategoryLabelCanIncludePrefix()
        {
            DeucarianLogSettings.IncludePrefixInUnityConsole = true;

            Assert.AreEqual("Deucarian.ReportViewer.Dev", FormatUnityConsoleCategoryLabel("ReportViewer.Dev"));
            Assert.AreEqual("Deucarian.ReportViewer.Dev", FormatUnityConsoleCategoryLabel("Deucarian.ReportViewer.Dev"));
        }

        [Test]
        public void LogsBelowMinimumLevelAreFiltered()
        {
            DeucarianLogSettings.MinimumLevel = DeucarianLogLevel.Warning;

            DLog.For("Inventory").Info("hidden");

            Assert.AreEqual(0, sink.Entries.Count);
        }

        [Test]
        public void RegisteredSinkReceivesLogsAtOrAboveMinimumLevel()
        {
            DeucarianLogSettings.MinimumLevel = DeucarianLogLevel.Warning;

            DLog.For("Inventory").Warning("visible");

            Assert.AreEqual(1, sink.Entries.Count);
            Assert.AreEqual(DeucarianLogLevel.Warning, sink.Entries[0].Level);
            Assert.AreEqual("visible", sink.Entries[0].Message);
        }

        [Test]
        public void RegisteredSinkReceivesRedactedMessages()
        {
            DLog.For("ApiHelper").Info("token=abc123");

            Assert.AreEqual(1, sink.Entries.Count);
            Assert.AreEqual("token=[REDACTED]", sink.Entries[0].Message);
        }

        [Test]
        public void DispatchRespectsEnabledFlag()
        {
            DeucarianLogSettings.Enabled = false;

            DLog.For("Inventory").Error("hidden");

            Assert.AreEqual(0, sink.Entries.Count);
        }

        [Test]
        public void ClearSinksRemovesRegisteredSinks()
        {
            DeucarianLog.ClearSinks();

            DLog.For("Inventory").Error("hidden");

            Assert.AreEqual(0, sink.Entries.Count);
        }

        [Test]
        public void ResetSinksToDefaultDoesNotThrow()
        {
            DeucarianLog.ClearSinks();

            Assert.DoesNotThrow(() => DeucarianLog.ResetSinksToDefault());
            Assert.DoesNotThrow(() => DLog.For("Inventory").Info("visible"));
        }

        [Test]
        public void ExceptionLogsPreserveExceptionObject()
        {
            var exception = new InvalidOperationException("boom");

            DLog.For("Inventory").Exception(exception, "failed");

            Assert.AreEqual(1, sink.Entries.Count);
            Assert.AreEqual(DeucarianLogLevel.Exception, sink.Entries[0].Level);
            Assert.AreSame(exception, sink.Entries[0].Exception);
            Assert.AreEqual("failed", sink.Entries[0].Message);
        }

        [Test]
        public void ScopedMeasurementLogsCompletion()
        {
            DeucarianLogSettings.MinimumLevel = DeucarianLogLevel.Debug;
            DLog log = DLog.For(DeucarianLogCategories.ObjectLoading);

            using (log.Measure("Load asset bundle"))
            {
            }

            Assert.AreEqual(2, sink.Entries.Count);
            Assert.AreEqual("Started: Load asset bundle", sink.Entries[0].Message);
            Assert.IsTrue(sink.Entries[1].Message.StartsWith("Completed: Load asset bundle in "));
            Assert.IsTrue(sink.Entries[1].Message.EndsWith(" ms"));
        }

        [Test]
        public void ScopedMeasurementDoesNotLogWhenLevelIsFiltered()
        {
            DeucarianLogSettings.MinimumLevel = DeucarianLogLevel.Warning;
            DLog log = DLog.For(DeucarianLogCategories.ObjectLoading);

            using (log.Measure("Load asset bundle", DeucarianLogLevel.Debug))
            {
            }

            Assert.AreEqual(0, sink.Entries.Count);
        }

        [Test]
        public void ScopedMeasurementCanBeDisposedTwice()
        {
            DeucarianLogSettings.MinimumLevel = DeucarianLogLevel.Debug;
            DLog log = DLog.For(DeucarianLogCategories.PackageInstaller);

            DeucarianLogScope scope = log.Scope("Install package");
            scope.Dispose();
            scope.Dispose();

            Assert.AreEqual(2, sink.Entries.Count);
        }

        [Test]
        public void SinkApiReceivesEntriesWithoutTelemetryDependency()
        {
            DeucarianLog.ClearSinks();
            var externalSink = new ExternalPackageSink();
            DeucarianLog.RegisterSink(externalSink);

            DLog.For("FutureTelemetryExtension").Info("local sink entry");

            Assert.AreEqual(1, externalSink.Count);
            Assert.AreEqual("FutureTelemetryExtension", externalSink.LastEntry.Category);
            Assert.AreEqual("local sink entry", externalSink.LastEntry.Message);
        }

        [Test]
        public void SinkExceptionsDoNotEscapeOrStopOtherSinks()
        {
            DeucarianLog.ClearSinks();
            DeucarianLog.RegisterSink(new ThrowingSink());
            DeucarianLog.RegisterSink(sink);

            Assert.DoesNotThrow(() => DLog.For("Inventory").Info("visible"));
            Assert.AreEqual(1, sink.Entries.Count);
            Assert.AreEqual("visible", sink.Entries[0].Message);
        }

        private sealed class CapturingSink : IDeucarianLogSink
        {
            private readonly List<DeucarianLogEntry> entries = new List<DeucarianLogEntry>();

            public IReadOnlyList<DeucarianLogEntry> Entries => entries;

            public void Log(in DeucarianLogEntry entry)
            {
                entries.Add(entry);
            }
        }

        private sealed class ExternalPackageSink : IDeucarianLogSink
        {
            public int Count { get; private set; }
            public DeucarianLogEntry LastEntry { get; private set; }

            public void Log(in DeucarianLogEntry entry)
            {
                Count++;
                LastEntry = entry;
            }
        }

        private sealed class ThrowingSink : IDeucarianLogSink
        {
            public void Log(in DeucarianLogEntry entry)
            {
                throw new InvalidOperationException("Expected test failure.");
            }
        }

        private static string FormatUnityConsoleCategoryLabel(string category)
        {
            MethodInfo method = typeof(UnityConsoleLogSink).GetMethod(
                "FormatCategoryLabel",
                BindingFlags.NonPublic | BindingFlags.Static);

            Assert.NotNull(method);
            return (string)method.Invoke(null, new object[] { category });
        }
    }
}
