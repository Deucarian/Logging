using System;
using System.Collections;
using System.Collections.Generic;

namespace Deucarian.Logging
{
    /// <summary>
    /// Runtime sink that keeps the most recent log entries in memory.
    /// </summary>
    public sealed class RingBufferLogSink : IDeucarianLogSink
    {
        /// <summary>
        /// Default number of entries retained by the ring buffer.
        /// </summary>
        public const int DefaultCapacity = 200;

        private readonly DeucarianLogEntry[] buffer;
        private readonly EntriesView entries;
        private int start;
        private int count;

        /// <summary>
        /// Creates a ring buffer sink.
        /// </summary>
        /// <param name="capacity">Maximum number of entries to retain.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity"/> is less than one.</exception>
        public RingBufferLogSink(int capacity = DefaultCapacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
            }

            buffer = new DeucarianLogEntry[capacity];
            entries = new EntriesView(this);
        }

        /// <summary>
        /// Gets the maximum number of entries retained by this sink.
        /// </summary>
        public int Capacity => buffer.Length;

        /// <summary>
        /// Gets retained entries in oldest-to-newest order.
        /// </summary>
        public IReadOnlyList<DeucarianLogEntry> Entries => entries;

        /// <summary>
        /// Stores a log entry, evicting the oldest entry when capacity is reached.
        /// </summary>
        /// <param name="entry">The entry to store.</param>
        public void Log(in DeucarianLogEntry entry)
        {
            if (count < buffer.Length)
            {
                buffer[(start + count) % buffer.Length] = entry;
                count++;
                return;
            }

            buffer[start] = entry;
            start = (start + 1) % buffer.Length;
        }

        /// <summary>
        /// Removes all retained entries.
        /// </summary>
        public void Clear()
        {
            Array.Clear(buffer, 0, buffer.Length);
            start = 0;
            count = 0;
        }

        private DeucarianLogEntry GetEntry(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return buffer[(start + index) % buffer.Length];
        }

        private sealed class EntriesView : IReadOnlyList<DeucarianLogEntry>
        {
            private readonly RingBufferLogSink owner;

            public EntriesView(RingBufferLogSink owner)
            {
                this.owner = owner;
            }

            public int Count => owner.count;

            public DeucarianLogEntry this[int index] => owner.GetEntry(index);

            public IEnumerator<DeucarianLogEntry> GetEnumerator()
            {
                for (int i = 0; i < owner.count; i++)
                {
                    yield return owner.GetEntry(i);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
