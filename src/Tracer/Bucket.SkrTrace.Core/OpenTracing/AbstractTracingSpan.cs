using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Transport;
using System;
using System.Collections.Generic;

namespace Bucket.SkrTrace.Core.OpenTracing
{
    public abstract class AbstractTracingSpan : ISpan
    {
        protected readonly string _spanId;
        protected string _parnetSpanId;
        protected string _operationName;
        protected SpanLayer? _layer;
        protected SpanComponent? _spanComponent;
        protected DateTimeOffset _startTime;
        protected DateTimeOffset _endTime;
        protected bool _errorOccurred = false;
        protected ICollection<ITraceSegmentRef> _refs;
        protected AbstractTracingSpan(string spanId, string parentSpanId, string operationName)
        {
            _operationName = operationName;
            _spanId = spanId;
            _parnetSpanId = parentSpanId;
            Tags = new TagCollection();
            Logs = new LogCollection();
        }

        public abstract bool IsEntry { get; }
        public abstract bool IsExit { get; }
        public virtual string SpanId => _spanId;
        public string ParentSpanId
        {
            get { return _parnetSpanId; }
            set { _parnetSpanId = value; }
        }
        public virtual string OperationName
        {
            get { return _operationName; }
            set { _operationName = value; }
        }
        public TagCollection Tags { get; }
        public LogCollection Logs { get; }
        public virtual ISpan ErrorOccurred()
        {
            _errorOccurred = true;
            return this;
        }
        public virtual ISpan Start()
        {
            _startTime = DateTimeOffset.UtcNow;
            return this;
        }
        public virtual ISpan Start(DateTimeOffset timestamp)
        {
            _startTime = timestamp;
            return this;
        }
        public virtual ISpan SetLayer(SpanLayer layer)
        {
            _layer = layer;
            return this;
        }
        public virtual ISpan SetComponent(SpanComponent spanComponent)
        {
            _spanComponent = spanComponent;
            return this;
        }
        public virtual bool Finish(ITraceSegment owner)
        {
            _endTime = DateTimeOffset.UtcNow;
            owner.Archive(this);
            return true;
        }
        public virtual SpanRequest Transform()
        {
            var spanRequest = new SpanRequest
            {
                SpanId = _spanId,
                ParentSpanId = _parnetSpanId,
                StartTime = _startTime,
                EndTime = _endTime,
                OperationName = _operationName,
                Duration = (_endTime - _startTime).TotalMilliseconds
            };

            if (IsEntry)
            {
                spanRequest.SpanType = 0;
            }
            else if (IsExit)
            {
                spanRequest.SpanType = 1;
            }
            else
            {
                spanRequest.SpanType = 2;
            }

            if (_layer.HasValue)
            {
                spanRequest.SpanLayer = (int)_layer.Value;
            }

            if (_spanComponent.HasValue)
            {
                spanRequest.Component = _spanComponent.ToString();
            }

            foreach (var tag in Tags)
            {
                spanRequest.Tags.Add(new KeyValuePair<string, string>(tag.Key, tag.Value));
            }

            if (Logs != null)
            {
                foreach (var logDataEntity in Logs)
                {
                    var logMessage = new LogDataRequest
                    {
                        Timestamp = logDataEntity.Timestamp
                    };
                    foreach (var log in logDataEntity.Fields)
                    {
                        logMessage.Data.Add(new KeyValuePair<string, string>(log.Key, log.Value));
                    }
                    spanRequest.Logs.Add(logMessage);
                }
            }
            if (_refs == null) return spanRequest;
            foreach (var traceSegmentRef in _refs)
            {
                spanRequest.References.Add(traceSegmentRef.Transform());
            }
            return spanRequest;
        }
        public virtual void Ref(ITraceSegmentRef traceSegmentRef)
        {
            if (_refs == null)
            {
                _refs = new List<ITraceSegmentRef>();
            }

            if (!_refs.Contains(traceSegmentRef))
            {
                _refs.Add(traceSegmentRef);
            }
        }
    }
}
