using System.Windows;
using System.Windows.Controls;

namespace LangLang.Views.StudentViews;

public partial class TeacherReviewModal : Window
{
    public TeacherReviewModal()
    {
        InitializeComponent();
    }
    
    public int Response => int.Parse(ResponseTextBox.Text);

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(ResponseTextBox.Text, out int number))
        {
            if (number is >= 1 and <= 10)
            {
                DialogResult = true;
                return;
            }
            MessageBox.Show("Please enter a number between 1 and 10.");
            ResponseTextBox.Text = "";
        }
        else
        {
            MessageBox.Show("Please enter a valid number.");
            ResponseTextBox.Text = "";
        }
    }

    private void ResponseTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}