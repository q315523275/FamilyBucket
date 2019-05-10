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

using Bucket.SkyApm.Common;

namespace Bucket.SkyApm.Tracing
{
    public interface ICarrier
    {
        bool HasValue { get; }

        bool? Sampled { get; }

        long TraceId { get; }

        long ParentSegmentId { get; }

        int ParentSpanId { get; }

        string ParentServiceName { get; }

        string EntryServiceName { get; }

        string Identity { get; }

        string NetworkAddress { get; }

        string EntryEndpoint { get; }

        string ParentEndpoint { get; }
    }
}