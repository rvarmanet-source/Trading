using System.Collections.Generic;

namespace Trading.Models
{
    public class OrdersData
    {
        public List<Order> OpenOrders { get; set; } = new();
        public List<Order> TradeHistory { get; set; } = new();
    }
}
