namespace As.Applications.Loggers
{

    /// <summary>
    /// Caliburn Micro Log4Net logger, redirecting all Caliburn Micro ILog 'Info' to 'Debug' level.
    /// </summary>
    internal class CmLog4NetLogger : ILogger
    {
        public static ILogger GetLogger(string name)
            => new CmLog4NetLogger(name);

        public static ILogger GetLogger(Type type)
            => new CmLog4NetLogger(type);

        public CmLog4NetLogger(string name)
        {
            log = log4net.LogManager.GetLogger(name);
        }

        public CmLog4NetLogger(Type type)
        {
            log = log4net.LogManager.GetLogger(type);
        }

        readonly log4net.ILog log;

        #region Caliburn.Micro.ILog
        void Caliburn.Micro.ILog.Error(Exception exception)
            => log.Error(exception.Message, exception);

        /// <summary>
        /// Info, redirected to Debug
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Caliburn.Micro.ILog.Info(string format, params object[] args)
            => log.DebugFormat(format, args);

        void Caliburn.Micro.ILog.Warn(string format, params object[] args)
            => log.WarnFormat(format, args);
        #endregion Caliburn.Micro.ILog

        #region log4net.ILog
        public bool IsDebugEnabled => log.IsDebugEnabled;

        public bool IsInfoEnabled => log.IsInfoEnabled;

        public bool IsWarnEnabled => log.IsWarnEnabled;

        public bool IsErrorEnabled => log.IsErrorEnabled;

        public bool IsFatalEnabled => log.IsFatalEnabled;

        public log4net.Core.ILogger Logger => log.Logger;

        public void Debug(object? message)
            => log.Debug(message);

        public void Debug(object? message, Exception? exception)
            => log.Debug(message, exception);

        public void DebugFormat(string format, params object?[]? args)
            => log.DebugFormat(format, args);

        public void DebugFormat(string format, object? arg0)
            => log.DebugFormat(format, arg0);

        public void DebugFormat(string format, object? arg0, object? arg1)
            => log.DebugFormat(format, arg0, arg1);

        public void DebugFormat(string format, object? arg0, object? arg1, object? arg2)
            => log.DebugFormat(format, arg0, arg1, arg2);

        public void DebugFormat(IFormatProvider? provider, string format, params object?[]? args)
            => log.DebugFormat(provider, format, args);

        public void Error(object? message)
            => log.Error(message);

        public void Error(object? message, Exception? exception)
            => log.Error(message, exception);

        public void ErrorFormat(string format, params object?[]? args)
            => log.ErrorFormat(format, args);
        public void ErrorFormat(string format, object? arg0)
            => log.ErrorFormat(format, arg0);

        public void ErrorFormat(string format, object? arg0, object? arg1)
            => log.ErrorFormat(format, arg0, arg1);

        public void ErrorFormat(string format, object? arg0, object? arg1, object? arg2)
            => log.ErrorFormat(format, arg0, arg1, arg2);

        public void ErrorFormat(IFormatProvider? provider, string format, params object?[]? args)
            => log.ErrorFormat(provider, format, args);

        public void Fatal(object? message)
            => log.Fatal(message);

        public void Fatal(object? message, Exception? exception)
            => log.Fatal(message, exception);

        public void FatalFormat(string format, params object?[]? args)
            => log.FatalFormat(format, args);

        public void FatalFormat(string format, object? arg0)
            => log.FatalFormat(format, arg0);

        public void FatalFormat(string format, object? arg0, object? arg1)
            => log.FatalFormat(format, arg0, arg1);

        public void FatalFormat(string format, object? arg0, object? arg1, object? arg2)
            => log.FatalFormat(format, arg0, arg1, arg2);

        public void FatalFormat(IFormatProvider? provider, string format, params object?[]? args)
            => log.FatalFormat(format, args);

        public void Info(object? message)
            => log.Info(message);

        public void Info(object? message, Exception? exception)
            => log.Info(message, exception);

        public void InfoFormat(string format, params object?[]? args)
            => log.InfoFormat(format, args);

        public void InfoFormat(string format, object? arg0)
            => log.InfoFormat(format, arg0);

        public void InfoFormat(string format, object? arg0, object? arg1)
            => log.InfoFormat(format, arg0, arg1);

        public void InfoFormat(string format, object? arg0, object? arg1, object? arg2)
            => log.InfoFormat(format, arg0, arg1, arg2);

        public void InfoFormat(IFormatProvider? provider, string format, params object?[]? args)
            => log.InfoFormat(provider, format, args);

        public void Warn(object? message)
            => log.Warn(message);

        public void Warn(object? message, Exception? exception)
            => log.Warn(message, exception);

        public void WarnFormat(string format, params object?[]? args)
            => log.WarnFormat(format, args);

        public void WarnFormat(string format, object? arg0)
            => log.WarnFormat(format, arg0);

        public void WarnFormat(string format, object? arg0, object? arg1)
            => log.WarnFormat(format, arg0, arg1);

        public void WarnFormat(string format, object? arg0, object? arg1, object? arg2)
            => log.WarnFormat(format, arg0, arg1, arg2);

        public void WarnFormat(IFormatProvider? provider, string format, params object?[]? args)
            => log.WarnFormat(provider, format, args);
        #endregion log4net.ILog
    }
}
