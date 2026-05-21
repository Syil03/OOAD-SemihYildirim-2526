using DokterspraktijkLib.Models;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PatientApp.Pages
{
    // Pagina waarmee een ingelogde patiënt een nieuwe afspraak kan boeken
    public partial class AfspraakBoekenPagina : Page
    {
        // De ingelogde patiënt, meegegeven via de constructor
        private Patient aangemeldePatient;

        // Constructor: ontvangt de ingelogde patiënt van de vorige pagina
        public AfspraakBoekenPagina(Patient patient)
        {
            InitializeComponent();
            aangemeldePatient = patient;
            Loaded += AfspraakBoekenPagina_Loaded;
        }

        // Initialiseer de pagina: vul naam, kalender, tijdstippen en dokterslijst in
        private void AfspraakBoekenPagina_Loaded(object sender, RoutedEventArgs e)
        {
            TxtPatientNaam.Text = aangemeldePatient.GeefVolledigeNaam();

            // Blokkeer datums in het verleden en vandaag; enkel toekomstige datums zijn selecteerbaar
            CalDatum.DisplayDateStart = DateTime.Today.AddDays(1);

            VulTijdstippenIn();
            LaadDokters();
        }

        // Vult de ComboBox met tijdstippen van 08:00 tot 17:00 in stappen van 30 minuten
        private void VulTijdstippenIn()
        {
            CmbTijdstip.Items.Clear();
            int uur = 8;
            int minuten = 0;

            while (uur < 17 || (uur == 17 && minuten == 0))
            {
                string tijdstip = uur.ToString("00") + ":" + minuten.ToString("00");
                CmbTijdstip.Items.Add(tijdstip);
                minuten += 30;
                if (minuten == 60)
                {
                    minuten = 0;
                    uur++;
                }
            }
        }

        // Laadt alle dokters uit de databank; het volledige Dokter-object wordt in Tag bewaard
        // zodat het infopaneel later de foto en contactgegevens kan tonen
        private void LaadDokters()
        {
            try
            {
                CmbDokter.Items.Clear();
                List<Dokter> dokters = Dokter.GeefAlleDokters();

                if (dokters.Count == 0)
                {
                    ToonFout("Er zijn momenteel geen dokters beschikbaar.");
                    return;
                }

                for (int i = 0; i < dokters.Count; i++)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = "Dr. " + dokters[i].Voornaam + " " + dokters[i].Achternaam;
                    // Bewaar het volledige object (niet enkel het ID) voor de detailweergave
                    item.Tag = dokters[i];
                    CmbDokter.Items.Add(item);
                }
            }
            catch (Exception fout)
            {
                ToonFout("Fout bij laden van dokters: " + fout.Message);
            }
        }

        // Toont het infopaneel met gegevens van de geselecteerde dokter
        private void CmbDokter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbDokter.SelectedItem == null)
            {
                PnlDokterInfo.Visibility = Visibility.Collapsed;
                return;
            }

            ComboBoxItem geselecteerdItem = (ComboBoxItem)CmbDokter.SelectedItem;
            Dokter dokter = (Dokter)geselecteerdItem.Tag;
            ToonDokterInfo(dokter);
        }

        // Vult het infopaneel met foto (of initialen), naam, e-mail en gsm van de dokter
        private void ToonDokterInfo(Dokter dokter)
        {
            TxtDokterNaam.Text = "Dr. " + dokter.Voornaam + " " + dokter.Achternaam;
            TxtDokterEmail.Text = dokter.Email;
            TxtDokterGsm.Text = dokter.Gsm.Trim();

            if (dokter.ProfielFotoData != null && dokter.ProfielFotoData.Length > 0)
            {
                using (MemoryStream stroom = new MemoryStream(dokter.ProfielFotoData))
                {
                    BitmapImage afbeelding = new BitmapImage();
                    afbeelding.BeginInit();
                    // Laad meteen in het geheugen zodat de stream veilig gesloten kan worden
                    afbeelding.CacheOption = BitmapCacheOption.OnLoad;
                    afbeelding.StreamSource = stroom;
                    afbeelding.EndInit();
                    ImgDokterFoto.Source = afbeelding;
                }
                // Knip de afbeelding bij tot een cirkel: middelpunt (32,32), straal 32
                ImgDokterFoto.Clip = new EllipseGeometry(new System.Windows.Point(32, 32), 32, 32);
                ImgDokterFoto.Visibility = Visibility.Visible;
                PnlDokterInitialen.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Geen foto: toon de donkerblauwe cirkel met initialen
                TxtDokterInitialen.Text = GeefInitialen(dokter.Voornaam + " " + dokter.Achternaam);
                ImgDokterFoto.Visibility = Visibility.Collapsed;
                PnlDokterInitialen.Visibility = Visibility.Visible;
            }

            PnlDokterInfo.Visibility = Visibility.Visible;
        }

        // Bouwt twee initialen op uit de voor- en achternaam (eerste letter van elk deel)
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

        // Valideert het formulier en slaat de afspraak op in de databank
        private void BtnBevestigen_Click(object sender, RoutedEventArgs e)
        {
            TxtFout.Visibility = Visibility.Collapsed;

            // --- Validatie 1: dokter geselecteerd ---
            if (CmbDokter.SelectedIndex < 0)
            {
                ToonFout("Gelieve een dokter te selecteren.");
                return;
            }

            // --- Validatie 2: datum geselecteerd ---
            if (CalDatum.SelectedDate == null)
            {
                ToonFout("Gelieve een datum te kiezen.");
                return;
            }

            DateTime gekozenDatum = CalDatum.SelectedDate.Value;

            // --- Validatie 3: datum moet in de toekomst liggen ---
            if (gekozenDatum <= DateTime.Today)
            {
                ToonFout("De gekozen datum moet in de toekomst liggen. Kies minstens morgen.");
                return;
            }

            // --- Validatie 4: tijdstip geselecteerd ---
            if (CmbTijdstip.SelectedIndex < 0)
            {
                ToonFout("Gelieve een tijdstip te kiezen.");
                return;
            }

            // --- Validatie 5: klacht niet leeg ---
            string klacht = TxtKlacht.Text.Trim();
            if (klacht.Length == 0)
            {
                ToonFout("Gelieve een reden van bezoek of klacht in te vullen.");
                return;
            }

            // Combineer de gekozen datum met het gekozen tijdstip tot één DateTime-waarde
            string gekozenTijdstip = (string)CmbTijdstip.SelectedItem;
            string[] tijdDelen = gekozenTijdstip.Split(':');
            int uur = int.Parse(tijdDelen[0]);
            int minuten = int.Parse(tijdDelen[1]);
            DateTime moment = new DateTime(gekozenDatum.Year, gekozenDatum.Month, gekozenDatum.Day, uur, minuten, 0);

            // Lees het Dokter-object op uit de Tag van het geselecteerde ComboBoxItem
            ComboBoxItem geselecteerdDokterItem = (ComboBoxItem)CmbDokter.SelectedItem;
            Dokter geselecteerdeDokter = (Dokter)geselecteerdDokterItem.Tag;

            try
            {
                Afspraak.AfspraakToevoegen(aangemeldePatient.Id, geselecteerdeDokter.Id, moment, klacht);
                NavigationService.GoBack();
            }
            catch (Exception fout)
            {
                ToonFout("Fout bij het boeken van de afspraak: " + fout.Message);
            }
        }

        // Navigeert terug naar de vorige pagina bij klikken op de terugknop
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        // Toont een foutmelding in rood in het TxtFout TextBlock
        private void ToonFout(string bericht)
        {
            TxtFout.Text = bericht;
            TxtFout.Foreground = new SolidColorBrush(Color.FromRgb(211, 47, 47));
            TxtFout.Visibility = Visibility.Visible;
        }
    }
}
