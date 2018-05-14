using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Cors;
using System.Net.Http.Formatting;
using WalterAPI.MessageHandlers;

namespace WalterAPI
{
    public static class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            /*config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));


            //enable cors
            EnableCrossSiteRequests(config);


            // Web API routes
            config.MapHttpAttributeRoutes();*/
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.EnableCors();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Message handlers that check the request headers for authentication
            config.MessageHandlers.Add(new APIKeyMessageHandler());
            config.MessageHandlers.Add(new ClientAccessKeyMessageHandler());

        }

        private static void EnableCrossSiteRequests(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*");
            config.EnableCors(cors);
        }
    }
}
