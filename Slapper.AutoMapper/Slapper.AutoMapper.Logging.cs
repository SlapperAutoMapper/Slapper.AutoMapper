/*  Slapper.AutoMapper v1.0.0.6 ( https://github.com/SlapperAutoMapper/Slapper.AutoMapper )

    MIT License:
   
    Copyright (c) 2016, Randy Burden ( http://randyburden.com ) and contributors. All rights reserved.
    All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
    associated documentation files (the "Software"), to deal in the Software without restriction, including 
    without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
    copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
    following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial 
    portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
    LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN 
    NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
    SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

    Description:
    
    Slapper.AutoMapper maps dynamic data to static types. Slap your data into submission!
    
    Slapper.AutoMapper ( Pronounced Slapper-Dot-Automapper ) is a single file mapping library that can convert 
    dynamic data into static types and populate complex nested child objects.
    It primarily converts C# dynamics and IDictionary<string, object> to strongly typed objects and supports
    populating an entire object graph by using underscore notation to underscore into nested objects.
*/

using System;
using System.Diagnostics;

namespace Slapper
{
    public static partial class AutoMapper
    {
        #region Logging

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

        #endregion Logging
    }
}
