using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.ConfigCenter.Util.Http
{
    public class HttpRequest
    {
        private string _url;

        /// <summary>
        /// Create the request for the url. </summary>
        /// <param name="url"> the url </param>
        public HttpRequest(string url)
        {
            _url = url;
        }

        public string Url
        {
            get
            {
                return _url;
            }
        }

        public int? Timeout { get; set; }
    }
}
