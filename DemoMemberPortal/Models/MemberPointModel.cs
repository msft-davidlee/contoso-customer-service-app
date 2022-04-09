using System.Text.Json.Serialization;

namespace DemoMemberPortal.Models
{
    public class MemberPointModel
    {
        [JsonPropertyName("totalPoints")]
        public int TotalPoints { get; set; }
    }
}
