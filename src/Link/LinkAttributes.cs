using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
}