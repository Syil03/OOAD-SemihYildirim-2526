using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PatientApp.Pages
{
    public partial class AfsprakenPagina : Page
    {
        private Patient aangemeldePatient;

        // Volledige lijst van afspraken; wordt gefilterd op basis van de radioknop
        private List<Afspraak> alleAfspraken;

        // De momenteel geselecteerde afspraak in de ListBox
        private Afspraak geselecteerdeAfspraak;

        public AfsprakenPagina(Patient patient)
        {
            InitializeComponent();
            aangemeldePatient = patient;
            alleAfspraken = new List<Afspraak>();
            // Loaded vuurt ook af bij terugkeer naar deze pagina, zodat de lijst telkens vernieuwt
            Loaded += AfsprakenPagina_Loaded;
        }

        // Herlaad de afspraken en ververs de lijst bij elke weergave van de pagina
        private void AfsprakenPagina_Loaded(object sender, RoutedEventArgs e)
        {
            LaadAlleAfspraken();
            ToonGefilterdeLijst();
        }

        // Haalt alle afspraken van de ingelogde patiënt op uit de databank
        private void LaadAlleAfspraken()
        {
            TxtFout.Visibility = Visibility.Collapsed;
            try
            {
                alleAfspraken = Afspraak.GeefAfsprakenVanPatient(aangemeldePatient.Id);
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het laden van afspraken: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
                alleAfspraken = new List<Afspraak>();
            }
        }

        // Filtert alleAfspraken op basis van de actieve radioknop en vult de ListBox
        private void ToonGefilterdeLijst()
        {
            LstAfspraken.Items.Clear();
            geselecteerdeAfspraak = null;
            BtnAnnuleren.IsEnabled = false;

            bool alleenToekomstig = RdoToekomstige.IsChecked == true;

            // Filter via een for-lus (geen LINQ)
            List<Afspraak> gefilterd = new List<Afspraak>();
            for (int i = 0; i < alleAfspraken.Count; i++)
            {
                if (!alleenToekomstig || alleAfspraken[i].Moment > DateTime.Now)
                {
                    gefilterd.Add(alleAfspraken[i]);
                }
            }

            if (gefilterd.Count == 0)
            {
                TxtGeenAfspraken.Text = alleenToekomstig
                    ? "U heeft geen toekomstige afspraken."
                    : "U heeft nog geen afspraken.";
                TxtGeenAfspraken.Visibility = Visibility.Visible;
                LstAfspraken.Visibility = Visibility.Collapsed;
                return;
            }

            TxtGeenAfspraken.Visibility = Visibility.Collapsed;
            LstAfspraken.Visibility = Visibility.Visible;

            for (int i = 0; i < gefilterd.Count; i++)
            {
                MaakAfspraakItem(gefilterd[i]);
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
            kaart.Margin = new Thickness(0, 0, 0, 10);
            kaart.Padding = new Thickness(18, 14, 18, 14);

            // Bovenste rij: dokternaam links (vet) en datum/tijdstip rechts
            Grid inhoud = new Grid();
            inhoud.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            inhoud.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Grid bovenste = new Grid();
            bovenste.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            bovenste.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            bovenste.Margin = new Thickness(0, 0, 0, 6);

            // Doktersnaam links in vetgedrukt
            TextBlock txtDokter = new TextBlock();
            txtDokter.Text = "Dr. " + afspraak.DokterNaam;
            txtDokter.FontSize = 15;
            txtDokter.FontWeight = FontWeights.Bold;
            txtDokter.Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            txtDokter.VerticalAlignment = VerticalAlignment.Center;

            // Datum en tijdstip rechts, netjes uitgelijnd
            StackPanel datumTijdPanel = new StackPanel();
            datumTijdPanel.HorizontalAlignment = HorizontalAlignment.Right;

            TextBlock txtDatum = new TextBlock();
            txtDatum.Text = afspraak.Moment.ToString("dd/MM/yyyy");
            txtDatum.FontSize = 13;
            txtDatum.FontWeight = FontWeights.SemiBold;
            txtDatum.Foreground = new SolidColorBrush(Color.FromRgb(55, 71, 79));
            txtDatum.TextAlignment = TextAlignment.Right;

            TextBlock txtTijd = new TextBlock();
            txtTijd.Text = afspraak.Moment.ToString("HH:mm");
            txtTijd.FontSize = 13;
            txtTijd.Foreground = new SolidColorBrush(Color.FromRgb(100, 121, 133));
            txtTijd.TextAlignment = TextAlignment.Right;

            datumTijdPanel.Children.Add(txtDatum);
            datumTijdPanel.Children.Add(txtTijd);

            Grid.SetColumn(txtDokter, 0);
            Grid.SetColumn(datumTijdPanel, 1);
            bovenste.Children.Add(txtDokter);
            bovenste.Children.Add(datumTijdPanel);

            // Reden van consultatie onderaan in grijs
            TextBlock txtKlacht = new TextBlock();
            txtKlacht.Text = afspraak.Klacht;
            txtKlacht.FontSize = 13;
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

        // Schakel de annulerknop in als en alleen als de geselecteerde afspraak in de toekomst ligt
        private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstAfspraken.SelectedItem == null)
            {
                BtnAnnuleren.IsEnabled = false;
                geselecteerdeAfspraak = null;
                return;
            }

            ListBoxItem geselecteerdItem = (ListBoxItem)LstAfspraken.SelectedItem;
            geselecteerdeAfspraak = (Afspraak)geselecteerdItem.Tag;

            if (geselecteerdeAfspraak.Moment > DateTime.Now)
            {
                BtnAnnuleren.IsEnabled = true;
            }
            else
            {
                BtnAnnuleren.IsEnabled = false;
            }
        }

        // Verwijdert de geselecteerde afspraak uit de databank en herlaadt de lijst
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            if (geselecteerdeAfspraak == null) return;

            TxtFout.Visibility = Visibility.Collapsed;
            try
            {
                geselecteerdeAfspraak.Verwijderen();
                LaadAlleAfspraken();
                ToonGefilterdeLijst();
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het annuleren van de afspraak: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        // Herfilter de lijst wanneer een andere radioknop geselecteerd wordt
        private void RdoFilter_Checked(object sender, RoutedEventArgs e)
        {
            // alleAfspraken is null zolang de pagina nog niet volledig geladen is
            if (alleAfspraken == null) return;
            ToonGefilterdeLijst();
        }

        // Navigeer naar de afspraak-boekenpagina en geef de ingelogde patiënt mee
        private void BtnNieuweAfspraak_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AfspraakBoekenPagina(aangemeldePatient));
        }

        // De ListBox onderschept MouseWheel-events standaard zelf, ook als interne scroll uit staat.
        // Markeer het event als afgehandeld en stuur het handmatig door naar de bovenliggende ScrollViewer.
        private void LstAfspraken_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            MouseWheelEventArgs doorsturenArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            doorsturenArgs.RoutedEvent = UIElement.MouseWheelEvent;
            doorsturenArgs.Source = sender;
            ScrAfspraken.RaiseEvent(doorsturenArgs);
        }
    }
}
