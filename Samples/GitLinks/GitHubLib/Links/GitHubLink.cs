using System.Net.Http;
using System.Threading.Tasks;
using Tavis;

namespace GitHubLib
{
    public class GitHubLink : Link
    {
        public static async Task<GithubDocument> ParseContent(HttpContent content, LinkFactory linkFactory)
        {
            var stream = await content.ReadAsStreamAsync();
            return new GithubDocument(stream, linkFactory);
        }
    }
}