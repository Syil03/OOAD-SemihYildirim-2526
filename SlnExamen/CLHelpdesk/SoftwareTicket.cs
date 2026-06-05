namespace CLHelpdesk
{
    public class SoftwareTicket : Ticket
    {
        public string Applicatie { get; set; }

        public override string GeefType()
        {
            return "Software";
        }

        public override string GeefExtraInfo()
        {
            return Applicatie;
        }
    }
}
