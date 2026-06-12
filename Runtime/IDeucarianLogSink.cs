namespace Deucarian.Logging
{
    /// <summary>
    /// Receives Deucarian log entries from the central dispatcher.
    /// </summary>
    public interface IDeucarianLogSink
    {
        /// <summary>
        /// Handles a log entry.
        /// </summary>
        /// <param name="entry">The log entry to handle.</param>
        void Log(in DeucarianLogEntry entry);
    }
}
