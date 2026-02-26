using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WpfLayoutUIOefenblad.Helpers;

namespace WpfLayoutUIOefenblad.Exercises
{
    [NavPage("NotepadApp", "Een eenvoudige tekstverwerker met bestandsdialoog", 11)]
    public partial class NotepadApp : Page
    {
        // Nodig voor de automatische weergave van de opdracht in de viewer
        public System.Windows.Controls.Frame FraAssignment { get => fraAssignment; }

        public NotepadApp()
        {
            InitializeComponent();
        }

        private void btnOpenen_Click(object sender, RoutedEventArgs e)
        {
            // Maak een nieuw dialoogvenster om bestanden te selecteren
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Stel filters in zodat je alleen tekstbestanden ziet
            openFileDialog.Filter = "Tekstbestanden (.txt)|.txt|Alle bestanden (.)|.";

            // Als de gebruiker een bestand kiest en op 'Openen' klikt
            if (openFileDialog.ShowDialog() == true)
            {
                // Lees de tekst uit het bestand en zet het in de TextBox
                txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Sluit de applicatie
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            // Toon een eenvoudig About-venster
            MessageBox.Show("NotepadApp - Een eenvoudige tekstverwerker", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}