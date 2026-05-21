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

            // Voeg per afspraak een kaartje toe met patiëntnaam, tijdstip en klacht
            for (int i = 0; i < gefilterd.Count; i++)
            {
                MaakAfspraakItem(gefilterd[i]);
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

        // Bouwt een klikbaar ListBoxItem voor één afspraak; het Afspraak-object zit in Tag
        private void MaakAfspraakItem(Afspraak afspraak)
        {
            ListBoxItem item = new ListBoxItem();
            item.Tag = afspraak;
            item.Padding = new Thickness(0);
            // Zorg dat het item de volledige breedte van de ListBox inneemt
            item.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            Border kaart = new Border();
            kaart.Background = Brushes.White;
            kaart.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 225, 230));
            kaart.BorderThickness = new Thickness(1);
            kaart.CornerRadius = new CornerRadius(6);
            kaart.Margin = new Thickness(0, 0, 0, 8);
            kaart.Padding = new Thickness(16, 12, 16, 12);

            // Twee rijen: bovenste rij (naam + tijdstip), onderste rij (klacht in grijs)
            Grid inhoud = new Grid();
            inhoud.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            inhoud.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Bovenste rij: patiëntnaam links (vet) en tijdstip rechts
            Grid bovenste = new Grid();
            bovenste.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            bovenste.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            bovenste.Margin = new Thickness(0, 0, 0, 6);

            TextBlock txtNaam = new TextBlock();
            txtNaam.Text = afspraak.PatientNaam;
            txtNaam.FontSize = 14;
            txtNaam.FontWeight = FontWeights.Bold;
            txtNaam.Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            txtNaam.VerticalAlignment = VerticalAlignment.Center;
            txtNaam.TextTrimming = TextTrimming.CharacterEllipsis;

            TextBlock txtTijd = new TextBlock();
            txtTijd.Text = afspraak.Moment.ToString("HH:mm");
            txtTijd.FontSize = 14;
            txtTijd.FontWeight = FontWeights.SemiBold;
            txtTijd.Foreground = new SolidColorBrush(Color.FromRgb(27, 42, 74));
            txtTijd.VerticalAlignment = VerticalAlignment.Center;
            txtTijd.Margin = new Thickness(12, 0, 0, 0);

            Grid.SetColumn(txtNaam, 0);
            Grid.SetColumn(txtTijd, 1);
            bovenste.Children.Add(txtNaam);
            bovenste.Children.Add(txtTijd);

            // Onderste rij: reden van consultatie in grijs
            TextBlock txtKlacht = new TextBlock();
            txtKlacht.Text = afspraak.Klacht;
            txtKlacht.FontSize = 12;
            txtKlacht.Foreground = new SolidColorBrush(Color.FromRgb(120, 144, 156));
            txtKlacht.TextWrapping = TextWrapping.Wrap;

            Grid.SetRow(bovenste, 0);
            Grid.SetRow(txtKlacht, 1);
            inhoud.Children.Add(bovenste);
            inhoud.Children.Add(txtKlacht);

            kaart.Child = inhoud;
            item.Content = kaart;
            LstAfspraken.Items.Add(item);
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
