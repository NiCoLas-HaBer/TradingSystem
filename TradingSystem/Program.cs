using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TradingSystem;
using System;
using System.Net;
using System.Net.Mail;


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


class Program
{
    static void Main()
    {
        try
        {
            OrderManager.EmailSender += EmailSender.bl_ProcessCompleted; // register with an event

            Stocks Astock = new Stocks("AAPL");
            Stocks Asstock = new Stocks("MSFT");
            EuropeanOptions options = new EuropeanOptions(Astock, "XCD5658", 150, 1, EuropeanOptions.BullOrBear.Call);
            EuropeanOptions optionss = new EuropeanOptions(Asstock, "XCD565848", 350, 1, EuropeanOptions.BullOrBear.Put);
          
            Dictionary<string, double> sensitivity = options.Sensitivity();

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
