using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bucket.Tracer
{
    public static class TraceTagExtensions
    {
        // 空格、回车、换行符、制表符正则表达式
        private static Regex _tbbrRegex = new Regex(@"\s*|\t|\r|\n", RegexOptions.IgnoreCase);
        public static TraceTags Set(this TraceTags traceTags, string key, string value)
        {
            if (traceTags == null)
            {
                throw new ArgumentNullException(nameof(traceTags));
            }
            traceTags[key] = value;
            return traceTags;
        }
        public static TraceTags Set(this TraceTags traceTags, string key, bool value)
        {
            return Set(traceTags, key, value.ToString());
        }

        public static TraceTags Set(this TraceTags traceTags, string key, int value)
        {
            return Set(traceTags, key, value.ToString());
        }

        public static TraceTags Set(this TraceTags traceTags, string key, long value)
        {
            return Set(traceTags, key, value.ToString());
        }

        public static TraceTags Set(this TraceTags traceTags, string key, float value)
        {
            return Set(traceTags, key, value.ToString());
        }

        public static TraceTags Set(this TraceTags traceTags, string key, double value)
        {
            return Set(traceTags, key, value.ToString());
        }

        public static TraceTags Component(this TraceTags traceTags, string component)
        {
            return Set(traceTags, TraceTagKeys.Component, component);
        }
        public static TraceTags Error(this TraceTags traceTags, string error)
        {
            return Set(traceTags, TraceTagKeys.Error, error);
        }

        public static TraceTags HttpMethod(this TraceTags traceTags, string httpMethod)
        {
            return Set(traceTags, TraceTagKeys.HttpMethod, httpMethod);
        }
        public static TraceTags HttpStatusCode(this TraceTags traceTags, int httpStatusCode)
        {
            return Set(traceTags, TraceTagKeys.HttpStatusCode, httpStatusCode);
        }

        public static TraceTags PeerAddress(this TraceTags traceTags, string peerAddress)
        {
            return Set(traceTags, TraceTagKeys.PeerAddress, peerAddress);
        }
        public static TraceTags PeerPort(this TraceTags traceTags, int peerPort)
        {
            return Set(traceTags, TraceTagKeys.PeerPort, peerPort);
        }

        public static TraceTags RequstBody(this TraceTags traceTags, string body)
        {
            return Set(traceTags, TraceTagKeys.RequestBody, _tbbrRegex.Replace(body, ""));
        }

        public static TraceTags ResponseBody(this TraceTags traceTags, string body)
        {
            return Set(traceTags, TraceTagKeys.ResponseBody, _tbbrRegex.Replace(body, ""));
        }
        public static TraceTags ContentType(this TraceTags traceTags, string mediaType)
        {
            return Set(traceTags, TraceTagKeys.ContentType, mediaType);
        }
    }
}
