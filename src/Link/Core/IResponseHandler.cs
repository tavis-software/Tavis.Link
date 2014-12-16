using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tavis
{
    public interface IResponseHandler
    {
        Task<HttpResponseMessage> HandleResponseAsync(HttpResponseMessage responseMessage);
    }
}
