using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Tavis;

namespace LinkTests
{
    public class AcceptHeaderRequestBuilder : DelegatingRequestBuilder
    {
        private readonly IEnumerable<MediaTypeWithQualityHeaderValue> _AcceptHeader;

        public AcceptHeaderRequestBuilder(IEnumerable<MediaTypeWithQualityHeaderValue> acceptHeaders )
        {
            _AcceptHeader = acceptHeaders;
        }

        public override HttpRequestMessage Build(Link link, Dictionary<string, object> uriParameters, HttpMethod method, HttpContent content)
        {
            var request = base.Build(link, uriParameters, method, content);
            request.Headers.Accept.Clear();
            foreach (var headerValue in _AcceptHeader)
            {
                request.Headers.Accept.Add(headerValue);    
            }
            return request;
        }


    }
}