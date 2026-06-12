namespace Deucarian.Logging
{
    /// <summary>
    /// Defines the severity of a Deucarian log entry.
    /// </summary>
    public enum DeucarianLogLevel
    {
        /// <summary>
        /// Very detailed diagnostic information.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Debug information useful during development.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// General informational messages.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Recoverable issues or noteworthy unexpected states.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Errors that should be investigated.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Exceptions that should preserve the original exception object.
        /// </summary>
        Exception = 5,

        /// <summary>
        /// Disables all log levels when used as the minimum level.
        /// </summary>
        None = 100
    }
}
