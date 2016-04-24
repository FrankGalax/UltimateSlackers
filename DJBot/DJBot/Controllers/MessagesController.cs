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
using SpotifyAPI.Web;
using System.Text.RegularExpressions;

namespace DJBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                //message : connect
                if (message.Text == "connect")
                {
                    String returnMessage = SpotifyAPI.Web.SpotifyConnector.GetSpotifyConnector().Connect();
                    return message.CreateReplyMessage(returnMessage);
                }
                //message : create playlist playlistName
                else if (message.Text.Substring(0, 16) == "create playlist ")
                {
                    String returnMessage = SpotifyAPI.Web.SpotifyConnector.GetSpotifyConnector().CreateNewPlaylist(message.Text.Substring(16));
                    return message.CreateReplyMessage(returnMessage);
                }
                //message : add song songName by artistName to playlistName
                else if (message.Text.Substring(0, 9) == "add song ")
                {
                    String[] substring = Regex.Split(message.Text.Substring(9), " by ");

                    String[] substring2 = Regex.Split(substring[1], " to ");

                    String returnMessage = SpotifyAPI.Web.SpotifyConnector.GetSpotifyConnector().AddSongToPlayList(substring2[1], substring[0], substring2[0]);
                    return message.CreateReplyMessage(returnMessage);
                }
                // invalid request
                return message.CreateReplyMessage($"DJBot has no clue whats going on");
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
    }
}