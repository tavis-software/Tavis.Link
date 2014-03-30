using System.Collections.Generic;
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
            
            return await _httpClient.SendAsync(link.BuildRequestMessage(new Dictionary<string, object> { { "searchTerms", param } }));
        }

        private async Task<OpenSearchDescription> LoadOpenSearchDescription()
        {
            var response = await _httpClient.SendAsync(_link.BuildRequestMessage());
            var desc = await response.Content.ReadAsStreamAsync();
            return new OpenSearchDescription(response.Content.Headers.ContentType, desc);
        }
       
    }
}