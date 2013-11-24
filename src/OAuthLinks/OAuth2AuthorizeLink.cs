using System;
using System.Net.Http;

namespace Tavis.OAuth
{
    [LinkRelationType("oauth2-authorize")]
    public class OAuth2AuthorizeLink : Link
    {
        public Uri AuthorizationServer { get; set; }
        
        public string ClientId { get; set; }
        public string ResponseType { get; set; }
        public Uri RedirectUri { get; set; }
        public string[] Scope { get; set; }
        public string State { get; set; }

        public override HttpRequestMessage CreateRequest()
        {
            Target = new Uri(AuthorizationServer, "{?client_id,scope,response_type,redirect_uri,state}");

            SetParameter("client_id", ClientId);
            SetParameter("scope", string.Join(" ",Scope));
            SetParameter("response_type", ResponseType);
            SetParameter("redirect_uri", RedirectUri.OriginalString);
            SetParameter("state", State);

            var request = base.CreateRequest();
            request.Method = HttpMethod.Get;

            return request;
        }

    }
}
