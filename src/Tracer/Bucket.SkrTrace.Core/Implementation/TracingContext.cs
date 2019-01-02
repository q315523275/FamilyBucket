using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.OpenTracing;
using Bucket.SkrTrace.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bucket.SkrTrace.Core.Implementation
{
    public class TracingContext : ITracerContext
    {
        private readonly ITraceSegment _segment;
        private readonly Stack<ISpan> _activeSpanStacks;
        public TracingContext()
        {
            _activeSpanStacks = new Stack<ISpan>();
            _segment = new TraceSegment();
        }

        public ISpan ActiveSpan => InternalActiveSpan();

        public ISpan CreateEntrySpan(string operationName)
        {
            _activeSpanStacks.TryPeek(out var parentSpan);
            var parentSpanId = parentSpan?.SpanId ?? "-1";
            if (parentSpan != null && parentSpan.IsEntry)
            {
                parentSpan.OperationName = operationName;
                return parentSpan.Start();
            }
            else
            {
                var entrySpan = new EntrySpan(RandomUtils.NextLong().ToString(), parentSpanId, operationName);
                entrySpan.Start();
                _activeSpanStacks.Push(entrySpan);
                return entrySpan;
            }
        }

        public ISpan CreateExitSpan(string operationName, string remotePeer)
        {
            _activeSpanStacks.TryPeek(out var parentSpan);
            if (parentSpan != null && parentSpan.IsExit)
            {
                return parentSpan.Start();
            }
            else
            {
                var parentSpanId = parentSpan?.SpanId ?? "-1";
                var exitSpan = new ExitSpan(RandomUtils.NextLong().ToString(), parentSpanId, operationName, remotePeer);
                _activeSpanStacks.Push(exitSpan);
                return exitSpan.Start();
            }
        }

        public void Extract(IContextCarrier carrier)
        {
            var traceSegmentRef = new TraceSegmentRef(carrier);
            _segment.Ref(traceSegmentRef);
            _segment.Identity = carrier.Identity;
            _segment.TraceSegmentId = carrier.TraceSegmentId;
            var span = InternalActiveSpan();
            if (span is EntrySpan)
            {
                span.ParentSpanId = carrier.SpanId;
                span.Ref(traceSegmentRef);
            }
        }

        public void Inject(IContextCarrier carrier)
        {
            var span = InternalActiveSpan();
            if (!span.IsExit)
            {
                throw new InvalidOperationException("Inject can be done only in Exit Span");
            }
            var spanWithPeer = span as IWithPeerInfo;

            carrier.PeerHost = spanWithPeer.Peer;
            carrier.TraceSegmentId = _segment.TraceSegmentId;
            carrier.SpanId = span.SpanId;
            carrier.Identity = _segment.Identity;

            carrier.ParentApplicationCode = _segment.ApplicationCode;

            var refs = _segment.Refs;
            var firstSpan = _activeSpanStacks.Last();

            carrier.ParentOperationName = firstSpan.OperationName;

            var metaValue = GetMetaValue(refs);

            carrier.EntryApplicationCode = metaValue.entryApplicationCode;
            carrier.EntryOperationName = metaValue.operationName;
        }

        public void StopSpan(ISpan span)
        {
            _activeSpanStacks.TryPeek(out var lastSpan);
            if (lastSpan == span)
            {
                if (lastSpan is AbstractTracingSpan tracingSpan)
                {
                    if (tracingSpan.Finish(_segment))
                    {
                        _activeSpanStacks.Pop();
                    }
                }
                else
                {
                    _activeSpanStacks.Pop();
                }
            }
            else
            {
                throw new InvalidOperationException("Stopping the unexpected span = " + span);
            }

            if (_activeSpanStacks.Count == 0)
            {
                Finish();
            }
        }

        public void SetIdentity(string identity)
        {
            _segment.Identity = identity;
        }

        private ISpan InternalActiveSpan()
        {
            if (!_activeSpanStacks.TryPeek(out var span))
            {
                throw new InvalidOperationException("No active span.");
            }

            return span;
        }

        private (string operationName, string entryApplicationCode) GetMetaValue(
            IEnumerable<ITraceSegmentRef> refs)
        {
            if (refs != null && refs.Any())
            {
                var segmentRef = refs.First();
                return (segmentRef.EntryOperationName, segmentRef.EntryApplicationCode);
            }
            else
            {
                var span = _activeSpanStacks.Last();
                return (span.OperationName, _segment.ApplicationCode);
            }
        }

        private void Finish()
        {
            var finishedSegment = _segment.Finish();
            ListenerManager.NotifyFinish(finishedSegment);
        }

        public static class ListenerManager
        {
            private static readonly IList<ITracingContextListener> _listeners = new List<ITracingContextListener>();


            [MethodImpl(MethodImplOptions.Synchronized)]
            public static void Add(ITracingContextListener listener)
            {
                _listeners.Add(listener);
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public static void Remove(ITracingContextListener listener)
            {
                _listeners.Remove(listener);
            }

            public static void NotifyFinish(ITraceSegment traceSegment)
            {
                foreach (var listener in _listeners)
                {
                    listener.AfterFinished(traceSegment);
                }
            }
        }
    }
}
