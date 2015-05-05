using System;
using System.Diagnostics;

namespace Slapper
{
    public static partial class AutoMapper
    {
        /// <summary>
        /// Contains the methods and members responsible for this libraries logging concerns.
        /// </summary>
        public class Logging
        {
            /// <summary>
            /// Logger for this library.
            /// </summary>
            public static ILogger Logger = new TraceLogger();

            /// <summary>
            /// Log levels.
            /// </summary>
            public enum LogLevel
            {
                /// <summary>
                /// Logs debug level messages.
                /// </summary>
                Debug,

                /// <summary>
                /// Logs info level messages.
                /// </summary>
                Info,

                /// <summary>
                /// Logs warning level messages.
                /// </summary>
                Warn,

                /// <summary>
                /// Logs error level messages.
                /// </summary>
                Error,

                /// <summary>
                /// Logs fatal level messages.
                /// </summary>
                Fatal
            }

            /// <summary>
            /// Contains methods for logging messages.
            /// </summary>
            public interface ILogger
            {
                /// <summary>
                /// Logs messages.
                /// </summary>
                /// <param name="logLevel">Log level.</param>
                /// <param name="format">Log message format.</param>
                /// <param name="args">Log message arguments.</param>
                void Log(LogLevel logLevel, string format, params object[] args);

                /// <summary>
                /// Logs exception messages.
                /// </summary>
                /// <param name="logLevel">Log level.</param>
                /// <param name="exception">Exception being logged.</param>
                /// <param name="format">Log message format.</param>
                /// <param name="args">Log message arguments.</param>
                void Log(LogLevel logLevel, Exception exception, string format, params object[] args);
            }

            /// <summary>
            /// Logs messages to any trace listeners.
            /// </summary>
            public class TraceLogger : ILogger
            {
                /// <summary>
                /// The minimum log level that this class will log messages for.
                /// </summary>
                public static LogLevel MinimumLogLevel = LogLevel.Error;

                #region Implementation of ILogger

                /// <summary>
                /// Logs messages to any trace listeners.
                /// </summary>
                /// <param name="logLevel">Log level.</param>
                /// <param name="format">Log message format.</param>
                /// <param name="args">Log message arguments.</param>
                public void Log(LogLevel logLevel, string format, params object[] args)
                {
                    if (logLevel >= MinimumLogLevel)
                    {
                        Trace.WriteLine(args == null ? format : string.Format(format, args));
                    }
                }

                /// <summary>
                /// Logs messages to any trace listeners.
                /// </summary>
                /// <param name="logLevel">Log level.</param>
                /// <param name="exception">Exception being logged.</param>
                /// <param name="format">Log message format.</param>
                /// <param name="args">Log message arguments.</param>
                public void Log(LogLevel logLevel, Exception exception, string format, params object[] args)
                {
                    if (logLevel >= MinimumLogLevel)
                    {
                        string output = args == null ? format : string.Format(format, args);

                        output += ". Exception: " + exception.Message;

                        Trace.WriteLine(output);
                    }
                }

                #endregion Implementation of ILogger
            }
        }
    }
}
