﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public string Rev { get;  set; }
        public string Title { get;  set; }
        public string HrefLang { get;  set; }
        public string Media { get;  set; }
        public string Type { get;  set; }

        public string GetLinkExtension(string name)
        {
            return _LinkExtensions[name];
        }
        public void SetLinkExtension(string name, string value)
        {
            _LinkExtensions[name] = value;
        }

        private readonly Dictionary<string, string> _LinkExtensions = new Dictionary<string, string>();
    }


    /// <summary>
    /// Provides behaviour to allow you to generate HTTP Requests based on Link information and also integrates URITemplating capability
    /// 
    /// This class can be subclassed with attributes and behaviour that is specific to a particular link relation
    /// </summary>
    public class Link : LinkRfc
    {
        private HttpRequestHeaders _requestHeaders;
        private readonly Dictionary<string, object> _Parameters = new Dictionary<string, object>();
        
        public HttpMethod Method { get; set; }
        public HttpContent Content { get; set; }

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

        public Link()
        {
            Method = HttpMethod.Get;
        }

        public HttpRequestMessage CreateRequest()
        {
            Uri resolvedTarget = Target;
            if (Target.OriginalString.Contains("{"))
            {
                var uriTemplate = new UriTemplate(Target.OriginalString);
                ApplyParameters(uriTemplate);
                resolvedTarget = new Uri(uriTemplate.Resolve(),UriKind.RelativeOrAbsolute);
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

        public void AddParameter(string name, object value)
        {
            _Parameters.Add(name, value);
        }

        private void ApplyParameters(UriTemplate uriTemplate)
        {
            foreach (var parameter in _Parameters)
            {
                if (parameter is IEnumerable<string>)
                {
                    uriTemplate.SetParameter(parameter.Key, (IEnumerable<string>)parameter.Value);
                }
                else if (parameter is IDictionary<string, string>)
                {
                    uriTemplate.SetParameter(parameter.Key, (IDictionary<string, string>)parameter.Value);
                }
                else
                {
                    uriTemplate.SetParameter(parameter.Key, parameter.Value.ToString());
                }
            }
        }
    }
}