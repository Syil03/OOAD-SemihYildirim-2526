using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Controls;

namespace PatientApp
{
    public partial class MainWindow : Window
    {
        // De ingelogde patiënt wordt bijgehouden om door te geven aan pagina's
        private Patient _aangemeldePatient;

        public MainWindow()
        {
            InitializeComponent();
            // Start de applicatie op de inlogpagina
            frameInhoud.Navigate(new Pages.InlogPagina());
        }

        // Wordt aangeroepen vanuit InlogPagina na een succesvolle aanmelding.
        // Toont de sidebar met de patiëntnaam en navigeert naar de afsprakenpagina.
        public void ToonSidebar(Patient patient)
        {
            _aangemeldePatient = patient;
            TxtPatientNaam.Text = patient.Voornaam + " " + patient.Achternaam;
            SidebarBorder.Visibility = Visibility.Visible;
            frameInhoud.Navigate(new Pages.AfsprakenPagina(_aangemeldePatient));
        }

        // Toont de afsprakenpagina van de ingelogde patiënt
        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            frameInhoud.Navigate(new Pages.AfsprakenPagina(_aangemeldePatient));
        }

        // Toont de profielpagina van de ingelogde patiënt
        private void BtnProfiel_Click(object sender, RoutedEventArgs e)
        {
            frameInhoud.Navigate(new Pages.ProfielPagina(_aangemeldePatient));
        }

        // Logt de patiënt uit: verbergt de sidebar en keert terug naar de inlogpagina
        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            _aangemeldePatient = null;
            TxtPatientNaam.Text = string.Empty;
            SidebarBorder.Visibility = Visibility.Collapsed;
            frameInhoud.Navigate(new Pages.InlogPagina());
        }
    }
}
