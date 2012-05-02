using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tavis;
using Xunit;

namespace LinkTests
{
    public class LinkFixture
    {

        [Fact]
        public void DefaultLinkShouldCreateAGetRequest()
        {
            var link = new Link() { Target = new Uri("Http://localhost") };

            var request = link.CreateRequest();

            Assert.Equal(request.Method, link.Method);
        }


        [Fact]
        public void SettingAnAcceptHeaderShouldBePassedToTheRequest()
        {
            var link = new Link { Target = new Uri("Http://localhost") };


            var acceptHeader = new MediaTypeWithQualityHeaderValue("application/vnd.hal");
            link.RequestHeaders.Accept.Add(acceptHeader);

            var request = link.CreateRequest();

            Assert.True(request.Headers.Accept.Where(h => h.MediaType == "application/vnd.hal").Any());
        }


        [Fact]
        public void SettingContentShouldBePassedToTheRequest()
        {
            var link = new Link { Target = new Uri("Http://localhost") };
            link.Content = new StringContent("Hello World");
            link.Method = HttpMethod.Post;
            var request = link.CreateRequest();

            Assert.Equal(request.Content.ReadAsStringAsync().Result, "Hello World");
        }


        [Fact]
        public void UseLinkToMakeRequest()
        {
            var link = new Link { Target = new Uri("Http://localhost") };
            var client = new HttpClient(new FakeMessageHandler());

            var response = client.SendAsync(link.CreateRequest()).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        //Add a parameter to a link
        [Fact]
        public void UseURITemplateAsLinkSource()
        {
            var link = new Link { Target = new Uri("Http://localhost") }; 
            var client = new HttpClient(new FakeMessageHandler());

            var response = client.SendAsync(link.CreateRequest()).Result;

            Assert.Equal("http://localhost/", response.RequestMessage.RequestUri.AbsoluteUri);
        }

        [Fact]
        public void AddParameterToLink()
        {
            var link = new Link(){ Target = new Uri("http://localhost/{?foo}")};
        link.AddParameter("foo","bar");
            
            var client = new HttpClient(new FakeMessageHandler());

            var response = client.SendAsync(link.CreateRequest()).Result;

            Assert.Equal("http://localhost/?foo=bar", response.RequestMessage.RequestUri.AbsoluteUri);
        }


        [Fact]
        public void AddMultipleParametersToLink()
        {
            var link = new Link() { Target = new Uri("http://localhost/api/{dataset}/customer{?foo,bar,baz}") };
            link.AddParameter("foo", "bar");
            link.AddParameter("baz", "99");
            link.AddParameter("dataset", "bob");

            var client = new HttpClient(new FakeMessageHandler());

            var response = client.SendAsync(link.CreateRequest()).Result;

            Assert.Equal("http://localhost/api/bob/customer?foo=bar&baz=99", response.RequestMessage.RequestUri.AbsoluteUri);
        }

    }

    public class FakeMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(new HttpResponseMessage() {RequestMessage =  request});
            return tcs.Task;
        }
    }
}
