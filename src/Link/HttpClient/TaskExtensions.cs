using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis
{
    public static class TaskExtensions
    {
        public static Task ApplyRepresentationToAsync(this Task<HttpResponseMessage> task, IResponseHandler responseHandler)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsCompleted)
                {
                    var response = t.Result;
                    string lr = "related";
                    if (response.RequestMessage.Properties.ContainsKey(HttpClientExtensions.PropertyKeyLinkRelation))
                    {
                        lr = response.RequestMessage.Properties[HttpClientExtensions.PropertyKeyLinkRelation] as string;
                    }
                    return responseHandler.HandleResponseAsync(lr, response);
                }
                return t;
            });
        }
    }
}