using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
    public class InlineRequestBuilder : DelegatingRequestBuilder
    {
        private readonly Func<HttpRequestMessage, HttpRequestMessage> _customizeRequest;

        public InlineRequestBuilder(Func<HttpRequestMessage, HttpRequestMessage> customizeRequest)
        {
            _customizeRequest = customizeRequest;
        }

        protected override HttpRequestMessage ApplyChanges(Link link,HttpRequestMessage request)
        {
            return _customizeRequest(request);
        }
    }
}