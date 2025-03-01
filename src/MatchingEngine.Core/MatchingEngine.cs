using System;
using System.Collections.Generic;

namespace MatchingEngine.Core
{
    public class MatchingEngine
    {
        private readonly OrderBook orderBook = new OrderBook();
        
        public string AddOrder(Order order)
        {
            switch (order.Type)
            {
                case OrderType.Market:
                    int originalQuantity = order.Quantity;
                    ExecuteMarketOrder(order);
                    return $"Market order executada: {order.Side} {originalQuantity}";
                    
                case OrderType.Limit:
                    string result = ExecuteLimitOrder(order);
                    if (order.Quantity > 0)
                    {
                        orderBook.AddOrder(order);
                        return $"Order criada: {order} {result}";
                    }
                    return $"Order executada por completo: {result}";
                    
                default:
                    throw new ArgumentException($"Type {order.Type} não suportado, utilize apenas limit ou market");
            }
        }
        
        private void ExecuteMarketOrder(Order order)
        {
            switch (order.Side)
            {
                case Side.Buy:
                    while (order.Quantity > 0)
                    {
                        Order match = orderBook.GetBestSellOrder();
                        if (match == null) break;
                        
                        int tradeQty = Math.Min(order.Quantity, match.Quantity);
                        Console.WriteLine($"Trade, price: {match.Price:F2}, qty: {tradeQty}");
                        
                        order.Quantity -= tradeQty;
                        match.Quantity -= tradeQty;
                        
                        if (match.Quantity == 0)
                        {
                            orderBook.RemoveOrder(match);
                        }
                    }
                    break;
                    
                case Side.Sell:
                    while (order.Quantity > 0)
                    {
                        Order match = orderBook.GetBestBuyOrder();
                        if (match == null) break;
                        
                        int tradeQty = Math.Min(order.Quantity, match.Quantity);
                        Console.WriteLine($"Trade, price: {match.Price:F2}, qty: {tradeQty}");
                        
                        order.Quantity -= tradeQty;
                        match.Quantity -= tradeQty;
                        
                        if (match.Quantity == 0)
                        {
                            orderBook.RemoveOrder(match);
                        }
                    }
                    break;
                    
                default:
                    throw new ArgumentException($"Side {order.Side} não suportado, utilize apenas buy ou sell");
            }
        }
        
        private string ExecuteLimitOrder(Order order)
        {
            string tradesInfo = "";
            
            switch (order.Side)
            {
                case Side.Buy:
                    while (order.Quantity > 0)
                    {
                        Order match = orderBook.GetBestSellOrder();
                        if (match == null || match.Price > order.Price)
                            break;
                        
                        int tradeQty = Math.Min(order.Quantity, match.Quantity);
                        Console.WriteLine($"Trade, price: {match.Price:F2}, qty: {tradeQty}");
                        tradesInfo += $"\nTrade, price: {match.Price:F2}, qty: {tradeQty}";
                        
                        order.Quantity -= tradeQty;
                        match.Quantity -= tradeQty;
                        
                        if (match.Quantity == 0)
                        {
                            orderBook.RemoveOrder(match);
                        }
                    }
                    break;
                    
                case Side.Sell:
                    while (order.Quantity > 0)
                    {
                        Order match = orderBook.GetBestBuyOrder();
                        if (match == null || match.Price < order.Price)
                            break;
                        
                        int tradeQty = Math.Min(order.Quantity, match.Quantity);
                        Console.WriteLine($"Trade, price: {match.Price:F2}, qty: {tradeQty}");
                        tradesInfo += $"\nTrade, price: {match.Price:F2}, qty: {tradeQty}";
                        
                        order.Quantity -= tradeQty;
                        match.Quantity -= tradeQty;
                        
                        if (match.Quantity == 0)
                        {
                            orderBook.RemoveOrder(match);
                        }
                    }
                    break;
                    
                default:
                    throw new ArgumentException($"Side {order.Side} não suportado, utilize apenas buy ou sell");
            }
            
            return tradesInfo;
        }
        
        public bool CancelOrder(string orderId)
        {
            Order order = orderBook.GetOrderById(orderId);
            if (order == null)
                return false;
                
            orderBook.RemoveOrder(order);
            return true;
        }
        
        public bool ModifyOrder(string orderId, float? newPrice, int? newQuantity)
        {
            Order order = orderBook.GetOrderById(orderId);
            if (order == null)
                return false;
                
            orderBook.RemoveOrder(order);
            
            if (newPrice.HasValue)
                order.Price = newPrice.Value;
                
            if (newQuantity.HasValue)
                order.Quantity = newQuantity.Value;
                
            orderBook.AddOrder(order);
            return true;
        }
        
        public string GetOrderBook()
        {
            return orderBook.PrintOrderBook();
        }
    }
}