using Elogbook.Core.Enums;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System.IO;
using System.Linq;

namespace Elogbook.Core.Log
{
    /// <summary>
    /// Log4net custom
    /// </summary>
    public static class LogCustom
    {
        private static ILog _log = null;

        /// <summary>
        /// Init log4net to raise log for API Service
        /// </summary>
        /// <param name="ApplicationPath"></param>
        public static void Initialize(string ApplicationPath)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(ApplicationPath, AppSettings.LOG4NET_CONFIG_FILE)));

            _log = LogManager.GetLogger(string.Empty);
        }

        /// <summary>
        /// Function log basic
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void LogMessage(TrackingLogLevel level, string message)
        {
            switch (level)
            {
                case TrackingLogLevel.DEBUG:
                    _log.Debug(message);
                    break;

                case TrackingLogLevel.INFO:
                    _log.Info(message);
                    break;

                case TrackingLogLevel.WARN:
                    _log.Warn(message);
                    break;

                case TrackingLogLevel.ERROR:
                    _log.Error(message);
                    break;

                case TrackingLogLevel.FATAL:
                    _log.Fatal(message);
                    break;
            }
        }

        /// <summary>
        /// Funtion log custom message
        /// </summary>
        /// <param name="level"></param>
        /// <param name="category"></param>
        /// <param name="message"></param>
        public static void LogMessage(TrackingLogLevel level, string category, object message)
        {
            switch (level)
            {
                case TrackingLogLevel.DEBUG:
                    _log.Debug(message);
                    break;

                case TrackingLogLevel.INFO:
                    _log.InfoFormat("{0}: {1}", category, message);
                    break;

                case TrackingLogLevel.WARN:
                    _log.Warn(message);
                    break;

                case TrackingLogLevel.ERROR:
                    _log.ErrorFormat("{0}: {1}", category, message);
                    break;

                case TrackingLogLevel.FATAL:
                    _log.Fatal(message);
                    break;
            }
        }

        /// <summary>
        /// Get file path write log
        /// </summary>
        /// <returns></returns>
        public static string GetLogFilePath()
        {
            var rootAppender = ((Hierarchy)LogManager.GetRepository())
                                         .Root.Appenders.OfType<FileAppender>()
                                         .FirstOrDefault();
            var filename = rootAppender != null ? rootAppender.File : string.Empty;

            if (!string.IsNullOrWhiteSpace(filename))
                return Path.GetDirectoryName(filename);
            else
                return string.Empty;
        }
    }
}
