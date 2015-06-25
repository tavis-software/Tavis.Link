using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tavis
{
    public class HttpResponseMachine : IResponseHandler
    {
        private readonly Dictionary<HttpStatusCode, IResponseHandler> _ResponseHandlers = new Dictionary<HttpStatusCode, IResponseHandler>();

        public HttpResponseMachine()
        {
            // Default No-op handlers
            _ResponseHandlers[HttpStatusCode.Continue] = new InlineResponseHandler((s, r) => { });
            _ResponseHandlers[HttpStatusCode.OK] = new InlineResponseHandler((s, r) => { });
            _ResponseHandlers[HttpStatusCode.MultipleChoices] = new InlineResponseHandler((s, r) => { });
            _ResponseHandlers[HttpStatusCode.BadRequest] = new InlineResponseHandler((s, r) => { });
            _ResponseHandlers[HttpStatusCode.InternalServerError] = new InlineResponseHandler((s, r) => { });

        }

        public async Task<HttpResponseMessage> HandleResponseAsync(string linkrelation, HttpResponseMessage response)
        {
            var statusCode = response.StatusCode;
            if (!_ResponseHandlers.ContainsKey(statusCode))
            {
                statusCode = GetDefaultStatusCode(response.StatusCode);
            }

            return await _ResponseHandlers[statusCode].HandleResponseAsync(linkrelation, response);
        }

        private HttpStatusCode GetDefaultStatusCode(HttpStatusCode httpStatusCode)
        {
            if ((int)httpStatusCode < 200)
            {
                return HttpStatusCode.Continue; // 100
            }
            else if ((int)httpStatusCode < 300)
            {
                return HttpStatusCode.OK; //200
            }
            else if ((int)httpStatusCode < 400)
            {
                return HttpStatusCode.MultipleChoices; // 300
            }
            else if ((int)httpStatusCode < 500)
            {
                return HttpStatusCode.BadRequest; // 400
            }
            else
            {
                return HttpStatusCode.InternalServerError; // 500
            }
        }

        public void AddResponseHandler(HttpStatusCode statusCode, IResponseHandler responseHandler)
        {
            _ResponseHandlers[statusCode] = responseHandler;
        }
    }
}
