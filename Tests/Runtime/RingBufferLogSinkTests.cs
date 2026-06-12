using System;
using NUnit.Framework;

namespace Deucarian.Logging.Tests
{
    public sealed class RingBufferLogSinkTests
    {
        [Test]
        public void KeepsOnlyLatestEntries()
        {
            var sink = new RingBufferLogSink(2);
            DeucarianLogEntry first = Entry("first");
            DeucarianLogEntry second = Entry("second");
            DeucarianLogEntry third = Entry("third");

            sink.Log(in first);
            sink.Log(in second);
            sink.Log(in third);

            Assert.AreEqual(2, sink.Entries.Count);
            Assert.AreEqual("second", sink.Entries[0].Message);
            Assert.AreEqual("third", sink.Entries[1].Message);
        }

        [Test]
        public void ClearRemovesEntries()
        {
            var sink = new RingBufferLogSink(2);
            DeucarianLogEntry entry = Entry("one");

            sink.Log(in entry);
            sink.Clear();

            Assert.AreEqual(0, sink.Entries.Count);
        }

        [Test]
        public void RejectsInvalidCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RingBufferLogSink(0));
        }

        private static DeucarianLogEntry Entry(string message)
        {
            return new DeucarianLogEntry(
                DateTime.UtcNow,
                0,
                DeucarianLogLevel.Info,
                "Tests",
                message);
        }
    }
}
