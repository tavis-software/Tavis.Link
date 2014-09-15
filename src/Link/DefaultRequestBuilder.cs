using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Tavis
{
    public class DefaultRequestBuilder : DelegatingRequestBuilder
    {
        protected override HttpRequestMessage ApplyChanges(HttpRequestMessage request)
        {
            return request;
        }
    }
}