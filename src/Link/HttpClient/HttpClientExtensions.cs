using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis
{
    public static class HttpClientExtensions
    {
        public const string PropertyKeyLinkRelation = "tavis.linkrelation";


        public static Task<HttpResponseMessage> FollowLinkAsync(
            this HttpClient httpClient, 
            IRequestFactory requestFactory, 
            IResponseHandler handler = null) {

            var httpRequestMessage = requestFactory.CreateRequest();
            httpRequestMessage.Properties[PropertyKeyLinkRelation] = requestFactory.LinkRelation;

            return httpClient.SendAsync(httpRequestMessage)
                .ApplyRepresentationToAsync(handler);
        }

    }
}
