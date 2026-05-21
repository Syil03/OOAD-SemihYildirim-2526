using DokterspraktijkLib.Models;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DokterApp.Pages
{
    public partial class AfsprakenPagina : Page
    {
        private Dokter aangemeldeDokter;

        // Volledige lijst van afspraken; wordt gefilterd per geselecteerde datum
        private List<Afspraak> alleAfspraken;

        // De momenteel geselecteerde afspraak in de ListBox
        private Afspraak geselecteerdeAfspraak;

        public AfsprakenPagina(Dokter dokter)
        {
            InitializeComponent();
            aangemeldeDokter = dokter;
            alleAfspraken = new List<Afspraak>();
            // Loaded vuurt ook af bij terugkeer naar deze pagina
            Loaded += AfsprakenPagina_Loaded;
        }

        // Initialiseer de pagina: laad de foto, haal alle afspraken op en selecteer vandaag
        private void AfsprakenPagina_Loaded(object sender, RoutedEventArgs e)
        {
            ToonProfielfoto();
            LaadAlleAfspraken();
            // Het instellen van SelectedDate activeert SelectedDatesChanged en vult de lijst
            Kalender.SelectedDate = DateTime.Today;
        }

        // Toont de profielfoto van de dokter als cirkel; valt terug op initialen als er geen foto is
        private void ToonProfielfoto()
        {
            TxtDokterNaamFoto.Text = "Dr. " + aangemeldeDokter.Voornaam + " " + aangemeldeDokter.Achternaam;

            if (aangemeldeDokter.ProfielFotoData != null && aangemeldeDokter.ProfielFotoData.Length > 0)
            {
                using (MemoryStream stroom = new MemoryStream(aangemeldeDokter.ProfielFotoData))
                {
                    BitmapImage afbeelding = new BitmapImage();
                    afbeelding.BeginInit();
                    // Laad meteen in het geheugen zodat de stream veilig gesloten kan worden
                    afbeelding.CacheOption = BitmapCacheOption.OnLoad;
                    afbeelding.StreamSource = stroom;
                    afbeelding.EndInit();
                    ImgProfielfoto.Source = afbeelding;
                }

                // Knip de afbeelding bij tot een cirkel: middelpunt (60,60), straal 60
                ImgProfielfoto.Clip = new EllipseGeometry(new Point(60, 60), 60, 60);
                ImgProfielfoto.Visibility = Visibility.Visible;
                PnlFotoPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Geen foto: toon de grijze cirkel met de initialen van de dokter
                TxtInitialen.Text = GeefInitialen(aangemeldeDokter.Voornaam + " " + aangemeldeDokter.Achternaam);
                ImgProfielfoto.Visibility = Visibility.Collapsed;
                PnlFotoPlaceholder.Visibility = Visibility.Visible;
            }
        }

        // Haalt alle afspraken van de ingelogde dokter op uit de databank
        private void LaadAlleAfspraken()
        {
            TxtFout.Visibility = Visibility.Collapsed;
            try
            {
                alleAfspraken = Afspraak.GeefAfsprakenVanDokter(aangemeldeDokter.Id);
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het laden van afspraken: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
                alleAfspraken = new List<Afspraak>();
            }
        }

        // Filtert alleAfspraken op de opgegeven datum en vult de ListBox
        private void LaadAfsprakenVoorDatum(DateTime datum)
        {
            LstAfspraken.Items.Clear();
            PnlDetail.Visibility = Visibility.Collapsed;
            geselecteerdeAfspraak = null;

            // Datum weergeven in leesbaar Nederlands formaat
            TxtGeselecteerdeDatum.Text = datum.ToString("dddd d MMMM yyyy", new CultureInfo("nl-BE"));

            // Filter via een for-lus (geen LINQ)
            List<Afspraak> gefilterd = new List<Afspraak>();
            for (int i = 0; i < alleAfspraken.Count; i++)
            {
                if (alleAfspraken[i].Moment.Date == datum.Date)
                {
                    gefilterd.Add(alleAfspraken[i]);
                }
            }

            if (gefilterd.Count == 0)
            {
                TxtGeenAfspraken.Visibility = Visibility.Visible;
                LstAfspraken.Visibility = Visibility.Collapsed;
                return;
            }

            TxtGeenAfspraken.Visibility = Visibility.Collapsed;
            LstAfspraken.Visibility = Visibility.Visible;

            // Voeg per afspraak een ListBoxItem toe met tijdstip en patiëntnaam
            for (int i = 0; i < gefilterd.Count; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Tag = gefilterd[i];
                item.Padding = new Thickness(12, 10, 12, 10);

                // Twee kolommen: tijdstip links, naam rechts
                Grid rij = new Grid();
                rij.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(56) });
                rij.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock txtTijd = new TextBlock();
                txtTijd.Text = gefilterd[i].Moment.ToString("HH:mm");
                txtTijd.FontSize = 14;
                txtTijd.FontWeight = FontWeights.SemiBold;
                txtTijd.Foreground = new SolidColorBrush(Color.FromRgb(27, 42, 74));
                txtTijd.VerticalAlignment = VerticalAlignment.Center;

                TextBlock txtNaam = new TextBlock();
                txtNaam.Text = gefilterd[i].PatientNaam;
                txtNaam.FontSize = 14;
                txtNaam.Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33));
                txtNaam.VerticalAlignment = VerticalAlignment.Center;
                txtNaam.TextTrimming = TextTrimming.CharacterEllipsis;

                Grid.SetColumn(txtTijd, 0);
                Grid.SetColumn(txtNaam, 1);
                rij.Children.Add(txtTijd);
                rij.Children.Add(txtNaam);

                item.Content = rij;
                LstAfspraken.Items.Add(item);
            }
        }

        // Datum geselecteerd in de kalender: ververs de afsprakenlijst
        private void Kalender_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Kalender.SelectedDate != null)
            {
                LaadAfsprakenVoorDatum(Kalender.SelectedDate.Value);
            }
        }

        // Item geselecteerd in de ListBox: toon de klacht en pas de knopstatus aan
        private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SelectedItem == null is betrouwbaarder dan SelectedIndex < 0 bij programmatisch
            // toegevoegde ListBoxItems: SelectedIndex kan transiënt -1 zijn tijdens opbouw
            if (LstAfspraken.SelectedItem == null)
            {
                PnlDetail.Visibility = Visibility.Collapsed;
                BtnAnnuleren.IsEnabled = false;
                geselecteerdeAfspraak = null;
                return;
            }

            ListBoxItem geselecteerdItem = (ListBoxItem)LstAfspraken.SelectedItem;
            geselecteerdeAfspraak = (Afspraak)geselecteerdItem.Tag;

            TxtKlacht.Text = geselecteerdeAfspraak.Klacht;

            // Knop expliciet in- of uitschakelen op basis van het tijdstip van de afspraak
            if (geselecteerdeAfspraak.Moment > DateTime.Now)
            {
                BtnAnnuleren.IsEnabled = true;
            }
            else
            {
                BtnAnnuleren.IsEnabled = false;
            }

            PnlDetail.Visibility = Visibility.Visible;
        }

        // Verwijdert de geselecteerde afspraak uit de databank en herlaadt de lijst
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            if (geselecteerdeAfspraak == null) return;

            TxtFout.Visibility = Visibility.Collapsed;

            try
            {
                geselecteerdeAfspraak.Verwijderen();

                // Herlaad alle afspraken en toon de huidige dag opnieuw
                LaadAlleAfspraken();
                if (Kalender.SelectedDate != null)
                {
                    LaadAfsprakenVoorDatum(Kalender.SelectedDate.Value);
                }
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het annuleren van de afspraak: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
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
    }
}
