using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// This application is used to demonstrate consuming the Telegram API
// In this case the arguments are acquired by means of environment variables

namespace SendTelegram_ev.Pages
{
    public class IndexModel : PageModel
    {
        private string yourBotApiKey = Environment.GetEnvironmentVariable("SENDTELEGRAM_BOTAPIKEY");
        private string yourChatId = Environment.GetEnvironmentVariable("SENDTELEGRAM_CHATID");

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        /// <summary>
        /// SendTelegramAsync is pillaged from Postman using its code snippet ability
        /// </summary>
        /// <param name="theMessage">Obviously the message to send to the Telegram channel</param>
        /// <returns></returns>
        private async Task SendTelegramAsync(string theMessage)
        {
            Console.WriteLine(">>>" + "[" + yourBotApiKey + "|" + yourChatId + "]" + theMessage);
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.telegram.org/bot" + yourBotApiKey + "/sendMessage");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("chat_id", yourChatId));
            collection.Add(new("text", theMessage));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public void OnPost(string action = "err", string myText = "err")
        {
            if (action == "sendtelegram")
            {
                SendTelegramAsync(myText);
            }
        }
    }
}