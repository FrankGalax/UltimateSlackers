using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace MemeBot
{
    // This class defines the personality of the bot.
    // The asnwers are written in the IntentAnswers.json file
    public class MemeBot
    {
        private Dictionary<string, string[]> Answers;
        private static Random random = new Random();

        private static int stillBotCount = 0;

        public MemeBot()
        {
            Answers = new Dictionary<string, string[]>();
            
            // Le current directory est caca... mais jai pas envie de débug ca live. Plis hardcode for now ;)
            using (StreamReader r = new StreamReader("C:\\Dev\\UltimateSlackers\\MemeBot\\MemeBot\\IntentAnswers.json"))
            {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);
                foreach (var intent in array)
                {
                    string intentName = intent.name;
                    string[] intentAnswers = intent.answers.ToObject(typeof(string[]));
                    Answers.Add(intentName, intentAnswers);
                }
            }
        }

        public string answerUser(string userIntent)
        {
            string answer;
            string[] answers = Answers[userIntent];
            switch (userIntent)
            {
                case "StillBot":
                    answer = answers[stillBotCount++];
                    break;
                default:
                    stillBotCount = 0;
                    answer = answers[random.Next(answers.Length)];
                    break;
            }
            return answer;
        }
    }
}