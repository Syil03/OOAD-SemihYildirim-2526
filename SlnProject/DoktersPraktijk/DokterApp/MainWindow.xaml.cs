using System.Windows;

namespace DokterApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Start de applicatie op de inlogpagina
            HoofdFrame.Navigate(new Pages.InlogPagina());
        }
    }
}
