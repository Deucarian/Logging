using System.Globalization;
using System.Text;

namespace Deucarian.Logging
{
    /// <summary>
    /// Default sink that forwards Deucarian log entries to Unity's built-in console.
    /// </summary>
    public sealed class UnityConsoleLogSink : IDeucarianLogSink
    {
        /// <summary>
        /// Writes a log entry to the Unity console.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
#if UNITY_2022_2_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        public void Log(in DeucarianLogEntry entry)
        {
            string formattedMessage = FormatMessage(in entry);

            switch (entry.Level)
            {
                case DeucarianLogLevel.Trace:
                case DeucarianLogLevel.Debug:
                case DeucarianLogLevel.Info:
                    UnityEngine.Debug.Log(formattedMessage, entry.Context);
                    break;
                case DeucarianLogLevel.Warning:
                    UnityEngine.Debug.LogWarning(formattedMessage, entry.Context);
                    break;
                case DeucarianLogLevel.Error:
                    UnityEngine.Debug.LogError(formattedMessage, entry.Context);
                    break;
                case DeucarianLogLevel.Exception:
                    LogException(in entry, formattedMessage);
                    break;
            }
        }

#if UNITY_2022_2_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        private static void LogException(in DeucarianLogEntry entry, string formattedMessage)
        {
            if (entry.Exception == null)
            {
                UnityEngine.Debug.LogError(formattedMessage, entry.Context);
                return;
            }

            UnityEngine.Debug.LogError(formattedMessage, entry.Context);
            UnityEngine.Debug.LogException(entry.Exception, entry.Context);
        }

#if UNITY_2022_2_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        private static string FormatMessage(in DeucarianLogEntry entry)
        {
            var builder = new StringBuilder(128);

            if (DeucarianLogSettings.IncludeTimestamp)
            {
                builder
                    .Append('[')
                    .Append(entry.TimestampUtc.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture))
                    .Append(" UTC] ");
            }

            if (DeucarianLogSettings.IncludeFrame && entry.Frame >= 0)
            {
                builder
                    .Append("[Frame ")
                    .Append(entry.Frame.ToString(CultureInfo.InvariantCulture))
                    .Append("] ");
            }

            builder
                .Append('[')
                .Append(FormatCategoryLabel(entry.Category))
                .Append(']');

            if (!string.IsNullOrEmpty(entry.Message))
            {
                builder.Append(' ').Append(entry.Message);
            }

            return builder.ToString();
        }

        private static string FormatCategoryLabel(string category)
        {
            return DeucarianLogSettings.IncludePrefixInUnityConsole
                ? DeucarianLog.FormatCategoryLabel(category)
                : DeucarianLog.NormalizeCategory(category);
        }
    }
}
