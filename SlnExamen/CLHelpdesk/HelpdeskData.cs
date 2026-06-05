using System;
using System.Collections.Generic;
using System.IO;

namespace CLHelpdesk
{
    public class HelpdeskData
    {
        private string _csvPad;
        private List<Ticket> _tickets;

        public HelpdeskData(string csvPad)
        {
            _csvPad = csvPad;
            _tickets = new List<Ticket>();
            LeesUitCsv();
        }

        private void LeesUitCsv()
        {
            _tickets = new List<Ticket>();

            if (!File.Exists(_csvPad))
            {
                return;
            }

            string[] lijnen = File.ReadAllLines(_csvPad);

            foreach (string lijn in lijnen)
            {
                if (string.IsNullOrWhiteSpace(lijn))
                {
                    continue;
                }

                string[] delen = lijn.Split(';');

                if (delen.Length < 7)
                {
                    continue;
                }

                int id = int.Parse(delen[0]);
                string type = delen[1];
                string beschrijving = delen[2];
                string indiener = delen[3];
                string status = delen[4];
                DateTime datum = DateTime.Parse(delen[5]);
                string extraInfo = delen[6];

                if (type == "Hardware")
                {
                    HardwareTicket ticket = new HardwareTicket();
                    ticket.Id = id;
                    ticket.Beschrijving = beschrijving;
                    ticket.Indiener = indiener;
                    ticket.Status = status;
                    ticket.Datum = datum;
                    ticket.Toestel = extraInfo;
                    _tickets.Add(ticket);
                }
                else if (type == "Software")
                {
                    SoftwareTicket ticket = new SoftwareTicket();
                    ticket.Id = id;
                    ticket.Beschrijving = beschrijving;
                    ticket.Indiener = indiener;
                    ticket.Status = status;
                    ticket.Datum = datum;
                    ticket.Applicatie = extraInfo;
                    _tickets.Add(ticket);
                }
            }
        }

        private void SchrijfNaarCsv()
        {
            List<string> lijnen = new List<string>();

            foreach (Ticket ticket in _tickets)
            {
                string lijn = ticket.Id + ";" +
                              ticket.GeefType() + ";" +
                              ticket.Beschrijving + ";" +
                              ticket.Indiener + ";" +
                              ticket.Status + ";" +
                              ticket.Datum.ToString("yyyy-MM-dd") + ";" +
                              ticket.GeefExtraInfo();
                lijnen.Add(lijn);
            }

            File.WriteAllLines(_csvPad, lijnen);
        }

        public List<Ticket> GeefAlleTickets()
        {
            return _tickets;
        }

        public void VoegTicketToe(Ticket nieuwTicket)
        {
            int maxId = 0;

            foreach (Ticket ticket in _tickets)
            {
                if (ticket.Id > maxId)
                {
                    maxId = ticket.Id;
                }
            }

            nieuwTicket.Id = maxId + 1;
            nieuwTicket.Status = "Open";
            nieuwTicket.Datum = DateTime.Now;
            _tickets.Add(nieuwTicket);
            SchrijfNaarCsv();
        }

        public void SluitTicketAf(int id)
        {
            Ticket gevondenTicket = null;

            foreach (Ticket ticket in _tickets)
            {
                if (ticket.Id == id)
                {
                    gevondenTicket = ticket;
                    break;
                }
            }

            if (gevondenTicket == null)
            {
                throw new Exception("Ticket met id " + id + " niet gevonden.");
            }

            gevondenTicket.Status = "Afgesloten";
            SchrijfNaarCsv();
        }
    }
}
