using DokterspraktijkLib.Models;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PatientApp.Pages
{
    public partial class ProfielPagina : Page
    {
        private Patient aangemeldePatient;

        public ProfielPagina(Patient patient)
        {
            InitializeComponent();
            aangemeldePatient = patient;
            VulVeldenIn();
        }

        // Vult alle velden in met de gegevens van de ingelogde patiënt
        private void VulVeldenIn()
        {
            try
            {
                // Naam zowel in de sidebar als als titel van de kaart tonen
                TxtSidebarNaam.Text = aangemeldePatient.GeefVolledigeNaam();
                TxtVolledigeNaam.Text = aangemeldePatient.GeefVolledigeNaam();
                TxtEmail.Text = aangemeldePatient.Email;
                TxtGsm.Text = aangemeldePatient.Gsm;
                TxtGeslacht.Text = GeefGeslachtTekst(aangemeldePatient.Geslacht);
                TxtGeboortedatum.Text = aangemeldePatient.Geboortedatum.ToString("dd/MM/yyyy");
                TxtNotificaties.Text = GeefNotificatieTekst(aangemeldePatient.Notificaties);

                ToonProfielfoto();
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het tonen van het profiel: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        // Zet de byte[] uit de databank om naar een afbeelding; toont anders een melding
        private void ToonProfielfoto()
        {
            if (aangemeldePatient.ProfielFotoData != null && aangemeldePatient.ProfielFotoData.Length > 0)
            {
                using (MemoryStream stroom = new MemoryStream(aangemeldePatient.ProfielFotoData))
                {
                    BitmapImage afbeelding = new BitmapImage();
                    afbeelding.BeginInit();
                    // Laad de volledige afbeelding meteen in het geheugen zodat de stream gesloten mag worden
                    afbeelding.CacheOption = BitmapCacheOption.OnLoad;
                    afbeelding.StreamSource = stroom;
                    afbeelding.EndInit();
                    ImgProfielfoto.Source = afbeelding;
                }
                TxtGeenFoto.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Geen foto in de databank: toon een tekstmelding in plaats van de afbeelding
                ImgProfielfoto.Source = null;
                TxtGeenFoto.Visibility = Visibility.Visible;
            }
        }

        // Zet de numerieke geslachtswaarde om naar leesbare tekst
        // In de databank geldt: 1 = Man, 2 = Vrouw, alle andere waarden = Onbekend
        private string GeefGeslachtTekst(int geslacht)
        {
            if (geslacht == 1)
            {
                return "Man";
            }
            else if (geslacht == 2)
            {
                return "Vrouw";
            }
            else
            {
                return "Onbekend";
            }
        }

        // Geeft een leesbare omschrijving van de notificatievoorkeur
        private string GeefNotificatieTekst(Notificatie notificatie)
        {
            switch (notificatie)
            {
                case Notificatie.Mail:
                    return "Via e-mail";
                case Notificatie.Sms:
                    return "Via sms";
                case Notificatie.Beide:
                    return "Via e-mail en sms";
                default:
                    return "Geen notificaties";
            }
        }

        // Keer terug naar de afsprakenpagina
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
