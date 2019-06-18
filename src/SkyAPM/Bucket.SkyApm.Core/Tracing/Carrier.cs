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

namespace Bucket.SkyApm.Tracing
{
    public class Carrier : ICarrier
    {
        public bool HasValue { get; } = true;

        public bool? Sampled { get; set; }

        public long TraceId { get; }

        public long ParentSegmentId { get; }

        public int ParentSpanId { get; }

        public string ParentServiceName { get; }

        public string EntryServiceName { get; }

        public string Identity { get; set; }

        public string NetworkAddress { get; set; }

        public string EntryEndpoint { get; set; }

        public string ParentEndpoint { get; set; }

        public Carrier(long traceId, long parentSegmentId, int parentSpanId, string parentServiceName, string entryServiceName)
        {
            TraceId = traceId;
            ParentSegmentId = parentSegmentId;
            ParentSpanId = parentSpanId;
            ParentServiceName = parentServiceName;
            EntryServiceName = entryServiceName;
        }
    }
}