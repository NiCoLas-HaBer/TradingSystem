using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystem
{
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
        public string comment { get; set; }


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
                if (this.Status == OrderStatus.Hedging)
                {
                    this.Price = value;
                }
            }
        }
    }
}
