using System;

namespace DemoPartnerCore.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public string MemberId { get; set; }

        public string ProductId { get; set; }

        public DateTime Created { get; set; }

        public string TrackingNumber { get; set; }

        public DateTime? Shipped { get; set; }
    }
}
