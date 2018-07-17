using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Config.Util
{
    public class ExceptionUtil
    {
        public static string GetDetailMessage(Exception ex)
        {
            if (string.IsNullOrWhiteSpace(ex.Message))
            {
                return string.Empty;
            }
            var builder = new StringBuilder(ex.Message);
            var causes = new LinkedList<Exception>();

            int counter = 0;
            Exception current = ex;
            //retrieve up to 10 causes
            while (current.InnerException != null && counter < 10)
            {
                Exception next = current.InnerException;
                causes.AddLast(next);
                current = next;
                counter++;
            }

            foreach (var cause in causes)
            {
                if (string.IsNullOrEmpty(cause.Message))
                {
                    counter--;
                    continue;
                }
                builder.Append(" [Cause: ").Append(cause.Message);
            }
            builder.Append(new string(']', counter));
            return builder.ToString();
        }
    }
}
