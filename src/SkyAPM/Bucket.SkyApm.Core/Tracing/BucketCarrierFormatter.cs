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

using Microsoft.Extensions.Options;

namespace Bucket.SkyApm.Tracing
{
    public class BucketCarrierFormatter : ICarrierFormatter
    {
        public BucketCarrierFormatter(IOptions<SkyApmConfig> configAccessor)
        {
            var config = configAccessor.Value;
            Key = HeaderVersions.Bucket;
            Enable = config.HeaderVersions != null && config.HeaderVersions.Contains(HeaderVersions.Bucket);
        }

        public string Key { get; }

        public bool Enable { get; }

        public ICarrier Decode(string content)
        {
            NullableCarrier Defer()
            {
                return NullableCarrier.Instance;
            }

            if (string.IsNullOrEmpty(content))
                return Defer();

            var parts = content.Split('|');
            if (parts.Length < 9)
                return Defer();

            if (!long.TryParse(parts[0], out var traceId))
                return Defer();

            if (!long.TryParse(parts[1], out var segmentId))
                return Defer();

            if (!int.TryParse(parts[2], out var parentSpanId))
                return Defer();

            return new Carrier(traceId, segmentId, parentSpanId, parts[3], parts[4])
            {
                Identity = parts[5],
                NetworkAddress = parts[6],
                EntryEndpoint = parts[7],
                ParentEndpoint = parts[8],
            };
        }

        public string Encode(ICarrier carrier)
        {
            if (!carrier.HasValue)
                return string.Empty;
            return string.Join("|",
                carrier.TraceId.ToString(),
                carrier.ParentSegmentId.ToString(),
                carrier.ParentSpanId.ToString(),
                carrier.ParentServiceName,
                carrier.EntryServiceName,
                carrier.Identity,
                carrier.NetworkAddress,
                carrier.EntryEndpoint,
                carrier.ParentEndpoint);
        }
    }
}