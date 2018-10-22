using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Tracing.DataContract.Tracing;
using Tracing.Server.Common;
using Tracing.Server.ViewModels;
using Tracing.Storage;
using Tracing.Storage.Query;
using Microsoft.AspNetCore.Mvc;

namespace Tracing.Server.Controllers
{
    [Route("api/[controller]")]
    public class TraceController : Controller
    {
        private readonly ISpanQuery _spanQuery;
        private readonly IMapper _mapper;

        public TraceController(ISpanQuery spanQuery, IMapper mapper)
        {
            _spanQuery = spanQuery;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<TraceViewModel>> Get(
            [FromQuery] string service, [FromQuery] string tags, 
            [FromQuery] long? startTimestamp, [FromQuery] long? finishTimestamp,
            [FromQuery] int? minDuration, [FromQuery] int? maxDuration, [FromQuery] int? limit)
        {
            var query = new TraceQuery
            {
                Tags = tags,
                ServiceName = service,
                StartTimestamp = TimestampHelpers.Convert(startTimestamp),
                FinishTimestamp = TimestampHelpers.Convert(finishTimestamp),
                MinDuration = minDuration,
                MaxDuration = maxDuration,
                Limit = limit.GetValueOrDefault(10)
            };

            var data = await _spanQuery.GetTraces(query);
            var traceViewModels = _mapper.Map<List<TraceViewModel>>(data);

            foreach (var trace in traceViewModels)
            {
                var item = data.FirstOrDefault(x => x.TraceId == trace.TraceId);
                trace.Services = GetTraceServices(item);
            }

            return traceViewModels;
        }

        [HttpGet("Histogram")]
        public async Task<IEnumerable<TraceHistogramViewModel>> GetTraceHistogram(
            [FromQuery] string service, [FromQuery] string tags,
            [FromQuery] long? startTimestamp, [FromQuery] long? finishTimestamp,
            [FromQuery] int? minDuration, [FromQuery] int? maxDuration, [FromQuery] int? limit)
        {
            var query = new TraceQuery
            {
                Tags = tags,
                ServiceName = service,
                StartTimestamp = TimestampHelpers.Convert(startTimestamp),
                FinishTimestamp = TimestampHelpers.Convert(finishTimestamp),
                MinDuration = minDuration,
                MaxDuration = maxDuration,
                Limit = limit.GetValueOrDefault(10)
            };

            var data = await _spanQuery.GetTraceHistogram(query);

            return _mapper.Map<List<TraceHistogramViewModel>>(data);
        }

        private List<TraceService> GetTraceServices(Trace trace)
        {
            var traceServices= new List<TraceService>();
            foreach (var span in trace.Spans)
            {
                traceServices.Add(new TraceService(ServiceHelpers.GetService(span)));
            }

            return traceServices;
        }
    }
}