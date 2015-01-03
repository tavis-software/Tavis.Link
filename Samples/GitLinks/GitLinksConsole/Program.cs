using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GitHubLib;
using Tavis;
using Tavis.IANA;

namespace GitLinksConsole
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }

    public static class LinkFactoryExtensions
    {
        public static T CreateLink<T>(this LinkFactory linkFactory, Uri target) where T : Link, new()
        {
            var link = linkFactory.CreateLink<T>();
            link.Target = target;
            return link;
        }
    }


    public class NonSuccessHandler : DelegatingResponseHandler
    {
        public override Task<HttpResponseMessage> HandleAsync(Link link, HttpResponseMessage responseMessage)
        {
            return base.HandleAsync(link, responseMessage);
        }
    }


    public class GithubClientState
    {
        private readonly LinkFactory _linkFactory;
        private GithubDocument _homeDocument;
        private UserLink.UserResult _currentUser;
        private GithubDocument _lastDocument;

        public GithubClientState(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
            ConfigureLinkBehaviour();
        }

        private void ConfigureLinkBehaviour()
        {
            _linkFactory.SetHandler<HomeLink>(new ActionResponseHandler(HandleHomeLinkResponse));
            _linkFactory.SetHandler<UserLink>(new ActionResponseHandler(HandleUserLinkResponse));
            _linkFactory.SetHandler<CodeSearchLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            _linkFactory.SetHandler<EmojisLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            _linkFactory.SetHandler<FollowingLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            _linkFactory.SetHandler<FollowersLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            _linkFactory.SetHandler<GistsLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            _linkFactory.SetHandler<ItemLink>(new ActionResponseHandler(HandleItemResponse));
        }

        public UserLink.UserResult CurrentUser
        {
            get { return _currentUser; }
        }

        public GithubDocument HomeDocument
        {
            get { return _homeDocument; }
        }

        public GithubDocument LastDocument
        {
            get { return _lastDocument; }
        }

        public List<GithubDocument> List
        {
            get { return _list; }
          
        }

        private async Task HandleHomeLinkResponse(Link link, HttpResponseMessage responseMessage)
        {
            _homeDocument = await responseMessage.Content.ReadAsGithubDocumentAsync(_linkFactory);
            _lastDocument = HomeDocument;
        }

        private async Task HandleUserLinkResponse(Link link, HttpResponseMessage responseMessage)
        {
            _lastDocument = await responseMessage.Content.ReadAsGithubDocumentAsync(_linkFactory);
            _currentUser = UserLink.InterpretResponse(_lastDocument);
        }

        private async Task HandleStandardDocumentResponse(Link link, HttpResponseMessage responseMessage)
        {
            _lastDocument = await responseMessage.Content.ReadAsGithubDocumentAsync(_linkFactory);
        }

        private async Task HandleItemResponse(Link link, HttpResponseMessage responseMessage)
        {
            var itemDoc = await responseMessage.Content.ReadAsGithubDocumentAsync(_linkFactory);
            List.Add(itemDoc);
        }

        internal void ClearList()
        {
            List.Clear();
        }

        private List<GithubDocument> _list = new List<GithubDocument>(); 
    }
}

