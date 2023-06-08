using System.Text.Json.Serialization;

namespace HealthCareSysAPI.CosmosModel
{
    public class Chat
    {


        [JsonPropertyName("content")]
        public string content { get; set; }

        [JsonPropertyName("sender")]
        public string sender { get; set; }
        [JsonPropertyName("receiver")]
        public string receiver { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime timestamp { get; set; } = DateTime.UtcNow;
    }
}
