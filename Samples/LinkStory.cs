using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tavis;
using Tavis.Http;
using Xunit;

namespace LinkTests
{
    public class LinkStory
    {

        [Fact]
        public async Task Make_a_request()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api.github.com/")
            };

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("test","1.0"));
            
            var response = await httpClient.SendAsync(request);

            Assert.Equal(200, (int)response.StatusCode);

        }

        [Fact]
        public async Task AbstractRequestAndResponse()
        {
            var request = CreateRequest();

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ApiClient", "1.0"));

            var response = await httpClient.SendAsync(request);

            HandleResponse(response);

        }

        [Fact]
        public async Task TestFollow()
        {
            await Follow(CreateRequest, HandleResponse);
        }

        [Fact]
        public async Task TestFollowLink()
        {
            var homeLink = new HomeLink()
            {
                Target = new Uri("https://api.github.com/")
            };
            
            await Follow(homeLink);
        }

        //[Fact]
        //public  Task CompareApproaches()
        //{
        //    //// Wrapper Service 
        //    //var customerService = new CustomerService();
        //    //var customer = customerService.GetCustomer(22);
        //    //application.Process(customer);

        //    //// Hypermedia Centric
        //    //var customerLink = linkFactory.Create<CustomerLink>();
        //    //customerLink.Id = 22;
        //    //application.FollowLink(customerLink);



        //}


        private static async Task Follow(IRequestFactory link)
        {
            var request = link.CreateRequest();

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("test", "1.0"));

            var response = await httpClient.SendAsync(request);
            var responseHandler = link as IResponseHandler;
            if (responseHandler != null)
            {
                await responseHandler.HandleResponseAsync(link, response);
            }
        }

        private static async Task Follow(Func<HttpRequestMessage> requestFactory, Action<HttpResponseMessage> responseHandler)
        {
            var request = requestFactory();

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("test", "1.0"));

            var response = await httpClient.SendAsync(request);

            responseHandler(response);
        }

        private static void HandleResponse(HttpResponseMessage response)
        {
            Assert.Equal(200, (int) response.StatusCode);
        }

        private static HttpRequestMessage CreateRequest()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api.github.com/")
            };
            return request;
        }
    }

    public class HomeLink : IRequestFactory, IResponseHandler
    {
        public Uri Target { get; set; }

        public HttpRequestMessage CreateRequest()
        {
            return new HttpRequestMessage() {RequestUri = Target};    
        }

        public Task<HttpResponseMessage> HandleResponseAsync(IRequestFactory link, HttpResponseMessage responseMessage)
        {
            Assert.Equal(200, (int)responseMessage.StatusCode);
            return Task.FromResult(responseMessage);
        }

        public string LinkRelation
        {
            get { return "home"; }
        }
    }
    
}
