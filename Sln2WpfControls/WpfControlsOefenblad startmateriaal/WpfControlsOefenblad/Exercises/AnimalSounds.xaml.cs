using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Animal sounds", Description = "Afbeeldingen van dieren en hun geluiden", Order = 10, IsVisible = true)]
    public partial class AnimalSounds : Page
    {
        public AnimalSounds()
        {
            InitializeComponent();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image? theImage = sender as Image;

            if (theImage != null && theImage.Tag != null)
            {
                string fileName = theImage.Tag.ToString()!;

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exercises", "Sounds", fileName);

                if (File.Exists(path))
                {
                    SoundPlayer player = new SoundPlayer(path);
                    player.Play();
                }
            }
        }
    }
}
