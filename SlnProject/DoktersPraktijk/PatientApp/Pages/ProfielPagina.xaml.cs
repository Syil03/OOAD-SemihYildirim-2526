using DokterspraktijkLib.Models;
using System.Globalization;
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
                // Naam tonen in de profielkaart
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

        // Vult het bewerkformulier met de huidige gegevens en toont de bewerkweergave
        private void BtnBewerken_Click(object sender, RoutedEventArgs e)
        {
            TxtFoutBewerken.Visibility = Visibility.Collapsed;
            TxtBewerkVoornaam.Text = aangemeldePatient.Voornaam;
            TxtBewerkAchternaam.Text = aangemeldePatient.Achternaam;
            TxtBewerkEmail.Text = aangemeldePatient.Email;
            // nchar-veld kan spaties bevatten: trimmen bij het invullen
            TxtBewerkGsm.Text = aangemeldePatient.Gsm.Trim();
            // SelectedIndex stemt overeen met de numerieke DB-waarde (0=Onbekend, 1=Man, 2=Vrouw)
            CmbBewerkGeslacht.SelectedIndex = aangemeldePatient.Geslacht;
            TxtBewerkGeboortedatum.Text = aangemeldePatient.Geboortedatum.ToString("dd/MM/yyyy");
            // SelectedIndex stemt overeen met de Notificatie-enum (0-3)
            CmbBewerkNotificaties.SelectedIndex = (int)aangemeldePatient.Notificaties;

            PnlLeesWeergave.Visibility = Visibility.Collapsed;
            PnlBewerkWeergave.Visibility = Visibility.Visible;
        }

        // Valideert het formulier, schrijft de gegevens naar de databank en keert terug naar de leesweergave
        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            TxtFoutBewerken.Visibility = Visibility.Collapsed;

            string voornaam = TxtBewerkVoornaam.Text.Trim();
            string achternaam = TxtBewerkAchternaam.Text.Trim();
            string email = TxtBewerkEmail.Text.Trim();
            string gsm = TxtBewerkGsm.Text.Trim();

            // --- Validatie ---
            if (voornaam.Length == 0)
            {
                ToonFoutBewerken("Voornaam mag niet leeg zijn.");
                return;
            }
            if (achternaam.Length == 0)
            {
                ToonFoutBewerken("Achternaam mag niet leeg zijn.");
                return;
            }
            if (email.Length == 0)
            {
                ToonFoutBewerken("E-mailadres mag niet leeg zijn.");
                return;
            }
            if (gsm.Length > 10)
            {
                ToonFoutBewerken("Gsm-nummer mag maximaal 10 tekens bevatten.");
                return;
            }
            if (CmbBewerkGeslacht.SelectedIndex < 0)
            {
                ToonFoutBewerken("Selecteer een geslacht.");
                return;
            }
            if (CmbBewerkNotificaties.SelectedIndex < 0)
            {
                ToonFoutBewerken("Selecteer een notificatievoorkeur.");
                return;
            }

            // Geboortedatum parsen via try-catch (geen out-parameter)
            DateTime geboortedatum;
            try
            {
                geboortedatum = DateTime.ParseExact(
                    TxtBewerkGeboortedatum.Text.Trim(),
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture);
            }
            catch
            {
                ToonFoutBewerken("Geboortedatum is ongeldig. Gebruik het formaat dd/MM/yyyy.");
                return;
            }

            if (geboortedatum >= DateTime.Today)
            {
                ToonFoutBewerken("Geboortedatum moet in het verleden liggen.");
                return;
            }

            // Gegevens bijwerken in het patiëntobject
            aangemeldePatient.Voornaam = voornaam;
            aangemeldePatient.Achternaam = achternaam;
            aangemeldePatient.Email = email;
            aangemeldePatient.Gsm = gsm;
            aangemeldePatient.Geslacht = CmbBewerkGeslacht.SelectedIndex;
            aangemeldePatient.Geboortedatum = geboortedatum;
            aangemeldePatient.Notificaties = (Notificatie)CmbBewerkNotificaties.SelectedIndex;

            try
            {
                aangemeldePatient.Opslaan();

                // Ververs de leesweergave met de zojuist opgeslagen gegevens
                VulVeldenIn();

                // Keer terug naar de leesweergave
                PnlBewerkWeergave.Visibility = Visibility.Collapsed;
                PnlLeesWeergave.Visibility = Visibility.Visible;
            }
            catch (Exception fout)
            {
                ToonFoutBewerken("Fout bij het opslaan: " + fout.Message);
            }
        }

        // Annuleert de bewerking en keert terug naar de leesweergave zonder iets op te slaan
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            TxtFoutBewerken.Visibility = Visibility.Collapsed;
            PnlBewerkWeergave.Visibility = Visibility.Collapsed;
            PnlLeesWeergave.Visibility = Visibility.Visible;
        }

        // Toont een validatie- of opslagfout in het bewerkformulier
        private void ToonFoutBewerken(string bericht)
        {
            TxtFoutBewerken.Text = bericht;
            TxtFoutBewerken.Visibility = Visibility.Visible;
        }
    }
}
