using Bucket.SkrTrace.Core.OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.SkrTrace.Core.Extensions
{
    public static class TagCollectionExtensions
    {
        public static TagCollection Set(this TagCollection tagCollection, string key, string value)
        {
            if (tagCollection == null)
            {
                throw new ArgumentNullException(nameof(tagCollection));
            }
            tagCollection[key] = value;
            return tagCollection;
        }

        public static TagCollection Set(this TagCollection tagCollection, string key, bool value)
        {
            return Set(tagCollection, key, value.ToString());
        }

        public static TagCollection Set(this TagCollection tagCollection, string key, int value)
        {
            return Set(tagCollection, key, value.ToString());
        }

        public static TagCollection Set(this TagCollection tagCollection, string key, long value)
        {
            return Set(tagCollection, key, value.ToString());
        }

        public static TagCollection Set(this TagCollection tagCollection, string key, float value)
        {
            return Set(tagCollection, key, value.ToString());
        }

        public static TagCollection Set(this TagCollection tagCollection, string key, double value)
        {
            return Set(tagCollection, key, value.ToString());
        }

        /// <summary>
        /// Database instance name. E.g. the "Initial Catalog" value from a SQL connection string.
        /// </summary>
        public static TagCollection DbInstance(this TagCollection tagCollection, string dbInstance)
        {
            return Set(tagCollection, Tags.DbInstance, dbInstance);
        }

        /// <summary>
        /// Database instance name. E.g. the "db" value from Redis.
        /// </summary>
        public static TagCollection DbInstance(this TagCollection tagCollection, int db)
        {
            return Set(tagCollection, Tags.DbInstance, db);
        }

        /// <summary>
        /// A database statement for the given database type.
        /// E.g., for db.type="sql", "SELECT * FROM wuser_table"; for db.type="redis", "SET mykey 'WuValue'".
        /// </summary>
        public static TagCollection DbStatement(this TagCollection tagCollection, string dbStatement)
        {
            return Set(tagCollection, Tags.DbStatement, dbStatement);
        }

        /// <summary>
        /// Database type. For any SQL database, "sql". For others, the lower-case database category,
        /// e.g. "cassandra", "hbase", or "redis".
        /// </summary>
        public static TagCollection DbType(this TagCollection tagCollection, string dbType)
        {
            return Set(tagCollection, Tags.DbType, dbType);
        }

        /// <summary>
        /// Username for accessing database. E.g., "readonly_user" or "reporting_user".
        /// </summary>
        public static TagCollection DbUser(this TagCollection tagCollection, string dbUser)
        {
            return Set(tagCollection, Tags.DbUser, dbUser);
        }

        /// <summary>
        /// HTTP method of the request for the associated Span. E.g., "GET", "POST".
        /// </summary>
        public static TagCollection HttpMethod(this TagCollection tagCollection, string httpMethod)
        {
            return Set(tagCollection, Tags.HttpMethod, httpMethod);
        }

        /// <summary>
        /// HTTP response status code for the associated Span. E.g., 200, 503, 404.
        /// </summary>
        public static TagCollection HttpStatusCode(this TagCollection tagCollection, int httpStatusCode)
        {
            return Set(tagCollection, Tags.HttpStatusCode, httpStatusCode);
        }

        /// <summary>
        /// URL of the request being handled in this segment of the trace, in standard URI format.
        /// E.g., "https://domain.net/path/to?resource=here".
        /// </summary>
        public static TagCollection HttpUrl(this TagCollection tagCollection, string httpUrl)
        {
            return Set(tagCollection, Tags.HttpUrl, httpUrl);
        }
        public static TagCollection HttpRequest(this TagCollection tagCollection, string request)
        {
            return Set(tagCollection, Tags.HttpRequest, request);
        }
        public static TagCollection HttpResponse(this TagCollection tagCollection, string request)
        {
            return Set(tagCollection, Tags.HttpResponse, request);
        }

        /// <summary>
        /// Either "client" or "server" for the appropriate roles in an RPC,
        /// and "producer" or "consumer" for the appropriate roles in a messaging scenario.
        /// </summary>
        public static TagCollection SpanKind(this TagCollection tagCollection, string spanKind)
        {
            return Set(tagCollection, Tags.SpanKind, spanKind);
        }

        /// <summary>
        /// A constant for setting the "span.kind" to indicate that it represents a "client" span.
        /// </summary>
        public static TagCollection Client(this TagCollection tagCollection)
        {
            return Set(tagCollection, Tags.SpanKind, Tags.SpanKindClient);
        }

        /// <summary>
        /// A constant for setting the "span.kind" to indicate that it represents a "server" span.
        /// </summary>
        public static TagCollection Server(this TagCollection tagCollection)
        {
            return Set(tagCollection, Tags.SpanKind, Tags.SpanKindServer);
        }

        /// <summary>
        /// A constant for setting the "span.kind" to indicate that it represents a "consumer" span,
        /// in a messaging scenario.
        /// </summary>
        public static TagCollection Consumer(this TagCollection tagCollection)
        {
            return Set(tagCollection, Tags.SpanKind, Tags.SpanKindConsumer);
        }

        /// <summary>
        /// A constant for setting the "span.kind" to indicate that it represents a "producer" span,
        /// in a messaging scenario.
        /// </summary>
        public static TagCollection Producer(this TagCollection tagCollection)
        {
            return Set(tagCollection, Tags.SpanKind, Tags.SpanKindProducer);
        }

    }
}
