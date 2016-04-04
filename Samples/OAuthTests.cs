using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavis.OAuth;
using Xunit;

namespace Samples
{
    public class OAuthTests
    {
        [Fact]
        public void CreateOAuthAuthRequest()
        {

            var authLink = new OAuth2AuthorizeLink()
            {
                AuthorizationServer = new Uri("https://login.live.com/oauth20_authorize.srf"),
                ClientId = "...",
                ResponseType = "code",
                RedirectUri = new Uri("https://login.live.com/oauth20_desktop.srf"),
                Scope = new[] { "wl.signin", "wl.basic", "wl.skydrive" },
                State = ""
            };

            var request = authLink.CreateRequest();

        }
    }
}
