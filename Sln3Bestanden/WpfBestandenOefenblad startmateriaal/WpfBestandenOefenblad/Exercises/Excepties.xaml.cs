using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Excepties", Description = "Kies een speciale map, zie bestanden in een ListBox en info in een TextBlock", Order = 5, IsVisible = true)]
public partial class Excepties : Page
{
    public Excepties()
    {
        InitializeComponent();
        cmbFolders.SelectedIndex = 0;
    }

    private void CmbFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        lstBestanden.Items.Clear();
        txtBestandInfo.Text = "";

        ComboBoxItem item = cmbFolders.SelectedItem as ComboBoxItem;
        if (item == null || item.Tag == null) return;

        string tag = item.Tag as string;
        if (!Enum.TryParse<Environment.SpecialFolder>(tag, out Environment.SpecialFolder folder)) return;

        string path = Environment.GetFolderPath(folder);

        string[] bestanden;
        try
        {
            bestanden = Directory.GetFiles(path);
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show("Je hebt geen toegang tot deze map.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            return; 
        }
        catch (DirectoryNotFoundException)
        {
            MessageBox.Show("De geselecteerde map kon niet worden gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        catch (IOException ex)
        {
            MessageBox.Show($"Er is een I/O fout opgetreden: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        foreach (string bestand in bestanden)
        {
            string naam = Path.GetFileName(bestand);
            lstBestanden.Items.Add(new ListBoxItem { Content = naam, Tag = bestand });
        }
    }

    private void LstBestanden_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        txtBestandInfo.Text = "";

        ListBoxItem? selectedItem = lstBestanden.SelectedItem as ListBoxItem;
        if (selectedItem == null) return;

        string path = selectedItem.Tag.ToString();

        try
        {
            FileInfo info = new(path);

            txtBestandInfo.Text = $@"Bestand: {info.Name}
Extensie: {info.Extension}
Grootte (bytes): {info.Length}
Aangemaakt: {info.CreationTime:dd/MM/yyyy HH:mm:ss}
Gewijzigd: {info.LastWriteTime:dd/MM/yyyy HH:mm:ss}";
        }
        catch (UnauthorizedAccessException)
        {
            txtBestandInfo.Text = "Fout: Je hebt geen rechten om de eigenschappen van dit bestand te bekijken.";
        }
        catch (FileNotFoundException)
        {
            txtBestandInfo.Text = "Fout: Het bestand kan niet meer gevonden worden. Mogelijk is het net verwijderd of verplaatst.";
        }
        catch (IOException ex)
        {
            txtBestandInfo.Text = $"Fout: Er trad een I/O probleem op bij het lezen van het bestand. {ex.Message}";
        }
    }
}
