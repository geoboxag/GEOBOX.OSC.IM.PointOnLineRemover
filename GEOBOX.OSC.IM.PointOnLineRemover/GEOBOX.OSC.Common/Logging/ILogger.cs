using System;

namespace GEOBOX.OSC.Common.Logging
{
    /// <summary>
    /// Logger interface.
    /// </summary>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// Writes message as information to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        void WriteInformation(string message);
        /// <summary>
        /// Writes message as warning to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        void WriteWarning(string message);
        /// <summary>
        /// Writes message as error to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        void WriteError(string message);

        /// <summary>
        /// Writes a line to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        void WriteLine(string message);
    }
}
