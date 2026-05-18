using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace DokterApp.Pages
{
    public partial class InlogPagina : Page
    {
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
            string paswoord = TxtPaswoord.Password;

            // Invoervalidatie
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(paswoord))
            {
                TxtFout.Text = "Vul uw e-mailadres en wachtwoord in.";
                TxtFout.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                Dokter? aangemeldeDokter = Dokter.Inloggen(email, paswoord);

                if (aangemeldeDokter == null)
                {
                    TxtFout.Text = "Ongeldig e-mailadres of wachtwoord.";
                    TxtFout.Visibility = Visibility.Visible;
                    return;
                }

                // Navigeer naar de afsprakenpagina en verwijder de inlogpagina uit de geschiedenis
                NavigationService.Navigate(new AfsprakenPagina(aangemeldeDokter));
                NavigationService.RemoveBackEntry();
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Er is een fout opgetreden: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }
    }
}
