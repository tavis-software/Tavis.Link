using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis
{
    public interface IHttpResponseHandler
    {
        Task<HttpResponseMessage> HandleAsync(Link link, HttpResponseMessage responseMessage);
    }
}