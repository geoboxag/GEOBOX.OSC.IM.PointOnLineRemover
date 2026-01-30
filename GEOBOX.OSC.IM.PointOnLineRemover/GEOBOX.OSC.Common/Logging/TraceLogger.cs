using System;

namespace GEOBOX.OSC.Common.Logging
{
    /// <summary>
    /// Write Log-Messages in Trace.
    /// Standard .NET Logger.
    /// </summary>
    public sealed class TraceLogger : ILogger
    {
        /// <summary>
        /// Writes message as information to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public void WriteInformation(string message)
        {
            WriteInternal(message, info => System.Diagnostics.Trace.TraceInformation(info));
        }

        /// <summary>
        /// Writes message as warning to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public void WriteWarning(string message)
        {
            WriteInternal(message, warning => System.Diagnostics.Trace.TraceWarning(warning));
        }

        /// <summary>
        /// Writes message as error to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public void WriteError(string message)
        {
            WriteInternal(message, error => System.Diagnostics.Trace.TraceError(error));
        }

        /// <summary>
        /// Writes a line to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public void WriteLine(string message)
        {
            WriteInternal(message, line => System.Diagnostics.Trace.WriteLine(line));
        }

        private void WriteInternal(string message, Action<string> writeAction)
        {
            writeAction(message);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose...
        }
    }
}
