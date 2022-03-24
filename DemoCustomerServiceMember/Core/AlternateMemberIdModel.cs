using System.Text.Json.Serialization;

namespace DemoCustomerServiceMember.Core
{
    public class AlternateMemberIdModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [JsonPropertyName("memberId")]
        public string MemberId { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
