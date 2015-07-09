using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GitHubLib;
using Tavis;
using Tavis.UriTemplates;
using Xunit;


namespace GitHubLibTests
{
    public class CodeSearchLinkTests
    {
        [Fact]
        public void CreateCodeSearchRequest()
        {
            var link = new CodeSearchLink();
            var request = link.CreateRequest();

            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Null(request.RequestUri);  // URL provided by hypermedia
        }
        
        [Fact]
        public void CreateCodeSearchRequestWithUriTemplate()
        {
            var link = new CodeSearchLink()
            {
                Template = new UriTemplate("http://api.github.com/search{?query,sort,page,per_page}"),
                Query = "panic"
            };
            
            var request = link.CreateRequest();

            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("http://api.github.com/search?query=panic", request.RequestUri.AbsoluteUri);  // URL provided by hypermedia
        }

        [Fact]
        public void CreateCodeSearchRequestWithUriTemplateMoreParameters()
        {
            var link = new CodeSearchLink()
            {
                Template = new UriTemplate("http://api.github.com/search{?query,sort,page,per_page}"),
                Query = "panic",
                PerPage = 40,
                Page=3
            };

            var request = link.CreateRequest();

            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("http://api.github.com/search?query=panic&page=3&per_page=40", request.RequestUri.AbsoluteUri);  // URL provided by hypermedia
        }

        [Fact]
        public void CreateCodeSearchRequestWithSort()
        {
            var link = new CodeSearchLink()
            {
                Template = new UriTemplate("http://api.github.com/search{?query,sort,page,per_page}"),
                Query = "panic alot",
                Sort = CodeSearchLink.SearchSort.stars
            };

            var request = link.CreateRequest();

            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("http://api.github.com/search?query=panic%20alot&sort=stars", request.RequestUri.AbsoluteUri);  // URL provided by hypermedia
        }

        // Handle response inline 
        [Fact]
        public async Task HandleCodeSearchResponseInLine()
        {
            var response = new HttpResponseMessage {RequestMessage = new HttpRequestMessage()};
            response.RequestMessage.Properties[HttpClientExtensions.PropertyKeyLinkRelation] = "codesearch";
            response.Content = new StringContent("fake response");
            var task = Task.FromResult<HttpResponseMessage>(response);
            HttpContent result = null;

            await task.ApplyRepresentationToAsync(
                new InlineResponseHandler(async (linkRelation,responseMessage)=>
                {
                     switch (linkRelation)
                    {
                        case "codesearch":
                            result = responseMessage.Content;
                            break;
                    }       
                }));
            
            Assert.NotNull(result);

        }


        // Handle response in global client state object
        [Fact]
        public async Task HandleCodeSearchResponse()
        {
            var response = new HttpResponseMessage();
            var clientState = new ClientState();
            response.RequestMessage = new HttpRequestMessage();
            response.RequestMessage.Properties[HttpClientExtensions.PropertyKeyLinkRelation] = "codesearch";
            response.Content = new StringContent("Fake content");
            var task = Task.FromResult<HttpResponseMessage>(response);
            CodeSearchLink.CodeSearchResults result = null;


            await task.ApplyRepresentationToAsync(clientState);

            Assert.NotNull(clientState.SearchResult);
        }


        // Handle response in HttpMachine
        [Fact]
        public async Task HandleCodeSearchResponseInMachine()
        {
            var response = new HttpResponseMessage();
            var clientState = new ClientState();
            response.RequestMessage = new HttpRequestMessage();
            response.RequestMessage.Properties[HttpClientExtensions.PropertyKeyLinkRelation] = "codesearch";
            response.Content = new StringContent("Fake content");
            var task = Task.FromResult<HttpResponseMessage>(response);
            CodeSearchLink.CodeSearchResults result = null;

            var httpMachine = new HttpResponseMachine();
            httpMachine.AddResponseHandler(clientState.HandleResponseAsync, System.Net.HttpStatusCode.OK);
            await task.ApplyRepresentationToAsync(httpMachine);

            Assert.NotNull(clientState.SearchResult);
        }
        // Handle response in link class
        // See above example!


    }
    public class ClientState : IResponseHandler
    {
        LinkFactory _linkFactory;
        public HttpContent SearchResult;

        public async Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage responseMessage)
        {
            switch (linkRelation)
            {
                case "codesearch":
                    SearchResult = responseMessage.Content;
                    break;
            }
            return responseMessage;
        }
    }
}
