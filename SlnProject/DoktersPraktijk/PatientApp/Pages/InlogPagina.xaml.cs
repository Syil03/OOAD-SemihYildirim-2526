using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PatientApp.Pages
{
    public partial class InlogPagina : Page
    {
        // Bijhoudt of het wachtwoord momenteel als leesbare tekst getoond wordt
        private bool paswooordZichtbaar = false;

        public InlogPagina()
        {
            InitializeComponent();
        }

        // Enter-toets in e-mail of wachtwoordveld activeert het inloggen
        private void Invoer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Inloggen();
            }
        }

        // Wisselt de zichtbaarheid van het wachtwoord:
        // PasswordBox ↔ TextBox zodat de gebruiker de invoer kan controleren
        private void BtnOogje_Click(object sender, RoutedEventArgs e)
        {
            paswooordZichtbaar = !paswooordZichtbaar;

            if (paswooordZichtbaar)
            {
                // Kopieer de ingevoerde waarde naar de TextBox en toon die
                TxtPaswooordZichtbaar.Text = TxtPaswoord.Password;
                TxtPaswoord.Visibility = Visibility.Collapsed;
                TxtPaswooordZichtbaar.Visibility = Visibility.Visible;
                TxtPaswooordZichtbaar.Focus();
            }
            else
            {
                // Kopieer de ingevoerde waarde terug naar de PasswordBox en verberg de TextBox
                TxtPaswoord.Password = TxtPaswooordZichtbaar.Text;
                TxtPaswooordZichtbaar.Visibility = Visibility.Collapsed;
                TxtPaswoord.Visibility = Visibility.Visible;
                TxtPaswoord.Focus();
            }
        }

        private void BtnInloggen_Click(object sender, RoutedEventArgs e)
        {
            Inloggen();
        }

        private void Inloggen()
        {
            // Verberg vorige foutmelding
            TxtFout.Visibility = Visibility.Collapsed;
            TxtFout.Text = string.Empty;

            string email = TxtEmail.Text.Trim();
            // Haal het wachtwoord op uit het momenteel zichtbare veld
            string paswoord = paswooordZichtbaar ? TxtPaswooordZichtbaar.Text : TxtPaswoord.Password;

            // Invoervalidatie
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(paswoord))
            {
                TxtFout.Text = "Vul uw e-mailadres en wachtwoord in.";
                TxtFout.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                Patient? aangemeldePatient = Patient.Inloggen(email, paswoord);

                if (aangemeldePatient == null)
                {
                    TxtFout.Text = "Ongeldig e-mailadres of wachtwoord.";
                    TxtFout.Visibility = Visibility.Visible;
                    return;
                }

                // Geef de ingelogde patiënt door aan het hoofdvenster zodat de sidebar getoond wordt
                MainWindow hoofdVenster = (MainWindow)Window.GetWindow(this);
                hoofdVenster.ToonSidebar(aangemeldePatient);
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Er is een fout opgetreden: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }
    }
}
