using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHttpRequestBuilder
    {
        HttpRequestMessage Build(HttpRequestMessage request);
    }
}