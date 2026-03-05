using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises;

[NavPage(Title = "Chatbox", Description = "multiline TextBox, TextBlock opmaak", Order = 3, IsVisible = true)]
public partial class ChatBox : Page
{
    public ChatBox()
    {
        InitializeComponent();
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        string naam = inpName.Text.Trim();
        string bericht = inpMessage.Text.Trim();

        if (!string.IsNullOrWhiteSpace(naam) && !string.IsNullOrWhiteSpace(bericht))
        {

            txtChat.Inlines.Add(new Bold(new Run(naam + ":")));

            txtChat.Inlines.Add(new Run(" " + bericht));

            txtChat.Inlines.Add(new LineBreak());
            txtChat.Inlines.Add(new LineBreak());

            inpName.Clear();
            inpMessage.Clear();
        }
    }
}
