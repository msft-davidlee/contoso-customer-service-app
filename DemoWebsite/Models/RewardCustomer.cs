using System.ComponentModel.DataAnnotations;

namespace DemoWebsite.Models
{
    public class RewardCustomer
    {
        [Key]
        public string MemberId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Points { get; set; }
    }
}
