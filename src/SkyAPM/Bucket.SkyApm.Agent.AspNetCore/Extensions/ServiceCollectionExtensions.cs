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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bucket.SkyApm.Diagnostics;
using Bucket.SkyApm.Sampling;
using Bucket.SkyApm.Service;
using Bucket.SkyApm.Tracing;
using Bucket.SkyApm.Transport;
using Bucket.SkyApm.AspNetCore.Diagnostics;
using Bucket.SkyApm.Diagnostics.HttpClient;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Bucket.SkyApm.Agent.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加链路追踪,来自skyapm,内置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SkyApmExtensions AddBucketSkyApmCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var configService = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)configService.ImplementationInstance;
            services.Configure<SkyApmConfig>(configuration.GetSection("SkyApm"));

            services.AddSingleton<ISegmentDispatcher, AsyncQueueSegmentDispatcher>();
            services.AddSingleton<IExecutionService, SegmentReportService>();
            services.AddSingleton<IInstrumentStartup, InstrumentStartup>();
            services.AddSingleton<IRuntimeEnvironment>(RuntimeEnvironment.Instance);
            services.AddSingleton<TracingDiagnosticProcessorObserver>();
            services.AddSingleton<IHostedService, InstrumentationHostedService>();
            services.AddTracing().AddSampling().AddTransport();

            return services.AddSkyApmExtensions().AddAspNetCoreHosting().AddHttpClient();
        }

        private static IServiceCollection AddTracing(this IServiceCollection services)
        {
            services.AddSingleton<ITracingContext, TracingContext>();
            services.AddSingleton<ICarrierPropagator, CarrierPropagator>();
            services.AddSingleton<ICarrierFormatter, BucketCarrierFormatter>();
            services.AddSingleton<ISegmentContextFactory, SegmentContextFactory>();
            services.AddSingleton<IEntrySegmentContextAccessor, EntrySegmentContextAccessor>();
            services.AddSingleton<ILocalSegmentContextAccessor, LocalSegmentContextAccessor>();
            services.AddSingleton<IExitSegmentContextAccessor, ExitSegmentContextAccessor>();
            services.AddSingleton<ISamplerChainBuilder, SamplerChainBuilder>();
            services.AddSingleton<ISegmentContextMapper, SegmentContextMapper>();
            services.AddSingleton<IBase64Formatter, Base64Formatter>();
            return services;
        }

        private static IServiceCollection AddSampling(this IServiceCollection services)
        {
            services.AddSingleton<SimpleCountSamplingInterceptor>();
            services.AddSingleton<ISamplingInterceptor>(p => p.GetService<SimpleCountSamplingInterceptor>());
            services.AddSingleton<IExecutionService>(p => p.GetService<SimpleCountSamplingInterceptor>());
            services.AddSingleton<ISamplingInterceptor, RandomSamplingInterceptor>();
            return services;
        }

        private static IServiceCollection AddTransport(this IServiceCollection services)
        {
            services.AddSingleton<ISegmentReporter, NullSegmentReporter>();
            return services;
        }
    }
}