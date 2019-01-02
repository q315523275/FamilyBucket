using Bucket.SkrTrace.Core.OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.SkrTrace.Core.Extensions
{
    public static class LogFieldExtensions
    {
        private static LogField Set(this LogField logField, string key, string value)
        {
            if (logField == null)
            {
                throw new ArgumentNullException(nameof(logField));
            }

            logField[key] = value;
            return logField;
        }

        public static LogField Event(this LogField logField, string eventName)
        {
            return logField.Set(LogFields.Event, eventName);
        }

        public static LogField EventError(this LogField logField)
        {
            return logField.Set(LogFields.Event, LogFields.Error);
        }

        public static LogField Message(this LogField logField, string message)
        {
            return logField.Set(LogFields.Message, message);
        }

        public static LogField Stack(this LogField logField, string stack)
        {
            return logField.Set(LogFields.Stack, stack);
        }

        public static LogField ErrorKind(this LogField logField, string errorKind)
        {
            return logField.Set(LogFields.ErrorKind, errorKind);
        }

        public static LogField ErrorObject(this LogField logField, Exception exception)
        {
            return logField.Set(LogFields.ErrorObject, exception.Message);
        }

        public static LogField ErrorKind<TException>(this LogField logField) where TException : Exception
        {
            return logField.ErrorKind(typeof(TException).FullName);
        }

        public static LogField ErrorKind<TException>(this LogField logField, TException exception) where TException : Exception
        {
            return logField.ErrorKind(exception?.GetType()?.FullName);
        }
    }
}
