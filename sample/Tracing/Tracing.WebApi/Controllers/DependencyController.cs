using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tracing.DataContract.Tracing;
using Tracing.Server.Common;
using Tracing.Server.ViewModels;
using Tracing.Storage;
using Tracing.Storage.Query;
using Microsoft.AspNetCore.Mvc;

namespace Tracing.Server.Controllers
{
    [Route("api/[controller]")]
    public class DependencyController : Controller
    {
        private readonly ISpanQuery _spanQuery;

        public DependencyController(ISpanQuery spanQuery)
        {
            _spanQuery = spanQuery;
        }

        [HttpGet]
        public async Task<DependencyViewModel> Get([FromQuery] long? startTimestamp, [FromQuery] long? finishTimestamp)
        {
            var spans = await _spanQuery.GetSpanDependencies(
                new DependencyQuery
                {
                    StartTimestamp = TimestampHelpers.Convert(startTimestamp),
                    FinishTimestamp = TimestampHelpers.Convert(finishTimestamp),
                });

            var dependency = new DependencyViewModel();

            dependency.Nodes = GetNodes(spans).ToList();

            var dependencies = spans.Where(x => x.References.Any(r => r.Reference == "ChildOf")).GroupBy(x =>
            {
                var @ref = x.References.First();
                var parent = spans.FirstOrDefault(s => s.SpanId == @ref.ParentId);
                return new {source = ServiceHelpers.GetService(parent), target = ServiceHelpers.GetService(x)};
            });

            foreach (var item in dependencies)
            {
                if (item.Key.source == item.Key.target)
                {
                    continue;
                }

                dependency.Edges.Add(new EdgeViewModel
                {
                    Source = item.Key.source,
                    Target = item.Key.target,
                    Value = item.Count()
                });
            }

            return dependency;
        }

        private IEnumerable<NodeViewModel> GetNodes(IEnumerable<Span> spans)
        {
            foreach (var service in spans.GroupBy(ServiceHelpers.GetService))
            {
                yield return new NodeViewModel {Name = service.Key, Value = service.Count()};
            }
        }
    }
}