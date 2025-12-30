using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AIMLTGBot
{
    public class TelegramService : IDisposable
    {
        private readonly TelegramBotClient client;
        private readonly AIMLService aiml;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public string Username { get; }

        public TelegramService(string token, AIMLService aimlService)
        {
            aiml = aimlService;

            client = new TelegramBotClient(token, cancellationToken: cts.Token);

            try
            {
                var meTask = client.GetMe();
                meTask.Wait();
                Username = meTask.Result.Username ?? "AIML_TG_BOT";
            }
            catch
            {
                Username = "AIML_TG_BOT";
            }

            client.OnMessage += OnMessage;
            client.OnError += OnError;
        }

        private async Task OnMessage(Message msg, UpdateType type)
        {
            if (msg.Text == null)
                return;

            var chatId = msg.Chat.Id;
            var username = msg.Chat.FirstName ?? string.Empty;
            var messageText = msg.Text;

            Console.WriteLine(
                $"Received '{messageText}' message in chat {chatId} from {username}.");

            var reply = aiml.Talk(chatId, username, messageText);

            await client.SendMessage(
                chatId: msg.Chat,
                text: reply,
                cancellationToken: cts.Token);
        }

        private Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine($"Telegram error ({source}): {exception}");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            client.OnMessage -= OnMessage;
            client.OnError -= OnError;

            cts.Cancel();
            cts.Dispose();
        }
    }
}
