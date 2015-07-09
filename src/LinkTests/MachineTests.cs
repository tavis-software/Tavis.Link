using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Tavis;
using Xunit;

namespace LinkTests
{
    public class MachineTests
    {

        [Fact]
        public async Task Handle404()
        {
            bool notfound = false;
            var machine = new HttpResponseMachine();

            machine.AddResponseHandler(async (l, r) => { notfound = true; return r;  }, HttpStatusCode.NotFound);

            await machine.HandleResponseAsync("", new HttpResponseMessage(HttpStatusCode.NotFound));

            Assert.True(notfound);
        }

        [Fact]
        public async Task HandleUnknown4XX()
        {
            bool badrequest = false;
            var machine = new HttpResponseMachine();

            machine.AddResponseHandler(async (l, r) => { badrequest = true; return r;}, HttpStatusCode.BadRequest);

            await machine.HandleResponseAsync("", new HttpResponseMessage(HttpStatusCode.ExpectationFailed));

            Assert.True(badrequest);
        }

        [Fact]
        public async Task Handle200Only()
        {
            bool ok = false;
            var machine = new HttpResponseMachine();

            machine.AddResponseHandler(async (l, r) => { ok = true; return r;}, System.Net.HttpStatusCode.OK);

            await machine.HandleResponseAsync("", new HttpResponseMessage(HttpStatusCode.OK));

            Assert.True(ok);
        }

        [Fact]
        public async Task Handle200AndContentType()
        {
            // Arrange
            JToken root = null;
            var machine = new HttpResponseMachine();

            machine.AddResponseHandler(async (l, r) =>
            {
                var text = await r.Content.ReadAsStringAsync();
                root = JToken.Parse(text);
                return r;
            },HttpStatusCode.OK,null, new MediaTypeHeaderValue("application/json"));

            machine.AddResponseHandler(async (l, r) => { return r;}, HttpStatusCode.OK, null,new MediaTypeHeaderValue("application/xml"));

            var byteArrayContent = new ByteArrayContent(Encoding.UTF8.GetBytes("{\"hello\" : \"world\"}"));
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            // Act
            await machine.HandleResponseAsync("", new HttpResponseMessage(HttpStatusCode.OK) { Content = byteArrayContent});

            // Assert
            Assert.NotNull(root);
        }


        [Fact]
        public async Task Handle200AndContentTypeUsingTypedMachine()
        {
            // Arrange
            var model = new Model<JToken>
            {
                Value = null
            };

            var machine = new HttpResponseMachine<Model<JToken>>(model);

            machine.AddResponseHandler(async (m, l, r) =>
            {
                var text = await r.Content.ReadAsStringAsync();
                m.Value = JToken.Parse(text);
                return r;
            }, HttpStatusCode.OK, null,new MediaTypeHeaderValue("application/json"));


            machine.AddResponseHandler(async (m, l, r) => { return r; }, HttpStatusCode.OK, null,new MediaTypeHeaderValue("application/xml"));

            var byteArrayContent = new ByteArrayContent(Encoding.UTF8.GetBytes("{\"hello\" : \"world\"}"));
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Act
            await machine.HandleResponseAsync("", new HttpResponseMessage(HttpStatusCode.OK) { Content = byteArrayContent });

            // Assert
            Assert.NotNull(model.Value);
        }


        [Fact]
        public async Task Handle200AndContentTypeAndLinkRelation()
        {
            // Arrange
            JToken root = null;
            var machine = new HttpResponseMachine();

            // Fallback handler
            machine.AddResponseHandler(async (l, r) =>
            {
                var text = await r.Content.ReadAsStringAsync();
                root = JToken.Parse(text);
                return r;
            }, HttpStatusCode.OK); 

            // More specific handler
            machine.AddResponseHandler(async (l, r) =>
            {
                var text = await r.Content.ReadAsStringAsync();
                root = JToken.Parse(text);
                return r;
            }, HttpStatusCode.OK,linkRelation: "foolink", contentType: new MediaTypeHeaderValue("application/json"), profile: null);

            machine.AddResponseHandler(async (l, r) => { return r; }, HttpStatusCode.OK, linkRelation: "foolink", contentType: new MediaTypeHeaderValue("application/xml"), profile: null);

            var byteArrayContent = new ByteArrayContent(Encoding.UTF8.GetBytes("{\"hello\" : \"world\"}"));
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // A
            await machine.HandleResponseAsync("foolink", new HttpResponseMessage(HttpStatusCode.OK) { Content = byteArrayContent });

            // Assert
            Assert.NotNull(root);
        }



    }
}
