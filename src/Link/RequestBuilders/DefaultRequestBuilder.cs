using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Xml.Linq;
using Tavis.RequestBuilders;
using Tavis.UriTemplates;

namespace Tavis
{
    public class DefaultRequestBuilder : DelegatingRequestBuilder
    {
        protected override HttpRequestMessage ApplyChanges(ILink stdlink, HttpRequestMessage request)
        {
            var link = stdlink as Link;

            if (link != null)
            {
                // if there is a template, then get parameters and try and find property values
                if (link.Template != null)
                {

                    var matchingProps = GetBindingProperties(link)
                        .Where(p => link.Template.GetParameterNames()
                                    .Any(n => String.Compare(n, p.Key, StringComparison.OrdinalIgnoreCase) == 0))
                        .ToList();

                    foreach (var prop in matchingProps)
                    {
                        object value = prop.Value.PropertyInfo.GetValue(link, null);
                        object defaultValue = GetDefault(prop.Value);
                        if (!value.Equals(defaultValue))
                        {
                            link.Template.AddParameter(prop.Key, value);
                        }
                    }
                    link.Target = new Uri(link.Template.Resolve(), UriKind.RelativeOrAbsolute);
                }

                request.Method = link.Method;
                
                request.Content = link.Content;

                request = ApplyHints(request, link.GetHints());
            }

            request.RequestUri = stdlink.Target;
            return request;
        }

        internal static object GetDefault(ParameterInfo info)
        {
            if (info.Attribute != null)
            {
                return info.Attribute.Default;
            }

            if (info.PropertyInfo.PropertyType.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(info.PropertyInfo.PropertyType);
            }
            return null;
        }

        private static Dictionary<string, ParameterInfo> GetBindingProperties(Link link)
        {
            var props = link.GetType()
                .GetTypeInfo()
                .DeclaredProperties 
                .Select(p => new ParameterInfo()
                {
                    PropertyInfo = p,
                    Attribute =  p.GetCustomAttributes<LinkParameterAttribute>()
                    .FirstOrDefault()
                })
                .ToDictionary(pi =>
                {
                    return pi.Attribute == null ? pi.PropertyInfo.Name.ToLowerInvariant() : pi.Attribute.Name;
                }, pr => pr);
            return props;
        }

        internal class ParameterInfo
        {
            public PropertyInfo PropertyInfo { get; set; }
            public LinkParameterAttribute Attribute { get; set; }
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

    }
}