using System.Windows;
using LangLang.Models;
using LangLang.ViewModels.CourseViewModels;

namespace LangLang.Views.CourseViews
{
    public partial class EditCourseView : Window
    {
        public EditCourseView(Course? course)
        {
            InitializeComponent();
            DataContext = new EditCourseViewModel(course);
        }
    }
}
