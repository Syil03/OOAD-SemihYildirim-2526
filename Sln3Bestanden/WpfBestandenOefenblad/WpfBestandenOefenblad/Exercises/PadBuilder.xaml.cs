using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Pad builder", Description = "Paden samenstellen uit basispad, map en bestandsnaam", Order = 1, IsVisible = true)]
public partial class PadBuilder : Page
{
    public PadBuilder()
    {
        InitializeComponent();
        btnGenereerPad.Click += BtnGenereerPad_Click;
    }

    private void BtnGenereerPad_Click(object sender, RoutedEventArgs e)
    {
        Environment.SpecialFolder specialeMap = Environment.SpecialFolder.MyDocuments;

        if (rdbAfbeeldingen.IsChecked == true)
        {
            specialeMap = Environment.SpecialFolder.MyPictures;
        }
        else if (rdbDesktop.IsChecked == true)
        {
            specialeMap = Environment.SpecialFolder.Desktop;
        }

        string basisPad = Environment.GetFolderPath(specialeMap);

        string subPad = txtPad.Text;
        string bestandsNaam = txtBestandsnaam.Text;

        string volledigPad = Path.Combine(basisPad, subPad, bestandsNaam);

        txtResultaat.Text = volledigPad.Replace('\\', '/');
    }
}