using System.ComponentModel.DataAnnotations;

namespace DemoWebsite.Models
{
    public class RewardCustomerPoints
    {
        [Key]
        public string MemberId { get; set; }

        public int Points { get; set; }
    }
}
