using DokterspraktijkLib.Models;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DokterApp.Pages
{
    // Toont de gegevens van één patiënt en laat de dokter die ook bewerken
    public partial class PatientDetailPagina : Page
    {
        private Patient huidigePatiënt;
        private Dokter aangemeldeDokter;

        // bewerkModus = true opent meteen het bewerkformulier
        public PatientDetailPagina(Patient patient, Dokter dokter, bool bewerkModus)
        {
            InitializeComponent();
            huidigePatiënt = patient;
            aangemeldeDokter = dokter;

            VulLeesVeldenIn();

            if (bewerkModus)
            {
                VulBewerkVeldenIn();
                PnlLeesWeergave.Visibility = Visibility.Collapsed;
                PnlBewerkWeergave.Visibility = Visibility.Visible;
            }
        }

        // Vult de leesweergave met de actuele gegevens van de patiënt
        private void VulLeesVeldenIn()
        {
            try
            {
                TxtVolledigeNaam.Text = huidigePatiënt.GeefVolledigeNaam();
                TxtGeslacht.Text = GeefGeslachtTekst(huidigePatiënt.Geslacht);
                TxtGeboortedatum.Text = huidigePatiënt.Geboortedatum.ToString("dd/MM/yyyy");
                TxtEmail.Text = huidigePatiënt.Email;
                TxtGsm.Text = huidigePatiënt.Gsm.Trim();
                TxtNotificaties.Text = GeefNotificatieTekst(huidigePatiënt.Notificaties);
                TxtPatientId.Text = huidigePatiënt.Id.ToString();
                TxtInitialen.Text = GeefInitialen(huidigePatiënt.GeefVolledigeNaam());

                ToonProfielfoto();
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het laden van de patiëntgegevens: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        // Toont de profielfoto als ronde afbeelding; valt terug op de initialen als er geen foto is
        private void ToonProfielfoto()
        {
            if (huidigePatiënt.ProfielFotoData != null && huidigePatiënt.ProfielFotoData.Length > 0)
            {
                using (MemoryStream stroom = new MemoryStream(huidigePatiënt.ProfielFotoData))
                {
                    BitmapImage afbeelding = new BitmapImage();
                    afbeelding.BeginInit();
                    afbeelding.CacheOption = BitmapCacheOption.OnLoad;
                    afbeelding.StreamSource = stroom;
                    afbeelding.EndInit();
                    ImgProfielfoto.Source = afbeelding;
                }
                // Knip de afbeelding bij tot een cirkel: middelpunt (60,60), straal 60
                ImgProfielfoto.Clip = new EllipseGeometry(new Point(60, 60), 60, 60);
                ImgProfielfoto.Visibility = Visibility.Visible;
                PnlInitialen.Visibility = Visibility.Collapsed;
            }
            else
            {
                ImgProfielfoto.Visibility = Visibility.Collapsed;
                PnlInitialen.Visibility = Visibility.Visible;
            }
        }

        // Vult het bewerkformulier met de huidige gegevens van de patiënt
        private void VulBewerkVeldenIn()
        {
            TxtFoutBewerken.Visibility = Visibility.Collapsed;
            TxtBewerkVoornaam.Text = huidigePatiënt.Voornaam;
            TxtBewerkAchternaam.Text = huidigePatiënt.Achternaam;
            TxtBewerkEmail.Text = huidigePatiënt.Email;
            TxtBewerkGsm.Text = huidigePatiënt.Gsm.Trim();
            // SelectedIndex stemt overeen met de numerieke DB-waarde (0=Man, 1=Vrouw)
            CmbBewerkGeslacht.SelectedIndex = huidigePatiënt.Geslacht;
            TxtBewerkGeboortedatum.Text = huidigePatiënt.Geboortedatum.ToString("dd/MM/yyyy");
            // SelectedIndex stemt overeen met de Notificatie-enum (0-3)
            CmbBewerkNotificaties.SelectedIndex = (int)huidigePatiënt.Notificaties;
        }

        // Opent de bewerkweergave en vult de velden met de huidige gegevens
        private void BtnBewerken_Click(object sender, RoutedEventArgs e)
        {
            VulBewerkVeldenIn();
            PnlLeesWeergave.Visibility = Visibility.Collapsed;
            PnlBewerkWeergave.Visibility = Visibility.Visible;
        }

        // Valideert het formulier, slaat de gegevens op en keert terug naar de leesweergave
        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            TxtFoutBewerken.Visibility = Visibility.Collapsed;

            string voornaam = TxtBewerkVoornaam.Text.Trim();
            string achternaam = TxtBewerkAchternaam.Text.Trim();
            string email = TxtBewerkEmail.Text.Trim();
            string gsm = TxtBewerkGsm.Text.Trim();

            // Validatie
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
            huidigePatiënt.Voornaam = voornaam;
            huidigePatiënt.Achternaam = achternaam;
            huidigePatiënt.Email = email;
            huidigePatiënt.Gsm = gsm;
            huidigePatiënt.Geslacht = CmbBewerkGeslacht.SelectedIndex;
            huidigePatiënt.Geboortedatum = geboortedatum;
            huidigePatiënt.Notificaties = (Notificatie)CmbBewerkNotificaties.SelectedIndex;

            try
            {
                huidigePatiënt.Opslaan();

                // Ververs de leesweergave met de zojuist opgeslagen gegevens
                VulLeesVeldenIn();

                PnlBewerkWeergave.Visibility = Visibility.Collapsed;
                PnlLeesWeergave.Visibility = Visibility.Visible;
            }
            catch (Exception fout)
            {
                ToonFoutBewerken("Fout bij het opslaan: " + fout.Message);
            }
        }

        // Annuleert de bewerking en keert terug naar de leesweergave zonder op te slaan
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            TxtFoutBewerken.Visibility = Visibility.Collapsed;
            PnlBewerkWeergave.Visibility = Visibility.Collapsed;
            PnlLeesWeergave.Visibility = Visibility.Visible;
        }

        // Gaat terug naar de patiëntenlijst
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        // Toont een validatie- of opslagfout in het bewerkformulier
        private void ToonFoutBewerken(string bericht)
        {
            TxtFoutBewerken.Text = bericht;
            TxtFoutBewerken.Visibility = Visibility.Visible;
        }

        // Zet de numerieke geslachtswaarde om naar leesbare tekst (0=Man, 1=Vrouw)
        private string GeefGeslachtTekst(int geslacht)
        {
            if (geslacht == 0) return "Man";
            if (geslacht == 1) return "Vrouw";
            return "Onbekend";
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

        // Bouwt twee initialen op uit de voor- en achternaam
        private string GeefInitialen(string volledigeNaam)
        {
            string initialen = string.Empty;
            string[] delen = volledigeNaam.Trim().Split(' ');
            for (int i = 0; i < delen.Length; i++)
            {
                if (delen[i].Length > 0)
                {
                    initialen += char.ToUpper(delen[i][0]);
                }
            }
            if (initialen.Length > 2)
            {
                initialen = initialen.Substring(0, 1) + initialen.Substring(initialen.Length - 1, 1);
            }
            return initialen;
        }
    }
}
