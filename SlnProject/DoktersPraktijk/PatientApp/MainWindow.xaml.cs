using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Shapes;

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
            TxtSidebarInitialen.Text = GeefInitialen(patient.Voornaam, patient.Achternaam);
            SidebarBorder.Visibility = Visibility.Visible;
            ZetActieveKnop(IndAfspraken);
            frameInhoud.Navigate(new Pages.AfsprakenPagina(_aangemeldePatient));
        }

        // Toont de afsprakenpagina van de ingelogde patiënt
        private void BtnAfspraken_Click(object sender, RoutedEventArgs e)
        {
            ZetActieveKnop(IndAfspraken);
            frameInhoud.Navigate(new Pages.AfsprakenPagina(_aangemeldePatient));
        }

        // Toont de profielpagina van de ingelogde patiënt
        private void BtnProfiel_Click(object sender, RoutedEventArgs e)
        {
            ZetActieveKnop(IndProfiel);
            frameInhoud.Navigate(new Pages.ProfielPagina(_aangemeldePatient));
        }

        // Logt de patiënt uit: verbergt de sidebar en keert terug naar de inlogpagina
        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            _aangemeldePatient = null;
            TxtPatientNaam.Text = string.Empty;
            TxtSidebarInitialen.Text = string.Empty;
            SidebarBorder.Visibility = Visibility.Collapsed;
            frameInhoud.Navigate(new Pages.InlogPagina());
        }

        // Verbergt alle actieve indicatoren en toont enkel de opgegeven indicator
        private void ZetActieveKnop(Rectangle actieveIndicator)
        {
            IndAfspraken.Visibility = Visibility.Collapsed;
            IndProfiel.Visibility = Visibility.Collapsed;
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
