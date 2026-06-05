namespace CLHelpdesk
{
    public class HardwareTicket : Ticket
    {
        public string Toestel { get; set; }

        public override string GeefType()
        {
            return "Hardware";
        }

        public override string GeefExtraInfo()
        {
            return Toestel;
        }
    }
}
