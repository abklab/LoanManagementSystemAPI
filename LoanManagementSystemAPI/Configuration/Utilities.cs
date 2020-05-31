using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proviso_1_2.Configurations
{
    public class Utilities
    {
        public static string GenerateREFID(string momoNumber)
        {          
            string datepart = DateTime.Now.ToString("MMdd-yyyy-hhmmss");
            string momoLast4 = momoNumber.Substring(momoNumber.Length - 4);
            string refid = $"{datepart}-{momoLast4}";
            return refid;
        }
    }
}