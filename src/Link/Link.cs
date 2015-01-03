using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinkTests;
using Tavis.UriTemplates;

namespace Tavis
{
    /// <summary>
    /// Link class augments the base LinkAttributes class with abilities to:
    ///     - create HttpRequestMessage
    ///     - attach link hints
    ///     - attach response handling behaviour
    ///     - support for URI templates
    /// 
    /// This class can be subclassed with attributes and behaviour that is specific to a particular link relation
    /// </summary>
    /// 
    /// 
    public class Link : LinkAttributes, IRequestFactory, IResponseHandler
    {
        // Client Defined Attributes
        public HttpContent Content { get; set; }
        public HttpMethod Method { get; set; }

        // Server Defined Attrbutes
        private Dictionary<string, Hint> _Hints = new Dictionary<string, Hint>();
        private UriTemplate _template;
        private DelegatingRequestBuilder _httpRequestBuilder;
        private DelegatingResponseHandler _httpResponseHandler;

        public UriTemplate Template
        {
            get
            {
                // Not sure how wise this is. I don't like having a get be unsafe
                if (_template == null && Target != null && Target.OriginalString.Contains("{"))
                {
                    _template = new UriTemplate(Target.OriginalString);
                }
                return _template;
            }
            set { _template = value; }
        }

        
        public Link()
        {
            Method = HttpMethod.Get;
            Relation = LinkHelper.GetLinkRelationTypeName(GetType());
            _httpRequestBuilder = new DefaultRequestBuilder();
        }

        public string LinkRelation
        {
            get { return Relation; }
        }
        public HttpRequestMessage CreateRequest()
        {
            return _httpRequestBuilder.Build(this,new HttpRequestMessage());
        }
 
        public Link Clone() 
        {
            var type = this.GetType();
            var newLink = (Link)Activator.CreateInstance(type);

            newLink._httpRequestBuilder = _httpRequestBuilder;  // Can these be copied by reference, or does it need to be by-value
            newLink._httpResponseHandler = _httpResponseHandler;
            newLink._Hints = _Hints;
            newLink.Method = Method;
            newLink.Content = Content;

            return newLink;
        }


        public void AddRequestBuilder(Func<HttpRequestMessage,HttpRequestMessage> requestBuilderFunc )
        {
            AddRequestBuilder( new InlineRequestBuilder(requestBuilderFunc));
        }

        public void AddRequestBuilder(DelegatingRequestBuilder requestBuilder)
        {
            if (_httpRequestBuilder != null)
            {
                _httpRequestBuilder.NextBuilder = requestBuilder;
            }
            else
            {
                _httpRequestBuilder = requestBuilder;
            }           
        }
        
        public Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage responseMessage)
        {
            if (_httpResponseHandler != null)
            {
                return _httpResponseHandler.HandleResponseAsync(this.Relation, responseMessage);
            }
            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(responseMessage);
            return tcs.Task;
        }
    
        public void AddHint(Hint hint)
        {
            _Hints.Add(hint.Name, hint);
        }

        public IEnumerable<Hint> GetHints()
        {
            return _Hints.Values;
        }

        public void AddResponseHandler(Action<string,HttpResponseMessage> responseHandlerFunc)
        {
            AddResponseHandler(new InlineResponseHandler(responseHandlerFunc, _httpResponseHandler));
        }

        public  void AddResponseHandler(DelegatingResponseHandler responseHandler)
        {

            if (_httpResponseHandler == null)
            {
                _httpResponseHandler = responseHandler;
            }
            else
            {
                var currentHandler = _httpResponseHandler as DelegatingResponseHandler;
                if (currentHandler == null) throw new Exception("Cannot add handler unless existing handler is a delegating handler");

                while (currentHandler != null)
                {
                    if (currentHandler.InnerResponseHandler == null)
                    {
                        currentHandler.InnerResponseHandler = responseHandler;
                        currentHandler = null;
                    }
                    else
                    {
                        currentHandler = currentHandler.InnerResponseHandler;
                    }
                }
            }
        }

        
    }
}
