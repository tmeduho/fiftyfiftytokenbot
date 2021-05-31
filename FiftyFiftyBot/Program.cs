using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using NetTelegramBotApi;
using NetTelegramBotApi.Requests;
using NetTelegramBotApi.Types;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using FiftyFiftyBot.DataTransferObjects;

namespace FiftyFiftyBot
{
  class Program
  {
    private static bool stopMe = false;
    private const string _pancakeApiUrl = "https://api.pancakeswap.info/api/v2/tokens/0x34D266A0cd7298A28D664F3FF9e16ccCa53F06f2";

    static void Main(string[] args)
    {
      var config = new ConfigurationBuilder()
        .SetBasePath(System.IO.Directory.GetCurrentDirectory())
        .AddJsonFile("config.json")
        .Build();

      var accessToken = config["AccessToken"];

      Console.WriteLine("Hello Telegram! Bot starting..");
      Console.WriteLine();

      var t = Task.Run(() => RunBot(accessToken));
      t.ContinueWith(task => { Console.WriteLine(t.Exception?.GetBaseException()); });

      Console.ReadLine();

      stopMe = true;
    }

    private static void RunBot(string accessToken)
    {
      var bot = new TelegramBot(accessToken, new HttpClient());

      var me = bot.MakeRequestAsync(new GetMe()).Result;
      if (me == null)
      {
        Console.WriteLine("Call to GetMe() Failed. Double check access key is set in config.json!");
        Console.WriteLine("(Press ANY Key to Quit)");
        Console.ReadKey();
        return;
      }

      Console.OutputEncoding = System.Text.Encoding.UTF8;
      Console.WriteLine($"{me.FirstName} (@{me.Username} connected!");
      Console.WriteLine();

      long offset = 0;
      while (!stopMe)
      {
        var updates = bot.MakeRequestAsync(new GetUpdates() { Offset = offset }).Result;
        foreach (var update in updates)
        {
          offset = update.UpdateId + 1;

          if (update.Message == null)
          {
            continue;
          }

          var from = update.Message.From;
          var text = update.Message.Text;
          var photos = update.Message.Photo;
          var contact = update.Message.Contact;
          var location = update.Message.Location;

          Console.WriteLine($"Msg from {from.FirstName} {from.LastName} ({from.Username}) at {update.Message.Date}: {text}");

          if (string.IsNullOrWhiteSpace(text))
          {
            continue;
          }

          if (text == "/hello")
          {
            var reqAction = new SendMessage(update.Message.Chat.Id, $"Hello {update.Message.From.Username}!");
            bot.MakeRequestAsync(reqAction).Wait();
            continue;
          }

          if (text == "/price")
          {
            // get response from pancakeswap api
            // https://api.pancakeswap.info/api/v2/tokens/0x34D266A0cd7298A28D664F3FF9e16ccCa53F06f2
            var httpRequest = new HttpClient().GetStringAsync(_pancakeApiUrl);
            httpRequest.Wait();
            var str = httpRequest.Result;
            var result = JsonSerializer.Deserialize<PancakeApiResult>(str);

            var reqAction = new SendMessage(update.Message.Chat.Id, $"Name: {result.Data.Name}\r\nSymbol: {result.Data.Symbol}\r\nCurrent Price: {result.Data.Price}");
            bot.MakeRequestAsync(reqAction).Wait();
          }
        }
      }
    }
  }
}
