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
            return httpClient.SendAsync(link.CreateRequest())
                .ContinueWith(t =>
                {
                    if (t.IsCompleted && link.HttpResponseHandler != null)
                    {
                        return link.HandleResponseAsync(t.Result);
                    }
                    return null;  // Not sure how to return the current CurrentWith task
                }).Unwrap();
        }
    }
}
