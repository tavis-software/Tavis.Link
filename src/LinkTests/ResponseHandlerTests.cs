using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tavis;
using Xunit;

namespace LinkTests
{
    public class ResponseHandlerTests 
    {
        [Fact]
        public Task Test()
        {
            var link = new Link(){Target = new Uri("http://localhost")};

            link.HttpResponseHandler = new NotFoundHandler(new OkHandler(null));

            var client = new HttpClient(new FakeHandler() {Response = new HttpResponseMessage() {StatusCode = HttpStatusCode.NotFound}});

            return client.FollowLinkAsync(link);
        }
    }
}
