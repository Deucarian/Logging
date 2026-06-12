using System;
using System.Collections.Generic;
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

        private sealed class ThrowingSink : IDeucarianLogSink
        {
            public void Log(in DeucarianLogEntry entry)
            {
                throw new InvalidOperationException("Expected test failure.");
            }
        }
    }
}
