/*
 * Licensed to the SkyAPM under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The SkyAPM licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.Net.Http;
using Bucket.SkyApm;
using Bucket.SkyApm.Common;
using Bucket.SkyApm.Tracing;
using Bucket.SkyApm.Tracing.Segments;

namespace Bucket.SkyApm.Diagnostics.HttpClient
{
    public class HttpClientTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = "HttpHandlerDiagnosticListener";

        private readonly ITracingContext _tracingContext;
        private readonly IExitSegmentContextAccessor _contextAccessor;

        public HttpClientTracingDiagnosticProcessor(ITracingContext tracingContext,
            IExitSegmentContextAccessor contextAccessor)
        {
            _tracingContext = tracingContext;
            _contextAccessor = contextAccessor;
        }

        [DiagnosticName("System.Net.Http.Request")]
        public void HttpRequest([Property(Name = "Request")] HttpRequestMessage request)
        {
            var context = _tracingContext.CreateExitSegmentContext(request.RequestUri.AbsolutePath.ToString(),
                $"{request.RequestUri.Host}:{request.RequestUri.Port}",
                new HttpClientICarrierHeaderCollection(request));

            context.Span.SpanLayer = SpanLayer.HTTP;
            context.Span.Component = Common.Components.HTTPCLIENT;
            context.Span.AddTag(Tags.URL, request.RequestUri.ToString());
            context.Span.AddTag(Tags.HTTP_METHOD, request.Method.ToString());
            if (request.Content != null && request.Method.ToString().ToUpper() == "POST")
                context.Span.AddTag(Tags.HTTP_REQUEST, request.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
        }

        [DiagnosticName("System.Net.Http.Response")]
        public void HttpResponse([Property(Name = "Response")] HttpResponseMessage response)
        {
            var context = _contextAccessor.Context;
            if (context == null)
            {
                return;
            }

            if (response != null)
            {
                var statusCode = (int)response.StatusCode;
                if (statusCode >= 400)
                {
                    context.Span.ErrorOccurred();
                }

                context.Span.AddTag(Tags.STATUS_CODE, statusCode);
                context.Span.AddTag(Tags.HTTP_RESPONSE, response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
            }

            _tracingContext.Release(context);
        }

        [DiagnosticName("System.Net.Http.Exception")]
        public void HttpException([Property(Name = "Request")] HttpRequestMessage request,
            [Property(Name = "Exception")] Exception exception)
        {
            _contextAccessor.Context?.Span?.ErrorOccurred(exception);
        }
    }
}