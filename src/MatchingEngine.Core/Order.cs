using System;

namespace MatchingEngine.Core
{
    public enum OrderType { Limit, Market }
    public enum Side { Buy, Sell }
    
    public class Order
    {
        public string Id { get; }
        public OrderType Type { get; }
        public Side Side { get; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreationTime { get; }
        
        public Order(OrderType type, Side side, float price, int quantity)
        {
            Id = Guid.NewGuid().ToString().Substring(0, 8);
            Type = type;
            Side = side;
            Price = price;
            Quantity = quantity;
            CreationTime = DateTime.Now;
        }
        
        public override string ToString()
        {
            return $"{Side} {Quantity} @ {(Type == OrderType.Market ? "MARKET" : Price.ToString("F2"))} (ID: {Id})";
        }
    }
}