using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Sender Oxo", Description = "OXO spel met één click event handler", Order = 8, IsVisible = true)]
    public partial class SenderOxo : Page
    {
        private bool isBeurtAanX = true;

        public SenderOxo()
        {
            InitializeComponent();
        }

        private void OxoButton_Click(object sender, RoutedEventArgs e)
        {
            Button geklikteKnop = sender as Button;

            if (geklikteKnop != null)
            {
                if (isBeurtAanX)
                {
                    geklikteKnop.Content = "X";
                }
                else
                {
                    geklikteKnop.Content = "O";
                }

                isBeurtAanX = !isBeurtAanX;

                geklikteKnop.IsEnabled = false;
            }
        }
    }
}
