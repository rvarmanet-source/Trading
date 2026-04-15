using System;

namespace Trading.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Side { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
