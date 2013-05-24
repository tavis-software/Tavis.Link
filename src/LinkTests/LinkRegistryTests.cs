using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
            var registry = new LinkRegistry();

            Assert.NotNull(registry);
        }


        [Fact]
        public void CreateAboutLink()
        {
            var registry = new LinkRegistry();

            var link = registry.CreateLink<AboutLink>();

            Assert.IsType<AboutLink>(link);
        }

        [Fact]
        public void SpecifyHandlerForAboutLink()
        {
            var foo = false;

            var registry = new LinkRegistry();
            registry.AddHandler<AboutLink>(new ActionResponseHandler((hrm) => foo = true));

            var link = registry.CreateLink<AboutLink>();


            Assert.IsType<ActionResponseHandler>(link.HttpResponseHandler);
        }

        [Fact]
        public Task SpecifyHandlerChainForAboutLink()
        {
            var foo = false;
            var bar = false;
            var registry = new LinkRegistry();
            registry.AddHandler<AboutLink>(new ActionResponseHandler((hrm) => foo = true));
            registry.AddHandler<AboutLink>(new ActionResponseHandler((hrm) => bar = true));

            var link = registry.CreateLink<AboutLink>();
            link.Target = new Uri("http://example.org");
            var httpClient = new HttpClient(new FakeHandler() { Response = new HttpResponseMessage()});

            return httpClient.FollowLinkAsync(link).ContinueWith(t =>
                {
                    Assert.True(foo);
                    Assert.True(bar);
                   
                });

        }

    }
}
