using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tavis;

namespace LinkTests
{
    public class ActionResponseHandler : DelegatingResponseHandler
    {
        private readonly Action<HttpResponseMessage> _action;

        public ActionResponseHandler(Action<HttpResponseMessage> action, DelegatingResponseHandler innerHandler = null)
        {
            InnerResponseHandler = innerHandler;
            _action = action;
        }

        public override Task<HttpResponseMessage> HandleAsync(Link link, HttpResponseMessage responseMessage)
        {
            _action(responseMessage);

            return base.HandleAsync(link, responseMessage);
            
        }
    }
}