using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinkTests;
using Tavis.UriTemplates;

namespace Tavis
{

    public class LinkAttributes: ILink
    {
        protected readonly Dictionary<string, string> _LinkExtensions = new Dictionary<string, string>();

        public Uri Context { get; set; }
        public Uri Target { get; set; }
        public string Relation { get; set; }
        public string Anchor { get; set; }
        public string Rev { get; set; }
        public string Title { get; set; }
        public Encoding TitleEncoding { get; set; }
        public List<CultureInfo> HrefLang { get; set; }
        public string Media { get; set; }
        public string Type { get; set; }
        public IEnumerable<KeyValuePair<string, string>> LinkExtensions { get { return _LinkExtensions; } }

        public LinkAttributes()
        {
            TitleEncoding = Encoding.UTF8;  // Should be ASCII but PCL does not support ascii and UTF8 does not change ASCII values 
            HrefLang = new List<CultureInfo>();
 
        }
        public string GetLinkExtension(string name)
        {
            return _LinkExtensions[name];
        }

        public void SetLinkExtension(string name, string value)
        {
            _LinkExtensions[name] = value;
        }
    }

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

        public IHttpResponseHandler HttpResponseHandler { get; set; }
        public IHttpRequestBuilder HttpRequestBuilder { get; set; }


        public Link()
        {
            Method = HttpMethod.Get;
            Relation = LinkHelper.GetLinkRelationTypeName(GetType());
            HttpRequestBuilder = new DefaultRequestBuilder();
        }

        public HttpRequestMessage CreateRequest()
        {
            return HttpRequestBuilder.Build(this,new HttpRequestMessage());
        }
 
        public Link Clone() 
        {
            var type = this.GetType();
            var newLink = (Link)Activator.CreateInstance(type);

            newLink.HttpRequestBuilder = HttpRequestBuilder;  // Can these be copied by reference, or does it need to be by-value
            newLink.HttpResponseHandler = HttpResponseHandler;
            newLink._Hints = _Hints;
            newLink.Method = Method;
            newLink.Content = Content;

            return newLink;
        }


        public void AddRequestBuilder(Func<HttpRequestMessage,HttpRequestMessage> requestBuilderFunc )
        {
            HttpRequestBuilder = new InlineRequestBuilder(requestBuilderFunc) { NextBuilder = HttpRequestBuilder }; ;
        }

        public void AddRequestBuilder(DelegatingRequestBuilder requestBuilder)
        {
            requestBuilder.NextBuilder = HttpRequestBuilder;
            HttpRequestBuilder = requestBuilder;
        }
        
        public Task<HttpResponseMessage> HandleResponseAsync(HttpResponseMessage responseMessage)
        {
            if (HttpResponseHandler != null)
            {
                return HttpResponseHandler.HandleAsync(this, responseMessage);
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

        
    }
}
