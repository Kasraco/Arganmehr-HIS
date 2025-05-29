using DomainClasses.Entities.Users;
using Common.Helpers.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.Logg
{
    public static class LoggingExtensions
    {

        public static void Debug(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, user);
        }

        public static void Information(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception, user);
        }

        public static void Warning(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception, user);
        }

        public static void Error(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception, user);
        }

        public static void Fatal(this ILogger logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception, user);
        }

        public static void Error(this ILogger logger, Exception exception, User user = null)
        {
            FilteredLog(logger, LogLevel.Error, exception.ToAllMessages(), exception, user);
        }

        public static void ErrorsAll(this ILogger logger, Exception exception, User user = null)
        {
            while (exception != null)
            {
                FilteredLog(logger, LogLevel.Error, exception.Message, exception, user);

                exception = exception.InnerException;
            }
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception = null, User user = null)
        {
            // don't log thread abort exception
            if ((exception != null) && (exception is System.Threading.ThreadAbortException))
                return;

            if (logger.IsEnabled(level))
            {
                string fullMessage = exception == null ? string.Empty : exception.StackTrace;
                logger.InsertLog(level, message, fullMessage, user);
            }
        }
    }
}
