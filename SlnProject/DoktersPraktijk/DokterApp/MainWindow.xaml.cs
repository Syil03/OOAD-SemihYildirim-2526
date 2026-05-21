using DokterspraktijkLib.Models;
using System.Windows;

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
            SidebarBorder.Visibility = Visibility.Visible;
            frameInhoud.Navigate(new Pages.AfsprakenPagina(_aangemeldeDokter));
        }

        // Toont de afsprakenpagina van de ingelogde dokter
        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            frameInhoud.Navigate(new Pages.AfsprakenPagina(_aangemeldeDokter));
        }

        // Toont de patiëntenlijst van de ingelogde dokter
        private void BtnPatienten_Click(object sender, RoutedEventArgs e)
        {
            frameInhoud.Navigate(new Pages.PatientenPagina(_aangemeldeDokter));
        }

        // Logt de dokter uit: verbergt de sidebar en keert terug naar de inlogpagina
        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            _aangemeldeDokter = null;
            TxtDokterNaam.Text = string.Empty;
            SidebarBorder.Visibility = Visibility.Collapsed;
            frameInhoud.Navigate(new Pages.InlogPagina());
        }
    }
}
