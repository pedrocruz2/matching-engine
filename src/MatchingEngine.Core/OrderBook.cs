using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatchingEngine.Core
{
    public class OrderBook
    {
        private readonly List<Order> buyOrders = new List<Order>();
        private readonly List<Order> sellOrders = new List<Order>();
        private readonly Dictionary<string, Order> ordersById = new Dictionary<string, Order>();
        
        public void AddOrder(Order order)
        {
            if (order.Type == OrderType.Market)
                throw new InvalidOperationException("Ordens a mercado não devem ser adicionadas no livro");
                
            var orderList = order.Side == Side.Buy ? buyOrders : sellOrders;
            orderList.Add(order);
            ordersById[order.Id] = order;
            
            // Reordenar lista depois de adicionar
            if (order.Side == Side.Buy)
                buyOrders.Sort((a, b) => b.Price.CompareTo(a.Price)); // Ordem decrescente de preço
            else
                sellOrders.Sort((a, b) => a.Price.CompareTo(b.Price)); // Ordem crescente de preço
        }
        
        public void RemoveOrder(Order order)
        {
            var orderList = order.Side == Side.Buy ? buyOrders : sellOrders;
            orderList.Remove(order);
            ordersById.Remove(order.Id);
        }
        
        public Order GetOrderById(string id)
        {
            ordersById.TryGetValue(id, out var order);
            return order;
        }
        
        public Order GetBestBuyOrder()
        {
            return buyOrders.FirstOrDefault();
        }
        
        public Order GetBestSellOrder()
        {
            return sellOrders.FirstOrDefault();
        }
        
        public string PrintOrderBook()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("===== ORDER BOOK =====");
            sb.AppendLine("SELL ORDERS:");
            
            foreach (var order in sellOrders)
            {
                sb.AppendLine($"  {order.ToString()}");
            }
            
            sb.AppendLine("BUY ORDERS:");
            
            foreach (var order in buyOrders)
            {
                sb.AppendLine($"  {order.ToString()}");
            }
            
            sb.AppendLine("=====================");
            
            return sb.ToString();
        }
    }
}