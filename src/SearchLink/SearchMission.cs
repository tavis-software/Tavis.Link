using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis.Search
{
    public class SearchMission
    {
        private readonly HttpClient _httpClient;
        private readonly Tavis.IANA.SearchLink _link;

        public SearchMission(HttpClient httpClient, Tavis.IANA.SearchLink link)
        {
            _httpClient = httpClient;
            _link = link;
        }

        public async Task<HttpResponseMessage> GoAsync(string param)
        {
            var openSearchDescription = await LoadOpenSearchDescription();
            var link = openSearchDescription.Url;
            link.SetParameter("searchTerms", param);
            return await _httpClient.SendAsync(link.CreateRequest());
        }

        private async Task<OpenSearchDescription> LoadOpenSearchDescription()
        {
            var response = await _httpClient.SendAsync(_link.CreateRequest());
            var desc = await response.Content.ReadAsStreamAsync();
            return new OpenSearchDescription(response.Content.Headers.ContentType, desc);
        }
       
    }
}