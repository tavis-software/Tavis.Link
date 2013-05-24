using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tavis;

namespace LinkTests
{
    
    public class NotFoundHandler : DelegatingResponseHandler
    {
        public NotFoundHandler(DelegatingResponseHandler innerHandler) : base(innerHandler)
        {
            
        }

        public override Task<HttpResponseMessage> HandleAsync(Link link, HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("Not found");
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(responseMessage);
                return tcs.Task;
            }
            else
            {
                return base.HandleAsync(link, responseMessage);
            }
        }
    }

    public class OkHandler : DelegatingResponseHandler
    {
        public OkHandler(DelegatingResponseHandler innerHandler) : base(innerHandler)
        {

        }

        public override Task<HttpResponseMessage> HandleAsync(Link link, HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("OK!");
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(responseMessage);
                return tcs.Task;
            }
            else
            {
                return base.HandleAsync(link, responseMessage);
            }
        }
    }
}