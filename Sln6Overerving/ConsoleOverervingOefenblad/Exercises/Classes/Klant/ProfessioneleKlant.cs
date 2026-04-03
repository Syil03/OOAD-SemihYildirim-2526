using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleOverervingOefenblad.Exercises.Classes.Klant
{
    public class ProfessioneleKlant : Klant
    {
        public string BedrijfsNaam { get; set; }
        public string BtwNummer { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} - {BedrijfsNaam} (BTW: {BtwNummer})";
        }
    }
}
