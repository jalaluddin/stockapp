using System;
using log4net;
using System.Text;
using System.Web;

namespace StockMarketSharedLibrary
{
    public class Log4NetLogHelper : ILogHelper
    {
        // Primary namespace root for all errors that has been handled
        private const string HANDLED_FILE_LOGGER_NAME = "Handled.Logging";
        // Informative logging 
        private const string INFO_LOGGER_NAME = "Info.Logging";
        // Admin activity logging
        private const string ADMIN_LOGGER_NAME = "Admin.Logging";

        /// <summary>
        /// Returns a log with namespace that is nested in the handled
        /// namespace root and has the class name specified
        /// </summary>
        /// <param name="className">Name of the class where the exception occured</param>
        /// <returns>Returns a logger object</returns>
        private ILog GetHandledFileLogger(string methodSignature)
        {
            methodSignature = CommonUtility.MakeAlphaNumeric(methodSignature);
            return LogManager.GetLogger(HANDLED_FILE_LOGGER_NAME + "." + methodSignature);
        }

        private ILog GetUnhandledFileLogger(string methodSignature)
        {
            methodSignature = CommonUtility.MakeAlphaNumeric(methodSignature);
            return LogManager.GetLogger(methodSignature);
        }

        private ILog GetDBLogger(string methodSignature)
        {
            methodSignature = CommonUtility.MakeAlphaNumeric(methodSignature);
            return LogManager.GetLogger(ADMIN_LOGGER_NAME + "." + methodSignature);
        }

        /// <summary>
        /// Returns a log with namespace that is nested in the handled
        /// namespace root and has the class name specified, used to
        /// monitor admin activity.
        /// </summary>
        /// <param name="className">Name of the class where the exception occured</param>
        /// <returns>Returns a logger object</returns>
        private ILog GetInfoLogger(string methodSignature)
        {
            methodSignature = CommonUtility.MakeAlphaNumeric(methodSignature);
            return LogManager.GetLogger(INFO_LOGGER_NAME + "." + methodSignature);
        }

        /// <summary>
        /// This method is used to log information but for exception logging use
        /// the other overload.
        /// </summary>
        /// <param name="className">Class name where the log is done</param>
        /// <param name="methodName">Method name where the log is done</param>
        /// <param name="message">The detail message that needs to be logged</param>
        public void WriteLog(string className, string methodName, string message)
        {
            ILog log;

            message = CustomizeErrorMessage(message);

            log = GetInfoLogger(className + "." + methodName);
            if (log.IsInfoEnabled)
                log.Info(message);
        }

        /// <summary>
        /// This version of WriteLog is used to log exceptions. To track information log 
        /// use the other overload of the WriteLog method
        /// </summary>
        /// <param name="type">How the exception was tracked</param>
        /// <param name="className">Class where the exception was caught</param>
        /// <param name="methodName">Method where the exception was logged</param>
        /// <param name="ex">The exception that is to be logged</param>
        /// <param name="message">The detail message about the exception log</param>
        public void WriteLog(LogType type, string className, string methodName,
            Exception ex, string message)
        {
            ILog log;

            message = CustomizeErrorMessage(message);

            if (type == LogType.HandledLog)
            {
                log = GetHandledFileLogger(className + "." + methodName);
                if (log.IsErrorEnabled)
                    log.Error(message, ex);
            }
            else if (type == LogType.DBLog)
            {
                log = GetDBLogger(className + "." + methodName);
                if (log.IsWarnEnabled)
                    log.Warn(message, ex);
            }
            else if (type == LogType.UnhandledLog)
            {
                log = GetUnhandledFileLogger(className + "." + methodName);
                if (log.IsFatalEnabled)
                    log.Fatal(message, ex);
            }
            else
            {
                log = GetUnhandledFileLogger(className + "." + methodName);
                if (log.IsFatalEnabled)
                    log.Fatal(message, ex);
            }
        }

        private string CustomizeErrorMessage(string message)
        {
            StringBuilder combinedMessage = new StringBuilder();
            combinedMessage.Append("In Site: ").Append(CommonUtility.GetDomainName()).Append(" -- ");
            combinedMessage.Append("IP: ").Append(CommonUtility.GetUserIPAddress()).Append(" -- ").Append("User: ");
            if (HttpContext.Current != null && HttpContext.Current.User != null &&
                HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
                combinedMessage.Append(HttpContext.Current.User.Identity.Name).Append(" -- ");
            else
                combinedMessage.Append("Guest").Append(" -- ");
            combinedMessage.Append("\n").Append(message);
            return combinedMessage.ToString();
        }
    }
}
