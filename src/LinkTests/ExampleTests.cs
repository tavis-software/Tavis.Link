using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using Newtonsoft.Json;
using Tavis;
using Tavis.IANA;
using Tavis.UriTemplates;
using Xunit;

namespace LinkTests
{
    public class ExampleTests
    {

        // Add a user agent header to every request
        // This is a bogus example, it should be done via HttpClient MessageHandler
        [Fact]
        public void Add_user_Agent_header()
        {
            

            var linkFactory = new LinkFactory();
            linkFactory.SetRequestBuilder<AboutLink>(new InlineRequestBuilder(r =>
            {
                r.Headers.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));
                return r;
            }));

            var aboutlink = linkFactory.CreateLink<AboutLink>();
            aboutlink.Target = new Uri("http://example.org/about");

            var request = aboutlink.CreateRequest();

            Assert.Equal("MyApp/1.0", request.Headers.UserAgent.ToString());
        }


        [Fact]
        // This is bogus too.  Also should be done by HttpClient Message handler
        public void Add_auth_header_aboutlink_request()
        {
            var linkFactory = new LinkFactory();
            linkFactory.SetRequestBuilder<AboutLink>(new InlineRequestBuilder(
                r => { r.Headers.Authorization = new AuthenticationHeaderValue("foo", "bar");
                         return r;
                }));

            var aboutlink = linkFactory.CreateLink<AboutLink>();
            aboutlink.Target = new Uri("http://example.org/about");

            var request = aboutlink.CreateRequest();

            Assert.Equal("foo bar", request.Headers.Authorization.ToString());
        }

        [Fact]
        public void Add_accept_header_to_stylesheet_link()
        {
            var linkFactory = new LinkFactory();

            var builders = new List<DelegatingRequestBuilder>()
            {
                new AcceptHeaderRequestBuilder(new[] {new MediaTypeWithQualityHeaderValue("text/css")}),
                new InlineRequestBuilder(r =>
                {
                    r.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    return r;
                })
            };

            linkFactory.SetRequestBuilder<StylesheetLink>(builders);

            var aboutlink = linkFactory.CreateLink<StylesheetLink>();
            aboutlink.Target = new Uri("http://example.org/about");

            var request = aboutlink.CreateRequest();

            Assert.Equal("text/css", request.Headers.Accept.ToString());
            Assert.Equal("gzip", request.Headers.AcceptEncoding.ToString());
        }
        
        [Fact]
        public void Set_path_parameters()
        {
            var linkFactory = new LinkFactory();
            var aboutlink = linkFactory.CreateLink<RelatedLink>();
            aboutlink.Template = new UriTemplate("http://example.org/customer/{id}");
            aboutlink.Template.AddParameter("id", 45 );

            var request = aboutlink.CreateRequest();

            Assert.Equal("http://example.org/customer/45", request.RequestUri.OriginalString);
        }


        
        [Fact]
        public void Set_templated_query_parameters()
        {
            var linkFactory = new LinkFactory();
            var aboutlink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer{?id}"));


            aboutlink.Template.AddParameter("id", 45);

            var request = aboutlink.CreateRequest();

            Assert.Equal("http://example.org/customer?id=45", request.RequestUri.OriginalString);
        }


        
        [Fact]
        public void Set_query_parameters_without_template()
        {
            var linkFactory = new LinkFactory();
            var aboutlink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer"));

            aboutlink.Template = new UriTemplate(aboutlink.Target.AbsoluteUri + "{?id}");
            aboutlink.Template.SetParameter("id", 45);
            var request = aboutlink.CreateRequest();

            Assert.Equal("http://example.org/customer?id=45", request.RequestUri.OriginalString);
        }

        
        [Fact]
        public void Update_query_parameters()
        {
            var linkFactory = new LinkFactory();
            var relatedLink = linkFactory.CreateLink<RelatedLink>();
            relatedLink.Target = new Uri("http://example.org/customer?id=23");

            relatedLink.Template = new UriTemplate(relatedLink.Target.GetLeftPart(UriPartial.Path) + "{?id}");
            relatedLink.Template.AddParameter("id",45);

            //relatedLink.Template.AddParameters(parameters,true);
            var request = relatedLink.CreateRequest();

            Assert.Equal("http://example.org/customer?id=45", request.RequestUri.OriginalString);
        }
        

        [Fact]
        public void Remove_a_query_parameters()
        {
            var linkFactory = new LinkFactory();
            var relatedLink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer?format=xml&id=23"));
            
            relatedLink.Template = relatedLink.Target.MakeTemplate();
            relatedLink.Template.ClearParameter("format");
            
            var request = relatedLink.CreateRequest();

            Assert.Equal("http://example.org/customer?id=23", request.RequestUri.OriginalString);
        }

        [Fact]
        public void Remove_a_query_parameters2()
        {
            
            var target = new Uri("http://example.org/customer?format=xml&id=23");

            var template = target.MakeTemplate();
            template.ClearParameter("format");

            
            Assert.Equal("http://example.org/customer?id=23", template.Resolve());
        }

        [Fact]
        public void Use_non_get_method()
        {
            var linkFactory = new LinkFactory();
            var relatedLink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer?format=xml&id=23"));

            relatedLink.Method = HttpMethod.Head;
            var request = relatedLink.CreateRequest();

            Assert.Equal(HttpMethod.Head, request.Method);
        }


        [Fact]
        public void Create_request_that_has_a_body()
        {
            var linkFactory = new LinkFactory();
            var relatedLink = linkFactory.CreateLink<RelatedLink>(new Uri("http://example.org/customer?format=xml&id=23"));

            relatedLink.Method = HttpMethod.Post;
            relatedLink.Content = new StringContent("");
            var request = relatedLink.CreateRequest();

            Assert.Equal(HttpMethod.Post, request.Method);
        }


        [Fact]
        public void DeserializeTest()
        
        {
  //          var p = typeof(ConfigurationManager);


//            var z = Activator.CreateInstance(Type.GetType("System.Net.Http.HttpClient, System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            var x = JsonConvert.DeserializeObject("{ '$type': 'System.Net.Http.HttpClient, System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'}", new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            var y = new UTF8Encoding();
            Assert.NotNull(x);
        }
    
    }
}
