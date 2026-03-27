using System;

namespace ConsoleStaticEnumOefenblad.Exercises.Classes
{
    internal static class TekstAnalyse
    {
        private static string[] verbodenWoorden = new string[] { "delete", "drop", "truncate" };
        private static char[] verbodenKarakters = new char[] { '!', '@', '#', '$', '%' };

        public static int AantalWoorden(string tekst)
        {
            if (string.IsNullOrWhiteSpace(tekst))
            {
                return 0;
            }
            return tekst.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static bool BevatVerbodenWoord(string tekst)
        {
            if (string.IsNullOrWhiteSpace(tekst)) return false;

            foreach (string woord in verbodenWoorden)
            {
                if (tekst.Contains(woord, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool BevatVerbodenKarakter(string tekst)
        {
            if (string.IsNullOrWhiteSpace(tekst)) return false;

            foreach (char karakter in verbodenKarakters)
            {
                if (tekst.Contains(karakter))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsGeschiktVoorTitel(string tekst)
        {
            if (string.IsNullOrWhiteSpace(tekst)) return false;

            if (tekst.Length < 5 || tekst.Length > 30)
            {
                return false;
            }

            if (BevatVerbodenWoord(tekst) || BevatVerbodenKarakter(tekst))
            {
                return false;
            }

            return true;
        }
    }
}