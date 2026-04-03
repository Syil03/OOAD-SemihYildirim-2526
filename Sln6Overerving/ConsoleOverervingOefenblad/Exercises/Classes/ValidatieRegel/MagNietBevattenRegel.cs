using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleOverervingOefenblad.Exercises.Classes.ValidatieRegel
{
    public class MagNietBevattenRegel : ValidatieRegel
    {
        private List<string> _verbodenWoorden;

        public MagNietBevattenRegel(List<string> verbodenWoorden)
        {
            _verbodenWoorden = verbodenWoorden;
        }

        public override string FoutBoodschap => $"Waarde mag geen van de volgende woorden bevatten: {string.Join(", ", _verbodenWoorden)}.";

        public override bool IsGeldig(string waarde)
        {
            foreach (var verboden in _verbodenWoorden)
            {
                if (waarde.Contains(verboden, StringComparison.OrdinalIgnoreCase)) return false;
            }
            return true;
        }
    }
}
