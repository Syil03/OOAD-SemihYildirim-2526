using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Kies afbeelding", Description = "OpenFileDialog om een jpg/jpeg te kiezen en in een Image te tonen", Order = 3, IsVisible = true)]
public partial class KiesAfbeelding : Page
{
    public KiesAfbeelding()
    {
        InitializeComponent();
    }

    private void BtnKies_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();

        dialog.Filter = "JPEG Afbeeldingen (*.jpg;*.jpeg)|*.jpg;*.jpeg";

        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        if (dialog.ShowDialog() == true)
        {
            string filename = dialog.FileName;

            txtBestandsnaam.Text = Path.GetFileName(filename);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filename, UriKind.Absolute);
            bitmap.EndInit();

            imgAfbeelding.Source = bitmap;
        }
        else
        {
            txtBestandsnaam.Text = "kiezen afbeelding geannuleerd";
            imgAfbeelding.Source = null;
        }
    }
}
