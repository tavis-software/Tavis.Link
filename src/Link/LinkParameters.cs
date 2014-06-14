using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Tavis
{
    public class LinkParameters
    {
        HttpMethod Method { get; set; }
        Dictionary<string, object> UrlParameters { get; set; }
        HttpContent Content { get; set; }
    }
}
