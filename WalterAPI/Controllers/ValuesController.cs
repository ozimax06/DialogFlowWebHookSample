using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WalterAPI.Models;
using ApiAiSDK;
using ApiAiSDK.Model;

namespace WalterAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {

        public HttpResponseMessage Get()
        {
     


        string topic = "computer";
            string desc = "it is a calculator";

            var intent = new Intent();

            intent.name = "what is a computer";
            intent.auto = false;

            intent.responses = new[]
            {
                new { resetContexts = false, affectedContexts = new String[1], speech = "Computer is a calculator"}

            };

            intent.userSays = new[]
            {
                 new {
                     data = new[] {  new { text = "What is computer?" } },
                     isTemplate = false, count = 0
                 },
                 new {
                     data = new[] {  new { text = "Could you tell me what a computer is?" } },
                     isTemplate = false, count = 0
                 },
                 new {
                     data = new[] {  new { text = "What does computer mean?" } },
                     isTemplate = false, count = 0
                 }
            };

            var jsonData = JsonConvert.SerializeObject(intent);



            var baseAddress = "https://api.api.ai/v1/intents?v=20150910";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Headers[HttpRequestHeader.Authorization] = "Bearer c85abe0d1cec4aa891c2bb49257e4589";
            http.Method = "POST";

            string parsedContent = jsonData;
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();



            return new HttpResponseMessage()
            {
                Content = new StringContent("GET: Test message")
            };
        }

        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            var jsonData = await request.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(jsonData);

            var text = data.result.action;

            var url = "";


            var resp = new HttpResponseMessage()
            {


                Content = new StringContent
                (

                    "{ \"speech\":\"" + text + "\", \"data\":{  \"speech\":\"my speech\", \"source\":\"my source\" , \"text\":\"textt\", \"displayText\":\"" + text + "\",  \"messages\": [{\"type\":\"" + url + "\", \"speech\":\"" + text + "\", \"formattedText\":\"" + text + "\", \"image\":{\"url\":\"" + url + "\"}}]}}"
                )
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }

        public HttpResponseMessage Put()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("PUT: Test message")
            };
        }

    }

    /*class Result
    {
        string speech;
        string source;
        string displayText;
        Message[] messages;
    }
    class Message
    {
        string type;
        string speech;
    }*/
}