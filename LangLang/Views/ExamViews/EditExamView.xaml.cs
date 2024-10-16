using LangLang.Models;
using LangLang.ViewModels.ExamViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LangLang.Views.ExamViews
{
    /// <summary>
    /// Interaction logic for EditExamView.xaml
    /// </summary>
    public partial class EditExamView : Window
    {
        public EditExamView(Exam exam)
        {
            DataContext = new AddExamViewModel(exam, this);
            InitializeComponent();
        }
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
