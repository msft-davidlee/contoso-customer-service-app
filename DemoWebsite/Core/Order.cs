using System;
using System.Collections.Generic;

namespace DemoWebsite.Core
{
    public class Order
    {
        public Guid Id { get; set; }

        public string MemberId { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public DateTime Created { get; set; }

        public string TrackingNumber { get; set; }

        public DateTime? Shipped { get; set; }

        internal List<Order> ToList()
        {
            throw new NotImplementedException();
        }
    }
}
