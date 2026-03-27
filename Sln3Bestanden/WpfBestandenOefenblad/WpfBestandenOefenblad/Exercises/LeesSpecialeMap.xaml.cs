using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Lees speciale map", Description = "Drie knoppen: Desktop, Documenten, Afbeeldingen; bestanden met grootte en aanmaakdatum in TextBlock", Order = 6, IsVisible = true)]
public partial class LeesSpecialeMap : Page
{
    public LeesSpecialeMap()
    {
        InitializeComponent();
    }

    private void BtnMap_Click(object sender, RoutedEventArgs e)
    {
        txtBestanden.Text = "";

        Button geklikteKnop = sender as Button;

        if (geklikteKnop == null) return;

        string tag = geklikteKnop.Tag.ToString();
        Environment.SpecialFolder specialeMap;

        switch (tag)
        {
            case "Desktop":
                specialeMap = Environment.SpecialFolder.Desktop;
                break;
            case "Documenten":
                specialeMap = Environment.SpecialFolder.MyDocuments;
                break;
            case "Afbeeldingen":
                specialeMap = Environment.SpecialFolder.MyPictures;
                break;
            default:
                return;
        }

        string mapPad = Environment.GetFolderPath(specialeMap);

        try
        {
            string[] bestanden = Directory.GetFiles(mapPad);

            foreach (string bestandPad in bestanden)
            {
                FileInfo info = new FileInfo(bestandPad);

                string bestandTekst = $"{info.Name} ({info.Length} bytes)\n";

                txtBestanden.Text += bestandTekst;
            }

            if (bestanden.Length == 0)
            {
                txtBestanden.Text = "Deze map bevat geen bestanden.";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fout bij het lezen van de map: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
