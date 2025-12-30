using System.Collections.Generic;
using AIMLbot; 

namespace AIMLTGBot
{
    public class AIMLService
    {
        private readonly Bot bot;
        private readonly Dictionary<long, User> users = new Dictionary<long, User>();

        public AIMLService()
        {
            bot = new Bot();
            bot.loadSettings();
            bot.isAcceptingUserInput = false;
            bot.loadAIMLFromFiles(); 
            bot.isAcceptingUserInput = true;
        }

        public string Talk(long userId, string userName, string phrase)
        {
            string result = string.Empty;
            User user;

            if (!users.ContainsKey(userId))
            {
                user = new User(userId.ToString(), bot);
                users.Add(userId, user);

                if (!string.IsNullOrEmpty(userName))
                {
                    var r = new Request("Меня зовут " + userName, user, bot);
                    result += bot.Chat(r).Output + System.Environment.NewLine;
                }
            }
            else
            {
                user = users[userId];
            }

            result += bot.Chat(new Request(phrase, user, bot)).Output;
            return result;
        }
    }
}
