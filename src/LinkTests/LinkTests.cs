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
        link.SetParameter("foo","bar");
            
            var client = new HttpClient(new FakeMessageHandler());

            var response = client.SendAsync(link.CreateRequest()).Result;

            Assert.Equal("http://localhost/?foo=bar", response.RequestMessage.RequestUri.AbsoluteUri);
        }


        [Fact]
        public void AddMultipleParametersToLink()
        {
            var link = new Link() { Target = new Uri("http://localhost/api/{dataset}/customer{?foo,bar,baz}") };
            link.SetParameter("foo", "bar");
            link.SetParameter("baz", "99");
            link.SetParameter("dataset", "bob");

            var uri = link.GetResolvedTarget();
            
            Assert.Equal("http://localhost/api/bob/customer?foo=bar&baz=99", uri.AbsoluteUri);
        }


        [Fact]
        public void IdentifyParametersInTemplate()
        {
            var link = new Link() { Target = new Uri("http://localhost/api/{dataset}/customer{?foo,bar,baz}") };

            var parameters = link.GetParameterNames();

            Assert.Equal(4, parameters.Count());
            Assert.True(parameters.Contains("dataset"));
            Assert.True(parameters.Contains("foo"));
            Assert.True(parameters.Contains("bar"));
            Assert.True(parameters.Contains("baz"));
        }




        [Fact]
        public void AddParameterToLinkMultipleTimes()
        {
            var link = new Link() { Target = new Uri("http://localhost/{?foo}") };
            link.SetParameter("foo", "bar");
            link.SetParameter("foo", "blah");

            var request = link.CreateRequest();

            Assert.Equal("http://localhost/?foo=blah", request.RequestUri.AbsoluteUri);
        }

        [Fact]
        public void UnsetParameterInLink()
        {
            var link = new Link() { Target = new Uri("http://localhost/{?foo}") };
            link.SetParameter("foo", "bar");
            link.UnsetParameter("foo");

            var request = link.CreateRequest();

            Assert.Equal("http://localhost/", request.RequestUri.AbsoluteUri);
        }

        [Fact]
        public void SetListParameterInLink()
        {
            var link = new Link() { Target = new Uri("http://localhost/{?foo}") };
            link.SetParameter("foo", new List<string>(){ "bar","baz","bler"});

            var request = link.CreateRequest();

            Assert.Equal("http://localhost/?foo=bar,baz,bler", request.RequestUri.AbsoluteUri);
        }


        [Fact]
        public void ExecutePublicWebUrl()
        {
            var link = new BingMapLink();
            link.SetCoordinates(45,-73);

            var request = link.CreateRequest();

            Assert.Equal("http://www.bing.com/maps/?v=2&cp=45~-73&lvl=10", request.RequestUri.AbsoluteUri);
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


    public class BingMapLink : Link {

        public BingMapLink()
        {
            Target = new Uri("http://www.bing.com/maps/?v=2&cp={lat}~{long}&lvl={level}");
            SetParameter("level", 10);
        }

        public void SetCoordinates(double latitude, double longitude)
        {
            SetParameter("lat", latitude.ToString());
            SetParameter("long", longitude.ToString());
        }
       
        public void SetLevel(int level)
        {
            SetParameter("level", level.ToString());
        }
    }
}
