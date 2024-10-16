using System.Text.RegularExpressions;
using System.Windows;
using LangLang.Models;
using LangLang.ViewModels.ExamViewModels;

namespace LangLang.Views.ExamViews
{
    /// <summary>
    /// Interaction logic for ExamView.xaml
    /// </summary>
    public partial class AddExamView : Window
    {
        public AddExamView()
        {
            DataContext = new AddExamViewModel(null, this);
            InitializeComponent();
        }
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
