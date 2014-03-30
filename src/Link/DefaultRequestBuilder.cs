using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
    public class DefaultRequestBuilder : IHttpRequestBuilder
    {
     

        public HttpRequestMessage Build(Link link,Dictionary<string, object> uriParameters, HttpMethod method,  HttpContent content)
        {

            Uri resolvedTarget = Link.GetResolvedTarget(link.Target, uriParameters, link.AddNonTemplatedParametersToQueryString);

            var requestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = resolvedTarget,
                Content = content
            };

         
            requestMessage = Link.ApplyHints(requestMessage, link.GetHints());
            return requestMessage;
        }
    }
}