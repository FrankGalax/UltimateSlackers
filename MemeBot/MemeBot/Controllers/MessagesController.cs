using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MemeBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private Dictionary<string, int> TemplateIds;
        private MemeBot memeBot;

        public MessagesController()
        {
            TemplateIds = new Dictionary<string, int>();
            TemplateIds.Add("successkid", 61544);
            memeBot = new MemeBot();
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                if (message.Text.StartsWith("meme "))
                {
                    return message.CreateReplyMessage(GetMemeLink(message.Text));
                }
                else
                {
                    // Get user intent with LUIS
                    MemeLuis mLuis = await Luis.ParseUserInput(message.Text);
                    // Get a response from our bot
                    string answer = memeBot.answerUser(mLuis.intents[0].intent);
                    // Post the response
                    return message.CreateReplyMessage(answer);
                }
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }

        private String GetMemeLink(string text)
        {
            text = text.Substring(5);
            int firstSpaceIndex = text.IndexOf(" ");
            string name = text.Substring(0, firstSpaceIndex);
            text = text.Substring(firstSpaceIndex + 1);
            string[] texts = text.Split(',');
            string text0 = texts[0];
            string text1 = texts.Length > 1 ? texts[1] : string.Empty;
            int generatorID = TemplateIds.ContainsKey(name) ? TemplateIds[name] : -1;
            if (generatorID == -1)
                return string.Format("meme named {0} not found", name);

            string URI = "https://api.imgflip.com/caption_image";
            string parameters = string.Format("template_id={0}&text0={1}&text1={2}&username={3}&password={4}",
                generatorID,
                text0,
                text1,
                "ultimateslackers",
                "polybotsarefun");
            
            string result = "something went wrong";

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                result = wc.UploadString(URI, parameters);

                try
                {
                    dynamic jsonResult = JsonConvert.DeserializeObject(result);
                    if (jsonResult.success == true)
                        result = jsonResult.data.url;
                    else
                        result = jsonResult.error_message;
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }

            return result;
        }
    }
}