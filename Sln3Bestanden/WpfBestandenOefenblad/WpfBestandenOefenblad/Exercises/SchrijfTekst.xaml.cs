using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Schrijf tekst", Description = "Tekst intikken in een TextBox en opslaan naar een bestand", Order = 4, IsVisible = true)]
public partial class SchrijfTekst : Page
{
    public SchrijfTekst()
    {
        InitializeComponent();
    }

    private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog dialog = new SaveFileDialog();

        dialog.Filter = "Tekstbestanden (*.txt)|*.txt";

        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        dialog.FileName = "untitled.txt";

        if (dialog.ShowDialog() == true)
        {
            string pad = dialog.FileName;

            string tekstOmTeSlaan = txtInvoer.Text;

            File.WriteAllText(pad, tekstOmTeSlaan);

            MessageBox.Show("Bestand succesvol opgeslagen!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
