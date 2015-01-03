using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis
{
    public static class TaskExtensions
    {

        public static async Task<HttpResponseMessage> ApplyRepresentationToAsync(this Task<HttpResponseMessage> task, IResponseHandler responseHandler)
        {
            // What do we do with exceptions that happen here?               
            
            HttpResponseMessage response = await task;
            if (task.IsCompleted && responseHandler != null)
            {
                response = task.Result;
                string lr = "related";
                if (response.RequestMessage.Properties.ContainsKey(HttpClientExtensions.PropertyKeyLinkRelation))
                {
                    lr = response.RequestMessage.Properties[HttpClientExtensions.PropertyKeyLinkRelation] as string;
                }
                response = await responseHandler.HandleResponseAsync(lr, response);
            }
            return response;
            
        }
    }
}