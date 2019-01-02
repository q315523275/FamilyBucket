namespace Bucket.SkrTrace.Core.OpenTracing
{
    public static class Tags
    {
        public const string SpanKind = "span.kind";
        public const string SpanKindClient = "client";
        public const string SpanKindConsumer = "consumer";
        public const string SpanKindProducer = "producer";
        public const string SpanKindServer = "server";

        public const string Component = "component";

        public const string DbInstance = "db.instance";
        public const string DbStatement = "db.statement";
        public const string DbType = "db.type";
        public const string DbUser = "db.user";

        public const string Error = "error";

        public const string HttpMethod = "http.method";
        public const string HttpStatusCode = "http.status_code";
        public const string HttpUrl = "http.url";
        public const string HttpRequest = "http.request";
        public const string HttpResponse = "http.response";
    }
}
