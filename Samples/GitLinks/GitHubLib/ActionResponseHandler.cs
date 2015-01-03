using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tavis;

namespace GitHubLib
{
    //public class InlineResponseHandler : DelegatingResponseHandler, IResponseHandler
    //{
    //    private readonly Func<string,HttpResponseMessage,Task> _action;

    //    public InlineResponseHandler(Func<string,HttpResponseMessage,Task> action)
    //    {
    //        _action = action;
    //    }

    //    public override async Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage responseMessage)
    //    {
    //        await _action(linkRelation, responseMessage);

    //        var response = await base.HandleResponseAsync(linkRelation, responseMessage);

    //        return response;
    //    }

    //}
}