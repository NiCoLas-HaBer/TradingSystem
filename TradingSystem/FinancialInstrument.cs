using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystem
{
    public abstract class FinancialInstrument
    {

        public string ISIN { get; set; }
        public string TickerSymbol{  get; set; }

        public double RiskFreeRate = 0.05;
        public virtual string comment {  get; set; }

        public virtual double Pricer()
        {
            return 0;
        }

        public virtual Dictionary<string, double>? Sensitivity()
        {
            return null;
        }




    }
}
