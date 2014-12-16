using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHttpRequestBuilder
    {
        HttpRequestMessage Build(Link link, HttpRequestMessage request);
    }
}