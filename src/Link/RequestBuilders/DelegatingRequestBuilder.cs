using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DelegatingRequestBuilder 
    {
        public DelegatingRequestBuilder NextBuilder { get; set; }

        public HttpRequestMessage Build(ILink link, HttpRequestMessage request)
        {
            request = ApplyChanges(link, request);

            if (NextBuilder != null)
            { 
                request = NextBuilder.Build(link,request);
            }
            return request;
            
        }

        protected abstract HttpRequestMessage ApplyChanges(ILink link,HttpRequestMessage request);
    }
}