namespace Pinzhi.Platform.Dto.Project
{
    public class SetProjectInput
    {
        public int Id { set; get; }
        /// <summary>
        /// 平台Key
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 应用平台
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 路由前缀
        /// </summary>
        public string RouteKey { set; get; }
        public bool IsDeleted { set; get; }
    }
}
