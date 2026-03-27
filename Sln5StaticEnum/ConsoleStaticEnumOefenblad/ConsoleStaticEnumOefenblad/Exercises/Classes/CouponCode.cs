using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleStaticEnumOefenblad.Exercises.Classes
{
    internal class CouponCode
    {
        private static string _couponRegex = @"^[A-Z]{3}\d{2}-[A-Z]{2}$";

        public string Code { get; private set; }

        public bool IsGeldig
        {
            get
            {
                return System.Text.RegularExpressions.Regex.IsMatch(Code, _couponRegex);
            }
        }

        public CouponCode(string code)
        {
            Code = code;
        }

        public static bool ControleerCode(string code)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(code, _couponRegex);
        }

        public static string Beschrijf(string code)
        {
            if (ControleerCode(code))
            {
                string prefix = code.Substring(0, 3);
                string nummer = code.Substring(3, 2);
                return $"Prefix={prefix}, Nummer={nummer}";
            }
            else
            {
                return "ongeldige code";
            }
        }


    }
}
