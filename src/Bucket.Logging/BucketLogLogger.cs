using Bucket.EventBus.Common.Events;
using Bucket.Logging.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bucket.Logging
{
    public class BucketLogLogger : ILogger
    {
        private readonly string _projectName;
        private readonly IEventBus _eventBus;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BucketLogLogger(IEventBus eventBus, IHttpContextAccessor httpContextAccessor, string projectName)
        {
            _eventBus = eventBus;
            _projectName = projectName;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
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
                var expt = exception;
                while (expt != null)
                {
                    message += "→" + (Convert.IsDBNull(expt.Message) ? "" : expt.Message) + "\r\n";
                    expt = expt.InnerException;
                }
                // message += exception.StackTrace;
            }

            if (!string.IsNullOrEmpty(message)
                || exception != null)
            {
                if (_eventBus != null)
                {
                    var url = string.Empty;
                    var ip = string.Empty;
                    if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null) {
                        ip = GetServerIp(_httpContextAccessor.HttpContext);
                        url = new StringBuilder().Append(_httpContextAccessor.HttpContext.Request.Scheme)
                                                 .Append("://")
                                                 .Append(_httpContextAccessor.HttpContext.Request.Host)
                                                 .Append(_httpContextAccessor.HttpContext.Request.PathBase)
                                                 .Append(_httpContextAccessor.HttpContext.Request.Path)
                                                 .Append(_httpContextAccessor.HttpContext.Request.QueryString)
                                                 .ToString();
                    }
                    if(message.Length > 5120)
                    {
                        // 日志长度过长,暂时不采用其他解决方案
                        message = Substring2(message, 5120);
                    }
                    var model = new LogEvent()
                    {
                        LogMessage = message,
                        LogType = logLevel.ToString(),
                        ProjectName = _projectName,
                        LogTag = url,
                        IP = ip
                    };
                    _eventBus.PublishAsync(model);
                }
                else
                    Console.WriteLine(message);
            }
        }

        private string Substring2(string message, int v)
        {
            throw new NotImplementedException();
        }

        private string GetUserIp(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
        private string GetServerIp(HttpContext context)
        {
            return context.Connection.LocalIpAddress.ToString();
        }
        /// <summary>
        /// 部分字符串获取
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlen"></param>
        /// <returns></returns>
        private string SubString2(string str, int maxlen)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (str.Length <= maxlen)
                return str;
            return str.Substring(0, maxlen);
        }
    }
}
