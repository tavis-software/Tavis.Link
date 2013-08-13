using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Tavis.UriTemplates;

namespace Tavis
{

    /// <summary>
    /// Link class with all properties as defined by RFC 5788
    /// http://tools.ietf.org/html/rfc5988
    /// </summary>
    public class LinkRfc
    {

        public Uri Context { get; set; }
        public Uri Target { get;  set; }
        public string Relation { get; set; }
      
        // TargetAttributes
        public string Anchor { get;  set; }
        public string Rev { get;  set; }  //Deprecated byRFC5988
        public string Title { get;  set; }
        public Encoding TitleEncoding { get; set; }  // Title in alternate character set - See RFC 5987
        public List<CultureInfo> HrefLang { get;  private set; }  // More than one of these should be supported
        public string Media { get;  set; }
        public MediaTypeHeaderValue Type { get;  set; }

        public string GetLinkExtension(string name)
        {
            return _LinkExtensions[name];
        }
        public void SetLinkExtension(string name, string value)
        {
            _LinkExtensions[name] = value;
        }

        protected readonly Dictionary<string, string> _LinkExtensions = new Dictionary<string, string>();

        public IEnumerable<KeyValuePair<string, string>> LinkExtensions { get { return _LinkExtensions; } } 

        public LinkRfc() {
            TitleEncoding = Encoding.UTF8;  // Should be ASCII but PCL does not support ascii and UTF8 does not change ASCII values 
            HrefLang = new List<CultureInfo>();
        }
    }


    /// <summary>
    /// Provides behaviour to allow you to generate HTTP Requests based on Link information and also integrates URITemplating capability
    /// 
    /// This class can be subclassed with attributes and behaviour that is specific to a particular link relation
    /// </summary>
    public class Link : LinkRfc
    {
        private HttpRequestHeaders _requestHeaders;
        private readonly Dictionary<string, LinkParameter> _Parameters = new Dictionary<string, LinkParameter>();
        
        public HttpMethod Method { get; set; }
        public HttpContent Content { get; set; }

        public IHttpResponseHandler HttpResponseHandler { get; set; }

        public readonly Dictionary<string, Hint> _Hints = new Dictionary<string, Hint>();
 

        // This allows Request headers to be set and re-used for multiple requests.  Current a HttpRequestMessage can only be used once. 
        public HttpRequestHeaders RequestHeaders
        {
            get
            {
                if (_requestHeaders == null)
                {
                    var dummyMessage = new HttpRequestMessage();  // Create fake request because RequestHeaders constructor is internal
                    _requestHeaders = dummyMessage.Headers;
                }
                return _requestHeaders;
            }
            
        }

        public Link() : base()
        {
            Method = HttpMethod.Get;
        }

        public virtual HttpRequestMessage CreateRequest()
        {
            Uri resolvedTarget = Target;
            if (Target.OriginalString.Contains("{"))
            {
                resolvedTarget = GetResolvedTarget();
            }
            var requestMessage = new HttpRequestMessage()
                                     {
                                         Method = Method,
                                         RequestUri = resolvedTarget,
                                         Content = Content
                                     };

            if (_requestHeaders != null)  // If _requestheaders were never accessed then there is nothing to copy
            {
                foreach (var httpRequestHeader in RequestHeaders)
                {
                    requestMessage.Headers.Add(httpRequestHeader.Key, httpRequestHeader.Value);
                }
            }
            
            requestMessage.Headers.Referrer = Context;
            
            return requestMessage;
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

    
        public IEnumerable<string> GetParameterNames()
        {
            var uriTemplate = new UriTemplate(Target.OriginalString);
            return uriTemplate.GetParameterNames();
        }

        public Uri GetResolvedTarget()
        {
            
            var uriTemplate = new UriTemplate(Target.OriginalString);
            ApplyParameters(uriTemplate);
            var resolvedTarget = new Uri(uriTemplate.Resolve(),UriKind.RelativeOrAbsolute);
            return resolvedTarget;
        }

        public IEnumerable<LinkParameter> GetParameters()
        {
            return _Parameters.Values;
        }

        public void AddHint(Hint hint)
        {
            _Hints.Add(hint.Name, hint);
        }

        public IEnumerable<Hint> GetHints()
        {
            return _Hints.Values;
        }

        public void SetParameter(string name, object value, Uri identifier)
        {
            _Parameters[name] = new LinkParameter() { Name = name, Value = value, Identifier = identifier };
        }
        public void SetParameter(string name, object value)
        {
            _Parameters[name] = new LinkParameter() {Name = name, Value = value};
        }

        public void UnsetParameter(string name)
        {
            _Parameters.Remove(name);
        }

        private void ApplyParameters(UriTemplate uriTemplate)
        {
            foreach (var parameter in _Parameters)
            {
                if (parameter.Value.Value is IEnumerable<string>)
                {
                    uriTemplate.SetParameter(parameter.Key, (IEnumerable<string>)parameter.Value.Value);
                }
                else if (parameter.Value.Value is IDictionary<string, string>)
                {
                    uriTemplate.SetParameter(parameter.Key, (IDictionary<string, string>)parameter.Value.Value);
                }
                else
                {
                    uriTemplate.SetParameter(parameter.Key, parameter.Value.Value.ToString());
                }
            }
        }

    }

    public class LinkParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public Uri Identifier { get; set; }
    }

   
}
