using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Tracing.DataContract.Tracing;
using Tracing.Server.ViewModels;
using Tracing.Storage.Query;

namespace Tracing.Server.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PageResult<Trace>, PageViewModel<TraceViewModel>>()
                .ForMember(destination => destination.PageNumber, option => option.MapFrom(source => source.CurrentPageNumber));

            CreateMap<Trace, TraceViewModel>()
                .ForMember(destination => destination.Duration, option => option.MapFrom(source => GetDuration(source.Spans)))
                .ForMember(destination => destination.StartTimestamp, option => option.MapFrom(source => ToLocalDateTime(source.Spans.Min(x => x.StartTimestamp))))
                .ForMember(destination => destination.FinishTimestamp, option => option.MapFrom(source => ToLocalDateTime(source.Spans.Max(x => x.FinishTimestamp))))
                .ForMember(destination => destination.Services, option => option.Ignore());

            CreateMap<Trace, TraceDetailViewModel>()
                .ForMember(destination => destination.Spans, option => option.Ignore())
                .ForMember(destination => destination.Duration, option => option.MapFrom(source => GetDuration(source.Spans)))
                .ForMember(destination => destination.StartTimestamp, option => option.MapFrom(source => ToLocalDateTime(source.Spans.Min(x => x.StartTimestamp))))
                .ForMember(destination => destination.FinishTimestamp, option => option.MapFrom(source => ToLocalDateTime(source.Spans.Max(x => x.FinishTimestamp))));

            CreateMap<Span, SpanViewModel>()
                .ForMember(destination => destination.Children, option => option.Ignore())
                .ForMember(destination => destination.ServiceName, option => option.MapFrom(span => GetService(span)))
                .ForMember(destination => destination.StartTimestamp, option => option.MapFrom(span => ToLocalDateTime(span.StartTimestamp)))
                .ForMember(destination => destination.FinishTimestamp, option => option.MapFrom(span => ToLocalDateTime(span.FinishTimestamp)));

            CreateMap<Span, SpanDetailViewModel>()
                .ForMember(destination => destination.ServiceName, option => option.MapFrom(span => GetService(span)))
                .ForMember(destination => destination.StartTimestamp, option => option.MapFrom(span => ToLocalDateTime(span.StartTimestamp)))
                .ForMember(destination => destination.FinishTimestamp, option => option.MapFrom(span => ToLocalDateTime(span.FinishTimestamp)));

            CreateMap<Tag, TagViewModel>();

            CreateMap<LogField, LogFieldViewModel>();

            CreateMap<Log, LogViewModel>()
                .ForMember(destination => destination.Timestamp, option => option.MapFrom(log => ToLocalDateTime(log.Timestamp)));

            CreateMap<SpanReference, ReferenceViewModel>();

            CreateMap<TraceHistogram, TraceHistogramViewModel>()
                .ForMember(destination => destination.Time, option => option.MapFrom(target => target.Time.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        private static long GetDuration(IEnumerable<Span> spans)
        {
            var timeSpan = spans.Max(x => x.FinishTimestamp) - spans.Min(x => x.StartTimestamp);
            return timeSpan.GetMicroseconds();
        }

        private static DateTime ToLocalDateTime(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.LocalDateTime;
        }

        private static string GetService(Span span)
        {
           return ServiceHelpers.GetService(span);
        }
    }
}