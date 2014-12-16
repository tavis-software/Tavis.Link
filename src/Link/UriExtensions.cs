using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Tavis.UriTemplates;

namespace LinkTests
{
    public static class UriExtensions
    {
        public static Uri AddToQuery<T>(this Uri requestUri,T dto)
        {
            Type t = typeof (T);
            var properties = t.GetProperties();
            var dictionary = properties.ToDictionary(info => info.Name, 
                info => info.GetValue(dto, null).ToString());
            var formContent = new FormUrlEncodedContent(dictionary);

            var uriBuilder = new UriBuilder(requestUri) {Query = formContent.ReadAsStringAsync().Result};

            return uriBuilder.Uri;
        }

        public static Uri AddToQuery(this Uri requestUri, Dictionary<string,string> values)
        {

            var formContent = new FormUrlEncodedContent(values);

            var uriBuilder = new UriBuilder(requestUri) { Query = formContent.ReadAsStringAsync().Result };

            return uriBuilder.Uri;
        }
        public static Dictionary<string, object> GetQueryStringParameters(this Uri target )
        {
            Uri uri = target;
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

        public static Uri AddParametersToQueryString(this Uri target, bool? replaceQueryString, string[] linkParameters)
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

        public static Uri GetResolvedTarget(this Uri resolvedTarget, Dictionary<string, object> linkParameters, bool addNonTemplatedParametersToQueryString)
        {
            if (resolvedTarget == null) return null;

            var uriTemplate = new UriTemplate(resolvedTarget.OriginalString);

            if (addNonTemplatedParametersToQueryString)
            {
                var templateParameters = uriTemplate.GetParameterNames();
                if (linkParameters.Any(p => !templateParameters.Contains(p.Key)))
                {
                    resolvedTarget = resolvedTarget.AddParametersToQueryString(null, linkParameters.Keys.ToArray());
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


        // This should be moved into URI Template library 
        public static void ApplyParametersToTemplate(this UriTemplate uriTemplate, Dictionary<string, object> linkParameters)
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

    }
}