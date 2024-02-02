using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystem
{
    public class Stocks : FinancialInstrument
    {
        public string Name { get; set; }
        //public string TickerSymbol { get; set; }
        public string ISIN { get; set; }
        protected double Price { get; set; }
        public double Yearly_Return { get; set; }
        public double YearlyVolatility { get; set; }
        public override string comment { get; set; }


        public Stocks(string tickerSymbol)
        {
            tickerSymbol = tickerSymbol.ToUpper();
            if (StockDic.TryGetValue(tickerSymbol, out var stockInfo))
            {
                TickerSymbol = tickerSymbol;
                Name = StockDic[tickerSymbol][0];
                ISIN = StockDic[tickerSymbol][1];
                Price = double.Parse(StockDic[tickerSymbol][2]);
                Yearly_Return = double.Parse(StockDic[tickerSymbol][3]);
                YearlyVolatility = double.Parse(StockDic[tickerSymbol][4]);
                comment = "stock of";


            }
            else
            {
                Console.WriteLine("This ticker symbol doesn't exist in our database. Try to integrated it using the .Add(Dictionary<string, string[]>) method");
            }

        }

        public override double Pricer()
        {
            return Price;   
        }
        static Dictionary<string, string[]> StockDic = new Dictionary<string, string[]>()
        {
            {"AAPL", new string[]{"Apple Inc.", "US0378331005", "150.25", "0.05", "0.15" } },
            {"GOOGL", new string[]{"Alphabet Inc.", "US02079K3059", "2800.00", "0.02", "0.10" } },
            {"MSFT", new string[]{"Microsoft Corporation", "US5949181045", "300.50", "0.04", "0.12" } },
            {"AMZN", new string[]{"Amazon.com Inc.", "US0231351067", "3400.75", "0.03", "0.14" } },
            {"TSLA", new string[]{"Tesla Inc.", "US88160R1014", "950.00", "0.08", "0.20" } }

        };

        public static void Add(string key, string[] values)
        {
            {
                
                if (!key.Any(char.IsDigit) && values.Length==5)
                {
                    
                    StockDic.Add(key.ToUpper(), values);
                }
                else
                {
                    Console.WriteLine("Please you have to enter a key value that doesn't contain any digit, and the number of elements in the array should be equal to 5");
                }

            }
        }
    }
}