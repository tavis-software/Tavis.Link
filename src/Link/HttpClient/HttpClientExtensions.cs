using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tavis
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> FollowLinkAsync(this HttpClient httpClient, Link link)
        {
            return httpClient.FollowLinkAsync(link as IRequestFactory, link as IResponseHandler);
        }


        public static Task<HttpResponseMessage> FollowLinkAsync(this HttpClient httpClient, IRequestFactory requestFactory, IResponseHandler handler)
        {
            return httpClient.SendAsync(requestFactory.CreateRequest())
                .ContinueWith(t =>
                {
                    if (t.IsCompleted && handler != null)
                    {
                        return handler.HandleResponseAsync(t.Result);
                    }
                    return t; 
                }).Unwrap();
        }


        public static Task<HttpResponseMessage> EmbedLinkAsync(this HttpClient httpClient, Link link, IEmbedTarget embedTarget)
        {
            return httpClient.SendAsync(link.CreateRequest())
                .ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        if (link.HttpResponseHandler != null)
                        {
                            return link.HandleResponseAsync(t.Result)
                                       .ContinueWith(t2 =>
                                           embedTarget.EmbedContent(link, t2.Result.Content).ContinueWith(t4 => t2.Result)
                                         ).Unwrap();
                        }
                        return embedTarget.EmbedContent(link, t.Result.Content)
                            .ContinueWith(t3 => t.Result);
                    }

                    return t;
                }).Unwrap();
        }
    
    }
}
