using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Tavis.IANA;
using Tavis.Search;
using Xunit;

namespace Samples
{
    public class SearchTests
    {
        [Fact]
        public void SearchForFoo()
        {
            var httpClient = new HttpClient();
            var searchLink = new SearchLink()
                {
                    Target = new Uri("http://www.stackoverflow.com/opensearch.xml"),
                    //Target = new Uri("https://www.w3.org/Bugs/Public/search_plugin.cgi"),
                    Type = "application/opensearchdescription+xml"
                };
            var searchMission = new SearchMission(httpClient, searchLink);
            var response = searchMission.GoAsync("evolvable").Result;
            var results = response.Content.ReadAsStringAsync().Result;

            var response2 = searchMission.GoAsync("issue 14").Result;
            var results2 = response2.Content.ReadAsStringAsync().Result;

        }
    }
}
