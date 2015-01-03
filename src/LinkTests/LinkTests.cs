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
using Tavis.UriTemplates;
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

            Assert.Equal(HttpMethod.Get,request.Method);
        }


        [Fact]
        public void SettingAnAcceptHeaderShouldBePassedToTheRequest()
        {
            var link = new Link
            {
                Target = new Uri("Http://localhost"),
                
            };
            link.AddRequestBuilder(new InlineRequestBuilder((r) =>
            {
                r.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hal"));
                r.Headers.UserAgent.Add(new ProductInfoHeaderValue("foo", "1.1"));
                return r;
            }));


            var request = link.CreateRequest();

            Assert.True(request.Headers.Accept.Any(h => h.MediaType == "application/vnd.hal"));
        }


        [Fact]
        public void SettingContentShouldBePassedToTheRequest()
        {
            var link = new Link
            {
                Target = new Uri("Http://localhost"),
                Method = HttpMethod.Post,
                Content = new StringContent("Hello World")
            };

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
        public async Task FollowLink()
        {
            var link = new Link { Target = new Uri("Http://localhost") };
            var client = new HttpClient(new FakeMessageHandler());

            var uri = string.Empty;
            link.AddResponseHandler(new InlineResponseHandler((rel,r) => uri = r.RequestMessage.RequestUri.AbsoluteUri));
            await client.FollowLinkAsync(link);

            

            Assert.Equal("http://localhost/", uri);
        }


        [Fact]
        public void AddParameterToLink()
        {
            var link = new Link(){ Template = new UriTemplate("http://localhost/{?foo}")};
        
            var client = new HttpClient(new FakeMessageHandler());

            link.Template.AddParameter("foo", "bar");
            

            var response = client.SendAsync(link.CreateRequest()).Result;

            Assert.Equal("http://localhost/?foo=bar", response.RequestMessage.RequestUri.AbsoluteUri);
        }


        [Fact]
        public void AddMultipleParametersToLink()
        {
            var link = new Link() { Template = new UriTemplate("http://localhost/api/{dataset}/customer{?foo,bar,baz}") };

            link.Template.ApplyParametersToTemplate(new Dictionary<string, object>
            {
                {"foo", "bar"},
                {"baz", "99"},
                {"dataset", "bob"}
            });

            var request = link.CreateRequest();
            
            Assert.Equal("http://localhost/api/bob/customer?foo=bar&baz=99", request.RequestUri.AbsoluteUri);
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
            var link = new Link() { Template = new UriTemplate("http://localhost/api/{dataset}/customer{?foo,bar,baz}") };

            var parameters = link.Template.GetParameterNames();

            Assert.Equal(4, parameters.Count());
            Assert.True(parameters.Contains("dataset"));
            Assert.True(parameters.Contains("foo"));
            Assert.True(parameters.Contains("bar"));
            Assert.True(parameters.Contains("baz"));
        }




        //[Fact]
        //public void AddParameterToLinkMultipleTimes()
        //{
        //    var link = new Link() { Target = new Uri("http://localhost/{?foo}") };
            

        //    var request = link.CreateRequest(new Dictionary<string, object>
        //    {
        //        {"foo", "bar"},
        //        {"foo", "blah"}
        //    });

        //    Assert.Equal("http://localhost/?foo=blah", request.RequestUri.AbsoluteUri);
        //}

        [Fact]
        public void UnsetParameterInLink()
        {
            var link = new Link() { Template = new UriTemplate("http://localhost/{?foo}") };

            var request = link.CreateRequest();

            Assert.Equal("http://localhost/", request.RequestUri.AbsoluteUri);
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
                Type = "application/foo"
            };
            link.HrefLang.Add(new CultureInfo("en-GB"));
            link.HrefLang.Add(new CultureInfo("en-CA"));
            Assert.Equal("<http://localhost/{?foo}>;rel=\"related\";hreflang=en-GB;hreflang=en-CA;type=\"application/foo\"", link.AsLinkHeader());
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
