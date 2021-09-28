using System;

namespace DemoPartnerCore
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; }

        public int EstimatedSeconds { get; set; }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
}