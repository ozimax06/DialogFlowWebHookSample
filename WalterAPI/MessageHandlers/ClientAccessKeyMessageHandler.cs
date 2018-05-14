using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WalterAPI.Helpers;

namespace WalterAPI.MessageHandlers
{
    public class ClientAccessKeyMessageHandler : DelegatingHandler
    {

        //Message Handler that validates DialogFlow Client Access Key
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            //determine if it is a valid key or not
            bool validKey = false;
            IEnumerable<string> requestHeaders;
            var checkAPIKeyExists = httpRequestMessage.Headers.TryGetValues("ClientAccessKey", out requestHeaders);

            if (checkAPIKeyExists)
            {
                    if (ConfigurationManager.AppSettings[requestHeaders.FirstOrDefault().ToString()] != null)
                    {
                        validKey = true;
                    }
            }


            if (!validKey)
            {
                return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Invalid Client Access Key");
            }

            var response = await base.SendAsync(httpRequestMessage, cancellationToken);

            return response;
        }
    }
}