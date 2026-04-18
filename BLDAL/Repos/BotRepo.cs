using System;
using System.Net.Http;
using System.Threading.Tasks;

class BotRepo
{
    public static async Task SendTelegramMessage()
    {
    string botToken = "8686026558:AAF_eEUAzESTAvsgkHCTOVk0dApynN1Kwg0";
    string chatId = "7483202573";
    string message = "Hallo dies ist mein erster test";

        using (HttpClient client = new HttpClient())
        {
            string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(message)}";
            var response = await client.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(result);
        }
    }
}