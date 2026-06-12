using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deucarian.Logging
{
    /// <summary>
    /// Central dispatcher and sink registry for Deucarian logging.
    /// </summary>
    public static class DeucarianLog
    {
        private const string DefaultCategory = "General";
        private static readonly object SyncRoot = new object();
        private static readonly List<IDeucarianLogSink> Sinks = new List<IDeucarianLogSink>();

        [ThreadStatic]
        private static bool isReportingSinkFailure;

        static DeucarianLog()
        {
            ResetSinksToDefault();
        }

        /// <summary>
        /// Registers a sink that will receive future log entries.
        /// </summary>
        /// <param name="sink">The sink to register.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sink"/> is null.</exception>
        public static void RegisterSink(IDeucarianLogSink sink)
        {
            if (sink == null)
            {
                throw new ArgumentNullException(nameof(sink));
            }

            lock (SyncRoot)
            {
                if (!Sinks.Contains(sink))
                {
                    Sinks.Add(sink);
                }
            }
        }

        /// <summary>
        /// Unregisters a sink if it is currently registered.
        /// </summary>
        /// <param name="sink">The sink to unregister.</param>
        public static void UnregisterSink(IDeucarianLogSink sink)
        {
            if (sink == null)
            {
                return;
            }

            lock (SyncRoot)
            {
                Sinks.Remove(sink);
            }
        }

        /// <summary>
        /// Removes every registered sink.
        /// </summary>
        public static void ClearSinks()
        {
            lock (SyncRoot)
            {
                Sinks.Clear();
            }
        }

        /// <summary>
        /// Restores the default Unity console sink as the only registered sink.
        /// </summary>
        public static void ResetSinksToDefault()
        {
            lock (SyncRoot)
            {
                Sinks.Clear();
                Sinks.Add(new UnityConsoleLogSink());
            }
        }

        /// <summary>
        /// Creates a category logger.
        /// </summary>
        /// <param name="category">The category name to use.</param>
        /// <returns>A logger for the normalized category.</returns>
        public static DLog CreateLogger(string category)
        {
            return new DLog(NormalizeCategory(category));
        }

        internal static void Dispatch(
            string category,
            DeucarianLogLevel level,
            string message,
            Exception exception,
            Object context)
        {
            if (!ShouldLog(level))
            {
                return;
            }

            IDeucarianLogSink[] sinks = SnapshotSinks();
            if (sinks.Length == 0)
            {
                return;
            }

            var entry = new DeucarianLogEntry(
                DateTime.UtcNow,
                GetCurrentFrame(),
                level,
                category,
                DeucarianLogUtility.RedactSensitiveData(message),
                exception,
                context);

            for (int i = 0; i < sinks.Length; i++)
            {
                IDeucarianLogSink sink = sinks[i];
                try
                {
                    sink.Log(in entry);
                }
                catch (Exception sinkException)
                {
                    ReportSinkFailure(sink, sinkException);
                }
            }
        }

        internal static string NormalizeCategory(string category)
        {
            string normalized = string.IsNullOrWhiteSpace(category)
                ? DefaultCategory
                : category.Trim();

            string prefix = DeucarianLogSettings.Prefix;
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                prefix = prefix.Trim();

                if (normalized.Equals(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return DefaultCategory;
                }

                string prefixWithSeparator = prefix + ".";
                if (normalized.StartsWith(prefixWithSeparator, StringComparison.OrdinalIgnoreCase))
                {
                    normalized = normalized.Substring(prefixWithSeparator.Length).Trim();
                }
            }

            return string.IsNullOrWhiteSpace(normalized) ? DefaultCategory : normalized;
        }

        internal static string FormatCategoryLabel(string category)
        {
            string normalized = NormalizeCategory(category);
            string prefix = DeucarianLogSettings.Prefix;

            if (string.IsNullOrWhiteSpace(prefix))
            {
                return normalized;
            }

            prefix = prefix.Trim();
            if (normalized.Equals(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return prefix;
            }

            return prefix + "." + normalized;
        }

        internal static bool IsEnabledFor(DeucarianLogLevel level)
        {
            return ShouldLog(level);
        }

        private static bool ShouldLog(DeucarianLogLevel level)
        {
            if (!DeucarianLogSettings.Enabled || level == DeucarianLogLevel.None)
            {
                return false;
            }

            return level >= DeucarianLogSettings.MinimumLevel;
        }

        private static IDeucarianLogSink[] SnapshotSinks()
        {
            lock (SyncRoot)
            {
                return Sinks.ToArray();
            }
        }

        private static int GetCurrentFrame()
        {
            try
            {
                return Time.frameCount;
            }
            catch
            {
                return -1;
            }
        }

        private static void ReportSinkFailure(IDeucarianLogSink sink, Exception exception)
        {
            if (isReportingSinkFailure)
            {
                return;
            }

            try
            {
                isReportingSinkFailure = true;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                string sinkName = sink == null ? "unknown sink" : sink.GetType().FullName;
                string exceptionName = exception == null ? "unknown error" : exception.GetType().Name;
                string message = exception == null ? string.Empty : exception.Message;
                UnityEngine.Debug.LogWarning(
                    "[Deucarian.Logging] Log sink '" + sinkName + "' failed: " + exceptionName + ": " + message);
#endif
            }
            catch
            {
                // Suppress recursive logging failures.
            }
            finally
            {
                isReportingSinkFailure = false;
            }
        }
    }
}
