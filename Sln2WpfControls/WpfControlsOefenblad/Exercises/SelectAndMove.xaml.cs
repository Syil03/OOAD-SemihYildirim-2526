using System;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Select And Move", Description = "Items verplaatsen tussen twee ListBoxes.", Order = 9, IsVisible = true)]
    public partial class SelectAndMove : Page
    {
        public SelectAndMove()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btnToSelected == null || btnToAvailable == null) return;

            btnToSelected.IsEnabled = lstAvailable.SelectedItem != null;

            btnToAvailable.IsEnabled = lstSelected.SelectedItem != null;
        }

        private void btnToSelected_Click(object sender, RoutedEventArgs e)
        {
            if (lstAvailable.SelectedItem is ListBoxItem gekozenItem)
            {
                lstAvailable.Items.Remove(gekozenItem);

                lstSelected.Items.Add(gekozenItem);
            }
        }

        private void btnToAvailable_Click(object sender, RoutedEventArgs e)
        {
            if (lstSelected.SelectedItem is ListBoxItem gekozenItem)
            {
                lstSelected.Items.Remove(gekozenItem);

                lstAvailable.Items.Add(gekozenItem);
            }
        }
    }
}
