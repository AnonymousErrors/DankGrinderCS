using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DankGrinderCS
{
    public class Farm
    {
        // Dank Memer White Space For Mini Games
        private static readonly string whitespace = "       ";
        async private static Task<HttpResponseMessage> CatchBoss(HttpClient client, string guild_id, JToken message)
        {
            // wait 2 seconds for dank memer to make a edit
            // yes dank mamer can maximum make one edit
            await Task.Delay(2000);
            string msg_content = message["content"].ToString();
            JToken msg_components = message["components"].First()["components"];

            // this is the line where the white space is at
            string catch_line = msg_content.Split('\n')[1];
            int correct_interaction = 0;
            if (catch_line.StartsWith(whitespace + whitespace))
                correct_interaction = 2;
            else if (catch_line.StartsWith(whitespace))
                correct_interaction = 1;


            JToken interact_with = msg_components[correct_interaction];
            return await API.interact(
                client,
                message["channel_id"].ToString(),
                int.Parse(interact_with["type"].ToString()),
                interact_with["custom_id"].ToString(),
                guild_id,
                message["id"].ToString());
        }

        // pls hunt and pls fish module because they're the same
        async private static Task<JToken> HFModule(HttpClient client, string cmd, string channel_id, string guild_id, JToken message, bool autobuy)
        {
            string msg_content = message["content"].ToString();
            string item_name = "";

            // attempt to get item name
            try { item_name = msg_content.Split(',')[0].Split(' ')[5]; }
            catch (Exception) { }

            // attempt auto buy
            if (autobuy && item_name != "")
            {
                await API.send_message(client, channel_id, $"pls buy { item_name } 1");
                await Task.Delay(1000);
                await API.send_message(client, channel_id, cmd);
                
                // get the message again
                message = await API.getLatestMessage(client, channel_id, "270904126974590976");
            }


            // attempt to catch the minigame
            JToken components = message["components"];
            if (components.Count() > 0)
                await CatchBoss(client, guild_id, message);

            return message;
        }
        async private static Task<HttpResponseMessage> HighLowModule(HttpClient client, string guild_id, JToken message)
        {
            // Get Embed Decriptiong
            string[] embed_desc = message["embeds"][0]["description"].ToString().Trim().Split('*');

            // It's forced to work, let me know if it gets patched
            int.TryParse(embed_desc[embed_desc.Length - 3], out int number);
            
            // default : higher
            int interact_with = 2;
            if (number >= 60)
                // bigger than 60 : lower
                interact_with = 0;

            JToken interaction = message["components"].First()["components"][interact_with];
            return await API.interact(
                client,
                message["channel_id"].ToString(),
                int.Parse(interaction["type"].ToString()),
                interaction["custom_id"].ToString(),
                guild_id,
                message["id"].ToString());
        }


        private static Random rand = new Random();
        async private static Task<HttpResponseMessage> MSModule(HttpClient client, string guild_id, JToken message, bool autobuy)
        {
            string channel_id = message["channel_id"].ToString();
            if (autobuy && message["content"].ToString().Contains("buy a laptop"))
            {
                await API.send_message(client, channel_id, $"pls buy laptop 1");
                await Task.Delay(1000);
                await API.send_message(client, channel_id, "pls pm");
                message = await API.getLatestMessage(client, channel_id, "270904126974590976");
            }

            JToken rand_interaction = message["components"].First()["components"][rand.Next(0, 2)];
            return await API.interact(
                client,
                message["channel_id"].ToString(),
                int.Parse(rand_interaction["type"].ToString()),
                rand_interaction["custom_id"].ToString(),
                guild_id,
                message["id"].ToString());
        }
        async public static Task<JToken> StartFarming(HttpClient client, string channel_id, string guild_id, string cmd_name, bool autobuy = false)
        {
            cmd_name = "pls " + cmd_name;
            HttpResponseMessage response = await API.send_message(client, channel_id, cmd_name);

            Console.WriteLine($"Sent the command { cmd_name }\nResponse Code: { response.IsSuccessStatusCode } ");
            while(!response.IsSuccessStatusCode)
                response = await API.send_message(client, channel_id, cmd_name);
            await Task.Delay(1000);

            // Search a message with dank memer id
            JToken latest_msg = await API.getLatestMessage(client, channel_id, "270904126974590976");
            // different interactions for different commands
            switch (cmd_name)
            {
                case "pls hunt":
                    {
                        await HFModule(client, cmd_name, channel_id, guild_id, latest_msg, autobuy);
                        await StartFarming(client, channel_id, guild_id, "fish", autobuy);
                        break;
                    }
                case "pls fish":
                    {
                        await HFModule(client, cmd_name, channel_id, guild_id, latest_msg, autobuy);
                        await StartFarming(client, channel_id, guild_id, "dig", autobuy);
                        break;
                    }
                case "pls dig":
                    {
                        await StartFarming(client, channel_id, guild_id, "beg", autobuy);
                        break;
                    }
                case "pls beg":
                    {
                        await StartFarming(client, channel_id, guild_id, "hl", autobuy);
                        break;
                    }
                case "pls hl":
                    {
                        await HighLowModule(client, guild_id, latest_msg);
                        await StartFarming(client, channel_id, guild_id, "pm", autobuy);
                        break;
                    }
                case "pls pm":
                    {

                        await MSModule(client, guild_id, latest_msg, autobuy);
                        await StartFarming(client, channel_id, guild_id, "search", autobuy);
                        break;
                    }
                // what are you gonna buy for searching? a magnifier?? haha
                case "pls search":
                    {
                        await MSModule(client, guild_id, latest_msg, false);
                        break;
                    }
            }
            return latest_msg;
        }
    }
}
