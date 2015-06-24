using System;
using System.Collections.Generic;
using System.Net.Http;
using Tavis.UriTemplates;

namespace Tavis.OAuth
{
    [LinkRelationType("oauth2-authorize")]
    public class OAuth2AuthorizeLink : Link
    {
       
        public string ClientId {get;set;}

        public OAuth2AuthorizeLink()
        {
            Template = new UriTemplate("{authorization_server}{?client_id,scope,response_type,redirect_uri,state}");
        }
        public Uri AuthorizationServer { get; set; }

        public HttpRequestMessage CreateRequest(string clientId, string responseType, Uri redirectUri, string[] scope, string state)
        {
            ClientId = clientId;
            var values = new Dictionary<string, object>
            {
                {"authorization_server", AuthorizationServer.AbsoluteUri},
                {"client_id", clientId},
                {"scope", responseType},
                {"response_type", redirectUri},
                {"redirect_uri", scope},
                {"state", state}
            };
            
            this.Template.ApplyParametersToTemplate(values);
            //var target = new Uri(Target, "{?client_id,scope,response_type,redirect_uri,state}");
            return CreateRequest();

        }


    }
}
