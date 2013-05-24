using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tavis
{
    public static class LinkExtensions
    {
        /// <summary>
        /// Add handler to end of chain of handlers
        /// </summary>
        /// <param name="link"></param>
        /// <param name="responseHandler"></param>
        public static void AddHandler(this Link link, DelegatingResponseHandler responseHandler)
        {
            
           
            if (link.HttpResponseHandler == null)
            {
                link.HttpResponseHandler = responseHandler;
            }
            else
            {
                var currentHandler = link.HttpResponseHandler as DelegatingResponseHandler;
                if (currentHandler == null) throw new Exception("Cannot add handler unless existing handler is a delegating handler");
            
                while (currentHandler != null)
                {
                    if (currentHandler.InnerResponseHandler == null)
                    {
                        currentHandler.InnerResponseHandler = responseHandler;
                        currentHandler = null;
                    }
                    else
                    {
                        currentHandler = currentHandler.InnerResponseHandler;
                    } 
                }

            }
        }
    }
}
