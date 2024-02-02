using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystem
{
    public class OrderManager
    {
        private static List<Order> orders = new List<Order>();
        private static bool IsLogin = false;

        public static event EventHandler<string[]> EmailSender;



        public static void LogOut()
        {
            IsLogin = false;
        }
        public static void PlaceOrder(Order order)
        {

            if (order.Symbol != null && IsLogin)
            {
                order.OrderId = GenerateOrderId();
                order.CreatedAt = DateTime.Now;
                order.Status = OrderStatus.New;

                orders.Add(order);



                Console.WriteLine($"Order placed: {order.OrderId} - {order.Type} {order.Quantity} {order.comment} of {order.Symbol} at {order.GetPrice()}");


                OrderManager.OnProcessCompleted(new string[] { order.Type.ToString(), order.Quantity.ToString(), order.Symbol, order.GetPrice().ToString() });

            }
            else
            {

                IsLogin = AuthenticationService.AuthenticateUser();
            }

        }
        protected static void OnProcessCompleted(string[] infos)
        {
            EmailSender?.Invoke(null, infos);
        }

        // Cancel an existing order
        public static void CancelOrder(int orderId)
        {
            if (IsLogin)
            {
                Order order = GetOrderById(orderId);

                if (order != null && order.Status == OrderStatus.New)
                {
                    order.Status = OrderStatus.Cancelled;
                    Console.WriteLine($"Order {orderId} cancelled.");
                }
                else
                {
                    Console.WriteLine($"Unable to cancel order {orderId}. Order not found or already executed.");
                }

            }
            else
            {
                IsLogin = AuthenticationService.AuthenticateUser();
            }

        }

        // Get details of all orders
        public static void DisplayOrders()
        {
            Console.WriteLine("List of Orders:");
            foreach (var order in orders)
            {
                Console.WriteLine($"{order.OrderId} - {order.Type} {order.Quantity} {order.Symbol} at {order.GetPrice()} ({order.Status})");
            }
        }

        // Helper method to generate a unique order ID
        private static int GenerateOrderId()
        {
            return orders.Count + 1;
        }

        // Helper method to get an order by its ID
        private static Order GetOrderById(int orderId)
        {
            return orders.Find(order => order.OrderId == orderId);
        }
    }

}
