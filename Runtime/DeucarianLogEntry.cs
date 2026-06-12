using System;
using Object = UnityEngine.Object;

namespace Deucarian.Logging
{
    /// <summary>
    /// Immutable data captured for a single Deucarian log event.
    /// </summary>
    public readonly struct DeucarianLogEntry
    {
        /// <summary>
        /// Gets the UTC timestamp captured when the entry was dispatched.
        /// </summary>
        public DateTime TimestampUtc { get; }

        /// <summary>
        /// Gets the Unity frame captured when the entry was dispatched, or -1 when unavailable.
        /// </summary>
        public int Frame { get; }

        /// <summary>
        /// Gets the severity level for the entry.
        /// </summary>
        public DeucarianLogLevel Level { get; }

        /// <summary>
        /// Gets the normalized category without the Deucarian prefix.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the preserved exception object for exception logs.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the optional Unity context object.
        /// </summary>
        public Object Context { get; }

        /// <summary>
        /// Creates a log entry.
        /// </summary>
        /// <param name="timestampUtc">UTC timestamp for the entry.</param>
        /// <param name="frame">Unity frame for the entry, or -1 when unavailable.</param>
        /// <param name="level">Severity level.</param>
        /// <param name="category">Log category.</param>
        /// <param name="message">Message text.</param>
        /// <param name="exception">Optional preserved exception.</param>
        /// <param name="context">Optional Unity context object.</param>
        public DeucarianLogEntry(
            DateTime timestampUtc,
            int frame,
            DeucarianLogLevel level,
            string category,
            string message,
            Exception exception = null,
            Object context = null)
        {
            TimestampUtc = timestampUtc;
            Frame = frame;
            Level = level;
            Category = DeucarianLog.NormalizeCategory(category);
            Message = message ?? string.Empty;
            Exception = exception;
            Context = context;
        }
    }
}
