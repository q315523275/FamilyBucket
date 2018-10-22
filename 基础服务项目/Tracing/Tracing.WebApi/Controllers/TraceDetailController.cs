using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Tracing.DataContract.Tracing;
using Tracing.Server.ViewModels;
using Tracing.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Tracing.Server.Controllers
{
    [Route("api/[controller]/{traceId}")]
    public class TraceDetailController : Controller
    {
        private readonly ISpanQuery _spanQuery;
        private readonly IMapper _mapper;

        public TraceDetailController(ISpanQuery spanQuery, IMapper mapper)
        {
            _spanQuery = spanQuery;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<TraceDetailViewModel> Get([FromRoute] string traceId)
        {
            var trace = await _spanQuery.GetTrace(traceId);
            var traceDetailViewModel = _mapper.Map<TraceDetailViewModel>(trace);

            var traceSpans = trace.Spans.OrderBy(x => x.StartTimestamp).ToList() ;

            var minReferences = traceSpans.Min(x => x.References?.Count);

            traceDetailViewModel.Spans = GetSpanChildren(traceSpans.Where(x => x.References?.Count == minReferences)).ToList();

            CalculateOffset(traceDetailViewModel.Spans, traceDetailViewModel.StartTimestamp);

            return traceDetailViewModel;

            IEnumerable<SpanViewModel> GetSpanChildren(IEnumerable<Span> spans)
            {
                foreach (var span in spans)
                {
                    var spanViewMode = _mapper.Map<SpanViewModel>(span);
                    spanViewMode.Children = GetSpanChildren(traceSpans.Where(x => x.References?.Count > minReferences && x.References.FirstOrDefault()?.ParentId == span.SpanId)).ToList();
                    yield return spanViewMode;
                }
            }
        }

        private void CalculateOffset(IEnumerable<SpanViewModel> spans, DateTime startTimestamp)
        {
            if (spans.Any())
            {
                foreach (var span in spans)
                {
                    var offsetTimespan = span.StartTimestamp - startTimestamp;
                    span.Offset = offsetTimespan.GetMicroseconds();
                    CalculateOffset(span.Children, startTimestamp);
                }
            }
        }
    }
}