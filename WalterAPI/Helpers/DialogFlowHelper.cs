using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Web;

namespace WalterAPI.Helpers
{
    public class DialogFlowHelper
    {

        public DialogFlowHelper()
        { }


        public static void AddContextOut(JsonObject jsonObject, String context, int lifespan)
        {
            JsonObject contextOut = new JsonObject();
            contextOut.Add("lifespan", lifespan);
            contextOut.Add("name", context);
            contextOut.Add("parameter", new JsonObject());

            if (!jsonObject.ContainsKey("contextOut"))
            {
                JsonArray contextOuts = new JsonArray();
                contextOuts.Add(contextOut);
                jsonObject.Add("contextOut", contextOuts);

            }
            else
            {
                var contexts = (JsonArray)jsonObject["contextOut"];
                contexts.Add(contextOut);
                jsonObject["contextOut"] = contexts;

            }
        }

        public static string GetDeveloperTokenByClientAccessToken(string clientAcessToken)
        {
            return ConfigurationManager.AppSettings[clientAcessToken].ToString();
        }

        public static void ClearContexts(String sessionId, string developerAccessToken)
        {
            var agentHostName = "https://api.dialogflow.com";
            var baseAddress = agentHostName + "/v1/contexts?sessionId=" + sessionId;
            var http = (System.Net.HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Headers[HttpRequestHeader.Authorization] = developerAccessToken;
            http.Method = "DELETE";


            var response = (HttpWebResponse)http.GetResponse();
            var jsonResponse = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                jsonResponse = sr.ReadToEnd();

            }


        }

        public static void AttachImage(JsonObject jsonObject, String imageURL)
        {
            var messages = (JsonObject)jsonObject["data"]["messages"][0];

            JsonObject image = new JsonObject();
            image.Add("url", imageURL);
            messages.Add("image", image);

            jsonObject["data"]["messages"][0] = messages;

        }

        public static JsonObject CreateJsonObjectResponse(string responseMessage)
        {
            JsonArray messages = new JsonArray();
            JsonObject message = new JsonObject();

            message.Add("type", responseMessage);
            message.Add("speech", responseMessage);
            message.Add("formattedText", responseMessage);

            messages.Add(message);

            JsonObject data = new JsonObject();
            data.Add("messages", messages);
            data.Add("displayText", responseMessage);
            data.Add("text", responseMessage);
            data.Add("source", responseMessage);
            data.Add("speech", responseMessage);

            JsonObject json = new JsonObject();

            json.Add("data", data);
            json.Add("speech", responseMessage);

            return json;
        }

    }
}