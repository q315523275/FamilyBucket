using SqlSugar;
using System;

namespace Pinzhi.User.EventSubscribe
{
    [SugarTable("tb_events")]
    public class UserEventInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public long Id { set; get; }
        public string Mode { set; get; }
        public string EventName { set; get; }
        public string EventKey { set; get; }
        public string EventCode { set; get; }
        public string EventValue { set; get; }
        public string UserKey { set; get; }
        public string Channel { set; get; }
        public string Source { set; get; }
        public DateTime CreateTime { set; get; }
    }
}
