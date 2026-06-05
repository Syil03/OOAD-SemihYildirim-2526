using System;

namespace CLHelpdesk
{
    public abstract class Ticket
    {
        public int Id { get; set; }
        public string Beschrijving { get; set; }
        public string Indiener { get; set; }
        public string Status { get; set; }
        public DateTime Datum { get; set; }

        public abstract string GeefType();
        public abstract string GeefExtraInfo();

        public override string ToString()
        {
            return string.Format("[{0}] {1} | {2} | {3} | Status: {4} | {5}",
                Id, GeefType(), Beschrijving, Indiener, Status, GeefExtraInfo());
        }
    }
}
