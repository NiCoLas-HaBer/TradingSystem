using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystem
{
    internal class Bonds : FinancialInstrument
    {
        public string Name { get; set; }
        public double FaceValue { get; set; }
        public double? CouponRate { get; set; }
        public int YearsToMaturity {  get; set; }
        public double YieldToMaturity { get; set; }
        public ZeroCouponOrCoupon Kind { get; set; }
        public override string comment { get; set; }
        


        //Include the ISIN

        public enum ZeroCouponOrCoupon
        {
            CouponBond,
            ZeroCouponBond
        }

        public Bonds(ZeroCouponOrCoupon kind, string name,string tickersymbol, double facevalue,int yearstomaturity, double yieldtomaturity,double? Couponrate = null)
        {
            Name = name;
            TickerSymbol = tickersymbol;
            YearsToMaturity = yearstomaturity;
            FaceValue = facevalue;
            YieldToMaturity = yieldtomaturity;
            Kind = kind;
            if(kind == ZeroCouponOrCoupon.CouponBond)
            {
                CouponRate = Couponrate;
                comment = "Bonds";
            }
            if(kind == ZeroCouponOrCoupon.ZeroCouponBond)
            {
                if(Couponrate != null)
                {
                    Console.WriteLine("You can't assign a coupon rate to a zero coupon bond. The object was created with a null coupon rate value");
                }
                CouponRate = null ;
                comment = "Zero-coupon-Bonds";
                Console.WriteLine("This is a zero coupon bond");

            }

        }

        public override double Pricer()
        {
            if(Kind == ZeroCouponOrCoupon.CouponBond)
            {
                
                    double totalCouponPayments = 0;

                    // Calculate total present value of coupon payments
                    for (int year = 1; year <= YearsToMaturity; year++)
                    {
                        double couponPayment = FaceValue * (double)CouponRate;
                        double presentValue = couponPayment / Math.Pow(1 + YieldToMaturity, year);
                        totalCouponPayments += presentValue;
                    }

                    // Calculate present value of face value payment at maturity
                    double presentValueOfFaceValue = FaceValue / Math.Pow(1 + YieldToMaturity, YearsToMaturity);

                    // Total present value is the sum of coupon payments and face value payment
                    double totalPresentValue = totalCouponPayments + presentValueOfFaceValue;

                    return totalPresentValue;
                

            }

            if (Kind == ZeroCouponOrCoupon.ZeroCouponBond)
            {

                // Calculate present value of face value payment at maturity
                double presentValueOfFaceValue = FaceValue / Math.Pow(1 + YieldToMaturity, YearsToMaturity);

                return presentValueOfFaceValue;

            }
            else
            {
                return 0;
            }
        }

        public override Dictionary<string, double>? Sensitivity()
        {
            if (Kind == ZeroCouponOrCoupon.CouponBond)
            {
                double modifiedDuration = 0;

                for (int year = 1; year <= YearsToMaturity; year++)
                {
                    double couponPayment = FaceValue * (double)CouponRate;
                    double presentValue = couponPayment / Math.Pow(1 + YieldToMaturity, year);
                    modifiedDuration += year * presentValue;
                }

                // Add the contribution of the face value payment at maturity
                double presentValueOfFaceValue = FaceValue / Math.Pow(1 + YieldToMaturity, YearsToMaturity);
                modifiedDuration += YearsToMaturity * presentValueOfFaceValue;

                // Calculate the total present value
                double totalPresentValue = Pricer();

                // Calculate the modified duration
                modifiedDuration /= totalPresentValue;
                Dictionary<string, double> result = new Dictionary<string, double> { { "Duration", modifiedDuration } };


                return result;
            }
            else
            {
                // For zero-coupon bonds, the modified duration is equal to the time to maturity
                Dictionary<string, double> result = new Dictionary<string, double> { { "Duration", YearsToMaturity } };
                return result;
            }
        }


    }
}
