using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace Tavis.Search
{
    public class OpenSearchDescription
    {
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string InputEncoding { get; set; }
        public Link Url { get; set; }
        public HttpMethod Method { get; set; }

        public OpenSearchDescription(MediaTypeHeaderValue contentType, Stream stream) : this(XDocument.Load(stream))
        {
            //if (contentType.MediaType != "application/opensearchdescription+xml")
            //{
            //    throw new NotSupportedException("Do not understand " + contentType.MediaType + " as a search description language");
            //}
        }

        public OpenSearchDescription(XDocument document)
        {
            if (document.Root == null) return;
        
            var ns = document.Root.Name.Namespace;
            var root = document.Root;

            ShortName = root.Element(ns + "ShortName").Value;
            Description = root.Element(ns + "Description").Value;
            InputEncoding = root.Element(ns + "InputEncoding").Value;
            var url = root.Element(ns + "Url");
            Url = new Link()
                {
                    Target = new Uri(url.Attribute("template").Value),
                    Type = url.Attribute("type").Value
                };
            Url.AddRequestBuilder(new InlineRequestBuilder((r) => {r.Method = new HttpMethod(url.Attribute("method").Value.ToUpper());
                                                                      return r;
            })); 

        }
    }
}