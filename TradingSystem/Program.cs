using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TradingSystem;
using System;
using System.Net;
using System.Net.Mail;

// Define an Order class to represent individual orders
public class Order
{
    public int OrderId { get; set; }
    public string Symbol { get; set; }
    public double Quantity { get; set; }
    protected double Price { get; set; } //The price is of type protected since it can't be changed by the outside. On the other hand, it should be used by the OrderManager class
    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string comment {  get; set; }


    public Order(FinancialInstrument asset) //In order to price automatically and integrate this in the order, we should give the order constructor an Options object as an input. However, Options inherits the FiancialInstrument class
    {
        this.Price = asset.Pricer();
        this.comment = asset.comment;
        Symbol = asset.TickerSymbol;

    }
    public double GetPrice() //This is a get function to retreive the price of the product when the price is called from outisde the class, since the access modifier is of type "protected"
    {
        return this.Price;
    }
    internal double pprice
    {
        set
            
        {
            if(this.Status == OrderStatus.Hedging)
            {
                this.Price = value;
            }
            
        }
    }

    // Additional properties and methods can be added based on requirements
}

// Enum to represent the type of order (Buy or Sell)
    public enum OrderType
    {
        Buy,
        Sell
    }

    // Enum to represent the status of an order
    public enum OrderStatus
    {
        New,
        Pending,
        Executed,
        Cancelled,
        Hedging
    }
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
                //OrderManager ordrmanagr = new OrderManager();
                
                OrderManager.OnProcessCompleted(new string[] {order.Type.ToString(), order.Quantity.ToString(), order.Symbol, order.GetPrice().ToString()});

            }
            else
            {

                IsLogin = AuthenticationService.AuthenticateUser();
            }

        }
        protected static void OnProcessCompleted(string[] infos)
        {
            EmailSender?.Invoke(null,infos);
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
    public static  void DisplayOrders()
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
        // Implement your logic for generating a unique order ID
        // This could involve using timestamps, a counter, or other strategies
        return orders.Count + 1;
    }

    // Helper method to retrieve an order by its ID
    private static Order GetOrderById(int orderId)
    {
        return orders.Find(order => order.OrderId == orderId);
    }
}

class Program
{
    static void Main()
    {
        try
        {
            //OrderManager bl = new OrderManager();
            OrderManager.EmailSender += EmailSender.bl_ProcessCompleted; // register with an event

            Stocks Astock = new Stocks("AAPL");
            Stocks Asstock = new Stocks("MSFT");
            EuropeanOptions options = new EuropeanOptions(Astock, "XCD5658", 150, 1, EuropeanOptions.BullOrBear.Call);
            EuropeanOptions optionss = new EuropeanOptions(Asstock, "XCD565848", 350, 1, EuropeanOptions.BullOrBear.Put);
          
            Dictionary<string, double> sensitivity = options.Sensitivity();
            //Console.WriteLine(sensitivity["delta"]);

            Order buyOrder = new Order(options)
            {
                Quantity = 10,
                Type = OrderType.Buy
            };
            Order sellOrder = new Order(optionss)
            {
                Quantity = 5,
                Type = OrderType.Sell
            };
            Order buyoOrder = new Order(options)
            {
                Quantity = 100,
                Type = OrderType.Buy
            };

            OrderManager.PlaceOrder(buyOrder);
            OrderManager.PlaceOrder(sellOrder);
            OrderManager.PlaceOrder(buyoOrder);
            options.deltaHedge();

            Order buystockOrder = new Order(Astock)
            {
                //Symbol = "ABC",
                Quantity = 100,

                Type = OrderType.Buy
            };

            OrderManager.PlaceOrder(buystockOrder);
            OrderManager.DisplayOrders();
            OrderManager.CancelOrder(1);
            OrderManager.CancelOrder(2);
            OrderManager.DisplayOrders();

            Bonds newbond = new Bonds(Bonds.ZeroCouponOrCoupon.ZeroCouponBond, "fdf", "sdddds", 10, 5, 0.05, 0.06);

            Console.WriteLine(newbond.Pricer());
            Console.WriteLine(newbond.Kind);
            Console.WriteLine(newbond.CouponRate);

            Stocks AAAstock = new Stocks("AAPL");
            Console.WriteLine(Astock.ISIN);
            Console.WriteLine(Astock.Pricer());

            Stocks.Add("ABC", new string[] { "Tesla Inc.", "US88160R1014", "950.00", "0.08", "0.20" });

            Stocks AAstock = new Stocks("ABC");
            Console.WriteLine(AAstock.ISIN);
            Console.WriteLine(AAstock.Pricer());

            EuropeanOptions option = new EuropeanOptions(Astock, "XCD5658", 30, 51, EuropeanOptions.BullOrBear.Put);
            Console.WriteLine(option.ISIN);
            Console.WriteLine(option.Pricer());

            EuropeanOptions optiont = new EuropeanOptions(AAstock, "XCD5658", 50, 50, EuropeanOptions.BullOrBear.Call);
            Console.WriteLine(optiont.ISIN);
            Console.WriteLine(optiont.Pricer());
            Dictionary<string, double> snsitivity = optiont.Sensitivity();
            Console.WriteLine(snsitivity["Gamma"]);


            OrderManager.LogOut();
        }
        catch  (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
