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

        public HttpRequestMessage Build(HttpRequestMessage request)
        {
            request = ApplyChanges(request);

            if (NextBuilder != null)
            { 
                request = NextBuilder.Build(request);
            }
            return request;
            
        }

        protected abstract HttpRequestMessage ApplyChanges(HttpRequestMessage request);
    }

    public abstract class BaseChainedRequestBuilder : IHttpRequestBuilder
    {
        private readonly IHttpRequestBuilder _NextActionResult;

        protected BaseChainedRequestBuilder(IHttpRequestBuilder actionResult)
        {
            _NextActionResult = actionResult;
        }

        public HttpRequestMessage Build(HttpRequestMessage request)
        {
            if (_NextActionResult == null)
            {
                return ApplyChanges(request);
            }
            else
            {
                request = _NextActionResult.Build(request);
                return ApplyChanges(request);
            }
        }

        public abstract HttpRequestMessage ApplyChanges(HttpRequestMessage request);

    }
}