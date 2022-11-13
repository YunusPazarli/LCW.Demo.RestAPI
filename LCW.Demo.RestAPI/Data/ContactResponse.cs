using Newtonsoft.Json;

namespace LCW.Demo.Data
{
    public class ContactResponse
    {
        [JsonProperty("response")]
        public string Response { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
