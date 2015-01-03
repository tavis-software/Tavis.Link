using System.Net.Http;
using System.Threading.Tasks;
using Tavis;

namespace GitHubLib
{
    public static class HttpContentExtensions
    {
        public static async Task<GithubDocument> ReadAsGithubDocumentAsync(this HttpContent content, LinkFactory linkFactory)
        {
            var stream = await content.ReadAsStreamAsync();
            return new GithubDocument(stream, linkFactory);
        }

    }
}