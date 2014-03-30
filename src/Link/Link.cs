using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tavis.UriTemplates;

namespace Tavis
{
    /// <summary>
    /// Link class augments the base LinkRfc class with abilities to:
    ///     - create HttpRequestMessage
    ///     - attach link hints
    ///     - attach response handling behaviour
    ///     - support for URI templates
    /// 
    /// This class can be subclassed with attributes and behaviour that is specific to a particular link relation
    /// </summary>
    public class Link : LinkRfc
    {
        public bool AddNonTemplatedParametersToQueryString { get; set; }

        /// <summary>
        /// A handler with knowledge of how to process the response to following a link.  
        /// </summary>
        /// <remarks>
        /// The use of reponse handlers is completely optional.  They become valuable when media type deserializers use the LinkFactory which is has behaviours pre-registered
        /// </remarks>
        public IHttpResponseHandler HttpResponseHandler { get; set; }

        /// <summary>
        /// Extension point for changing the way this Link builds HttpRequestMessages
        /// </summary>
        public IHttpRequestBuilder HttpRequestBuilder { get; set; }

        /// <summary>
        /// Create an instance of a link.  
        /// </summary>
        /// <remarks>
        /// The empty constructor makes it easier for deserializers to create links.
        /// </remarks>
        public Link()
        {
            Relation = LinkHelper.GetLinkRelationTypeName(GetType());
            HttpRequestBuilder = new DefaultRequestBuilder();
        }

        /// <summary>
        /// Create an HTTPRequestMessage based on the information stored in the link.
        /// </summary>
        /// <returns></returns>
        public HttpRequestMessage BuildRequestMessage()
        {
            return BuildRequestMessage(null, HttpMethod.Get);
        }

        public HttpRequestMessage BuildRequestMessage(HttpMethod method , HttpContent content = null)
        {
            return BuildRequestMessage(null, method, content);
        }

        public HttpRequestMessage BuildRequestMessage(Dictionary<string, object> linkParameters, HttpMethod method = null, HttpContent content = null)
        {
            if (linkParameters == null) linkParameters = new Dictionary<string, object>();
            if (method == null) method = HttpMethod.Get;

            return HttpRequestBuilder.Build(this, linkParameters, method, content);
        }

        public void AddRequestBuilder(DelegatingRequestBuilder requestBuilder)
        {
            requestBuilder.InnerBuilder = HttpRequestBuilder;
            HttpRequestBuilder = requestBuilder;
        }

        /// <summary>
        /// Entry point for triggering the execution of the assigned HttpResponseHandler if one exists
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
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
    
        /// <summary>
        /// Returns list of URI Template parameters specified in the Target URI
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetParameterNames()
        {
            var uriTemplate = new UriTemplate(Target.OriginalString);
            return uriTemplate.GetParameterNames();
        }


        /// <summary>
        /// Add a hint to the link.  These hints can be used for serializing into representations on the server, or used to modify the behaviour of the CreateRequestMessage method
        /// </summary>
        /// <param name="hint"></param>
        public void AddHint(Hint hint)
        {
            _Hints.Add(hint.Name, hint);
        }

        /// <summary>
        /// Returns a list of assigned link hints
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Hint> GetHints()
        {
            return _Hints.Values;
        }

        public Dictionary<string, object> GetQueryStringParameters()
        {
            Uri uri = Target;
            var parameters = new Dictionary<string, object>();

            var reg = new Regex(@"([-A-Za-z0-9._~]*)=([^&]*)&?");		// Unreserved characters: http://tools.ietf.org/html/rfc3986#section-2.3
            foreach (Match m in reg.Matches(uri.Query))
            {
                string key = m.Groups[1].Value.ToLowerInvariant();
                string value = m.Groups[2].Value;
                parameters.Add(key, value);
            }
            return parameters;
        }

        public static HttpRequestMessage ApplyHints(HttpRequestMessage requestMessage, IEnumerable<Hint> hints)
        {
            foreach (var hint in hints)
            {
                if (hint.ConfigureRequest != null)
                {
                    requestMessage = hint.ConfigureRequest(hint, requestMessage);
                }
            }
            return requestMessage;
        }

        public static void CopyDefaultHeaders(HttpRequestMessage requestMessage, HttpRequestHeaders defaultRequestHeaders)
        {
            if (defaultRequestHeaders != null) // If _requestheaders were never accessed then there is nothing to copy
            {
                foreach (var httpRequestHeader in defaultRequestHeaders)
                {
                    requestMessage.Headers.Add(httpRequestHeader.Key, httpRequestHeader.Value);
                }
            }

        }

        public static Uri GetResolvedTarget(Uri resolvedTarget, Dictionary<string, object> linkParameters, bool addNonTemplatedParametersToQueryString)
        {
            if (resolvedTarget == null) return null;

            var uriTemplate = new UriTemplate(resolvedTarget.OriginalString);

            if (addNonTemplatedParametersToQueryString)
            {
                var templateParameters = uriTemplate.GetParameterNames();
                if (linkParameters.Any(p => !templateParameters.Contains(p.Key)))
                {
                    resolvedTarget = AddParametersToQueryString(null, linkParameters.Keys.ToArray(), resolvedTarget);
                    uriTemplate = new UriTemplate(resolvedTarget.OriginalString);
                }
            }

            if (resolvedTarget.OriginalString.Contains("{"))
            {
                ApplyParametersToTemplate(uriTemplate, linkParameters);
                resolvedTarget = new Uri(uriTemplate.Resolve(), UriKind.RelativeOrAbsolute);
                return resolvedTarget;
            }

            return resolvedTarget;
        }

        

        private static Uri AddParametersToQueryString(bool? replaceQueryString, string[] linkParameters, Uri target)
        {
            var queryTokens = String.Join(",", linkParameters.
                Where(k => !target.OriginalString.Contains("{" + k + "}"))
                .Select(p => p).ToArray());

            // If query string already contains a parameter, then assume replace.
            replaceQueryString = replaceQueryString ?? linkParameters.Any(k => target.Query.Contains(k + "="));

            string queryStringTemplate = null;
            if (replaceQueryString == true || String.IsNullOrEmpty(target.Query))
            {
                queryStringTemplate = "{?" + queryTokens + "}";
            }
            else
            {
                queryStringTemplate = "{&" + queryTokens + "}";
            }

            var targetUri = target.OriginalString;

            if (replaceQueryString == true)
            {
                var queryStart = targetUri.IndexOf("?");
                if (queryStart >= 0)
                {
                    targetUri = targetUri.Substring(0, queryStart);
                }
            }

            return new Uri(targetUri + queryStringTemplate);
        }

        private static void ApplyParametersToTemplate(UriTemplate uriTemplate, Dictionary<string, object> linkParameters)
        {
            foreach (var parameter in linkParameters)
            {
                if (parameter.Value is IEnumerable<string>)
                {
                    uriTemplate.SetParameter(parameter.Key, (IEnumerable<string>)parameter.Value);
                }
                else if (parameter.Value is IDictionary<string, string>)
                {
                    uriTemplate.SetParameter(parameter.Key, (IDictionary<string, string>)parameter.Value);
                }
                else
                {
                    uriTemplate.SetParameter(parameter.Key, parameter.Value.ToString());
                }
            }
        }

        private readonly Dictionary<string, Hint> _Hints = new Dictionary<string, Hint>();

    }



    public class DefaultRequestBuilder : IHttpRequestBuilder
    {
     

        public HttpRequestMessage Build(Link link,Dictionary<string, object> uriParameters, HttpMethod method,  HttpContent content)
        {

            Uri resolvedTarget = Link.GetResolvedTarget(link.Target, uriParameters, link.AddNonTemplatedParametersToQueryString);

            var requestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = resolvedTarget,
                Content = content
            };

            //if (link.RequestHeaders != null) Link.CopyDefaultHeaders(requestMessage, link.RequestHeaders);

            requestMessage = Link.ApplyHints(requestMessage, link.GetHints());
            return requestMessage;
        }
    }
}
