using System.Windows;

namespace PatientApp
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
