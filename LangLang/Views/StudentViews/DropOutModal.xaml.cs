using System.Windows;

namespace LangLang.Views.StudentViews;

public partial class DropOutModal : Window
{
    public DropOutModal()
    {
        InitializeComponent();
    }
    
    public string ResponseText => ResponseTextBox.Text;

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}