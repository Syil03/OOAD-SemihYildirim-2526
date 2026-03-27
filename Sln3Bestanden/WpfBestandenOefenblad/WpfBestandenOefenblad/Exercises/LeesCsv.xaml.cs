using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Lees CSV", Description = "CSV inlezen (Product;Quantity;Price), regels tonen en totaal verkoopbedrag", Order = 2, IsVisible = true)]
public partial class LeesCsv : Page
{
    public LeesCsv()
    {
        InitializeComponent();
    }

    private void BtnLaad_Click(object sender, RoutedEventArgs e)
    {
        lstVerkoop.Items.Clear();
        txtTotaal.Text = "";

        string pad = @"C:\Users\scyem\OneDrive\Documents\GitHub\OOAD-SemihYildirim-2526\Sln3Bestanden\WpfBestandenOefenblad startmateriaal\verkoop.csv";

        if (!File.Exists(pad))
        {
            MessageBox.Show("Het bestand verkoop.csv is niet gevonden. Ben je 'Copy to Output Directory' vergeten?");
            return;
        }

        string[] regels = File.ReadAllLines(pad);

        decimal totaalVerkoopBedrag = 0;

        for (int i = 1; i < regels.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(regels[i])) continue;

            string[] onderdelen = regels[i].Split(';');

            string product = onderdelen[0];
            int aantal = int.Parse(onderdelen[1]);
            decimal prijs = decimal.Parse(onderdelen[2]);

            decimal subTotaal = aantal * prijs;

            totaalVerkoopBedrag += subTotaal;

            string weergaveTekst = $"{product} x{aantal} aan €{prijs:F2} = €{subTotaal:F2}";
            lstVerkoop.Items.Add(weergaveTekst);
        }

        txtTotaal.Text = $"Totaal verkoopbedrag: €{totaalVerkoopBedrag:F2}";
    }
}