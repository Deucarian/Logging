using System;
using System.Diagnostics;
using System.Globalization;
using Object = UnityEngine.Object;

namespace Deucarian.Logging
{
    /// <summary>
    /// Disposable measurement scope that logs operation start and completion.
    /// </summary>
    public struct DeucarianLogScope : IDisposable
    {
        private readonly DLog logger;
        private readonly string operation;
        private readonly DeucarianLogLevel level;
        private readonly Object context;
        private readonly long startTimestamp;
        private bool active;
        private bool disposed;

        private DeucarianLogScope(
            DLog logger,
            string operation,
            DeucarianLogLevel level,
            Object context,
            bool active)
        {
            this.logger = logger;
            this.operation = string.IsNullOrWhiteSpace(operation) ? "Operation" : operation.Trim();
            this.level = level;
            this.context = context;
            this.active = active;
            disposed = false;
            startTimestamp = active ? Stopwatch.GetTimestamp() : 0L;
        }

        internal static DeucarianLogScope Start(DLog logger, string operation, DeucarianLogLevel level, Object context)
        {
            if (logger == null || !DeucarianLog.IsEnabledFor(level))
            {
                return default;
            }

            var scope = new DeucarianLogScope(logger, operation, level, context, true);
            DeucarianLog.Dispatch(logger.Category, level, "Started: " + scope.operation, null, context);
            return scope;
        }

        /// <summary>
        /// Logs operation completion once. Calling this method more than once is safe.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (!active)
            {
                return;
            }

            active = false;
            double elapsedMilliseconds = GetElapsedMilliseconds();
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "Completed: {0} in {1:0.###} ms",
                operation,
                elapsedMilliseconds);

            DeucarianLog.Dispatch(logger.Category, level, message, null, context);
        }

        private double GetElapsedMilliseconds()
        {
            long elapsedTicks = Stopwatch.GetTimestamp() - startTimestamp;
            return elapsedTicks * 1000.0 / Stopwatch.Frequency;
        }
    }
}
