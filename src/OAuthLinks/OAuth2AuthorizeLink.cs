using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Tavis.OAuth
{
    [LinkRelationType("oauth2-authorize")]
    public class OAuth2AuthorizeLink : Link
    {

        public OAuth2AuthorizeLink()
        {
            AddNonTemplatedParametersToQueryString = true;
        }

        public HttpRequestMessage CreateRequest(string clientId, string responseType, Uri redirectUri, string[] scope, string state)
        {
            var values = new Dictionary<string, object>
            {
                {"client_id", clientId},
                {"scope", responseType},
                {"response_type", redirectUri},
                {"redirect_uri", scope},
                {"state", state}
            };

            //var target = new Uri(Target, "{?client_id,scope,response_type,redirect_uri,state}");
            return base.BuildRequestMessage(values);

        }


    }
}
