using System;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Welkom", Description = "TextBox met eenvoudige validatie.", Order = 2, IsVisible = true)]
    public partial class Welkom : Page
    {
        public Welkom()
        {
            InitializeComponent();
        }

        private void btnZegHallo_Click(object sender, RoutedEventArgs e)
        {
            string ingevoerdeNaam = txtNaam.Text.Trim();

            if (string.IsNullOrWhiteSpace(ingevoerdeNaam))
            {
                txtFoutmelding.Visibility = Visibility.Visible;
                txtWelkomsttekst.Text = "...";
            }
            else
            {
                txtFoutmelding.Visibility = Visibility.Hidden;
                txtWelkomsttekst.Text = $"Welkom, {ingevoerdeNaam}!";
            }
        }
    }
}
