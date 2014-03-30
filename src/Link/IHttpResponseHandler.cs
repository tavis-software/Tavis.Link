using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis
{
    /// <summary>
    /// This interface is used to provide link objects with behaviour to be performed on the response
    /// from following a link.
    /// </summary>
    public interface IHttpResponseHandler
    {
        Task<HttpResponseMessage> HandleAsync(Link link, HttpResponseMessage responseMessage);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IHttpRequestBuilder
    {
        HttpRequestMessage Build(Link link, Dictionary<string,object> uriParameters, HttpMethod method, HttpContent content);
    }

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

    public class ActionRequestBuilder : DelegatingRequestBuilder
    {
        private readonly Action<HttpRequestMessage> _buildRequest;

        public ActionRequestBuilder(Action<HttpRequestMessage> buildRequest) 
        {
            _buildRequest = buildRequest;
        }

        public override HttpRequestMessage Build(Link link, Dictionary<string, object> uriParameters, HttpMethod method, HttpContent content)
        {
            var request = base.Build(link, uriParameters, method, content);
            _buildRequest(request);
            return request;
        }
    }


    
}