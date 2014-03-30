using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using Tavis;
using Tavis.IANA;
using Xunit;

namespace LinkTests
{
    public class ExampleTests
    {

        // Add a user agent header to every request
        [Fact]
        public void Add_user_Agent_header()
        {
            var linkFactory = new LinkFactory();
            linkFactory.SetRequestBuilder<AboutLink>(new ActionRequestBuilder(r => r.Headers.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"))));

            var aboutlink = linkFactory.CreateLink<AboutLink>();
            aboutlink.Target = new Uri("http://example.org/about");

            var request = aboutlink.BuildRequestMessage();

            Assert.Equal("MyApp/1.0", request.Headers.UserAgent.ToString());
        }


        [Fact]
        public void Add_auth_header_aboutlink_request()
        {
            var linkFactory = new LinkFactory();
            linkFactory.SetRequestBuilder<AboutLink>(new ActionRequestBuilder(
                r => { r.Headers.Authorization = new AuthenticationHeaderValue("foo", "bar"); }));

            var aboutlink = linkFactory.CreateLink<AboutLink>();
            aboutlink.Target = new Uri("http://example.org/about");

            var request = aboutlink.BuildRequestMessage();

            Assert.Equal("foo bar", request.Headers.Authorization.ToString());
        }

        [Fact]
        public void Add_accept_header_to_stylesheet_link()
        {
            var linkFactory = new LinkFactory();

            var builders = new List<DelegatingRequestBuilder>()
            {
                new AcceptHeaderRequestBuilder(new[] {new MediaTypeWithQualityHeaderValue("text/css")}),
                new ActionRequestBuilder(r => r.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")))
            };

            linkFactory.SetRequestBuilder<StylesheetLink>(builders);

            var aboutlink = linkFactory.CreateLink<StylesheetLink>();
            aboutlink.Target = new Uri("http://example.org/about");

            var request = aboutlink.BuildRequestMessage();

            Assert.Equal("text/css", request.Headers.Accept.ToString());
            Assert.Equal("gzip", request.Headers.AcceptEncoding.ToString());
        }
        
        [Fact]
        public void Set_path_parameters()
        {
            var linkFactory = new LinkFactory();
            var aboutlink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer/{id}"));


            var request = aboutlink.BuildRequestMessage(new Dictionary<string,object> {{"id",45}});

            Assert.Equal("http://example.org/customer/45", request.RequestUri.OriginalString);
        }


        
        [Fact]
        public void Set_templated_query_parameters()
        {
            var linkFactory = new LinkFactory();
            var aboutlink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer{?id}"));

            var request = aboutlink.BuildRequestMessage(new Dictionary<string, object> { { "id", 45 } });

            Assert.Equal("http://example.org/customer?id=45", request.RequestUri.OriginalString);
        }


        
        [Fact]
        public void Set_query_parameters_without_template()
        {
            var linkFactory = new LinkFactory();
            var aboutlink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer"));
            aboutlink.AddNonTemplatedParametersToQueryString = true;

            var request = aboutlink.BuildRequestMessage(new Dictionary<string, object> { { "id", 45 } });

            Assert.Equal("http://example.org/customer?id=45", request.RequestUri.OriginalString);
        }

        
        [Fact]
        public void Update_query_parameters()
        {
            var linkFactory = new LinkFactory();
            var relatedLink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer?id=23"));
            relatedLink.AddNonTemplatedParametersToQueryString = true;

            var parameters = relatedLink.GetQueryStringParameters();
            parameters["id"] = 45;
            var request = relatedLink.BuildRequestMessage(parameters);

            Assert.Equal("http://example.org/customer?id=45", request.RequestUri.OriginalString);
        }
        

        [Fact]
        public void Remove_a_query_parameters()
        {
            var linkFactory = new LinkFactory();
            var relatedLink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer?format=xml&id=23"));
            relatedLink.AddNonTemplatedParametersToQueryString = true;

            var parameters = relatedLink.GetQueryStringParameters();
            parameters.Remove("format");

            var request = relatedLink.BuildRequestMessage(parameters);

            Assert.Equal("http://example.org/customer?id=23", request.RequestUri.OriginalString);
        }


        [Fact]
        public void Use_non_get_method()
        {
            var linkFactory = new LinkFactory();
            var relatedLink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer?format=xml&id=23"));
            
            
            var request = relatedLink.BuildRequestMessage(HttpMethod.Head);

            Assert.Equal(HttpMethod.Head, request.Method);
        }


        [Fact]
        public void Create_request_that_has_a_body()
        {
            var linkFactory = new LinkFactory();
            var relatedLink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer?format=xml&id=23"));


            var request = relatedLink.BuildRequestMessage(HttpMethod.Post, new StringContent(""));

            Assert.Equal(HttpMethod.Post, request.Method);
        }
        


    
    }
}
