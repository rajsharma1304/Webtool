using System;

namespace SICT.BusinessLayer.V1
{
    public static class Extensions
    {
        public static double Round(this double Value)
        {
            return System.Math.Round(Value, 2, System.MidpointRounding.AwayFromZero);
        }

        public static string DateFormat(this System.DateTime Date)
        {
            return Date.ToString("MM/dd/yyyy");
        }
    }
}
