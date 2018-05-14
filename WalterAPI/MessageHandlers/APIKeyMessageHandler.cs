using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;

namespace WalterAPI.MessageHandlers
{
 
    public class APIKeyMessageHandler : DelegatingHandler
    {

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            //determine if it is a valid key or not
            bool validKey = false;
            IEnumerable<string> requestHeaders;
            var checkAPIKeyExists = httpRequestMessage.Headers.TryGetValues("APIKey", out requestHeaders);

            if (checkAPIKeyExists)
            {
                if (requestHeaders.FirstOrDefault().Equals(ConfigurationManager.AppSettings["APIKey"].ToString()))
                {
                    validKey = true;
                }
            }

            if (!validKey)
            {
                return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Invalid API Key");
            }

            var response = await base.SendAsync(httpRequestMessage, cancellationToken);

            return response;
        }
    }
}