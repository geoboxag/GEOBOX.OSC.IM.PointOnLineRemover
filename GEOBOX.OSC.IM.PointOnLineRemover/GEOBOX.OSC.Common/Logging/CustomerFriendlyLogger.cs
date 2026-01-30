using System;

namespace GEOBOX.OSC.Common.Logging
{
    /// <summary>
    /// CustomerFriendlyLogger. Logs messages and remembers statistics
    /// to make summaries for the end user (or "log reader").
    /// </summary>
    public class CustomerFriendlyLogger : ILogger
    {
        /// <summary>
        /// The logger that information is logged to.
        /// </summary>
        public ILogger ToLogger { get; private set; }

        private int errorCount;
        /// <summary>
        /// Count of errors.
        /// </summary>
        public int ErrorCount => errorCount;

        private int warningCount;
        /// <summary>
        /// Count of warnings.
        /// </summary>
        public int WarningCount => warningCount;

        private int infoCount;
        private readonly bool disposeLogger;
        private bool isDisposed;

        /// <summary>
        /// Count of informations.
        /// </summary>
        public int InfoCount => infoCount;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="toLogger">Logger to write to.</param>
        /// <param name="disposeLogger">if true, ToLogger will be disposed if this object is disposed.</param>
        public CustomerFriendlyLogger(ILogger toLogger, bool disposeLogger)
        {
            ToLogger = toLogger;
            this.disposeLogger = disposeLogger;
        }

        /// <summary>
        ///Destructor.
        /// </summary>
        ~CustomerFriendlyLogger()
        {
            Dispose(/*isDisposing*/false);
        }

        /// <summary>
        /// Writes message as information to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public virtual void WriteInformation(string message)
        {
            ToLogger.WriteInformation($"INF: {message}");
            infoCount++;
        }

        /// <summary>
        /// Writes message as warning to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public virtual void WriteWarning(string message)
        {
            ToLogger.WriteWarning($"WRN: {message}");
            warningCount++;
        }

        /// <summary>
        /// Writes message as error to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public virtual void WriteError(string message)
        {
            ToLogger.WriteError($"ERR: {message}");
            errorCount++;
        }

        /// <summary>
        /// Writes a line to the logger.
        /// </summary>
        /// <param name="message">Message</param>
        public virtual void WriteLine(string message)
        {
            ToLogger.WriteLine(message);
        }

        /// <summary>
        /// Writes a time entry to the log (format 0:dd.MM.yyyy HH:mm:ss)
        /// </summary>
        public virtual void WriteTime()
        {
            WriteLine($"TIM: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        }


        /// <summary>
        /// Writes a file header.
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="documentName">Document name</param>
        public virtual void WriteHeader(string productName, string comment)
        {
            WriteLine($"** GEOBOX Logger - {productName} **");
            WriteLine($"** {DateTime.Now:dd.MM.yyyy HH:mm:ss} - {comment} **");
            WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes a commentbox with a comment to the logger.
        /// </summary>
        /// <param name="comment">Comment</param>
        /// <param name="withTime">with Timestamp</param>
        public virtual void WriteCommentBox(string comment, bool withTime = false)
        {
            WriteCommentBox(new[] { comment }, withTime);
        }

        /// <summary>
        /// Writes a commentbox with any comments to the logger.
        /// </summary>
        /// <param name="comments">Comments</param>
        /// <param name="withTime">with Timestamp</param>
        public virtual void WriteCommentBox(string[] comments, bool withTime = false)
        {
            WriteLine(string.Empty);
            WriteComment("============================================");
            foreach (var comment in comments)
            {
                WriteComment($"{(withTime ? $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} - {comment}" : comment)}");
            }
            WriteComment("============================================");
            WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes a comment to the logger.
        /// </summary>
        /// <param name="comment">Comment</param>
        public virtual void WriteComment(string comment)
        {
            ToLogger.WriteLine($"// {comment}");
        }

        /// <summary>
        /// Writes a file footer.
        /// </summary>
        public virtual void WriteFooter()
        {
            WriteLine(string.Empty);

            WriteLine(
                $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} - {((HasErrors || HasWarnings) ? "Mit Fehler und/oder Warnungen beendet." : "Ohne Meldungen beendet.")}");
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
                if (ToLogger != null && disposeLogger)
                {
                    ToLogger.Dispose();
                    ToLogger = null;
                }

            }
        }

        /// <summary>
        /// Returns true if HasErrors count > 0
        /// </summary>
        public bool HasErrors => errorCount > 0;

        /// <summary>
        /// Returns true if HasWarnings Count > 0
        /// </summary>
        public bool HasWarnings => warningCount > 0;

        /// <summary>
        /// Returns true if information count is > 0.
        /// </summary>
        public bool HasInformations => infoCount > 0;
    }
}
