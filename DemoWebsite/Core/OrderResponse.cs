using System;

namespace DemoWebsite.Core
{
    public class OrderResponse
    {
        public int EstimatedSeconds { get; set; }
        public Guid? OrderId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
