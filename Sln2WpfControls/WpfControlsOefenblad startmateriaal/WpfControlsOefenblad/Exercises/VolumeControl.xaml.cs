using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Volume Control", Description = "Slider gebruiken met ValueChanged en property-aanpassing.", Order = 7, IsVisible = true)]
    public partial class VolumeControl : Page
    {
        public VolumeControl()
        {
            InitializeComponent();
        }

        private void sldVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtVolume == null || brdVolume == null || stpVolumeControl == null)
            {
                return;
            }

            double volume = sldVolume.Value;

            txtVolume.Text = $"Volume: {volume}%";

            double maxWidth = stpVolumeControl.ActualWidth > 0 ? stpVolumeControl.ActualWidth : 500;
            brdVolume.Width = (volume / 100.0) * maxWidth;

            if (volume < 20)
            {
                brdVolume.Background = Brushes.Green;
            }
            else if (volume < 40)
            {
                brdVolume.Background = Brushes.Yellow;
            }
            else if (volume < 60)
            {
                brdVolume.Background = Brushes.Orange;
            }
            else if (volume < 80)
            {
                brdVolume.Background = Brushes.Red;
            }
            else
            {
                brdVolume.Background = Brushes.DarkRed;
            }
        }
    }
}
