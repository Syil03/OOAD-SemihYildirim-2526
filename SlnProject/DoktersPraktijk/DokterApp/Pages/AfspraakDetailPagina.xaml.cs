using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Controls;

namespace DokterApp.Pages
{
    public partial class AfspraakDetailPagina : Page
    {
        private Afspraak huidigeAfspraak;
        private Dokter aangemeldeDokter;

        public AfspraakDetailPagina(Afspraak afspraak, Dokter dokter)
        {
            InitializeComponent();
            huidigeAfspraak = afspraak;
            aangemeldeDokter = dokter;
            VulVeldenIn();
        }

        // Vul alle velden in met de gegevens van de afspraak
        private void VulVeldenIn()
        {
            TxtMoment.Text = huidigeAfspraak.Moment.ToString("dd/MM/yyyy") +
                             " om " + huidigeAfspraak.Moment.ToString("HH:mm");
            TxtPatientNaam.Text = huidigeAfspraak.PatientNaam;
            TxtKlacht.Text = huidigeAfspraak.Klacht;
        }

        // Ga terug naar de afsprakenpagina (OnNavigatedTo herlaadt de lijst automatisch)
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            TxtFout.Visibility = Visibility.Collapsed;
            try
            {
                huidigeAfspraak.Verwijderen();
                // Ga terug na verwijdering; de afsprakenpagina herlaadt zichzelf
                NavigationService.GoBack();
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het verwijderen: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }
    }
}
