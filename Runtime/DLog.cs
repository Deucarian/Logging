using System;
using Object = UnityEngine.Object;

namespace Deucarian.Logging
{
    /// <summary>
    /// Category logger used as the primary public Deucarian logging API.
    /// </summary>
    public sealed class DLog
    {
        internal DLog(string category)
        {
            Category = DeucarianLog.NormalizeCategory(category);
        }

        /// <summary>
        /// Gets the normalized category for this logger.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Creates a logger for a category.
        /// </summary>
        /// <param name="category">Category name such as Inventory, SaveSystem, Audio, UI, or Networking.</param>
        /// <returns>A logger for the normalized category.</returns>
        public static DLog For(string category)
        {
            return DeucarianLog.CreateLogger(category);
        }

        /// <summary>
        /// Logs a trace message.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="context">Optional Unity context object.</param>
#if UNITY_2022_2_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        public void Trace(string message, Object context = null)
        {
            Log(DeucarianLogLevel.Trace, message, null, context);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="context">Optional Unity context object.</param>
#if UNITY_2022_2_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        public void Debug(string message, Object context = null)
        {
            Log(DeucarianLogLevel.Debug, message, null, context);
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="context">Optional Unity context object.</param>
#if UNITY_2022_2_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        public void Info(string message, Object context = null)
        {
            Log(DeucarianLogLevel.Info, message, null, context);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="context">Optional Unity context object.</param>
#if UNITY_2022_2_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        public void Warning(string message, Object context = null)
        {
            Log(DeucarianLogLevel.Warning, message, null, context);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="context">Optional Unity context object.</param>
#if UNITY_2022_2_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        public void Error(string message, Object context = null)
        {
            Log(DeucarianLogLevel.Error, message, null, context);
        }

        /// <summary>
        /// Logs an exception while preserving the original exception object.
        /// </summary>
        /// <param name="exception">The exception to preserve and forward to sinks.</param>
        /// <param name="message">Optional message text. If omitted, the exception message is used.</param>
        /// <param name="context">Optional Unity context object.</param>
#if UNITY_2022_2_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        public void Exception(Exception exception, string message = null, Object context = null)
        {
            Log(DeucarianLogLevel.Exception, message ?? exception?.Message, exception, context);
        }

        /// <summary>
        /// Logs the start and completion of an operation at debug level.
        /// </summary>
        /// <param name="operation">Operation name to include in log messages.</param>
        /// <param name="context">Optional Unity context object.</param>
        /// <returns>A disposable measurement scope.</returns>
        public DeucarianLogScope Scope(string operation, Object context = null)
        {
            return Measure(operation, DeucarianLogLevel.Debug, context);
        }

        /// <summary>
        /// Logs the start and completion of an operation at the requested level.
        /// </summary>
        /// <param name="operation">Operation name to include in log messages.</param>
        /// <param name="level">Log level used for start and completion messages.</param>
        /// <param name="context">Optional Unity context object.</param>
        /// <returns>A disposable measurement scope.</returns>
        public DeucarianLogScope Scope(string operation, DeucarianLogLevel level, Object context = null)
        {
            return Measure(operation, level, context);
        }

        /// <summary>
        /// Logs the start and completion of an operation at debug level.
        /// </summary>
        /// <param name="operation">Operation name to include in log messages.</param>
        /// <param name="context">Optional Unity context object.</param>
        /// <returns>A disposable measurement scope.</returns>
        public DeucarianLogScope Measure(string operation, Object context = null)
        {
            return Measure(operation, DeucarianLogLevel.Debug, context);
        }

        /// <summary>
        /// Logs the start and completion of an operation at the requested level.
        /// </summary>
        /// <param name="operation">Operation name to include in log messages.</param>
        /// <param name="level">Log level used for start and completion messages.</param>
        /// <param name="context">Optional Unity context object.</param>
        /// <returns>A disposable measurement scope.</returns>
        public DeucarianLogScope Measure(string operation, DeucarianLogLevel level, Object context = null)
        {
            return DeucarianLogScope.Start(this, operation, level, context);
        }

        private void Log(DeucarianLogLevel level, string message, Exception exception, Object context)
        {
            DeucarianLog.Dispatch(Category, level, message, exception, context);
        }
    }
}
