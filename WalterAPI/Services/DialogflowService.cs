using ApiAiSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using WalterAPI.Helpers;
using WalterAPI.Models;
using ApiAiSDK.Model;

namespace WalterAPI.Services
{
    public class DialogflowService
    {
        public string agentHostName;
        private string developerAccessToken;
        private string clientAccessToken;
        public DialogflowService(string clientToken)
        {
            agentHostName = "https://api.dialogflow.com";
            this.clientAccessToken = clientToken;
            this.developerAccessToken = DialogFlowHelper.GetDeveloperTokenByClientAccessToken(this.clientAccessToken);
        }


        public HttpResponseMessage GetRandomSubjectPeople(string sessionId)
        {
            var text = "";
            var context = "";

            Random rnd = new Random();
            int topicIndex = rnd.Next(1, 3);

            switch (topicIndex)
            {
                case 1:
                    text = "Do you want to know who our CEO is?";
                    context = "request-ceo";
                    break;
                case 2:
                    text = "Would you like to talk about our CCO?";
                    context = "request-cco";
                    break;
                case 3:
                    text = "How about talking about our CSO?";
                    context = "request-cso";
                    break;
            }
            var json = DialogFlowHelper.CreateJsonObjectResponse(text);
            DialogFlowHelper.AddContextOut(json, context, 1);

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;

        }

        internal HttpResponseMessage GetGreetingMessage()
        {
            var text = "Hello. I’m K-base, Landor’s chatbot and I’m here to help. How can I help you?";

            var json = DialogFlowHelper.CreateJsonObjectResponse(text);

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;
        }

        public HttpResponseMessage GetRandomSubjectAboutLandor(string sessionId)
        {
            var text = "";
            var context = "";

            Random rnd = new Random();
            int topicIndex = rnd.Next(1, 5);

            switch (topicIndex)
            {
                case 1:
                    text = "Do you want to know the way how we approach to branding?";
                    context = "request-approach";
                    break;
                case 2:
                    text = "Would you be interested in learning about the boat?";
                    context = "request-boat";
                    break;
                case 3:
                    text = "How about talking about what we are famous for?";
                    context = "request-famous";
                    break;
                case 4:
                    text = "Would you like to see some of the work we have done for our clients?";
                    context = "request-portfolio";
                    break;
                case 5:
                    text = "How about talking about our awards?";
                    context = "request-award";
                    break;
            }


            var json = DialogFlowHelper.CreateJsonObjectResponse(text);
            DialogFlowHelper.AddContextOut(json, context, 2);

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;
        }

        public HttpResponseMessage ClearPreviousContexts(string sessionId)
        {
            DialogFlowHelper.ClearContexts(sessionId, developerAccessToken);

            /*var config = new AIConfiguration(this.clientAccessToken, SupportedLanguage.English);
            var apiAi = new ApiAi(config);
            var response = apiAi.TextRequest(userSays);

            var intentName = response.Result.Metadata.IntentName.ToString();

            var responseText = response.Result.Fulfillment.Speech.ToString();

            var json = DialogFlowHelper.CreateJsonObjectResponse(responseText);

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;*/

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent("OK")
            };

            return resp;
        }

    }

        public HttpResponseMessage GetIntentFromContext(String sessionId, String contxt)
        {
            DialogFlowHelper.ClearContexts(sessionId, developerAccessToken);
            var eventName = contxt;


            var baseAddress = agentHostName + "/api/query?v=20150910&e=" + contxt + "&lang=en" + "&sessionId=" + sessionId;
            var jsonResponse = "";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Headers[HttpRequestHeader.Authorization] = developerAccessToken;
            http.Method = "GET";

            var response = (HttpWebResponse)http.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                jsonResponse = sr.ReadToEnd();

            }


            dynamic data = JObject.Parse(jsonResponse);

            var text = (String)data.result.fulfillment.speech;
            var json = DialogFlowHelper.CreateJsonObjectResponse(text);

            //load contexts from the result intent if any
            //excluding any context that contains fallback
            if (data.result.contexts.Count > 0)
            {
                for (int i = 0; i < data.result.contexts.Count; i++)
                {
                    var context = (String)data.result.contexts[i].name;

                    if (!context.Contains("fallback"))
                    {
                        var lifespan = (int)data.result.contexts[i].lifespan;
                        DialogFlowHelper.AddContextOut(json, context, lifespan);
                    }

                }

            }


            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;

        }

        public HttpResponseMessage SendEmail(string fullName, string message)
        {
            LandornetSQLEntities db = new LandornetSQLEntities();

            var emp = db.Employees.Where(x => x.FullNameCalc.Contains(fullName)).FirstOrDefault();
            var text = "I dont't know how the person is";

            if (emp != null)
            {
                var recipients = new List<String>();
                recipients.Add(emp.Email);
                var subject = "Message from Walter";
                var body = message;
               // Landor.Utility.Email.Emailv2 em = new Landor.Utility.Email.Emailv2("noreply@landor.com", "Walter Landor", recipients, body, subject, null, null);
                //em.SendMessage();
                text = "message sent";
            }

            var json = DialogFlowHelper.CreateJsonObjectResponse(text);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;
        }


        public HttpResponseMessage Teach(string topic, string definition)
        {

            var intent = new Intent();

            intent.name = topic + "-definition-intent";
            intent.auto = true;

            intent.responses = new[]
            {
                new { resetContexts = false, affectedContexts = new List<String>(), speech = definition}

            };

            intent.userSays = new[]
            {
                new {
                     data = new[] {  new { text = "Explain me " + topic +"?" } },
                     isTemplate = false, count = 0
                 },
                 new {
                     data = new[] {  new { text = "What is " + topic +"?" } },
                     isTemplate = false, count = 0
                 },
                 new {
                     data = new[] {  new { text = "Could you tell me what a "+topic+" is?" } },
                     isTemplate = false, count = 0
                 },
                 new {
                     data = new[] {  new { text = "What does " +topic+ " mean?" } },
                     isTemplate = false, count = 0
                 }
                 ,
                 new {
                     data = new[] {  new { text = "Tell me about " +topic } },
                     isTemplate = false, count = 0
                 }
                 ,
                 new {
                     data = new[] {  new { text = "Can you explain me what " +topic+ " is?" } },
                     isTemplate = false, count = 0
                 }
            };

            var jsonData = JsonConvert.SerializeObject(intent);

            var baseAddress = agentHostName + "/v1/intents?v=20150910";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Headers[HttpRequestHeader.Authorization] = developerAccessToken;
            http.Method = "POST";

            string parsedContent = jsonData;
            System.Text.ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();
            var text = "Got it. I will memorize it in a few seconds!";

            var json = DialogFlowHelper.CreateJsonObjectResponse(text);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;

        }

        public HttpResponseMessage GetOfficesByCity(string city)
        {
            LandornetSQLEntities db = new LandornetSQLEntities();
            var text = "We don't have a studio in " + city;
            if (db.Offices.Where(o => o.City.Contains(city) && o.ActiveFlag == true).ToList().Count > 0)
            {
                text = "Yes, we have a studio in " + city + " .";

            }

            var json = DialogFlowHelper.CreateJsonObjectResponse(text);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };


            return resp;
        }

        public HttpResponseMessage GetOfficesByCountry(string country)
        {
            LandornetSQLEntities db = new LandornetSQLEntities();
            var text = "We don't have a studio in " + country;
            if (db.Offices.Where(o => (country.Contains(o.Country)) && o.ActiveFlag == true).ToList().Count > 0)
            {

                var offices = db.Offices.Where(o => (country.Contains(o.Country)) && o.ActiveFlag == true).Select(c => c.City).ToList().Distinct().ToList();

                if (offices.Count > 1)//more than one offices in the given country/region
                {
                    text = "Sure, we are in ";
                    for (int i = 0; i < offices.Count; i++)
                    {
                        if (i != offices.Count - 1)
                        {
                            text += offices[i] + ", ";
                        }
                        else
                        {
                            text += offices[i] + ".";
                        }
                    }
                }
                else
                {
                    text = "Sure, we have a studio in " + offices[0];
                }

            }

            var json = DialogFlowHelper.CreateJsonObjectResponse(text);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;
        }

        public HttpResponseMessage GetRandomSubject(String sessionId)
        {
            var text = "";
            var context = "";

            Random rnd = new Random();
            int topicIndex = rnd.Next(1, 4);

            switch (topicIndex)
            {
                case 1:
                    text = "How about talking about our locations?";
                    context = "request-location";
                    break;
                case 2:
                    text = "How about talking about our people?";
                    context = "request-people";
                    break;
                case 3:
                    text = "How about talking about Landor?";
                    context = "request-about";
                    break;
                case 4:
                    text = "How about talking about our parent company?";
                    context = "request-parent";
                    break;
            }

            var con = new Models.Context();
            con.lifespan = 1;
            con.name = context;



            var json = DialogFlowHelper.CreateJsonObjectResponse(text);
            DialogFlowHelper.AddContextOut(json, context, 2);

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;
        }

        public HttpResponseMessage GetEmployeTitle(string fullName)
        {
            LandornetSQLEntities db = new LandornetSQLEntities();

            var emp = db.Employees.Where(x => x.FullNameCalc.Contains(fullName)).FirstOrDefault();
            var text = "I dont't know";

            if (emp != null)
            {
                text = fullName + " is our " + emp.Title;
            }

            var json = DialogFlowHelper.CreateJsonObjectResponse(text);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;
        }

        public HttpResponseMessage GetLandorWorkExample()
        {
            Random rnd = new Random();
            int result = rnd.Next(1, 10);

            var url = "";
            var text = "";

            switch (result)
            {
                case 1:
                    url = "https://landor-prod.imgix.net/app/uploads/2017/07/25105822/M_G_Stationery_0021.jpg";
                    text = "This is for M&G";
                    break;
                case 2:
                    url = "https://landor-prod.imgix.net/app/uploads/2017/01/18110245/AO_Melbourne.jpg";
                    text = "Our work for Australian Open";
                    break;
                case 3:
                    url = "https://landor-prod.imgix.net/app/uploads/2015/08/27135714/Etihad-1.jpg";
                    text = "Ethiad!";
                    break;
                case 4:
                    url = "https://landor-prod.imgix.net/app/uploads/2015/09/01224051/Jameson-bottle-concepts2.jpg";
                    text = "For Jameson";
                    break;
                case 5:
                    url = "https://landor-prod.imgix.net/app/uploads/2015/09/01220214/P90159734_highRes.jpg";
                    text = "This is for BMW";
                    break;
                case 6:
                    url = "https://landor-prod.imgix.net/app/uploads/2015/09/02171955/Ad-template-2.jpg";
                    text = "This is for  UEM Edgenta";
                    break;
                case 7:
                    url = "https://landor-prod.imgix.net/app/uploads/2015/09/02165906/Oscar_003-e1453772474270.jpg";
                    text = "Butcher Thick Cut Bacon";
                    break;
                case 8:
                    url = "https://landor-prod.imgix.net/app/uploads/2015/08/27135714/Intu-2.jpg";
                    text = "We made this for Intu";
                    break;
                case 9:
                    url = "https://landor-prod.imgix.net/app/uploads/2016/03/29132038/AP_Deodorant_HR_2-ext.jpg";
                    text = "Our work for Old Spice";
                    break;
                case 10:
                    url = "https://landor-prod.imgix.net/app/uploads/2015/08/27135714/Less-1.jpg";
                    text = "We made this for Less";
                    break;

            }

            var json = DialogFlowHelper.CreateJsonObjectResponse(text);
            DialogFlowHelper.AttachImage(json, url);

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json.ToString())
            };

            return resp;
        }

    }
}