using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tavis;
using Tavis.Http;
using Tavis.RequestBuilders;
using Tavis.UriTemplates;
using Xunit;

namespace LinkTests
{
    public class DerivedLinkTests
    {




        [Fact]
        public void ResolveBingMapLinkUsingCreate()
        {
            var link = new BingMapLink()
            {
                Lat = 45,
                Long = -73,
                Level = 10
            };

            var request = link.CreateRequest();

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
            var uri2 = uri.MakeTemplate(Tavis.UriExtensions.ObjectToDictionary(foo));

            Assert.Equal("http://example.org/blah?Bar=hello%20world&Baz=10", uri2.Resolve());
        }



        [Fact]
        public void ResolveTranslateLinkUsingCreate()
        {
            var link = new TranslationLink()
            {
                FromLanguage = "English",
                ToLanguage = "French",
                FromPhrase = "Hello"
            };

            var request = link.CreateRequest();

            Assert.Equal("http://api.microsofttranslator.com/V2/Http.svc/Translate?text=Hello&to=French&from=English", request.RequestUri.AbsoluteUri);
        }
        [Fact]
        public void ResolveTranslateLinkUsingCreate2()
        {
            var link = new TranslationLink2()
            {
                FromLanguage = "English",
                ToLanguage = "French",
                FromPhrase = "Hello"
            };

            var request = link.CreateRequest();

            Assert.Equal("http://api.microsofttranslator.com/V2/Http.svc/Translate?text=Hello&to=French&from=English", request.RequestUri.AbsoluteUri);
        }
    }

    public class Foo
    {
        public string Bar { get; set; }
        public int Baz { get; set; }
    }

    public class TranslationLink : Link
    {
        [LinkParameter("from")]
        public string FromLanguage { get; set; }
        [LinkParameter("to")]
        public string ToLanguage { get; set; }
        [LinkParameter("text")]
        public string FromPhrase { get; set; }

        public TranslationLink()
        {
            Template = new UriTemplate("http://api.microsofttranslator.com/V2/Http.svc/Translate{?text,to,from}");
        }

        
    }

    // Does not derive from Link
    public class TranslationLink2 : IRequestFactory
    {
        private UriTemplate _Template;
        
        public TranslationLink2()
        {
            _Template = new UriTemplate("http://api.microsofttranslator.com/V2/Http.svc/Translate?text={fromphrase}&to={tolanguage}&from={fromlanguage}");

        }

        public string FromLanguage { get; set; }
        public string ToLanguage { get; set; }
        public string FromPhrase { get; set; }

        public string LinkRelation { get { return "TransalationLink"; } }

        public HttpRequestMessage CreateRequest()
        {

            return  new HttpRequestMessage()
            {
                RequestUri = new Uri( _Template
                    .AddParameter("fromlanguage",FromLanguage)
                    .AddParameter("tolanguage",ToLanguage)
                    .AddParameter("fromphrase",FromPhrase)
                    .Resolve())
            };
        }
    }



    public class BingMapLink : Link
    {
        public double Lat { get; set; }
        public double Long { get; set; }
        public int Level { get; set; }

        public BingMapLink()
        {
            Template = new UriTemplate("http://www.bing.com/maps/?v=2&cp={lat}~{long}&lvl={level}");
        }

        //public HttpRequestMessage CreateRequest(double latitude, double longitude, int level = 10)
        //{
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"lat", latitude },
        //        {"long", longitude},
        //        {"level", level}
        //    };
        //    var link = this.ApplyParameters(parameters);
        //    return link.CreateRequest();
        //}
    }

    public class BingMapLink2 : Link
    {
        private Dictionary<string, object> _Parameters; 
        public BingMapLink2()
        {
            Target = new Uri("http://www.bing.com/maps/?v=2&cp={lat}~{long}&lvl={level}");
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
