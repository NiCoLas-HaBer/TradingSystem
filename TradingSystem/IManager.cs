using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystem
{
    interface IManager
    {
        double Pricer();
        Dictionary<string, double> Sensitivity();
       
    }
}
