using System.Collections.Generic;

namespace Bucket.Authorize.Abstractions
{
    /// <summary>
    /// 权限
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 资源
        /// </summary>
        public string Path { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Method { set; get; }
        /// <summary>
        /// 资源所需权限
        /// </summary>
        public List<string> Scope { set; get; }
    }
}
