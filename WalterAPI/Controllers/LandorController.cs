

using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WalterAPI.Helpers;
using WalterAPI.Models;
using WalterAPI.Services;

namespace WalterAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LandorController : ApiController
    {
        public string agentHostName = "https://api.dialogflow.com";

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] {
                "This is a CORS response.",
                "It works from any origin."
            };
        }

        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            var jsonData = await request.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(jsonData);
            var clientAccessKey = Request.Headers.GetValues("ClientAccessKey").First().ToString();

            var action = (String)data.result.action;
            var sessionId = (String)data.sessionId;
            DialogflowService service = new DialogflowService(clientAccessKey);
            HttpResponseMessage resp = new HttpResponseMessage();

            switch (action)
            {
                case "input.welcome":
                    resp = service.GetGreetingMessage();
                    break;
                case "portfolio":
                    resp = service.GetLandorWorkExample();
                    break;
                case "employees":
                    var fullName = (String)data.result.parameters.fullName;
                    resp = service.GetEmployeTitle(fullName);
                    break;
                case "learn":
                    var any = (String)data.result.parameters.any;
                    var topic = (String)data.result.parameters.topic;
                    var desc = (String)data.result.parameters.description;
                    resp = service.Teach(topic, desc);
                    break;
                case "email":
                    var receiver = (String)data.result.parameters.fullName;
                    var message = (String)data.result.parameters.message;
                    resp = service.SendEmail(receiver, message);
                    break;
                case "randomTopic":
                    resp = service.GetRandomSubject(sessionId);
                    break;
                case "ClearPreviousContexts":
                    //var userSays = (String)data.result.resolvedQuery;
                    resp = service.ClearPreviousContexts(sessionId);
                    break;
                case "randomTopicPeople":
                    resp = service.GetRandomSubjectPeople(sessionId);
                    break;
                case "randomTopicAbout":
                    resp = service.GetRandomSubjectAboutLandor(sessionId);
                    break;
                case "getIntentByEvent":
                    //Make sure that the context value is an event name on an intent
                    var context = (String)data.result.contexts[0].name;

                    //get the context that contain request keyword
                    //that will be the name of the event to be executed
                    if (data.result.contexts.Count > 0)
                    {
                      
                        for (int i = 0; i < data.result.contexts.Count; i++)
                        {
                            context = (String)data.result.contexts[i].name;

                            if (context.Contains("request"))
                            {
                                context = (String)data.result.contexts[i].name;
                                break;
                            }
                           
                        }

                    }

                    resp = service.GetIntentFromContext(sessionId, context);                  
                    break;
                case "location":
                    var city= (String)data.result.parameters.locationCity;
                    var country = (String)data.result.parameters.locationCountry;

                    if (country != null && country != "")
                    {
                        resp = service.GetOfficesByCountry(country);
                    }
                    else if (city != null && city != "")
                    {
                        resp = service.GetOfficesByCity(city);
                    }
                    else
                    {
                        var txt = "City or Country?";
                        var jsn = DialogFlowHelper.CreateJsonObjectResponse(txt);
                        resp = new HttpResponseMessage()
                        {
                            Content = new StringContent(jsn.ToString())
                        };

                    }
                    break;

                default:
                    var text = "Action couldn't be found. Please talk to your developer.";
                    var json = DialogFlowHelper.CreateJsonObjectResponse(text);
                    resp = new HttpResponseMessage()
                    {
                        Content = new StringContent(json.ToString())
                    };

                    break;
            }

            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

     

        // GET api/values/another
        [HttpGet]
        [EnableCors(origins: "http://www.bigfont.ca", headers: "*", methods: "*")]
        public IEnumerable<string> Another()
        {
            return new string[] {
                "This is a CORS response. ",
                "It works only from two origins: ",
                "1. www.bigfont.ca ",
                "2. the same origin."
            };
        }
    }

}