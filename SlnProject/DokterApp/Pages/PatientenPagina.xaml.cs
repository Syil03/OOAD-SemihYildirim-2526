using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DokterApp.Pages
{
    // Pagina met het overzicht van patiënten die minstens één afspraak hebben bij de ingelogde dokter
    public partial class PatientenPagina : Page
    {
        private Dokter aangemeldeDokter;

        public PatientenPagina(Dokter dokter)
        {
            InitializeComponent();
            aangemeldeDokter = dokter;
            // Loaded vuurt bij eerste weergave én bij terugkeer vanuit een detail-pagina,
            // zodat de lijst na opslaan, wijzigen of verwijderen altijd vernieuwd is
            Loaded += PatientenPagina_Loaded;
        }

        private void PatientenPagina_Loaded(object sender, RoutedEventArgs e)
        {
            LaadAllePatient();
        }

        // Haalt alle patiënten van de ingelogde dokter op en toont ze als kaarten
        private void LaadAllePatient()
        {
            PnlPatienten.Children.Clear();
            TxtFout.Visibility = Visibility.Collapsed;

            try
            {
                List<Patient> patienten = Patient.GeefAllePatientenVanDokter(aangemeldeDokter.Id);

                if (patienten.Count == 0)
                {
                    TextBlock geenData = new TextBlock();
                    geenData.Text = "U heeft nog geen patiënten.";
                    geenData.FontSize = 14;
                    geenData.Foreground = new SolidColorBrush(Color.FromRgb(144, 164, 174));
                    geenData.Margin = new Thickness(0, 40, 0, 0);
                    PnlPatienten.Children.Add(geenData);
                    return;
                }

                for (int i = 0; i < patienten.Count; i++)
                {
                    MaakPatientKaart(patienten[i]);
                }
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het laden van patiënten: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        // Zoekt patiënten op naam via de zoekterm en toont de resultaten als kaarten
        private void Zoeken()
        {
            string zoekterm = TxtZoeken.Text.Trim();

            // Lege zoekterm: toon alle patiënten van de dokter
            if (zoekterm.Length == 0)
            {
                LaadAllePatient();
                return;
            }

            PnlPatienten.Children.Clear();
            TxtFout.Visibility = Visibility.Collapsed;

            try
            {
                List<Patient> patienten = Patient.Zoeken(zoekterm);

                if (patienten.Count == 0)
                {
                    TextBlock geenData = new TextBlock();
                    geenData.Text = "Geen patiënten gevonden voor \"" + zoekterm + "\".";
                    geenData.FontSize = 14;
                    geenData.Foreground = new SolidColorBrush(Color.FromRgb(144, 164, 174));
                    geenData.Margin = new Thickness(0, 40, 0, 0);
                    PnlPatienten.Children.Add(geenData);
                    return;
                }

                for (int i = 0; i < patienten.Count; i++)
                {
                    MaakPatientKaart(patienten[i]);
                }
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het zoeken: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        // Bouwt een kaart (280 px breed) voor één patiënt met contactgegevens en drie actieknoppen
        private void MaakPatientKaart(Patient patient)
        {
            Border kaart = new Border();
            kaart.Width = 280;
            kaart.Background = Brushes.White;
            kaart.BorderBrush = new SolidColorBrush(Color.FromRgb(207, 216, 220));
            kaart.BorderThickness = new Thickness(1);
            kaart.CornerRadius = new CornerRadius(6);
            kaart.Margin = new Thickness(0, 0, 14, 14);
            kaart.Padding = new Thickness(16, 14, 16, 14);

            StackPanel inhoud = new StackPanel();

            // Initialen-cirkel
            Border cirkel = new Border();
            cirkel.Width = 48;
            cirkel.Height = 48;
            cirkel.CornerRadius = new CornerRadius(24);
            cirkel.Background = new SolidColorBrush(Color.FromRgb(176, 190, 197));
            cirkel.Margin = new Thickness(0, 0, 0, 10);

            TextBlock txtInitialen = new TextBlock();
            txtInitialen.Text = GeefInitialen(patient.GeefVolledigeNaam());
            txtInitialen.FontSize = 16;
            txtInitialen.FontWeight = FontWeights.Bold;
            txtInitialen.Foreground = Brushes.White;
            txtInitialen.HorizontalAlignment = HorizontalAlignment.Center;
            txtInitialen.VerticalAlignment = VerticalAlignment.Center;
            cirkel.Child = txtInitialen;

            // Volledige naam
            TextBlock txtNaam = new TextBlock();
            txtNaam.Text = patient.GeefVolledigeNaam();
            txtNaam.FontSize = 14;
            txtNaam.FontWeight = FontWeights.SemiBold;
            txtNaam.Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            txtNaam.TextWrapping = TextWrapping.Wrap;
            txtNaam.Margin = new Thickness(0, 0, 0, 4);

            // Geslacht
            TextBlock txtGeslacht = new TextBlock();
            txtGeslacht.Text = GeefGeslachtTekst(patient.Geslacht);
            txtGeslacht.FontSize = 12;
            txtGeslacht.Foreground = new SolidColorBrush(Color.FromRgb(84, 110, 122));
            txtGeslacht.Margin = new Thickness(0, 0, 0, 2);

            // Geboortedatum
            TextBlock txtGeboortedatum = new TextBlock();
            txtGeboortedatum.Text = patient.Geboortedatum.ToString("dd/MM/yyyy");
            txtGeboortedatum.FontSize = 12;
            txtGeboortedatum.Foreground = new SolidColorBrush(Color.FromRgb(144, 164, 174));
            txtGeboortedatum.Margin = new Thickness(0, 0, 0, 2);

            // E-mailadres
            TextBlock txtEmail = new TextBlock();
            txtEmail.Text = patient.Email;
            txtEmail.FontSize = 12;
            txtEmail.Foreground = new SolidColorBrush(Color.FromRgb(84, 110, 122));
            txtEmail.TextTrimming = TextTrimming.CharacterEllipsis;
            txtEmail.Margin = new Thickness(0, 0, 0, 2);

            // Gsm-nummer (nchar-veld kan spaties bevatten aan het einde)
            TextBlock txtGsm = new TextBlock();
            txtGsm.Text = patient.Gsm.Trim();
            txtGsm.FontSize = 12;
            txtGsm.Foreground = new SolidColorBrush(Color.FromRgb(144, 164, 174));
            txtGsm.Margin = new Thickness(0, 0, 0, 12);

            // Drie actieknoppen naast elkaar
            StackPanel knoppen = new StackPanel();
            knoppen.Orientation = Orientation.Horizontal;

            Button btnDetails = new Button();
            btnDetails.Content = "Details";
            btnDetails.Tag = patient;
            btnDetails.Padding = new Thickness(8, 5, 8, 5);
            btnDetails.Margin = new Thickness(0, 0, 6, 0);
            btnDetails.FontSize = 12;
            btnDetails.Background = new SolidColorBrush(Color.FromRgb(27, 42, 74));
            btnDetails.Foreground = Brushes.White;
            btnDetails.BorderThickness = new Thickness(0);
            btnDetails.Cursor = Cursors.Hand;
            btnDetails.Click += BtnDetails_Click;

            Button btnWijzigen = new Button();
            btnWijzigen.Content = "Wijzigen";
            btnWijzigen.Tag = patient;
            btnWijzigen.Padding = new Thickness(8, 5, 8, 5);
            btnWijzigen.Margin = new Thickness(0, 0, 6, 0);
            btnWijzigen.FontSize = 12;
            btnWijzigen.Background = new SolidColorBrush(Color.FromRgb(84, 110, 122));
            btnWijzigen.Foreground = Brushes.White;
            btnWijzigen.BorderThickness = new Thickness(0);
            btnWijzigen.Cursor = Cursors.Hand;
            btnWijzigen.Click += BtnWijzigen_Click;

            Button btnVerwijderen = new Button();
            btnVerwijderen.Content = "Verwijderen";
            btnVerwijderen.Tag = patient;
            btnVerwijderen.Padding = new Thickness(8, 5, 8, 5);
            btnVerwijderen.FontSize = 12;
            btnVerwijderen.Background = new SolidColorBrush(Color.FromRgb(211, 47, 47));
            btnVerwijderen.Foreground = Brushes.White;
            btnVerwijderen.BorderThickness = new Thickness(0);
            btnVerwijderen.Cursor = Cursors.Hand;
            btnVerwijderen.Click += BtnVerwijderen_Click;

            knoppen.Children.Add(btnDetails);
            knoppen.Children.Add(btnWijzigen);
            knoppen.Children.Add(btnVerwijderen);

            inhoud.Children.Add(cirkel);
            inhoud.Children.Add(txtNaam);
            inhoud.Children.Add(txtGeslacht);
            inhoud.Children.Add(txtGeboortedatum);
            inhoud.Children.Add(txtEmail);
            inhoud.Children.Add(txtGsm);
            inhoud.Children.Add(knoppen);

            kaart.Child = inhoud;
            PnlPatienten.Children.Add(kaart);
        }

        // Navigeert naar de leesweergave van de patiëntdetailpagina
        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            Button knop = (Button)sender;
            Patient patient = (Patient)knop.Tag;
            NavigationService.Navigate(new PatientDetailPagina(patient, aangemeldeDokter, false));
        }

        // Navigeert naar de bewerkweergave van de patiëntdetailpagina
        private void BtnWijzigen_Click(object sender, RoutedEventArgs e)
        {
            Button knop = (Button)sender;
            Patient patient = (Patient)knop.Tag;
            NavigationService.Navigate(new PatientDetailPagina(patient, aangemeldeDokter, true));
        }

        // Verwijdert de patiënt uit de databank en herlaadt de lijst
        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            Button knop = (Button)sender;
            Patient patient = (Patient)knop.Tag;
            TxtFout.Visibility = Visibility.Collapsed;

            try
            {
                patient.Verwijderen();
                LaadAllePatient();
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het verwijderen van de patiënt: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
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

        // Zet de numerieke geslachtswaarde om naar leesbare tekst (0=Man, 1=Vrouw)
        private string GeefGeslachtTekst(int geslacht)
        {
            if (geslacht == 0)
            {
                return "Man";
            }
            else if (geslacht == 1)
            {
                return "Vrouw";
            }
            else
            {
                return "Onbekend";
            }
        }

        // Voert de zoekopdracht uit bij het klikken op de zoekknop
        private void BtnZoeken_Click(object sender, RoutedEventArgs e)
        {
            Zoeken();
        }

        // Enter-toets in het zoekveld activeert de zoekopdracht
        private void TxtZoeken_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Zoeken();
            }
        }

        // Wist het zoekveld en toont opnieuw alle patiënten van de dokter
        private void BtnAllePatient_Click(object sender, RoutedEventArgs e)
        {
            TxtZoeken.Text = string.Empty;
            LaadAllePatient();
        }

        // Wist de zoekterm en herlaadt de volledige lijst; aangeroepen vanuit PatientDetailPagina
        // nadat een nieuwe patiënt opgeslagen is, zodat de lijst altijd up-to-date is
        public void HerlaadLijst()
        {
            TxtZoeken.Text = string.Empty;
            LaadAllePatient();
        }

        // Navigeert naar het formulier voor een nieuwe patiënt (patient == null → INSERT-modus);
        // geeft een referentie naar deze pagina mee zodat de lijst na opslaan herladen kan worden
        private void BtnNieuwePatient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientDetailPagina(null, aangemeldeDokter, true, this));
        }
    }
}
