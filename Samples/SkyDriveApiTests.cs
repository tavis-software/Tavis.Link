using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Tavis;
using Tavis.OAuth;
using Xunit;

namespace Samples
{
    public class SkyDriveApiTests
    {


        [Fact]
        public void CreateRequestForAuthCode()
        {
            var authLink = new OAuth2AuthorizeLink()
            {
                AuthorizationServer = new Uri("https://login.live.com/oauth20_authorize.srf"),
                ClientId = "...",
                ResponseType = "code",
                RedirectUri = new Uri("https://login.live.com/oauth20_desktop.srf"),
                Scope = new[] { "wl.signin", "wl.basic", "wl.skydrive_update" }
            };

            var req = authLink.CreateRequest();
            
            // Copy this URI into the browser, sign in, and then copy the auth code from URL in the browser
        }


        [Fact]
        public void GetAccessToken()
        {
            var tokenLink = new OAuth2TokenLink
                {
                    Target = new Uri("https://login.live.com/oauth20_token.srf"),
                    RedirectUri = new Uri("https://login.live.com/oauth20_desktop.srf"),
                    ClientId = "...",
                    ClientSecret = "...",
                    GrantType = "authorization_code",
                    AuthorizationCode = "3eab54b1-86fa-4596-ce5e-91cb4e55bbd3"
                };

            var client = new HttpClient();
            var response = client.SendAsync(tokenLink.CreateRequest()).Result;
            var body = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var token = OAuth2TokenLink.ParseTokenBody(body);
            }
            else
            {
                var error = OAuth2TokenLink.ParseErrorBody(body);
            }
        }

        [Fact]
        public void UploadFileToSkyDrive()
        {

            ServicePointManager.Expect100Continue = false;
            
            var content = new StreamContent(new FileStream("C:\\Users\\Darrel\\Pictures\\868.jpg",FileMode.Open));

            var link = new Link {
                Target = new Uri("https://apis.live.net/v5.0/me/skydrive/files/{filename}{?access_token}"), 
                Method = HttpMethod.Put,
                Content = content
            };

            link.SetParameter("access_token", "EwAwAq1DBAAUGCCXc8wU/zFu9QnLdZXy+YnElFkAAS/+qh/DHgp3W+UIGoihj9+udq6NrLD+s8hQ51NxPcii0QKiV0Vcd/n8w01XHFxpqboQblkhywILJ+CgOdsg0e8UgzPPjt4pmhWoRShFaGrANtUHzzfxmonXTZ8QGPBU9wGmqmfBpEe1KH51q/UiAEGP4t/twPyu3m68Q8ANh0FBlORNmQXAmkIhImuEHbQm+K5Xfi0EoZ2bLdimY1OwCQnUgSlVPj4raodooGOH0ZlkyUrd3VfH0Jwc90HkiOLfxWvspaA/px5NDLwBxb8Xo1B6Y7VMhA6poaPn1jf+y2sfFi0I7ToklBb+n7GTQJ9EiWfxf0mDX2hj1PbvLsYffGkDZgAACKqqGYH2n911AAHEORFJ5DYFe4zF1V4lcljDsWvil0CWOmbOsCsWMHGCs29qFIu/ZJzkAHYZk7Lg5meFsnQVdAnEWO2j7NEJJuI118VuBNasJfkV208nq4K/B5K/QrM/ZxqYG9IjMHnGbpcZSisKT8TkPK4z7yvR6f+ap6Zx9cGLjwS0WajHGCBjyiNPDV7UoP3Rmnr1fzwivdplydgQg9aE4BV9hYn/2nBX3e6c0OfFDFQzinJukJesc/2zgYf4tR9b8L4qmtXDYCp0d7akVxmzdXFb/b71gLZmHFHCrPSv8t4f0NIllKhCBhiFyitOGu3iBRpSy6jPgkaSp2NwBu/WICrgae1wEITrAAA=");
            link.SetParameter("filename","868.jpg");
            var req = link.CreateRequest();

            var client = new HttpClient();

            var response = client.SendAsync(req).Result;

            var body = response.Content.ReadAsStringAsync().Result;

        }

    }
}
