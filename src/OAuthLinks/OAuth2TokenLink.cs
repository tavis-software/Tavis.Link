using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Tavis.OAuth
{
    [LinkRelationType("oauth2-token")]
    public class OAuth2TokenLink : Link
    {
        public class RequestBody
        {
            private readonly Dictionary<string, string> _BodyParameters = new Dictionary<string, string>();

            public Uri RedirectUri
            {
                get { return new Uri(_BodyParameters["redirect_uri"]); }
                set
                {
                    _BodyParameters["redirect_uri"] = value.OriginalString;
                }
            }



            public string ClientId
            {
                get { return _BodyParameters["client_id"]; }
                set { _BodyParameters["client_id"] = value; }
            }
            public string ClientSecret
            {
                get { return _BodyParameters["client_secret"]; }
                set { _BodyParameters["client_secret"] = value; }
            }

            public string GrantType
            {
                get { return _BodyParameters["grant_type"]; }
                set { _BodyParameters["grant_type"] = value; }
            }

            public string AuthorizationCode
            {
                get { return _BodyParameters["code"]; }
                set { _BodyParameters["code"] = value; }
            }

            public string Username
            {
                get { return _BodyParameters["username"]; }
                set { _BodyParameters["username"] = value; }
            }

            public string Password
            {
                get { return _BodyParameters["password"]; }
                set { _BodyParameters["password"] = value; }
            }

            public HttpContent AsHttpContent()
            {
                return new FormUrlEncodedContent(_BodyParameters);
            }
        }
        

        public OAuth2TokenLink()
        {
            Method = HttpMethod.Post;
        }

        
        public static Oauth2Token ParseTokenBody(string tokenBody)
        {
            var jobject = JToken.Parse(tokenBody) as JObject;
            var token = new Oauth2Token();
            foreach (var jprop in jobject.Properties())
            {
                switch (jprop.Name)
                {
                    case "token_type":
                        token.TokenType = (string)jprop.Value;
                        break;
                    case "expires_in" :
                        token.ExpiryDate = DateTime.Now + new TimeSpan(0, 0,(int) jprop.Value);
                        break;
                    case "access_token":
                        token.AccessToken = (string) jprop.Value;
                        break;
                    case "refresh_token":
                        token.RefreshToken = (string)jprop.Value;
                        break;
                    case "scope":
                        token.Scope = ((string) jprop.Value).Split(' ');
                        break;
                }
            }

            return token;

        }

        public static object ParseErrorBody(string tokenBody)
        {
            var jobject = JToken.Parse(tokenBody) as JObject;
            var token = new OAuth2Error();
            foreach (var jprop in jobject.Properties())
            {
                switch (jprop.Name)
                {
                    case "error":
                        token.Error = (string)jprop.Value;
                        break;
                    case "error_description":
                        token.ErrorDescription = (string)jprop.Value;
                        break;
                    case "error_uri":
                        token.ErrorUri = (string)jprop.Value;
                        break;
                    
                }
            }

            return token;
        }
    }

    public class OauthTokenRequestBody
    {
        
    }
}