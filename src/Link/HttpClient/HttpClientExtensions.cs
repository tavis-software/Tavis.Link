using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis
{
    public static class HttpClientExtensions
    {
        public const string PropertyKeyLinkRelation = "tavis.linkrelation";

        public static Task<HttpResponseMessage> FollowLinkAsync(this HttpClient httpClient, Link link)
        {
            return httpClient.FollowLinkAsync(link as IRequestFactory, link as IResponseHandler);
        }


        public static Task<HttpResponseMessage> FollowLinkAsync(this HttpClient httpClient, IRequestFactory requestFactory, IResponseHandler handler)
        {

            var httpRequestMessage = requestFactory.CreateRequest();
            httpRequestMessage.Properties[PropertyKeyLinkRelation] = requestFactory.LinkRelation;
            return httpClient.SendAsync(httpRequestMessage)
                .ContinueWith(t =>
                {
                    if (t.IsCompleted && handler != null)
                    {
                        return handler.HandleResponseAsync(requestFactory.LinkRelation, t.Result);
                    }
                    return t;
                }).Unwrap();
        }

        public static Task<HttpResponseMessage> FollowLinkAsync(this HttpClient httpClient, IRequestFactory requestFactory)
        {
            var httpRequestMessage = requestFactory.CreateRequest();
            httpRequestMessage.Properties[PropertyKeyLinkRelation] = requestFactory.LinkRelation;
            return httpClient.SendAsync(httpRequestMessage);
        }
    }
}
