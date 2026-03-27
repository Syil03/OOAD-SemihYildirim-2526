using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises;

[NavPage(Title = "Language Selector", Description = "ComboBox SelectionChanged event en ComboBoxItem", Order = 6, IsVisible = true)]
public partial class LanguageSelector : Page
{
    public LanguageSelector()
    {
        InitializeComponent();

        string[] languages = { "Nederlands", "English", "Français" };

        foreach (string language in languages)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.Content = language;
            cmbLanguages.Items.Add(item);
        }
    }

    private void cmbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (txtGreeting == null)
        {
            return;
        }

        if (cmbLanguages.SelectedItem is ComboBoxItem selectedItem)
        {
            foreach (ComboBoxItem item in cmbLanguages.Items)
            {
                item.FontWeight = FontWeights.Normal;
            }

            selectedItem.FontWeight = FontWeights.Bold;

            string chosenLanguage = selectedItem.Content?.ToString();

            switch (chosenLanguage)
            {
                case "Nederlands":
                    txtGreeting.Text = "Hallo";
                    break;
                case "English":
                    txtGreeting.Text = "Hello";
                    break;
                case "Français":
                    txtGreeting.Text = "Bonjour";
                    break;
                default:
                    txtGreeting.Text = "";
                    break;
            }
        }
    }
}
