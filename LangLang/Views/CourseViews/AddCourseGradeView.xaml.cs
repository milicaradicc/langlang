using LangLang.ViewModels.CourseViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace LangLang.Views.CourseViews
{
    /// <summary>
    /// Interaction logic for AddCourseGradeView.xaml
    /// </summary>
    public partial class AddCourseGradeView : Window
    {
        public AddCourseGradeView(int studentId, int courseId)
        {
            InitializeComponent();
            DataContext = new AddCourseGradeViewModel(studentId, courseId, this);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
