using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Tavis;
using Tavis.IANA;
using Xunit;

namespace LinkTests
{
    public class HintTests
    {

        [Fact]
        public void GivenAnAllowHintWithAMethodAddingItAgainDoesNotFail()
        {
            // Arrange
            var hint = new AllowHint();
            hint.AddMethod(HttpMethod.Get);
            // Act
            hint.AddMethod(HttpMethod.Get);

            //Asset
            // No error occured
            
        }

        [Fact]
        public void UseAllowHintToSetRequestMethodOnLink()
        {
            // Arrange
            var hint = new AllowHint();
            hint.AddMethod(HttpMethod.Post);
            hint.ConfigureRequest = (h, r) =>
                {
                    // Really not sure if this is a good idea to allow hints to actually change the request.
                    // Should they just validate the current request?
                    // How do we know if someone has explicitly set link.method or if it is just a default Get.
                    var allowHint = ((AllowHint)h);
                    if ( !allowHint.Methods.Contains(r.Method))
                    {
                        r.Method = allowHint.Methods.First();
                    }
                    return r;
                };

            var link = new Link();
            link.AddHint(hint);
            
            // Act
            var request = link.BuildRequestMessage();

            //Asset
            Assert.Equal(HttpMethod.Post,request.Method);            

        }


        [Fact]
        public void CreateAHintByName()
        {
            var hf = new HintFactory();
            var hint = hf.CreateHint("formats");

            Assert.IsType<FormatsHint>(hint);
        }


        [Fact]
        public void SpecifyHandlerForAllowhint()
        {
            var foo = false;

            var registry = new HintFactory();
            registry.SetHandler<AllowHint>((h, r) =>
                { foo = true;
                    return r;
                });

            var hint = registry.CreateHint<AllowHint>();

            var link = new Link();
            link.AddHint(hint);

            // Act
            var request = link.BuildRequestMessage();

            Assert.Equal(foo,true);
        }


        [Fact]
        public void CreateFormatHint()
        {
            var registry = new HintFactory();

            var hint = registry.CreateHint<FormatsHint>();
            hint.AddMediaType("application/json");

            var obj = (JObject) hint.Content;
            Assert.NotNull(obj.Property("application/json"));   
        }

        [Fact]
        public void CreateDeprecatedFormatHint()
        {
            var registry = new HintFactory();

            var hint = registry.CreateHint<FormatsHint>();
            hint.AddMediaType("application/json",true);

            var obj = (JObject)hint.Content;
            Assert.NotNull(obj.Property("application/json"));
            var thing = obj.Property("application/json").Value as JObject;
            Assert.Equal(true,(bool)thing.Property("deprecated").Value);
        }

        [Fact]
        public void CreatePreferHint()
        {
            var registry = new HintFactory();

            var hint = registry.CreateHint<AcceptPreferHint>();
            hint.AddPreference("handling");

            var arr = (JArray)hint.Content;
            Assert.Equal("handling", arr[0]);
        }
    
    }

    
}

