using DokterspraktijkLib.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace DokterApp.Pages
{
    public partial class AfsprakenPagina : Page
    {
        private Dokter aangemeldeDokter;
        private bool toonToekomstig;

        public AfsprakenPagina(Dokter dokter)
        {
            InitializeComponent();
            aangemeldeDokter = dokter;
            toonToekomstig = true;
        }

        // Herlaad de afspraken telkens wanneer naar deze pagina genavigeerd wordt (ook bij terugkeer)
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            TxtDokterNaam.Text = "Dr. " + aangemeldeDokter.GeefVolledigeNaam();
            LaadAfspraken();
        }

        private void LaadAfspraken()
        {
            PnlAfspraken.Children.Clear();
            TxtFout.Visibility = Visibility.Collapsed;

            try
            {
                List<Afspraak> afspraken;

                if (toonToekomstig)
                    afspraken = Afspraak.GeefToekomstigeAfspraken(aangemeldeDokter.Id);
                else
                    afspraken = Afspraak.GeefAfsprakenVanDokter(aangemeldeDokter.Id);

                if (afspraken.Count == 0)
                {
                    TextBlock geenData = new TextBlock();
                    geenData.Text = "Geen afspraken gevonden.";
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

        // Bouwt een klikbare kaart voor één afspraak en voegt die toe aan het paneel
        private void MaakAfspraakKaart(Afspraak afspraak)
        {
            Border kaart = new Border();
            kaart.Background = Brushes.White;
            kaart.BorderBrush = new SolidColorBrush(Color.FromRgb(207, 216, 220));
            kaart.BorderThickness = new Thickness(1);
            kaart.CornerRadius = new CornerRadius(6);
            kaart.Margin = new Thickness(0, 0, 0, 10);
            kaart.Padding = new Thickness(16, 14, 16, 14);
            kaart.Cursor = Cursors.Hand;
            kaart.Tag = afspraak;
            kaart.MouseLeftButtonUp += Kaart_MouseLeftButtonUp;

            // Lay-out: patiëntnaam + klacht links, datum rechts
            Grid inhoud = new Grid();
            inhoud.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            inhoud.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            StackPanel links = new StackPanel();

            TextBlock txtPatient = new TextBlock();
            txtPatient.Text = afspraak.PatientNaam;
            txtPatient.FontSize = 15;
            txtPatient.FontWeight = FontWeights.SemiBold;
            txtPatient.Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            txtPatient.Margin = new Thickness(0, 0, 0, 4);

            TextBlock txtKlacht = new TextBlock();
            txtKlacht.Text = afspraak.Klacht;
            txtKlacht.FontSize = 13;
            txtKlacht.Foreground = new SolidColorBrush(Color.FromRgb(84, 110, 122));
            txtKlacht.TextTrimming = TextTrimming.CharacterEllipsis;

            links.Children.Add(txtPatient);
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

        // Navigeer naar de detailpagina van de aangeklikte afspraak
        private void Kaart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border kaart = (Border)sender;
            Afspraak afspraak = (Afspraak)kaart.Tag;
            NavigationService.Navigate(new AfspraakDetailPagina(afspraak, aangemeldeDokter));
        }

        private void BtnToekomstig_Click(object sender, RoutedEventArgs e)
        {
            toonToekomstig = true;
            BtnToekomstig.Background = new SolidColorBrush(Color.FromRgb(25, 118, 210));
            BtnToekomstig.Foreground = Brushes.White;
            BtnAlle.Background = new SolidColorBrush(Color.FromRgb(236, 239, 241));
            BtnAlle.Foreground = new SolidColorBrush(Color.FromRgb(84, 110, 122));
            LaadAfspraken();
        }

        private void BtnAlle_Click(object sender, RoutedEventArgs e)
        {
            toonToekomstig = false;
            BtnAlle.Background = new SolidColorBrush(Color.FromRgb(25, 118, 210));
            BtnAlle.Foreground = Brushes.White;
            BtnToekomstig.Background = new SolidColorBrush(Color.FromRgb(236, 239, 241));
            BtnToekomstig.Foreground = new SolidColorBrush(Color.FromRgb(84, 110, 122));
            LaadAfspraken();
        }
    }
}
