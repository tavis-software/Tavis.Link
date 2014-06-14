using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
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