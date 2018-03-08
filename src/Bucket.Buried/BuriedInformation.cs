using Bucket.Values;
using System;

namespace Bucket.Buried
{
    public class BuriedInformation : BuriedInformationInput
    {
        /// <summary>
        /// 项目名称(pzgo上游输入)
        /// </summary>
        public string ModName { get; set; }
        /// <summary>
        /// 流程名称 active1(上游输入)
        /// </summary>
        public string ProcName { get; set; }
        /// <summary>
        /// BatchId（BatchId上游输入）
        /// </summary>
        public string BatchId { get; set; }
        /// <summary>
        /// 流程发起人（UdcId上游输入）
        /// </summary>
        public string LaunchId { get; set; }
        /// <summary>
        /// 流程Token (上游输入)
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 父级队列序号 格式 特殊标识_上游输入的Seq整形部分 (FK_3) 默认为 特殊标识_-1 (没有父级序列号)
        /// </summary>
        public string ParentSeq { get; set; }
        /// <summary>
        /// 当前队列顺序号 格式 特殊标识_Num Num每次从0开始  依次累加
        /// </summary>
        public string Seq { get; set; }
        /// <summary>
        /// 必填 调用源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 埋点时间 unix 时间戳
        /// </summary>
        public string BuriedTime { get; set; }
    }
}
