using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises;

[NavPage(Title = "Klik mij", Description = "Eenvoudige Button met Click-event", Order = 1, IsVisible = true)]
public partial class KlikMij : Page
{
    public KlikMij()
    {
        InitializeComponent();
    }

    private void btnKlikMij_Click(object sender, RoutedEventArgs e)
    {
        txtBericht.Text = "er is op de button geklikt";
    }
}
