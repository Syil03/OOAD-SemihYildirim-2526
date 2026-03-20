using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleKlassenOefenblad.Exercises.Classes
{
    internal class Werknemer
    {
        public int Id { get; set; }

        public string Naam { get; set; }
        public decimal Salaris { get; set; }
        public DateOnly InDienstSinds { get; set; }
        public int Ancienniteit => DateOnly.FromDateTime(DateTime.Now).Year - InDienstSinds.Year;
        public string Seniority
        {
            get
            {
                if (Ancienniteit < 2)
                    return "Junior";
                else if (Ancienniteit < 5)
                    return "Medior";
                else
                    return "Senior";
            }
        }
        public void GeefOpslag(double percentage)
        {
            decimal factor = (decimal)(1 + (percentage / 100));
            Salaris *= factor;
        }
    }
}
