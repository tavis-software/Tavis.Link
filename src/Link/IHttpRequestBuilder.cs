using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHttpRequestBuilder
    {
        HttpRequestMessage Build(Link link, Dictionary<string,object> uriParameters, HttpMethod method, HttpContent content);
    }
}