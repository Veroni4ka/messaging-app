using Newtonsoft.Json;

namespace MessagingApp
{
    class Request
    {
        [JsonProperty("Comment_text")]
        public string Comment { get; set; } = string.Empty;
    }
}