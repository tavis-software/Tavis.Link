using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GitHubLib;
using Tavis;
using Tavis.IANA;
using Xunit;

namespace GitLinksConsole
{
    public class Given_a_GitHub_ClientState
    {
        private LinkFactory _linkFactory;
        private GithubClientState _clientstate;
        private HttpClient _httpClient;

        public Given_a_GitHub_ClientState()
        {
            _linkFactory = new LinkFactory();

            GitHubHelper.RegisterGitHubLinks(_linkFactory);

            _clientstate = new GithubClientState(_linkFactory);

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyAmazingApp", "1.0"));

        }



        [Fact]
        public async Task Get_a_user()
        {

            var homeLink = _linkFactory.CreateLink<HomeLink>(new Uri("https://api.github.com"));

            await _httpClient.FollowLinkAsync(homeLink);

            var userLink = _clientstate.HomeDocument.GetLink<UserLink>();
            userLink.User = "darrelmiller";

            await _httpClient.FollowLinkAsync(userLink);

            var userDoc = _clientstate.LastDocument;
            var followersCount2 = _clientstate.CurrentUser.Followers;
            var followersCount = (int)userDoc.Properties["followers"];

            var followersLink = userDoc.GetLink<FollowersLink>();
            await _httpClient.FollowLinkAsync(followersLink);

            var followers = _clientstate.LastDocument;

            _clientstate.ClearList();
            foreach (var doc in followers.Items)
            {
                var itemLink = doc.GetLink<ItemLink>();
                await _httpClient.FollowLinkAsync(itemLink);
            }
            var results = _clientstate.List.Select(s => UserLink.InterpretResponse(s)).Where(u => u.Hireable && u.Followers > 50).ToList();

        }


        [Fact]
        public async Task Search_for_some_code()
        {

            var homeLink = _linkFactory.CreateLink<HomeLink>(new Uri("https://api.github.com"));

            await _httpClient.FollowLinkAsync(homeLink);

            var codeSearchLink = _clientstate.HomeDocument.GetLink<CodeSearchLink>();
            //codeSearchLink.Query = "addClass in:file language:js repo:jquery/jquery";
            codeSearchLink.Query = "uritemplate in:file language:csharp user:darrelmiller";

            await _httpClient.FollowLinkAsync(codeSearchLink);

            var searchResults = _clientstate.LastDocument;

            var count = codeSearchLink.InterpretResponse(searchResults);

        }

    }
}