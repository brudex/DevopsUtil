﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Reflection;
using log4net;

namespace DevopsUtil
{
    public static class Logger
    {

        private static readonly string LOG_CONFIG_FILE = @"log4net.config";
        private static readonly Dictionary<Type, ILog> _loggers = new Dictionary<Type, ILog>();
        private static bool _logInitialized;
        private static readonly object _lock = new object();

        public static string SerializeException(Exception exception)
        {
            return SerializeException(exception, string.Empty);
        }

        private static string SerializeException(Exception e, string exceptionMessage)
        {
            if (e == null) return string.Empty;
            exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                "{0}{1}{2}\n{3}",
                exceptionMessage,
                string.IsNullOrEmpty(exceptionMessage) ? string.Empty : "\n\n",
                e.Message,
                e.StackTrace);
            if (e.InnerException != null)
                exceptionMessage = SerializeException(e.InnerException, exceptionMessage);
            return exceptionMessage;
        }

        private static ILog getLogger(Type source)
        {
            lock (_lock)
            {
                if (_loggers.ContainsKey(source))
                {
                    return _loggers[source];
                }
                else
                {
                    ILog logger = LogManager.GetLogger(source);
                    _loggers.Add(source, logger);
                    return logger;
                }
            }
        }

        /* Log a message object */

        public static void Debug(object source, string message)
        {
            Debug(source.GetType(), message);
        }

        public static void Debug(object source, string message, params object[] ps)
        {
            Debug(source.GetType(), string.Format(message, ps));
        }

        public static void Debug(Type source, string message)
        {
            ILog logger = getLogger(source);
            if (logger.IsDebugEnabled)
                logger.Debug(message);
        }

        public static void Info(object source, object message)
        {
            Info(source.GetType(), message);
        }

        public static void Info(Type source, object message)
        {
            ILog logger = getLogger(source);
            if (logger.IsInfoEnabled)
                logger.Info(message);
        }

        public static void Warn(object source, object message)
        {
            Warn(source.GetType(), message);
        }

        public static void Warn(Type source, object message)
        {
            ILog logger = getLogger(source);
            if (logger.IsWarnEnabled)
                logger.Warn(message);
        }

        public static void Error(object source, object message)
        {
            Error(source.GetType(), message);
        }

        public static void Error(Type source, object message)
        {
            ILog logger = getLogger(source);
            if (logger.IsErrorEnabled)
                logger.Error(message);
        }

        public static void Fatal(object source, object message)
        {
            Fatal(source.GetType(), message);
        }

        public static void Fatal(Type source, object message)
        {
            ILog logger = getLogger(source);
            if (logger.IsFatalEnabled)
                logger.Fatal(message);
        }

        /* Log a message object and exception */

        public static void Debug(object source, object message, Exception exception)
        {
            Debug(source.GetType(), message, exception);
        }

        public static void Debug(Type source, object message, Exception exception)
        {
            getLogger(source).Debug(message, exception);
        }

        public static void Info(object source, object message, Exception exception)
        {
            Info(source.GetType(), message, exception);
        }

        public static void Info(Type source, object message, Exception exception)
        {
            getLogger(source).Info(message, exception);
        }

        public static void Warn(object source, object message, Exception exception)
        {
            Warn(source.GetType(), message, exception);
        }

        public static void Warn(Type source, object message, Exception exception)
        {
            getLogger(source).Warn(message, exception);
        }

        public static void Error(object source, object message, Exception exception)
        {
            Error(source.GetType(), message, exception);
        }

        public static void Error(Type source, object message, Exception exception)
        {
            getLogger(source).Error(message, exception);
        }

        public static void Fatal(object source, object message, Exception exception)
        {
            Fatal(source.GetType(), message, exception);
        }

        public static void Fatal(Type source, object message, Exception exception)
        {
            getLogger(source).Fatal(message, exception);
        }

        private static void initialize(string appPath)
        {

            _logInitialized = true;
            XmlDocument log4netConfig = new XmlDocument();
            if (string.IsNullOrEmpty(appPath))
            {
                log4netConfig.Load(File.OpenRead(LOG_CONFIG_FILE));
            }
            else
            {
                log4netConfig.Load(File.OpenRead(System.IO.Path.Combine(appPath,LOG_CONFIG_FILE)));
            }
            
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }

        public static void EnsureInitialized(string appPath="")
        {
            if (!_logInitialized)
            {
                initialize(appPath);
            }
        }
    }
}
