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
using LangLang.ViewModels.ExamViewModels;

namespace LangLang.Views.ExamViews
{
    /// <summary>
    /// Interaction logic for AddExamGradeView.xaml
    /// </summary>
    public partial class AddExamGradeView : Window
    {
        public AddExamGradeView(int studentId, int examId)
        {
            InitializeComponent();
            DataContext = new AddExamGradeViewModel(studentId, examId, this);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
