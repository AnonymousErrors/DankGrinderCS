using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DankGrinderCS
{
    class Program
    {
        private static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            // init stuff
            Console.WriteLine("Before you start this:\nYou need to provide your token, server id and channel id");
            Console.WriteLine("How to find your token: https://youtu.be/YEgFvgg7ZPI");
            Console.WriteLine("How to find server and channel id: https://youtu.be/NLWtSHWKbAI");
            Console.WriteLine("Enter your token: ");
            string token = Console.ReadLine();
            Console.WriteLine("Enter guild id: ");
            string guild_id = Console.ReadLine();
            Console.WriteLine("Enter the channel id: ");
            string channel_id = Console.ReadLine();
            // set the headers
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/0.0.309 Chrome/83.0.4103.122 Electron/9.3.5 Safari/537.36");
            
            // SSL Certificate Bypass
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            // start the farm
            new Thread(async () =>
            {
                // wait 2 seconds for the header to set
                Console.WriteLine("Starting...");
                await Task.Delay(2000);
                while (true)
                {
                    await Farm.StartFarming(client, channel_id, guild_id, "hunt");
                    await Task.Delay(30000);
                }
            }).Start();
            Console.ReadLine();
        }
    }
}
