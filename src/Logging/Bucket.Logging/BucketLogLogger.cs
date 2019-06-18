using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Bucket.Logging
{
    public class BucketLogLogger : ILogger
    {
        private readonly string _applicationName;
        private readonly string _className;
        private readonly ILoggerDispatcher _loggerDispatcher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BucketLogLogger(ILoggerDispatcher loggerDispatcher, IHttpContextAccessor httpContextAccessor, string applicationName, string className)
        {
            _loggerDispatcher = loggerDispatcher;
            _applicationName = applicationName;
            _className = className;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <summary>
        /// Logs an exception into the log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event Id.</param>
        /// <param name="state">The state.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="formatter">The formatter.</param>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <exception cref="ArgumentNullException">Throws when the <paramref name="formatter"/> is null.</exception>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            if (null == formatter)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = null;
            if (null != formatter)
            {
                message = formatter(state, exception);
            }

            if (null != exception)
            {
                message = string.Format("【抛出信息】：{0} <br />【异常类型】：{1} <br />【异常信息】：{2} <br />【堆栈调用】：{3}", new object[] { message, exception.GetType().Name, exception.Message, exception.StackTrace });
                message = message.Replace("\r\n", "<br />");
                message = message.Replace("位置", "<strong style=\"color:red\">位置</strong>");
            }

            if (!string.IsNullOrEmpty(message)
                || exception != null)
            {
                if (_loggerDispatcher != null)
                {
                    var localPort = string.Empty;
                    if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
                        localPort = ":" + _httpContextAccessor.HttpContext?.Connection?.LocalPort.ToString();
                    // 日志长度过长,暂时不采用其他解决方案
                    if (message.Length > 5120)
                        message = Substring2(message, 5120);
                    _loggerDispatcher.Dispatch(new LogMessageEntry()
                    {
                        ProjectName = _applicationName,
                        ClassName = _className,
                        LocalIp = GetLanIp() + localPort,
                        AddTime = DateTime.Now,
                        LogMessage = message,
                        LogType = logLevel.ToString(),
                        LogTag = _httpContextAccessor?.HttpContext?.Request?.GetDisplayUrl(),
                        TraceHead = _httpContextAccessor?.HttpContext?.Request?.Headers["skyapm"].FirstOrDefault()
                    });
                }
                else
                    Console.WriteLine(message);
            }
        }

        /// <summary>
        /// 获取局域网IP
        /// </summary>
        private string GetLanIp()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address))
                .FirstOrDefault()?.Address.ToString();
        }
        /// <summary>
        /// 部分字符串获取
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlen"></param>
        /// <returns></returns>
        private string Substring2(string str, int maxlen)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (str.Length <= maxlen)
                return str;
            return str.Substring(0, maxlen);
        }
    }
}
