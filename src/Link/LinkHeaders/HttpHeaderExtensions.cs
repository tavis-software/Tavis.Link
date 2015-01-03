using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Tavis
{
    public static class HttpHeaderExtensions
    {
        
        public static void AddLinkHeader(this HttpHeaders headers, ILink link )
        {
            var headerValue = link.AsLinkHeader();
            headers.Add("Link", headerValue);
        }

        public static void AddLinkHeaders(this HttpHeaders headers, List<ILink> links)
        {
            string headerValue = string.Empty;
            foreach (var link in links)
            {
                headerValue += link.AsLinkHeader();
                headerValue += ", ";
            }

            headerValue = headerValue.Substring(0, headerValue.Length - 2);

            headers.Add("Link", headerValue);
        }

        
        
        public static List<ILink> ParseLinkHeaders(this HttpResponseMessage responseMessage, ILinkFactory linkRegistry)
        {
            return ParseLinkHeaders(responseMessage.Headers, responseMessage.RequestMessage.RequestUri, linkRegistry);
        }

        public static List<ILink> ParseLinkHeaders(this HttpHeaders headers, Uri baseUri, ILinkFactory linkRegistry)
        {
            var list = new List<ILink>();
            var parser = new LinkHeaderParser(linkRegistry);
            var linkHeaders = headers.GetValues("Link");
            foreach (var linkHeader in linkHeaders)
            {
                list.AddRange(parser.Parse(baseUri, linkHeader));
            }
            return list;
        }

        public static IList<ILink> ParseLinkHeader(this ILink link, string linkHeader, ILinkFactory linkRegistry)
        {
            var parser = new LinkHeaderParser(linkRegistry);
            return parser.Parse(link.Target, linkHeader);
        }


    }
}
