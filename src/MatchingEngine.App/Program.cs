using System;
using MatchingEngine.Core;

namespace MatchingEngine.App
{
    class Program
    {
        static void Main()
        {
            var engine = new Core.MatchingEngine();
            
            Console.WriteLine("Matching Engine iniciada!");
            Console.WriteLine("Comandos disponíveis:");
            Console.WriteLine("  limit buy/sell <preço> <quantidade>");
            Console.WriteLine("  market buy/sell <quantidade>");
            Console.WriteLine("  cancel <id_da_ordem>");
            Console.WriteLine("  modify <id_da_ordem> price <novo_preço>");
            Console.WriteLine("  modify <id_da_ordem> qty <nova_quantidade>");
            Console.WriteLine("  modify <id_da_ordem> price <novo_preço> qty <nova_quantidade>");
            Console.WriteLine("  book - exibe o livro de ordens");
            Console.WriteLine("  exit - encerra o programa");
            
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input)) 
                    continue;
                    
                if (input.ToLower() == "exit") 
                    break;
                    
                if (input.ToLower() == "book")
                {
                    Console.WriteLine(engine.GetOrderBook());
                    continue;
                }
                
                string[] parts = input.Split();
                
                try
                {
                    if (parts[0].ToLower() == "limit" && parts.Length == 4)
                    {
                        ProcessLimitOrder(engine, parts);
                    }
                    else if (parts[0].ToLower() == "market" && parts.Length == 3)
                    {
                        ProcessMarketOrder(engine, parts);
                    }
                    else if (parts[0].ToLower() == "cancel" && parts.Length == 2)
                    {
                        string orderId = parts[1];
                        bool success = engine.CancelOrder(orderId);
                        
                        if (success)
                            Console.WriteLine("Order cancelled");
                        else
                            Console.WriteLine("Order not found");
                    }
                    else if (parts[0].ToLower() == "modify")
                    {
                        ProcessModifyOrder(engine, parts);
                    }
                    else
                    {
                        PrintUsage();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                }
            }
            
            Console.WriteLine("Encerrando Matching Engine...");
        }
        
        static void ProcessLimitOrder(Core.MatchingEngine engine, string[] parts)
        {
            string sideInput = parts[1].ToLower();
            
            if (sideInput != "buy" && sideInput != "sell")
            {
                Console.WriteLine("Side inválido. Use 'buy' ou 'sell'.");
                return;
            }

            if (!float.TryParse(parts[2], out float price))
            {
                Console.WriteLine("Preço deve ser um número válido.");
                return;
            }
            
            if (!int.TryParse(parts[3], out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Quantidade deve ser um número inteiro positivo.");
                return;
            }
            
            Side side = sideInput == "buy" ? Side.Buy : Side.Sell;
            Order order = new Order(OrderType.Limit, side, price, quantity);
            
            string result = engine.AddOrder(order);
            Console.WriteLine(result);
        }
        
        static void ProcessMarketOrder(Core.MatchingEngine engine, string[] parts)
        {
            string sideInput = parts[1].ToLower();
            
            if (sideInput != "buy" && sideInput != "sell")
            {
                Console.WriteLine("Side inválido. Use 'buy' ou 'sell'.");
                return;
            }
            
            if (!int.TryParse(parts[2], out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Quantidade deve ser um número inteiro positivo.");
                return;
            }
            
            Side side = sideInput == "buy" ? Side.Buy : Side.Sell;
            Order order = new Order(OrderType.Market, side, 0, quantity);
            
            string result = engine.AddOrder(order);
            Console.WriteLine(result);
        }
        
        static void ProcessModifyOrder(Core.MatchingEngine engine, string[] parts)
        {
            if (parts.Length < 4)
            {
                PrintUsage();
                return;
            }
            
            string orderId = parts[1];
            float? newPrice = null;
            int? newQuantity = null;
            
            for (int i = 2; i < parts.Length; i += 2)
            {
                if (i + 1 >= parts.Length)
                {
                    PrintUsage();
                    return;
                }
                
                string param = parts[i].ToLower();
                string value = parts[i + 1];
                
                if (param == "price")
                {
                    if (!float.TryParse(value, out float price))
                    {
                        Console.WriteLine("Preço deve ser um número válido.");
                        return;
                    }
                    newPrice = price;
                }
                else if (param == "qty")
                {
                    if (!int.TryParse(value, out int qty) || qty <= 0)
                    {
                        Console.WriteLine("Quantidade deve ser um número inteiro positivo.");
                        return;
                    }
                    newQuantity = qty;
                }
                else
                {
                    PrintUsage();
                    return;
                }
            }
            
            bool success = engine.ModifyOrder(orderId, newPrice, newQuantity);
            
            if (success)
                Console.WriteLine("Order modified");
            else
                Console.WriteLine("Order not found");
        }
        
        static void PrintUsage()
        {
            Console.WriteLine("Formato inválido! Use:");
            Console.WriteLine("  limit buy/sell <preço> <quantidade>");
            Console.WriteLine("  market buy/sell <quantidade>");
            Console.WriteLine("  cancel <id_da_ordem>");
            Console.WriteLine("  modify <id_da_ordem> price <novo_preço>");
            Console.WriteLine("  modify <id_da_ordem> qty <nova_quantidade>");
            Console.WriteLine("  modify <id_da_ordem> price <novo_preço> qty <nova_quantidade>");
        }
    }
}