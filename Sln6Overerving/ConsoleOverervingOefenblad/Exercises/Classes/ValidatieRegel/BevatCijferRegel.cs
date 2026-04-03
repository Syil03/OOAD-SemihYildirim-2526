using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleOverervingOefenblad.Exercises.Classes.ValidatieRegel
{
    public class BevatCijferRegel : ValidatieRegel
    {
        public override string FoutBoodschap => "Waarde moet minstens één cijfer bevatten.";

        public override bool IsGeldig(string waarde)
        {
            return waarde.Any(char.IsDigit);
        }
    }
}
