using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace TradingSystem
{
    public delegate void hedgingOrder(Order order);
    public class EuropeanOptions : FinancialInstrument

    {
        //public string TickerSymbol;
        public double Spot_Price {  get; set; }
        public double Strike_price {  get; set; }
        public int Time_To_Expiration {  get; set; }

        public BullOrBear kind { get; set; }

        public double Vol{  get; set; }
        public Stocks Stck {  get; set; }
        public override string comment {  get; set; }

        protected double delta {  get; set; }
        protected double gamma { get; set; }
        protected Dictionary<string, double> sensitivities { get; set; }

        public EuropeanOptions(Stocks stock,string iSIN, 
            double strike_price, int timeToExpiration, BullOrBear Kind)
        {

            ISIN = iSIN;
            //RiskFreeRate = riskFreeRate;
            TickerSymbol = stock.TickerSymbol;
            Spot_Price = stock.Pricer();
            Strike_price = strike_price;
            Time_To_Expiration = timeToExpiration;
            Vol = stock.YearlyVolatility;
            kind = Kind;
            Stck = stock;
            sensitivities = Sensitivity();
            delta = Sensitivity()["Delta"];
            gamma = Sensitivity()["Gamma"];


            if (kind == BullOrBear.Call)
            {
                this.comment = "Call option";
            }
            else
            {
                comment = "put option";
            } 
        }
        public enum BullOrBear
        {
            Call,
            Put
        }
        public override double Pricer()
        {
            double d1 = (Math.Log(Spot_Price / Strike_price) + (RiskFreeRate + (Math.Pow(Vol, 2) / 2)) * Time_To_Expiration) / (Vol * Math.Sqrt(Time_To_Expiration));
            double d2 = d1 - Vol * Math.Sqrt(Time_To_Expiration);

            if (kind == BullOrBear.Call) // Call option
            {
                // Cumulative distribution function of the standard normal distribution
                double N_d1 = Normal.CDF(0, 1, (d1));     
                double N_d2 = Normal.CDF(0, 1, (d2)); 

                // Calculate the price of the European call option
                double callOptionPrice = Spot_Price * N_d1 - Strike_price * Math.Exp(-RiskFreeRate * Time_To_Expiration) * N_d2;
                //Console.WriteLine("The price of this option is: {0}", callOptionPrice);
                return callOptionPrice;
            }
            if(kind == BullOrBear.Put)
            {
                // Cumulative distribution function of the standard normal distribution
                double N_d1 = Normal.CDF(0, 1, (-d1));
                double N_d2 = Normal.CDF(0, 1, (-d2)); // Note the change in sign for d2


                // Calculate the price of the European put option
                double putOptionPrice = Strike_price * Math.Exp(-RiskFreeRate * Time_To_Expiration) * N_d2 - Spot_Price * N_d1;

                return putOptionPrice;
            }
            else
            {
                throw new InvalidOperationException("Unsupported option kind");
            }

        }
        public override Dictionary<string, double> Sensitivity()
        {
            double d1 = (Math.Log(Spot_Price / Strike_price) + (RiskFreeRate + Math.Pow(Vol, 2) / 2) * Time_To_Expiration) / (Vol * Math.Sqrt(Time_To_Expiration));
            double d2 = d1 - Vol * Math.Sqrt(Time_To_Expiration);

            if (kind == BullOrBear.Call)
            {
                double delta = Normal.CDF(0, 1, d1);
                double Gamma = Math.Exp(-d1 * d1 / 2) / (Spot_Price * Vol * Math.Pow(Time_To_Expiration, 0.5)) / Math.Sqrt(2 * Math.PI);
                double theta = -Spot_Price * Math.Exp(-d1 * d1 / 2) * Vol / (2 * Math.Sqrt(2 * Math.PI * Time_To_Expiration))
                                  - RiskFreeRate * Strike_price * Math.Exp(-RiskFreeRate * Time_To_Expiration) * Normal.CDF(0, 1, d2);
                double Theta = theta / 365; // Convert to daily theta
                double vega = Spot_Price * Math.Exp(-d1 * d1 / 2) * Math.Sqrt(Time_To_Expiration) / Math.Sqrt(2 * Math.PI);

                Dictionary<string, double> Sensitivity = new Dictionary<string, double>()
                {
                    { "Delta",delta },
                    { "Gamma", Gamma},
                    { "Theta",Theta },
                    { "Vega",vega}

                };

                return Sensitivity;

            }

            if (kind == BullOrBear.Put)
            {
                double delta = Normal.CDF(0, 1, d1) - 1;
                double Gamma = Math.Exp(-d1 * d1 / 2) / (Spot_Price * Vol * Math.Pow(Time_To_Expiration, 0.5)) / Math.Sqrt(2 * Math.PI);

                double theta = -Spot_Price * Math.Exp(-d1 * d1 / 2) * Vol / (2 * Math.Sqrt(2 * Math.PI * Time_To_Expiration))
                              + RiskFreeRate * Strike_price * Math.Exp(-RiskFreeRate * Time_To_Expiration) * Normal.CDF(0, 1, -d2);
                double Theta = theta / 365; // Convert to daily theta
                double vega = Spot_Price * Math.Exp(-d1 * d1 / 2) * Math.Sqrt(Time_To_Expiration) / Math.Sqrt(2 * Math.PI);

                Dictionary<string, double> Sensitivity = new Dictionary<string, double>()
                {
                    { "Delta",delta },
                    { "Gamma", Gamma},
                    { "Theta",Theta },
                    { "Vega",vega}

                };

                return Sensitivity;
            }
            else
            {
                throw new InvalidOperationException("Unsupported option kind");
            }

        }
        public void deltaHedge()
        {
            hedgingOrder hedge = OrderManager.PlaceOrder;
            Console.Write("How many option contracts do you want to hedge? ");
            string input = Console.ReadLine();
            double qtq = Convert.ToDouble(input);
            
            //double qtq = Convert.ToDouble(Console.ReadLine("How many option contracts do you want to hedge?"));
            Order order = new Order(Stck);
           
            order.Symbol = Stck.TickerSymbol;
            order.Quantity = Math.Abs(delta)*qtq;//////////////////////////////////////////////////////////////////////
            order.Status = OrderStatus.Hedging;
            order.pprice = Stck.Pricer();

            if (kind == BullOrBear.Call)
            {
                order.Type = OrderType.Sell;
            }
            else
            {
                order.Type = OrderType.Buy;
            }
            
            //order.GetPrice();
            hedge(order);
            order.Status = OrderStatus.Hedging;
            //order.pprice = Stck.Pricer();

        }



    }
}


















