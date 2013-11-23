using System;

namespace Tavis.OAuth
{
    
    public class Oauth2Token
    {
        public string TokenType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string[] Scope { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}