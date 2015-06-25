using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tavis;
using Tavis.IANA;
using Xunit;

namespace LinkTests
{
    public class LinkRegistryTests
    {

        [Fact]
        public void CreateRegistry()
        {
            var registry = new LinkFactory();

            Assert.NotNull(registry);
        }


        [Fact]
        public void CreateAboutLink()
        {
            var registry = new LinkFactory();

            var link = registry.CreateLink<AboutLink>();

            Assert.IsType<AboutLink>(link);
        }


       


   
       

  

        [Fact]
        public Task SpecifyHandlerChainForAboutLink()
        {
            var foo = false;
            var bar = false;
            var baz = false;
  
            var registry = new LinkFactory();
            var grh = new InlineResponseHandler((rel,hrm) => baz = true,
                new InlineResponseHandler((rel, hrm) => foo = true,
                    new InlineResponseHandler((rel, hrm) => bar = true)));

            var machine = new HttpResponseMachine();
            machine.AddResponseHandler(System.Net.HttpStatusCode.OK, grh);
            

            var link = registry.CreateLink<AboutLink>();
            link.Target = new Uri("http://example.org");
            var httpClient = new HttpClient(new FakeHandler() { Response = new HttpResponseMessage()});

            return httpClient.FollowLinkAsync(link,machine).ContinueWith(t =>
                {
                    Assert.True(foo);
                    Assert.True(bar);
                    Assert.True(baz);
                });

        }

    }
}
