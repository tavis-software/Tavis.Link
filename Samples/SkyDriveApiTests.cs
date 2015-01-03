using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Tavis;
using Tavis.OAuth;
using Tavis.UriTemplates;
using Xunit;

namespace Samples
{
    public class SkyDriveApiTests
    {


        //[Fact]
        public void CreateRequestForAuthCode()
        {
            var authLink = new OAuth2AuthorizeLink()
            {
                Target = new Uri("https://login.live.com/oauth20_authorize.srf")
            };

            var req = authLink.CreateRequest(
                clientId: "...",
                responseType: "code",
                redirectUri: new Uri("https://login.live.com/oauth20_desktop.srf"),
                scope : new[] { "wl.signin", "wl.basic", "wl.skydrive_update" },
                state : null);
            
            // Copy this URI into the browser, sign in, and then copy the auth code from URL in the browser
        }


        //[Fact]
        public void GetAccessToken()
        {
            var tokenLink = new OAuth2TokenLink()
            {
                Target = new Uri("https://login.live.com/oauth20_token.srf")
            };

            var requestbody = new OAuth2TokenLink.RequestBody()
                {
                    RedirectUri = new Uri("https://login.live.com/oauth20_desktop.srf"),
                    ClientId = "...",
                    ClientSecret = "...",
                    GrantType = "authorization_code",
                    AuthorizationCode = "3eab54b1-86fa-4596-ce5e-91cb4e55bbd3"
                };
            tokenLink.Content = requestbody.AsHttpContent();

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

        //[Fact]
        public void UploadFileToSkyDrive()
        {

            ServicePointManager.Expect100Continue = false;
            
            var content = new StreamContent(new FileStream("C:\\Users\\Darrel\\Pictures\\868.jpg",FileMode.Open));

            var link = new Link {
                Template = new UriTemplate("https://apis.live.net/v5.0/me/skydrive/files/{filename}{?access_token}"), 
            };

            link.Template.AddParameters(new 
            {
                access_token = "EwAwAq1DBAAUGCCXc8wU/zFu9QnLdZXy+YnElFkAAS/+qh/DHgp3W+UIGoihj9+udq6NrLD+s8hQ51NxPcii0QKiV0Vcd/n8w01XHFxpqboQblkhywILJ+CgOdsg0e8UgzPPjt4pmhWoRShFaGrANtUHzzfxmonXTZ8QGPBU9wGmqmfBpEe1KH51q/UiAEGP4t/twPyu3m68Q8ANh0FBlORNmQXAmkIhImuEHbQm+K5Xfi0EoZ2bLdimY1OwCQnUgSlVPj4raodooGOH0ZlkyUrd3VfH0Jwc90HkiOLfxWvspaA/px5NDLwBxb8Xo1B6Y7VMhA6poaPn1jf+y2sfFi0I7ToklBb+n7GTQJ9EiWfxf0mDX2hj1PbvLsYffGkDZgAACKqqGYH2n911AAHEORFJ5DYFe4zF1V4lcljDsWvil0CWOmbOsCsWMHGCs29qFIu/ZJzkAHYZk7Lg5meFsnQVdAnEWO2j7NEJJuI118VuBNasJfkV208nq4K/B5K/QrM/ZxqYG9IjMHnGbpcZSisKT8TkPK4z7yvR6f+ap6Zx9cGLjwS0WajHGCBjyiNPDV7UoP3Rmnr1fzwivdplydgQg9aE4BV9hYn/2nBX3e6c0OfFDFQzinJukJesc/2zgYf4tR9b8L4qmtXDYCp0d7akVxmzdXFb/b71gLZmHFHCrPSv8t4f0NIllKhCBhiFyitOGu3iBRpSy6jPgkaSp2NwBu/WICrgae1wEITrAAA=",
                filename = "868.jpg"}
            );

            link.Method = HttpMethod.Put;
            link.Content = content;

            var req = link.CreateRequest();            

            var client = new HttpClient();

            var response = client.SendAsync(req).Result;

            var body = response.Content.ReadAsStringAsync().Result;

        }

        [Fact]
        public void DownloadFolderContentsFromSkyDrive()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "EwA4Aq1DBAAUGCCXc8wU/zFu9QnLdZXy+YnElFkAAYhf1bgSt9B/dWMmZfEF2rBqHwg9q3CclBY0t6eEbXR0CoQG/JmhGOHkwkz2FXYGPF2gQYBkSjBrZpiMzVZV3xcW3HAjLOJ0psokGXmZO7Bf+PizaolmtfJtu/ZuomcLCkGE/6W/r3CqKpn5NBvX1T96/Ri3M/dV6OTUgcSQ/lRGxxCrLFcTO78tQTFG2x2hldoeJErDhndbpUmleo+7pGMN7eZfHS5ht1Y5Mn6Z8W+r950UsIpjgiqW8UITDNB6XCUZJFCZQT++/wN+Q0Tso4HbdsgSMTfcVc0tZsjr4dmSLLiMykh+nzECQKRoZqqu7oRStTJEXMTAE9RQpgSD6xUDZgAACFOVfhYLm9uHCAFSZLM344beauG+0sCtOQSkIX7qSUaB6y3v+PJPNARXzBTCwqdwKD9faRnHEOmZ5YCvn0Rg9+ex2fd/PGfQbm7I+K/INqTS8Tu8BA+UDwWymjnIvZB3tkbEX4yBEYPQXkNCpae9j3uZtRm2NfLWacq4x4/wxGqVVy+rCxNwo7rkWwVGyPTSTu41mqa9O2MqB33SEMtoDb2LE+GV3hGVbqr0pKGtVeJJ+bzqtW5i8e2pFiKN6RXj4wTaijjz8N6wwiB913WzTbwNNOvP2XcnY1TC6Cz3p9Fh7+AHOGR9R50A5FjjWcROa/wmXjzZrMVMxbw5db0X0KutgPOdj9QNBoMerjHEfYV3i+EAAA==");
            // var response = client.GetAsync(new Uri("https://apis.live.net/v5.0/me/skydrive")).Result;
            // var response = client.GetAsync(new Uri("https://apis.live.net/v5.0/folder.e73792dc59d3ef31/files")).Result;
            var response = client.GetAsync(new Uri("https://ctzxcq.dm2301.livefilestore.com/y2mgKMJ0KyFGfBIno866zDLpVJZxEJrzDS0kpcgpCeUkr0ygRZuHqGwUdemJbkhCp1efMGY5303eAYFhJZtRZeDzuBmMrUZWAJF25GmfbyN5us/test.txt?psid=1")).Result;

            var body = response.Content.ReadAsStringAsync().Result;

        }
    }
}
