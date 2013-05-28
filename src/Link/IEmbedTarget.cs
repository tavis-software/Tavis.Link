using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis
{
    public interface IEmbedTarget
    {
        Task EmbedContent(Link link, HttpContent content);
    }
}