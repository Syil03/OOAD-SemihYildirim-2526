using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PatientApp.Pages
{
    // Pagina waarmee een ingelogde patiënt een nieuwe afspraak kan boeken
    public partial class AfspraakBoekenPagina : Page
    {
        // De ingelogde patiënt, meegegeven via de constructor
        private Patient aangemeldePatient;

        // Timer die na 2 seconden terug navigeert bij een succesvolle boeking
        private DispatcherTimer terugTimer;

        // Constructor: ontvangt de ingelogde patiënt van de vorige pagina
        public AfspraakBoekenPagina(Patient patient)
        {
            InitializeComponent();
            aangemeldePatient = patient;
            terugTimer = new DispatcherTimer();
            // Stel de timer in op 2 seconden; bij het afgaan navigeren we terug
            terugTimer.Interval = TimeSpan.FromSeconds(2);
            terugTimer.Tick += TerugNaVertraging;
            Loaded += AfspraakBoekenPagina_Loaded;
        }

        // Wordt uitgevoerd zodra de pagina geladen is
        private void AfspraakBoekenPagina_Loaded(object sender, RoutedEventArgs e)
        {
            // Toon de naam van de ingelogde patiënt in de bovenbalk
            TxtPatientNaam.Text = aangemeldePatient.GeefVolledigeNaam();

            // Blokkeer datums in het verleden en vandaag; enkel toekomstige datums zijn selecteerbaar
            CalDatum.DisplayDateStart = DateTime.Today.AddDays(1);

            // Vul de tijdstippen en de dokterslijst in
            VulTijdstippenIn();
            LaadDokters();
        }

        // Vult de ComboBox met tijdstippen van 08:00 tot 17:00 in stappen van 30 minuten
        private void VulTijdstippenIn()
        {
            CmbTijdstip.Items.Clear();
            int uur = 8;
            int minuten = 0;

            // Blijf tijdstippen toevoegen tot we 17:00 bereikt hebben
            while (uur < 17 || (uur == 17 && minuten == 0))
            {
                // Formatteer als "HH:MM" met voorloopnul
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

        // Laadt alle dokters uit de databank en toont ze als ComboBoxItem in de keuzelijst
        // Het dokter-ID wordt opgeslagen in de Tag-eigenschap van elk item
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

                // Maak per dokter een ComboBoxItem aan met zichtbare naam en verborgen ID in Tag
                foreach (Dokter d in dokters)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = "Dr. " + d.Voornaam + " " + d.Achternaam;
                    item.Tag = d.Id;
                    CmbDokter.Items.Add(item);
                }
            }
            catch (Exception fout)
            {
                ToonFout("Fout bij laden van dokters: " + fout.Message);
            }
        }

        // Verwerkt de klik op "Afspraak bevestigen": valideert invoer en slaat de afspraak op
        private void BtnBevestigen_Click(object sender, RoutedEventArgs e)
        {
            // Verberg eventuele vorige foutmeldingen
            TxtFout.Visibility = Visibility.Collapsed;

            // --- Validatie 1: datum geselecteerd ---
            if (CalDatum.SelectedDate == null)
            {
                ToonFout("Gelieve een datum te kiezen.");
                return;
            }

            DateTime gekozenDatum = CalDatum.SelectedDate.Value;

            // --- Validatie 2: datum moet in de toekomst liggen ---
            if (gekozenDatum <= DateTime.Today)
            {
                ToonFout("De gekozen datum moet in de toekomst liggen. Kies minstens morgen.");
                return;
            }

            // --- Validatie 3: tijdstip geselecteerd ---
            if (CmbTijdstip.SelectedIndex < 0)
            {
                ToonFout("Gelieve een tijdstip te kiezen.");
                return;
            }

            // --- Validatie 4: klacht niet leeg ---
            string klacht = TxtKlacht.Text.Trim();
            if (klacht.Length == 0)
            {
                ToonFout("Gelieve een reden van bezoek of klacht in te vullen.");
                return;
            }

            // --- Validatie 5: dokter geselecteerd ---
            if (CmbDokter.SelectedIndex < 0)
            {
                ToonFout("Gelieve een dokter te selecteren.");
                return;
            }

            // Combineer de gekozen datum met het gekozen tijdstip tot één DateTime-waarde
            string gekozenTijdstip = (string)CmbTijdstip.SelectedItem;
            string[] tijdDelen = gekozenTijdstip.Split(':');
            int uur = int.Parse(tijdDelen[0]);
            int minuten = int.Parse(tijdDelen[1]);
            DateTime moment = new DateTime(gekozenDatum.Year, gekozenDatum.Month, gekozenDatum.Day, uur, minuten, 0);

            // Lees het dokter-ID op uit de Tag van het geselecteerde ComboBoxItem
            ComboBoxItem geselecteerdItem = (ComboBoxItem)CmbDokter.SelectedItem;
            int dokterId = (int)geselecteerdItem.Tag;

            try
            {
                // Sla de afspraak op via de library-methode
                Afspraak.AfspraakToevoegen(aangemeldePatient.Id, dokterId, moment, klacht);

                // Toon succesmelding in het groen
                ToonSucces("Afspraak succesvol geboekt! U wordt teruggestuurd naar uw overzicht...");

                // Deactiveer de bevestigingsknop zodat er niet twee keer wordt ingediend
                BtnBevestigen.IsEnabled = false;

                // Start de terugkeertimer: na 2 seconden navigeren we automatisch terug
                terugTimer.Start();
            }
            catch (Exception fout)
            {
                ToonFout("Fout bij het boeken van de afspraak: " + fout.Message);
            }
        }

        // Wordt aangeroepen door de DispatcherTimer na 2 seconden: navigeert terug naar de afsprakenpagina
        private void TerugNaVertraging(object? sender, EventArgs e)
        {
            terugTimer.Stop();
            NavigationService.GoBack();
        }

        // Navigeert terug naar de vorige pagina bij klikken op de terugknop
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            // Stop de terugkeertimer indien die nog loopt na een succesvolle boeking
            terugTimer.Stop();
            NavigationService.GoBack();
        }

        // Toont een foutmelding in rood in het TxtFout TextBlock
        private void ToonFout(string bericht)
        {
            TxtFout.Text = bericht;
            TxtFout.Foreground = new SolidColorBrush(Color.FromRgb(211, 47, 47));
            TxtFout.Visibility = Visibility.Visible;
        }

        // Toont een succesmelding in groen in het TxtFout TextBlock
        private void ToonSucces(string bericht)
        {
            TxtFout.Text = bericht;
            TxtFout.Foreground = new SolidColorBrush(Color.FromRgb(46, 125, 50));
            TxtFout.Visibility = Visibility.Visible;
        }
    }
}
