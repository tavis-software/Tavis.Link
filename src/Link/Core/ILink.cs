using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Tavis
{

    public interface ILink
    {
            Uri Context { get; set; }
            Uri Target { get; set; }
            string Relation { get; set; }
            string Anchor { get; set; }
            string Rev { get; set; }
            string Title { get; set; }
            Encoding TitleEncoding { get; set; }
            List<CultureInfo> HrefLang { get; set; }
            string Media { get; set; }
            string Type { get; set; }
            string GetLinkExtension(string name);
            void SetLinkExtension(string name, string value);
            IEnumerable<KeyValuePair<string, string>> LinkExtensions { get ; }
    }
}
