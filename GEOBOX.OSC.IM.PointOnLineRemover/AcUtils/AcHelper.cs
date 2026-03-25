using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace GEOBOX.OSC.IM.PointOnLineRemover.AcUtils
{
    /// <summary>
    /// Several functions to simplify getting AutoCAD-objects or calling AutoCAD functions.
    /// Helper for AutoCAD - Editor, Document, Transaction, and many more
    /// </summary>
    public class AcHelper
    {
        /// <summary>
        /// Gets the editor associated with the active document.
        /// </summary>
        /// <returns></returns>
        public static Editor Editor() { return Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor; }

        /// <summary>
        /// Accesses the active document
        /// </summary>
        /// <returns></returns>
        public static Document Doc() { return Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument; }

        /// <summary>
        /// Accesses the active document
        /// </summary>
        /// <returns></returns>
        public static Document MdiActiveDocument
        {
            get { return Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument; }
        }

        /// <summary>
        /// Accesses the DocumentManager from ApplicationServices
        /// </summary>
        /// <returns></returns>
        public static DocumentCollection DocumentManager
        {
            get { return Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager; }
        }

        /// <summary>
        /// Accesses the working database
        /// </summary>
        /// <returns></returns>
        public static Database DB() { return Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase; }

        /// <summary>
        /// Accesses the working database
        /// </summary>
        /// <returns></returns>
        public static Database Database
        {
            get { return Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase; }
        }

        /// <summary>
        /// Accesses the TransactionManager from WorkingDatabase
        /// </summary>
        /// <returns></returns>
        public static Autodesk.AutoCAD.DatabaseServices.TransactionManager TM() { return HostApplicationServices.WorkingDatabase.TransactionManager; }

        /// <summary>
        /// Accesses the TransactionManager from WorkingDatabase
        /// </summary>
        /// <returns></returns>
        public static Autodesk.AutoCAD.DatabaseServices.TransactionManager TransactionManager
        {
            get { return HostApplicationServices.WorkingDatabase.TransactionManager; }
        }

        /// <summary>
        /// Static field to a transaction object (handle with care -> could be null)
        /// </summary>
        public static Autodesk.AutoCAD.DatabaseServices.Transaction TR = null;

        #region message to CmdLine
        /// <summary>
        /// writes a message to the AutoCAD Command Line (with NewLine)
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteMessage(string msg)
        {
            Editor().WriteMessage("\r\n" + msg);
        }
        /// <summary>
        /// appends a message to the AutoCAD Command Line
        /// </summary>
        /// <param name="msg"></param>
        public static void AppendMessage(string msg)
        {
            Editor().WriteMessage(msg);
        }

        /// <summary>
        /// writes a message to the AutoCAD Command Line (with NewLine)
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteMessageToCmdLine(string msg)
        {
            Editor().WriteMessage("\r\n" + msg);
        }
        /// <summary>
        /// appends a message to the AutoCAD Command Line
        /// </summary>
        /// <param name="msg"></param>
        public static void AppendMessageToCmdLine(string msg)
        {
            Editor().WriteMessage(msg);
        }
        #endregion

        #region various
        /// <summary>
        /// Send a string to execute in the AutoCAD CommandLine (silent)
        /// </summary>
        /// <param name="cmd"></param>
        public static void SendStringToExecute(string cmd)
        {
            Doc().SendStringToExecute(AddNewLineSuffix(cmd), true, false, false);
        }

        /// <summary>
        /// Send a string to execute in the AutoCAD CommandLine with echo
        /// </summary>
        /// <param name="cmd"></param>
        public static void SendStringToExecuteWithEcho(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
            {
                return;
            }

            Doc().SendStringToExecute(AddNewLineSuffix(cmd), /*activate*/true,/*warpUpInactiveDoc*/false,/*echoCommand*/true);
        }

        /// <summary>
        /// Adds NewLine string if text is not suffixed with NewLine.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>text with NewLine as suffix.</returns>
        private static string AddNewLineSuffix(string text)
        {
            if (!text.EndsWith("\n"))
            {
                text = text + "\n"; // needs to be \n and not \r\n, environment.newline or anything else!!
            }
            return text;
        }

        /// <summary>
        /// Send a string to execute in the AutoCAD CommandLine in DebugMode with echo
        /// </summary>
        /// <param name="cmd"></param>
        public static void SendStringToExecuteD(String cmd)
        {
#if _DEBUG
			SendStringToExecuteWithEcho(cmd);
#else
            SendStringToExecute(cmd);
#endif
        }

        /// <summary>
        /// Send Escape Command to AutoCAD CommandLine
        /// </summary>
        public static void SendEscape()
        {
            // http://through-the-interface.typepad.com/through_the_interface/2006/08/cancelling_an_a.html
            MdiActiveDocument.SendStringToExecute("\x1B", /*activate*/false,/*wrapUpInactiveDoc*/true,/*echoCommand*/false);
        }
        #endregion

        #region Error Handler
        /// <summary>
        /// handles a Exception and writes the message to the AutoCAD Command Line
        /// </summary>
        /// <param name="ex"></param>
        static public void ErrorToCmdLine(System.Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            string errText = string.Empty;
            //string errText = "Error - Failed to perform the operation \n";
            errText += ex.GetType() + Environment.NewLine;
            // exceptions are likely to contain inner exceptions that provide further detail about the error.
            errText += GetAllExceptionMessages(ex);
            WriteMessage(errText);
            // MessageBox.Show(this, errText, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Gets all exception messages (also from all InnerExceptions).
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>all exception messages separated by NewLine.</returns>
        private static string GetAllExceptionMessages(System.Exception ex)
        {
            // exceptions are likely to contain inner exceptions that provide further detail about the error.
            var errText = new StringBuilder();
            while (ex != null)
            {
                errText.AppendLine(ex.Message);
                ex = ex.InnerException;
            }
            return errText.ToString();
        }

        /// <summary>
        /// handles a Exception and writes the message to the AutoCAD Command Line
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        static public void ErrorToCmdLine(System.Exception ex, string message)
        {
            if (ex == null)
            {
                return;
            }

            string errText = message + Environment.NewLine + "Error - Failed to perform the operation \n";

            errText += GetAllExceptionMessages(ex);
            WriteMessage(errText);
            // MessageBox.Show(this, errText, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region Colors
        /// <summary>
        /// create and returns a Color from given colorIndex
        /// </summary>
        /// <param name="colorIndex"></param>
        /// <returns></returns>
        public static Color FromColorIndex(short colorIndex)
        {
            return Color.FromColorIndex(ColorMethod.ByPen, colorIndex);
        }

        /// <summary>
        /// create and returns a Color from given colorString
        /// Allowed string formats are: "0xFF008B8B" or "ff010203"
        /// </summary>
        /// <param name="colorString">ColorString</param>
        /// <returns>AutoCAD Color</returns>
        public static Autodesk.AutoCAD.Colors.Color ConvertColor(string colorString)
        {
            Color color = new Color();
            try
            {
                // parse "0xFF008B8B" or "ff010203"
                short begin = -1;
                if (colorString.Length == 10)
                    begin = 04;
                else if (colorString.Length == 8)
                    begin = 02;
                else if (colorString.Length == 6)
                    begin = 00;
                if (begin > 0)
                {
                    byte red = Convert.ToByte(colorString.Substring(begin, 2), 16);
                    byte green = Convert.ToByte(colorString.Substring(begin + 2, 2), 16);
                    byte blue = Convert.ToByte(colorString.Substring(begin + 4, 2), 16);
                    color = Color.FromRgb(red, green, blue);
                }

                //colorConverter.ConvertFromInvariantString("ff010203"); // -> Exception
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                string message = string.Format("\r\nMethod failed:{0} \r\nErrorStatus: {1} \r\n{2}",
                     MethodBase.GetCurrentMethod().ToString(), ex.Message, ex.StackTrace);
                AcHelper.WriteMessage(message);
            }
            catch (System.Exception ex)
            {
                string message = string.Format("Method failed:{0} \r\n{1} \r\n{2}",
                     MethodBase.GetCurrentMethod().ToString(), ex.Message, ex.StackTrace);
                Debug.WriteLine(message);
                AcHelper.Editor().WriteMessage(message);
            }
            return color;
        }
        #endregion

        #region other Commands
        /// <summary>
        /// Write a simple message "Ok." to the AutoCAD Command Line
        /// </summary>
        [CommandMethod("Ac_Ok", CommandFlags.Modal | CommandFlags.NoHistory)]
        public void AcOk()
        {
            AcHelper.WriteMessageToCmdLine("Ok.");
        }

        /// <summary>
        /// Call Ac_Ok Command for write a "Ok."
        /// </summary>
        public static void SendAcOk()
        {
            AcHelper.SendStringToExecute("Ac_ok");
        }
        #endregion
    }
}