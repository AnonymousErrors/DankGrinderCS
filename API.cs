using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DankGrinderCS
{
    class API
    {

        private static readonly string API_URL = "https://discord.com/api/v9/";

        /// <summary>
        /// Sends a request to the url using a http client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="method"></param>
        /// <param name="request_url"></param>
        /// <param name="body"></param>
        /// <returns> Returns a HttpResponseMessage </returns>
        async static public Task<HttpResponseMessage> send_request(HttpClient client, string method,string request_url, HttpContent body = null)
        {
            HttpResponseMessage response = null;
            switch (method.ToUpper())
            {
                case "GET":
                    {
                        response = await client.GetAsync(API_URL+request_url);
                        break;
                    }
                case "POST":
                    {
                        response = await client.PostAsync(API_URL + request_url, body);
                        break;
                    }
            }
            return response;
        }

        /// <summary>
        /// Sends a message to the specified channel
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channel_id"></param>
        /// <param name="content"></param>
        /// <returns> Returns a HttpResponseMessage </returns>
        async static public Task<HttpResponseMessage> send_message(HttpClient client, string channel_id, string content)
        {
            return await send_request(
            client,
            "POST",
            $"channels/{channel_id}/messages",
            new StringContent($"{{\"content\":\"{content}\"}}", Encoding.UTF8, "application/json")
        );
        }


        /// <summary>
        /// Gets the newest message sent in the channel, user_id is optional
        /// Recursive Function
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channel_id"></param>
        /// <param name="user_id"></param>
        /// <returns> Returns a JToken with the message data </returns>
        async static public Task<JToken> getLatestMessage(HttpClient client, string channel_id, string user_id = "")
        {
            HttpResponseMessage response = await send_request(client, "GET", $"channels/{channel_id}/messages?limit=1");
            JToken message = JsonConvert.DeserializeObject<JArray>(await response.Content.ReadAsStringAsync())[0]; 
            if (user_id != "" && user_id != message["author"]["id"].ToString())
            {
                await Task.Delay(1000);
                return await getLatestMessage(client, channel_id, user_id);
            }
            return JsonConvert.DeserializeObject<JArray>(await response.Content.ReadAsStringAsync()).First();
        }
        

        /// <summary>
        /// Interaction API, still in progress, but it's enough to support highlow, search and postmeme
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channel_id"></param>
        /// <param name="component_type"></param>
        /// <param name="custom_id"></param>
        /// <param name="guild_id"></param>
        /// <param name="message_id"></param>
        /// <returns> Returns a HttpResponseMessage </returns>
        async static public Task<HttpResponseMessage> interact(HttpClient client, string channel_id, int component_type, string custom_id, string guild_id, string message_id)
        {
            string request_json = $"{{ \"application_id\" : \"270904126974590976\", \"channel_id\" : \"{ channel_id }\", \"data\" : {{ \"component_type\" : { component_type }, \"custom_id\" : \"{ custom_id }\" }}, \"type\": 3, \"guild_id\" : \"{ guild_id }\", \"message_id\" : \"{ message_id }\" }}";
            var response = await send_request(
            client,
            "POST",
            "interactions",
            new StringContent(
                request_json,
                Encoding.UTF8, 
                "application/json"
                )
            );
            return response;
        }
    }
}
