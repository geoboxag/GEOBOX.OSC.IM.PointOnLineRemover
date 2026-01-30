using System;
using System.IO;
using System.Text;

namespace GEOBOX.OSC.Common.Logging
{
    /// <summary>
    /// File-Logger, write Log-Messages in Text-File.
    /// </summary>
    public sealed class FileLogger : ILogger
    {
        /// <summary>
        /// Full file name for the log file.
        /// </summary>
        public string FullFilename {get; private set;}
        private readonly StreamWriter logFileWriter;
        private readonly bool disposeWriter;
        private bool isDisposed;

        /// <summary>
        /// Constructor for FileLogger
        /// </summary>
        /// <param name="logFileWriter">StreamWriter</param>
        /// <param name="fullFilename">The full filename</param>
        /// <param name="disposeWriter">True if writer should be disposed if this object is disposed.</param>
        public FileLogger(StreamWriter logFileWriter, string fullFilename, bool disposeWriter)
        {
            this.logFileWriter = logFileWriter;
            FullFilename = fullFilename;
            this.disposeWriter = disposeWriter;
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~FileLogger()
        {
            Dispose(/*isDisposing*/false);
        }

        /// <summary>
        /// Creates and initiliazes a new FileLogger.
        /// </summary>
        /// <param name="fullFilename">Name of the full log file.</param>
        /// <param name="append">True; if we should write a the end of an existing file.</param>
        /// <returns></returns>
        public static ILogger Create(string fullFilename, bool append = true)
        {
            try
            {
                var streamWriter = new StreamWriter(fullFilename, /*append*/append, Encoding.UTF8);
                streamWriter.AutoFlush = true;
                return new FileLogger(streamWriter, fullFilename, /*disposeWriter*/true);
            }
            catch
            {
                return new TraceLogger();
            }
        }

        /// <summary>
        /// Writes message as information to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public void WriteInformation(string message)
        {
            WriteLine(message);
        }

        /// <summary>
        /// Writes message as warning to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public void WriteWarning(string message)
        {
            WriteLine(message);
        }

        /// <summary>
        /// Writes message as error to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public void WriteError(string message)
        {
            WriteLine(message);
        }

        /// <summary>
        /// Writes a line to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public void WriteLine(string message)
        {
            logFileWriter.WriteLine(message);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(/*isDisposing*/true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposed)
            {
                return;
            }
            isDisposed = true;

            if (isDisposing)
            {
                if (disposeWriter)
                {
                    logFileWriter?.Dispose();
                }
            }
        }


    }
}
