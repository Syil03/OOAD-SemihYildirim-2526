using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PatientApp.Pages
{
    public partial class AfsprakenPagina : Page
    {
        private Patient aangemeldePatient;

        public AfsprakenPagina(Patient patient)
        {
            InitializeComponent();
            aangemeldePatient = patient;
            // Loaded vuurt ook af bij terugkeer naar deze pagina, zodat de lijst telkens vernieuwt
            Loaded += AfsprakenPagina_Loaded;
        }

        // Herlaad de afspraken telkens wanneer de pagina wordt weergegeven (ook bij terugkeer)
        private void AfsprakenPagina_Loaded(object sender, RoutedEventArgs e)
        {
            TxtPatientNaam.Text = aangemeldePatient.GeefVolledigeNaam();
            LaadAfspraken();
        }

        private void LaadAfspraken()
        {
            PnlAfspraken.Children.Clear();
            TxtFout.Visibility = Visibility.Collapsed;

            try
            {
                List<Afspraak> afspraken = Afspraak.GeefAfsprakenVanPatient(aangemeldePatient.Id);

                if (afspraken.Count == 0)
                {
                    TextBlock geenData = new TextBlock();
                    geenData.Text = "U heeft nog geen afspraken.";
                    geenData.FontSize = 14;
                    geenData.Foreground = new SolidColorBrush(Color.FromRgb(144, 164, 174));
                    geenData.HorizontalAlignment = HorizontalAlignment.Center;
                    geenData.Margin = new Thickness(0, 40, 0, 0);
                    PnlAfspraken.Children.Add(geenData);
                    return;
                }

                for (int i = 0; i < afspraken.Count; i++)
                {
                    MaakAfspraakKaart(afspraken[i]);
                }
            }
            catch (Exception fout)
            {
                TxtFout.Text = "Fout bij het laden van afspraken: " + fout.Message;
                TxtFout.Visibility = Visibility.Visible;
            }
        }

        // Bouwt een kaart voor één afspraak en voegt die toe aan het paneel
        private void MaakAfspraakKaart(Afspraak afspraak)
        {
            Border kaart = new Border();
            kaart.Background = Brushes.White;
            kaart.BorderBrush = new SolidColorBrush(Color.FromRgb(207, 216, 220));
            kaart.BorderThickness = new Thickness(1);
            kaart.CornerRadius = new CornerRadius(6);
            kaart.Margin = new Thickness(0, 0, 0, 10);
            kaart.Padding = new Thickness(16, 14, 16, 14);

            // Lay-out: dokternaam + klacht links, datum rechts
            Grid inhoud = new Grid();
            inhoud.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            inhoud.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            StackPanel links = new StackPanel();

            TextBlock txtDokter = new TextBlock();
            txtDokter.Text = "Dr. " + afspraak.DokterNaam;
            txtDokter.FontSize = 15;
            txtDokter.FontWeight = FontWeights.SemiBold;
            txtDokter.Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            txtDokter.Margin = new Thickness(0, 0, 0, 4);

            TextBlock txtKlacht = new TextBlock();
            txtKlacht.Text = afspraak.Klacht;
            txtKlacht.FontSize = 13;
            txtKlacht.Foreground = new SolidColorBrush(Color.FromRgb(84, 110, 122));
            txtKlacht.TextWrapping = TextWrapping.Wrap;

            links.Children.Add(txtDokter);
            links.Children.Add(txtKlacht);

            TextBlock txtMoment = new TextBlock();
            txtMoment.Text = afspraak.Moment.ToString("dd/MM/yyyy") + "\n" + afspraak.Moment.ToString("HH:mm");
            txtMoment.FontSize = 13;
            txtMoment.Foreground = new SolidColorBrush(Color.FromRgb(84, 110, 122));
            txtMoment.TextAlignment = TextAlignment.Right;
            txtMoment.VerticalAlignment = VerticalAlignment.Center;
            txtMoment.Margin = new Thickness(16, 0, 0, 0);

            Grid.SetColumn(links, 0);
            Grid.SetColumn(txtMoment, 1);
            inhoud.Children.Add(links);
            inhoud.Children.Add(txtMoment);

            kaart.Child = inhoud;
            PnlAfspraken.Children.Add(kaart);
        }

        // Navigeer naar de afspraak-boekenpagina en geef de ingelogde patiënt mee
        private void BtnNieuweAfspraak_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AfspraakBoekenPagina(aangemeldePatient));
        }

        // Navigeer naar de profielpagina van de ingelogde patiënt
        private void BtnProfiel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfielPagina(aangemeldePatient));
        }

        // Keer terug naar de inlogpagina en wis de navigatiegeschiedenis
        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InlogPagina());
            NavigationService.RemoveBackEntry();
        }
    }
}
