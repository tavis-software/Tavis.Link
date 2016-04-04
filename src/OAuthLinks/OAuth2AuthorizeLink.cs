using System;
using System.Collections.Generic;
using System.Net.Http;
using Tavis.RequestBuilders;
using Tavis.UriTemplates;

namespace Tavis.OAuth
{
    [LinkRelationType("oauth2-authorize")]
    public class OAuth2AuthorizeLink : Link
    {

        [LinkParameter(name: "client_id")]
        public string ClientId { get; set; }

        [LinkParameter(name: "response_type")]
        public string ResponseType { get; set; }

        [LinkParameter(name: "redirect_uri")]
        public Uri RedirectUri { get; set; }

        [LinkParameter(name: "state")]
        public string State { get; set; }

        [LinkParameter(name: "authorization_server")]
        public Uri AuthorizationServer { get; set; }

        public string[] Scope { get; set; }

        public OAuth2AuthorizeLink()
        {
            Template = new UriTemplate("{authorization_server}{?client_id,scope,response_type,redirect_uri,state}");
        }



    }
}
