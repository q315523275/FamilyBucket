using System;

namespace Bucket.Authorize
{
    /// <summary>
    /// back token
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Token Value
        /// </summary>
        public string TokenValue
        { get; set; }
        /// <summary>
        /// Expires (unit second)
        /// </summary>
        public DateTime? Expires
        { get; set; }
        /// <summary>
        /// token type
        /// </summary>
        public string TokenType
        { get; set; }
    }
}
