using System.ComponentModel.DataAnnotations;

namespace DemoCustomerServiceMember.Models
{
    public class RewardCustomer
    {
        [Key]
        public string MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
