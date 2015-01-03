using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis
{
    public class InlineResponseHandler : DelegatingResponseHandler, IResponseHandler
    {
        private readonly Action<String, HttpResponseMessage> _action;

        public InlineResponseHandler(Action<String,HttpResponseMessage> action, DelegatingResponseHandler innerHandler = null)
        {
            InnerResponseHandler = innerHandler;
            _action = action;
        }

        public override Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage responseMessage)
        {
            _action(linkRelation, responseMessage);

            return base.HandleResponseAsync(linkRelation, responseMessage);
            
        }
    }
}