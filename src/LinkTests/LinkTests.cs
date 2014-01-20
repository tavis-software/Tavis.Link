using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
        public void FollowLink()
        {
            var link = new Link { Target = new Uri("Http://localhost") };
            var client = new HttpClient(new FakeMessageHandler());

            var uri = string.Empty;
            link.HttpResponseHandler = new ActionResponseHandler(r => uri = r.RequestMessage.RequestUri.AbsoluteUri);
            var task = client.FollowLinkAsync(link);

            task.Wait();

            Assert.Equal("http://localhost/", uri);
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


        // How do we deal with links that are not URI templates?
        // If we have parameters that are set that are not in the URI template we have
        // two choices.  Ignore it, or add it as a query param.
        // How should URI template parameters interact with LinkParameters?
        // Should link parameters be dependent on the Target?

        // Proposal 1:  Link parameters are independent of target.  We expose a DiscoverParameter method that will create parameters
        // from the URI template.
        // 
        // Proposal 2:  We discover parameters in the Target and update link parameters if they exist.  What happens if we have templated path
        // parameter, but want to add query parameters.
        // 

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
        public void CreateLinkHeader()
        {
            var link = new Link() { Target = new Uri("http://localhost/{?foo}") };
            Assert.Equal("<http://localhost/{?foo}>;rel=\"related\"", link.AsLinkHeader());
        }

        [Fact]
        public void CreateLinkHeaderWithRelation()
        {
            var link = new Link() { Target = new Uri("http://localhost/{?foo}"),
                Relation = "related",
                Title = "foo"
            };
            Assert.Equal("<http://localhost/{?foo}>;rel=\"related\";title=\"foo\"", link.AsLinkHeader());
        }

        [Fact]
        public void CreateLinkHeaderWithMediaTypeAndLanguages()
        {
            var link = new Link()
            {
                Target = new Uri("http://localhost/{?foo}"),
                Type = new MediaTypeHeaderValue("application/foo")
            };
            link.HrefLang.Add(new CultureInfo("en-GB"));
            link.HrefLang.Add(new CultureInfo("en-CA"));
            Assert.Equal("<http://localhost/{?foo}>;rel=\"related\";hreflang=en-GB;hreflang=en-CA;type=\"application/foo\"", link.AsLinkHeader());
        }


        [Fact]
        public void ExecutePublicWebUrl()
        {
            var link = new BingMapLink();
            link.SetCoordinates(45,-73);

            var request = link.CreateRequest();

            Assert.Equal("http://www.bing.com/maps/?v=2&cp=45~-73&lvl=10", request.RequestUri.AbsoluteUri);
        }

        [Fact]
        public void CreateJson()
        {
            var jtoken = JToken.Parse("\"hello\" ") as JValue;
            Assert.Equal(jtoken.Value,"hello");

        }
        [Fact]
        public void CreateJson2()
        {
            var jtoken = JToken.Parse("{\"hello\": 99 } ") as JObject;
            Assert.Equal(jtoken["hello"], 99);

        }


        [Fact]
        public void SOQuestion18302092()
        {
            var link = new Link();
            link.Target = new Uri("http://www.myBase.com/get{?a,b}");

            link.SetParameter("a","1");
            link.SetParameter("b", "c");

            var request = link.CreateRequest();
            Assert.Equal("http://www.myBase.com/get?a=1&b=c", request.RequestUri.OriginalString);
            
            
        }

        //[Fact]
        //public void TestTranslateLink()
        //{
        //    var link = new TranslationLink();
        //    link.SetParameters("en","fr","Credit Limit");

        //    var request = link.CreateRequest();

        //    var httpClient = new HttpClient();
        //    var response = httpClient.SendAsync(request).Result;
        //    var responseText = response.Content.ReadAsStringAsync().Result;
        //    Assert.Equal("http://www.bing.com/maps/?v=2&cp=45~-73&lvl=10", responseText);
        //}


        //[Fact]
        //public void ODataLink()
        //{
        //    var link = new Link();
        //    link.Target = new Uri("http://localhost/api/products{?$filter,");

        //    var request = link.CreateRequest();

        //    Assert.Equal("http://www.bing.com/maps/?v=2&cp=45~-73&lvl=10", request.RequestUri.AbsoluteUri);
        //}

    }

    public class TranslationLink : Link {

        public TranslationLink() {
            Target = new Uri("http://api.microsofttranslator.com/V2/Http.svc/Translate?text={fromphrase}&tl={tolanguage}&q={fromphrase}");
        }

        public void SetParameters(string fromLanguage, string toLanguage, string fromPhrase) {
            SetParameter("fromlanguage",fromLanguage);
            SetParameter("tolanguage", toLanguage);
            SetParameter("fromphrase", fromPhrase);
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
