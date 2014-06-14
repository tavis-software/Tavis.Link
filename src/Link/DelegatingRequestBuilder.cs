using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegatingRequestBuilder : IHttpRequestBuilder
    {
        public IHttpRequestBuilder InnerBuilder { get; set; }

        public virtual HttpRequestMessage Build(Link link, Dictionary<string, object> uriParameters, HttpMethod method, HttpContent content)
        {
            return InnerBuilder.Build(link, uriParameters, method, content);
        }
    }
}