namespace System
{
    public static class ExtExceptionHelper
    {
        /// <summary>
        /// 获取详细错误堆栈信息
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlen"></param>
        /// <returns></returns>
        public static string DetailMessage(this Exception exp)
        {
            var expt = exp; string message = "";
            while (expt != null)
            {
                message += "→" + (Convert.IsDBNull(expt.Message) ? "" : expt.Message) + "\r\n";
                expt = expt.InnerException;
            }
            return message;
        }
    }
}
