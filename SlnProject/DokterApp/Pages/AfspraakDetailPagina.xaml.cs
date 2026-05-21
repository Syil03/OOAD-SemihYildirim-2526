using DokterspraktijkLib.Models;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DokterApp.Pages
{
    public partial class AfspraakDetailPagina : Page
    {
        private Afspraak huidigeAfspraak;
        private Dokter aangemeldeDokter;

        public AfspraakDetailPagina(Afspraak afspraak, Dokter dokter)
        {
            InitializeComponent();
            huidigeAfspraak = afspraak;
            aangemeldeDokter = dokter;
            VulVeldenIn();
        }

        // Vul alle velden in met de gegevens van de afspraak en de patiënt
        private void VulVeldenIn()
        {
            // Gegevens die rechtstreeks uit de afspraak komen
            TxtMoment.Text = huidigeAfspraak.Moment.ToString("dd/MM/yyyy") +
                             " om " + huidigeAfspraak.Moment.ToString("HH:mm");
            TxtPatientNaam.Text = huidigeAfspraak.PatientNaam;
            TxtKlacht.Text = huidigeAfspraak.Klacht;

            try
            {
                // Haal de volledige patiënt op uit de databank voor geslacht en profielfoto
                Patient? patient = Patient.OphalenOpId(huidigeAfspraak.PatientId);
                if (patient != null)
                {
                    TxtGeslacht.Text = GeefGeslachtTekst(patient.Geslacht);
                    ToonProfielfoto(patient);
                }
                else
                {
                    TxtGeslacht.Text = "Onbekend";
                    ToonInitialen();
                }
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het laden van de patiëntgegevens: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
                // Val terug op de initialen wanneer de gegevens niet geladen konden worden
                ToonInitialen();
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

        // Toont de profielfoto van de patiënt als ronde afbeelding; valt anders terug op de initialen
        private void ToonProfielfoto(Patient patient)
        {
            if (patient.ProfielFotoData != null && patient.ProfielFotoData.Length > 0)
            {
                using (MemoryStream stroom = new MemoryStream(patient.ProfielFotoData))
                {
                    BitmapImage afbeelding = new BitmapImage();
                    afbeelding.BeginInit();
                    // Laad de volledige afbeelding meteen in het geheugen zodat de stream gesloten mag worden
                    afbeelding.CacheOption = BitmapCacheOption.OnLoad;
                    afbeelding.StreamSource = stroom;
                    afbeelding.EndInit();
                    ImgPatientFoto.Source = afbeelding;
                }

                // Maak de foto rond: cirkel met middelpunt (60,60) en straal 60 binnen het 120x120-vlak
                ImgPatientFoto.Clip = new EllipseGeometry(new Point(60, 60), 60, 60);

                ImgPatientFoto.Visibility = Visibility.Visible;
                PnlPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                ToonInitialen();
            }
        }

        // Toont de grijze cirkel met de initialen van de patiënt (wanneer er geen foto is)
        private void ToonInitialen()
        {
            TxtInitialen.Text = GeefInitialen(huidigeAfspraak.PatientNaam);
            ImgPatientFoto.Visibility = Visibility.Collapsed;
            PnlPlaceholder.Visibility = Visibility.Visible;
        }

        // Bouwt de initialen op uit de eerste letter van de voor- en achternaam
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
            // Hou maximaal twee letters over: de eerste en de laatste
            if (initialen.Length > 2)
            {
                initialen = initialen.Substring(0, 1) + initialen.Substring(initialen.Length - 1, 1);
            }
            return initialen;
        }

        // Ga terug naar de afsprakenpagina (de lijst herlaadt automatisch)
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            TxtFout.Visibility = Visibility.Collapsed;
            try
            {
                huidigeAfspraak.Verwijderen();
                // Ga terug na verwijdering; de afsprakenpagina herlaadt zichzelf
                NavigationService.GoBack();
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het verwijderen: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }
    }
}
