using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Order Builder", Description = "CheckBox + RadioButton met samenvatting en reset.", Order = 5, IsVisible = true)]
    public partial class OrderBuilder : Page
    {
        public OrderBuilder()
        {
            InitializeComponent();
        }

        private void btnBevestig_Click(object sender, RoutedEventArgs e)
        {
            string levering = "";

            if (rdoAfhalen.IsChecked == true) levering = "Afhalen";
            else if (rdoLevering.IsChecked == true) levering = "Levering";
            else if (rdoTerPlaatse.IsChecked == true) levering = "Ter plaatse";

            if (string.IsNullOrEmpty(levering))
            {
                txtSummary.Text = "Kies eerst een leveringsmethode";
                return;
            }

            List<string> extras = new List<string>();
            if (chkKaas.IsChecked == true) extras.Add("Kaas");
            if (chkSpek.IsChecked == true) extras.Add("Spek");
            if (chkExtraSaus.IsChecked == true) extras.Add("Extra saus");
            if (chkUi.IsChecked == true) extras.Add("Ui");

            string extraTekst = string.Join(", ", extras);

            txtSummary.Text = $"Levering: {levering}\nExtra's: {extraTekst}";
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            chkKaas.IsChecked = true;
            chkSpek.IsChecked = false;
            chkExtraSaus.IsChecked = true;
            chkUi.IsChecked = false;

            rdoAfhalen.IsChecked = false;
            rdoLevering.IsChecked = false;
            rdoTerPlaatse.IsChecked = false;

            txtSummary.Text = "...";
        }
    }
}
