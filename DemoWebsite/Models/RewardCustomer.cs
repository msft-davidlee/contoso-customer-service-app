using System.Text.Json.Serialization;

namespace DemoWebsite.Models
{
    public class RewardCustomer
    {
        [JsonPropertyName("memberId")]
        public string MemberId { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }
}
