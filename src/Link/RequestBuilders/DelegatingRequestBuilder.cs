using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DelegatingRequestBuilder : IHttpRequestBuilder
    {
        public IHttpRequestBuilder NextBuilder { get; set; }

        public HttpRequestMessage Build(Link link, HttpRequestMessage request)
        {
            request = ApplyChanges(link, request);

            if (NextBuilder != null)
            { 
                request = NextBuilder.Build(link,request);
            }
            return request;
            
        }

        protected abstract HttpRequestMessage ApplyChanges(Link link,HttpRequestMessage request);
    }

    //public abstract class BaseChainedRequestBuilder : IHttpRequestBuilder
    //{
    //    private readonly IHttpRequestBuilder _NextActionResult;

    //    protected BaseChainedRequestBuilder(IHttpRequestBuilder actionResult)
    //    {
    //        _NextActionResult = actionResult;
    //    }

    //    public HttpRequestMessage Build(Link link,HttpRequestMessage request)
    //    {
    //        if (_NextActionResult == null)
    //        {
    //            return ApplyChanges(request);
    //        }
    //        else
    //        {
    //            request = _NextActionResult.Build(link,request);
    //            return ApplyChanges(request);
    //        }
    //    }

    //    public abstract HttpRequestMessage ApplyChanges(Link link,HttpRequestMessage request);

    //}
}