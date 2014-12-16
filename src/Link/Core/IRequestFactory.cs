using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Tavis
{
    public interface IRequestFactory
    {
        HttpRequestMessage CreateRequest();
    }
}
