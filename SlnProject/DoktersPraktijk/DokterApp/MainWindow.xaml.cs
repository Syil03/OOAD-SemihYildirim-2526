using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Shapes;

namespace DokterApp
{
    public partial class MainWindow : Window
    {
        // De ingelogde dokter wordt bijgehouden om door te geven aan pagina's
        private Dokter _aangemeldeDokter;

        public MainWindow()
        {
            InitializeComponent();
            // Start de applicatie op de inlogpagina
            frameInhoud.Navigate(new Pages.InlogPagina());
        }

        // Wordt aangeroepen vanuit InlogPagina na een succesvolle aanmelding.
        // Toont de sidebar met de dokternaam en navigeert naar de afsprakenpagina.
        public void ToonSidebar(Dokter dokter)
        {
            _aangemeldeDokter = dokter;
            TxtDokterNaam.Text = "Dr. " + dokter.Voornaam + " " + dokter.Achternaam;
            TxtSidebarInitialen.Text = GeefInitialen(dokter.Voornaam, dokter.Achternaam);
            SidebarBorder.Visibility = Visibility.Visible;
            ZetActieveKnop(IndAfspraken);
            frameInhoud.Navigate(new Pages.AfsprakenPagina(_aangemeldeDokter));
        }

        // Toont de afsprakenpagina van de ingelogde dokter
        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            ZetActieveKnop(IndAfspraken);
            frameInhoud.Navigate(new Pages.AfsprakenPagina(_aangemeldeDokter));
        }

        // Toont de patiëntenlijst van de ingelogde dokter
        private void BtnPatienten_Click(object sender, RoutedEventArgs e)
        {
            ZetActieveKnop(IndPatienten);
            frameInhoud.Navigate(new Pages.PatientenPagina(_aangemeldeDokter));
        }

        // Logt de dokter uit: verbergt de sidebar en keert terug naar de inlogpagina
        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            _aangemeldeDokter = null;
            TxtDokterNaam.Text = string.Empty;
            TxtSidebarInitialen.Text = string.Empty;
            SidebarBorder.Visibility = Visibility.Collapsed;
            frameInhoud.Navigate(new Pages.InlogPagina());
        }

        // Verbergt alle actieve indicatoren en toont enkel de opgegeven indicator
        private void ZetActieveKnop(Rectangle actieveIndicator)
        {
            IndAfspraken.Visibility = Visibility.Collapsed;
            IndPatienten.Visibility = Visibility.Collapsed;
            actieveIndicator.Visibility = Visibility.Visible;
        }

        // Haalt de eerste letter van voor- en achternaam op als initialen
        private string GeefInitialen(string voornaam, string achternaam)
        {
            string initialen = string.Empty;
            if (voornaam.Length > 0)
            {
                initialen += char.ToUpper(voornaam[0]);
            }
            if (achternaam.Length > 0)
            {
                initialen += char.ToUpper(achternaam[0]);
            }
            return initialen;
        }
    }
}
