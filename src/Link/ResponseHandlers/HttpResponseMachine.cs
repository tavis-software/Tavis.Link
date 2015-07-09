using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tavis
{
   

    public class HttpResponseMachine : HttpResponseMachine<object>
    {
        public HttpResponseMachine()
            : base(null)
        {
        }

        public void AddResponseHandler(Func<string, HttpResponseMessage, Task<HttpResponseMessage>> responseHandler, HttpStatusCode statusCode, string linkRelation = null, MediaTypeHeaderValue contentType = null, Uri profile = null)
        {
            this.AddResponseHandler((m, l, r) => responseHandler(l, r), statusCode, linkRelation: linkRelation, contentType: contentType, profile: profile);
        }

    }

    public class HttpResponseMachine<T> : IResponseHandler
    {
        private readonly T _Model;
        private readonly List<HandlerKey> _ResponseHandlers = new List<HandlerKey>();

        public delegate Task<HttpResponseMessage> ResponseHandler<T>(T clientstate, string linkRelation, HttpResponseMessage response);

        public HttpResponseMachine(T model)
        {
            _Model = model;
        }

        public async Task<HttpResponseMessage> HandleResponseAsync(string linkrelation, HttpResponseMessage response)
        {
            var handlerKey = new HandlerKey(response, linkrelation);

            var handlerResult = FindHandler(response, handlerKey);

            return await handlerResult.ResponseHandler(_Model, linkrelation, response);
        }

        private HandlerResult FindHandler(HttpResponseMessage response, HandlerKey responseHandlerKey)
        {
            var statusHandlers = _ResponseHandlers.Where(h => h.StatusCode == responseHandlerKey.StatusCode);
            if (!statusHandlers.Any())
            {
                responseHandlerKey.StatusCode = GetDefaultStatusCode(response.StatusCode);
            }


            var handlerResults = statusHandlers.Where(h => h.StatusCode == responseHandlerKey.StatusCode
                                                           && (h.ContentType == null ||
                                                            h.ContentType.Equals(responseHandlerKey.ContentType))
                                                           && (h.Profile == null || h.Profile == responseHandlerKey.Profile)
                                                           && (String.IsNullOrEmpty(h.LinkRelation) ||
                                                            h.LinkRelation == responseHandlerKey.LinkRelation))
                .Select(h => new HandlerResult()
                {
                    ResponseHandler = h.ResponseHandler,
                    Score = (h.ContentType != null ? 8 : 0) + (h.LinkRelation != null ? 2 : 0) + (h.Profile != null ? 2 : 0)
                });

            var handler = handlerResults.OrderByDescending(h => h.Score).First();
            return handler;
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


        public void AddResponseHandler(ResponseHandler<T> responseHandler, HttpStatusCode statusCode, string linkRelation = null, MediaTypeHeaderValue contentType = null, Uri profile = null)
        {
            var key = new HandlerKey()
            {
                StatusCode = statusCode,
                ContentType = contentType,
                Profile = profile,
                LinkRelation = linkRelation,
                ResponseHandler = responseHandler
            };
            _ResponseHandlers.Add(key);
        }


        private class HandlerKey
        {
            public HandlerKey()
            {

            }

            public HandlerKey(HttpResponseMessage response, string linkRelation)
            {
                StatusCode = response.StatusCode;
                if (response.Content != null)
                {
                    ContentType = response.Content.Headers.ContentType;
                    // Hunt for profile (m/t Parameters, Link Header)
                }
                LinkRelation = linkRelation;
            }
            public HttpStatusCode StatusCode { get; set; }
            public MediaTypeHeaderValue ContentType { get; set; }
            public Uri Profile { get; set; }
            public string LinkRelation { get; set; }
            public ResponseHandler<T> ResponseHandler { get; set; }

        }

        private class HandlerResult
        {
            public int Score { get; set; }
            public ResponseHandler<T> ResponseHandler { get; set; }
        }


    }

    public class Model<T>
    {
        public T Value { get; set; }
    }
}
