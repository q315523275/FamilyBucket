using SqlSugar;
using System;

namespace Pinzhi.Logging.EventSubscribe
{
    [SugarTable("tb_logs")]
    public class ErrorLogInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public long Id { set; get; }
        public string ClassName { set; get; }
        public string ProjectName { set; get; }
        public string LogTag { set; get; }
        public string LogType { set; get; }
        public string LogMessage { set; get; }
        public string IP { set; get; }
        public DateTime AddTime { set; get; }
    }
}
