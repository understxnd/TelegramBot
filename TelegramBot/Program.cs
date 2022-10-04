using Telegram.Bot;
using Telegram.Bot.Types;
using System.Net;
using Newtonsoft.Json.Linq;


namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {


            var request = new GetRequest("https://yesno.wtf/api");
            request.Run();
            var response = request.Response;
            var json = JObject.Parse(response);
            var link = json["image"];


            var client = new TelegramBotClient("5176617173:AAF3G2Mg_9emdKvCkz4y5L1zV4ZZGp_bsTQ");
            client.StartReceiving(Update, Error);

            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message.Text != null)
            {
                //Вывод сообщения в консоль
                Console.WriteLine($"{message.Chat.FirstName}  |   {message.Text}");

                //Обработка сообщений пользователя
                if (message.Text.ToLower().Contains("привет"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Приветик!\nТы можешь спросить у меня:\nСколько времени?\nДа или нет?");
                    return;
                }

                if (message.Text.ToLower().Contains("сколько времени?"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Хм, сейчас {message.Date.AddHours(5)}");
                    return;
                }

                if (message.Text.ToLower().Contains("да или нет?"))
                {
                    var request = new GetRequest("https://yesno.wtf/api");
                    request.Run();
                    var response = request.Response;
                    var json = JObject.Parse(response);
                    var link = json["image"];
                    if (link != null)
                    {
                        await botClient.SendAnimationAsync(message.Chat.Id, (string)link);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Что-то пошло не так...");
                    }
                    return;
                }

            }
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }


        public class GetRequest
        {
            HttpWebRequest _request;
            string _address;

            public string Response { get; set; }

            public GetRequest(string address)
            {
                _address = address;
            }

            public void Run()
            {
                _request = (HttpWebRequest)WebRequest.Create(_address);
                _request.Method = "GET";

                try
                {
                    HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                    var stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        Response = new StreamReader(stream).ReadToEnd();
                    }
                }
                catch (Exception ex)
                {

                }


            }
        }
    }
}