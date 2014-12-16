using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Xml.Linq;
using Tavis.UriTemplates;

namespace Tavis
{
    public class DefaultRequestBuilder : DelegatingRequestBuilder
    {
        protected override HttpRequestMessage ApplyChanges(Link link, HttpRequestMessage request)
        {
            // if there is a template, then get parameters and try and find property values
            if (link.Template != null)
            {
                var parameters = link.Template.GetParameterNames();
                var props = link.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var matchingProps = props.Where(p => 
                    parameters.Any(n => String.Compare(n,p.Name,StringComparison.OrdinalIgnoreCase) == 0)).ToList();

                foreach (var prop in matchingProps)
                {
                    link.Template.AddParameter(prop.Name.ToLower(), prop.GetValue(link,null));
                }
                link.Target = new Uri(link.Template.Resolve());
            }

            request.Method = link.Method;
            request.RequestUri = link.Target;
            request.Content = link.Content;

            request = ApplyHints(request, link.GetHints());


            return request;
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