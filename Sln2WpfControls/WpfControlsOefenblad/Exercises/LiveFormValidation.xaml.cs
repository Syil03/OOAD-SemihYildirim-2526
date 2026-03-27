using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Live Form Validation", Description = "TextChanged gebruiken voor live validatie en IsEnabled.", Order = 4, IsVisible = true)]
    public partial class LiveFormValidation : Page
    {
        public LiveFormValidation()
        {
            InitializeComponent();
        }

        private void txtPaswoord_TextChanged(object sender, TextChangedEventArgs e)
        {
            string password = txtPaswoord.Text;

            if (string.IsNullOrWhiteSpace(password))
            {
                txtStatus.Text = "...";
                txtStatus.Foreground = Brushes.Black;
                btnSave.IsEnabled = false;
                return;
            }

            bool isLangGenoeg = password.Length >= 8;
            bool heeftHoofdletter = password.Any(char.IsUpper);
            bool heeftCijfer = password.Any(char.IsDigit);

            if (isLangGenoeg && heeftHoofdletter && heeftCijfer)
            {
                txtStatus.Text = "Geldig paswoord";
                txtStatus.Foreground = Brushes.DarkGreen;
                btnSave.IsEnabled = true;
            }
            else
            {
                string foutmelding = "Ongeldig paswoord:\n";

                if (!isLangGenoeg)
                    foutmelding += "Minstens 8 tekens vereist.\n";
                if (!heeftHoofdletter)
                    foutmelding += "Minstens één hoofdletter vereist.\n";
                if (!heeftCijfer)
                    foutmelding += "Minstens één cijfer vereist.\n";

                txtStatus.Text = foutmelding.TrimEnd();
                txtStatus.Foreground = Brushes.Red;
                btnSave.IsEnabled = false;
            }
        }
    }
}
