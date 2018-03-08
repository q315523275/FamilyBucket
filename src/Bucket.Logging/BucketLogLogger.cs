using Bucket.EventBus.Common.Events;
using Bucket.Logging.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Logging
{
    public class BucketLogLogger : ILogger
    {
        /// <summary>
        /// event
        /// </summary>
        private readonly IEventBus eventBus;
        public BucketLogLogger(IEventBus eventBus)
        {
            this.eventBus = eventBus;
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
                if (eventBus != null)
                    eventBus.PublishAsync(new PublishLogEvent(logLevel.ToString(), message));
                else
                    Console.WriteLine(message);
            }
        }
    }
}
