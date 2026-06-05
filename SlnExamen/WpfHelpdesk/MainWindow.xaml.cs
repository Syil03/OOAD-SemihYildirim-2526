using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CLHelpdesk;

namespace WpfHelpdesk
{
    public partial class MainWindow : Window
    {
        private HelpdeskData _helpdeskData;
        private string _csvPad;

        public MainWindow()
        {
            InitializeComponent();

            _csvPad = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "helpdesk_tickets.csv");
            _helpdeskData = new HelpdeskData(_csvPad);

            CbxType.Items.Add("Hardware");
            CbxType.Items.Add("Software");
            CbxType.SelectedIndex = 0;

            LaadTickets();
        }

        private void LaadTickets()
        {
            LstTickets.Items.Clear();
            List<Ticket> tickets = _helpdeskData.GeefAlleTickets();

            foreach (Ticket ticket in tickets)
            {
                LstTickets.Items.Add(ticket.ToString());
            }
        }

        private void CbxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LblExtraInfo == null)
            {
                return;
            }

            if (CbxType.SelectedItem != null && CbxType.SelectedItem.ToString() == "Hardware")
            {
                LblExtraInfo.Content = "Toestel:";
            }
            else
            {
                LblExtraInfo.Content = "Applicatie:";
            }
        }

        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBeschrijving.Text))
            {
                MessageBox.Show("Gelieve een beschrijving in te geven.", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtIndiener.Text))
            {
                MessageBox.Show("Gelieve een indiener in te geven.", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CbxType.SelectedItem == null)
            {
                MessageBox.Show("Gelieve een type te selecteren.", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtExtraInfo.Text))
            {
                MessageBox.Show("Gelieve extra informatie in te geven.", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Ticket nieuwTicket;

            if (CbxType.SelectedItem.ToString() == "Hardware")
            {
                HardwareTicket hardwareTicket = new HardwareTicket();
                hardwareTicket.Beschrijving = TxtBeschrijving.Text;
                hardwareTicket.Indiener = TxtIndiener.Text;
                hardwareTicket.Toestel = TxtExtraInfo.Text;
                nieuwTicket = hardwareTicket;
            }
            else
            {
                SoftwareTicket softwareTicket = new SoftwareTicket();
                softwareTicket.Beschrijving = TxtBeschrijving.Text;
                softwareTicket.Indiener = TxtIndiener.Text;
                softwareTicket.Applicatie = TxtExtraInfo.Text;
                nieuwTicket = softwareTicket;
            }

            _helpdeskData.VoegTicketToe(nieuwTicket);

            TxtBeschrijving.Text = string.Empty;
            TxtIndiener.Text = string.Empty;
            TxtExtraInfo.Text = string.Empty;
            CbxType.SelectedIndex = 0;

            LaadTickets();

            MessageBox.Show("Ticket succesvol toegevoegd.", "Succes",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAfsluiten_Click(object sender, RoutedEventArgs e)
        {
            if (LstTickets.SelectedItem == null)
            {
                return;
            }

            MessageBoxResult resultaat = MessageBox.Show(
                "Weet u zeker dat u dit ticket wilt afsluiten?",
                "Bevestiging",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultaat == MessageBoxResult.Yes)
            {
                try
                {
                    int geselecteerdeIndex = LstTickets.SelectedIndex;
                    List<Ticket> tickets = _helpdeskData.GeefAlleTickets();
                    Ticket geselecteerdTicket = tickets[geselecteerdeIndex];
                    int id = geselecteerdTicket.Id;

                    _helpdeskData.SluitTicketAf(id);
                    LaadTickets();

                    MessageBox.Show("Ticket succesvol afgesloten.", "Succes",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij afsluiten van ticket: " + ex.Message, "Fout",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
