using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Tavis;
using Tavis.IANA;
using Xunit;

namespace LinkTests
{
    public class LinkHeaderTests
    {

        [Fact]
        public void CreateALinkHeader()
        {
            var request = new HttpRequestMessage();
            var about = new AboutLink() { Target = new Uri("http://example.org/about")};

            request.Headers.AddLinkHeader(about);
            

            var header = request.Headers.GetValues("Link").First().ToString();
            Assert.Equal("<http://example.org/about>;rel=\"about\"",header);
        }

        [Fact]
        public void CreateTwoLinkHeaders()
        {
            var request = new HttpRequestMessage();
            var about = new AboutLink() { Target = new Uri("http://example.org/about") };
            var help = new HelpLink() { Target = new Uri("http://example.org/help") };

            request.Headers.AddLinkHeader(about);
            request.Headers.AddLinkHeader(help);

            var headers = request.Headers.GetValues("Link");

            Assert.Equal("<http://example.org/about>;rel=\"about\"", headers.First().ToString());
            Assert.Equal("<http://example.org/help>;rel=\"help\"", headers.Last().ToString());
        }

        [Fact]
        public void ParseLinkHeaders()
        {
            var linkRegistry = new LinkFactory();
            
            var response = new HttpResponseMessage();
            response.RequestMessage = new HttpRequestMessage() { RequestUri = new Uri("http://example.org/") };
            response.Headers.AddLinkHeader(new AboutLink() { Target = new Uri("http://example.org/about") });
            response.Headers.AddLinkHeader(new HelpLink() { Target = new Uri("http://example.org/help") });

            var links = response.ParseLinkHeaders(linkRegistry);

            Assert.Equal(2,links.Count);
            Assert.NotNull(links.Where(l => l is AboutLink).FirstOrDefault());
            Assert.NotNull(links.Where(l => l is HelpLink).FirstOrDefault());
        }

        [Fact]
        public void ParseLinkHeaders2()
        {
            var linkRegistry = new LinkFactory();
            var response = new HttpResponseMessage();
            response.RequestMessage = new HttpRequestMessage() { RequestUri = new Uri("http://example.org/") };
            response.Headers.TryAddWithoutValidation("Link", "<http://example.org/about>;rel=\"about\", " 
                                + "<http://example.org/help>;rel=\"help\"");
            var links = response.ParseLinkHeaders(linkRegistry);

            Assert.NotNull(links.Where(l => l is AboutLink).FirstOrDefault());
            Assert.NotNull(links.Where(l => l is HelpLink).FirstOrDefault());
            

        }

        [Fact]
        public void ParseLinkHeaders3()
        {
            var linkRegistry = new LinkFactory();
            var response = new HttpResponseMessage();
            response.RequestMessage = new HttpRequestMessage() { RequestUri = new Uri("http://example.org/") };
            var list = new List<Link>() { 
                new AboutLink() { Target = new Uri("http://example.org/about")},
                new HelpLink() { Target = new Uri("http://example.org/help") }
            };
            response.Headers.AddLinkHeaders(list);

            var links = response.ParseLinkHeaders(linkRegistry);

            Assert.NotNull(links.Where(l => l is AboutLink).FirstOrDefault());
            Assert.NotNull(links.Where(l => l is HelpLink).FirstOrDefault());


        }
    }
}
