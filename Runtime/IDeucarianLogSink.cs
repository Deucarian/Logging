namespace Deucarian.Logging
{
    /// <summary>
    /// Receives Deucarian log entries from the central dispatcher.
    /// Future packages can implement this interface to forward entries elsewhere without changing this package.
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
