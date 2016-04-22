using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MemeBot
{
    // This class is the Language Understanding Intelligent Service (LUIS)
    // It calls the microsoft service to understand the intent of the user
    public class Luis
    {
        public static async Task<MemeLuis> ParseUserInput(string input)
        {
            using (var client = new HttpClient())
            {
                string url = "https://api.projectoxford.ai/luis/v1/application?id=c19fb6c3-11f6-401f-a877-c7218c8dd3de&subscription-key=6f0f725317364f06986d03912d0b9e29&q=" + input;
                HttpResponseMessage msg = await client.GetAsync(url);

                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var _Data = JsonConvert.DeserializeObject<MemeLuis>(jsonResponse);
                    return _Data;
                }

                throw new ArgumentException("The query was not successful.");
            }
        }
    }

    public class MemeLuis
    {
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public object[] entities { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

}