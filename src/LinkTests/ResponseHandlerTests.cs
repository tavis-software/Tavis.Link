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
        public async Task Test()
        {
            var link = new Link(){Target = new Uri("http://localhost")};

            var notFoundHandler = new NotFoundHandler(new OkHandler(null));
            link.AddResponseHandler(notFoundHandler);

            var client = new HttpClient(new FakeHandler() {Response = new HttpResponseMessage() {StatusCode = HttpStatusCode.NotFound}});

            await client.FollowLinkAsync(link);

            Assert.True(notFoundHandler.NotFound);
        }

        [Fact]
        public async Task FollowAndApplyInDistinctSteps()
        {
            var link = new Link() {Target = new Uri("http://example.org/")};

            var client = new HttpClient(new FakeHandler() { Response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound
            } });
            
            var clientState = new ClientApplicationState();

            await client.FollowLinkAsync(link)
                        .ApplyRepresentationToAsync(clientState);

            Assert.Equal(HttpStatusCode.NotFound, clientState.StatusCode);
        }
    }
    public class ClientApplicationState : IResponseHandler
    {
        public HttpStatusCode StatusCode;
        public Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage responseMessage)
        {
            StatusCode = responseMessage.StatusCode;
            return Task.FromResult<HttpResponseMessage>(responseMessage);
        }
    }
}
