using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tavis;
using Xunit;

namespace LinkTests
{
    public class DerivedLinkTests
    {


        

        [Fact]
        public void ResolveBingMapLinkUsingCreate()
        {
            var link = new BingMapLink();

            var request = link.CreateRequest(45,-73);

            Assert.Equal("http://www.bing.com/maps/?v=2&cp=45~-73&lvl=10", request.RequestUri.AbsoluteUri);
        }


        [Fact]
        public void Foo()
        {
            var foo = new Foo()
            {
                Bar = "hello world",
                Baz = 10
            };
            
            var uri = new Uri("http://example.org/blah");
            var uri2 = uri.AddToQuery(foo);

            Assert.Equal("http://example.org/blah?Bar=hello+world&Baz=10", uri2.AbsoluteUri);
        }



        [Fact]
        public void ResolveTranslateLinkUsingCreate()
        {
            var link = new TranslationLink();

            var request = link.CreateRequest("English", "French",  "Hello");

            Assert.Equal("http://api.microsofttranslator.com/V2/Http.svc/Translate?text=Hello&to=French&from=English", request.RequestUri.AbsoluteUri);
        }
    }

    public class Foo
    {
        public string Bar { get; set; }
        public int Baz { get; set; }
    }

    public static class UriExtensions
    {
        public static Uri AddToQuery<T>(this Uri requestUri,T dto)
        {
            Type t = typeof (T);
            var properties = t.GetProperties();
            var dictionary = properties.ToDictionary(info => info.Name, 
                                                     info => info.GetValue(dto, null).ToString());
            var formContent = new FormUrlEncodedContent(dictionary);

            var uriBuilder = new UriBuilder(requestUri) {Query = formContent.ReadAsStringAsync().Result};

            return uriBuilder.Uri;
        }
    }
    public class TranslationLink : Link
    {

        public TranslationLink()
        {
            Target = new Uri("http://api.microsofttranslator.com/V2/Http.svc/Translate?text={fromphrase}&to={tolanguage}&from={fromlanguage}");
        }

        public HttpRequestMessage CreateRequest(string fromLanguage, string toLanguage, string fromPhrase)
        {
            var parameters = new Dictionary<string, object>
            {
                {"fromlanguage", fromLanguage},
                {"tolanguage", toLanguage},
                {"fromphrase", fromPhrase}
            };
            var link = ApplyParameters(parameters);

            return link.BuildRequestMessage();
        }
    }




    public class BingMapLink : Link
    {
        public BingMapLink()
        {
            Target = new Uri("http://www.bing.com/maps/?v=2&cp={lat}~{long}&lvl={level}");
        }

        public HttpRequestMessage CreateRequest(double latitude, double longitude, int level = 10)
        {
            var parameters = new Dictionary<string, object>
            {
                {"lat", latitude },
                {"long", longitude},
                {"level", level}
            };
            var link = this.ApplyParameters(parameters);
            return link.BuildRequestMessage();
        }
    }

    public class BingMapLink2 : Link
    {
        private Dictionary<string, object> _Parameters; 
        public BingMapLink2()
        {
            Target = new Uri("http://www.bing.com/maps/?v=2&cp={lat}~{long}&lvl={level}");
            this.HttpRequestBuilder = new InlineRequestBuilder((r) =>
            {
                var link = this.ApplyParameters(_Parameters);
                return link.BuildRequestMessage();
            });
        }

        
        public void SetLocation(double latitude, double longitude, int level = 10)
        {
            _Parameters = new Dictionary<string, object>
            {
                {"lat", latitude },
                {"long", longitude},
                {"level", level}
            };
           
        }
    }

}
