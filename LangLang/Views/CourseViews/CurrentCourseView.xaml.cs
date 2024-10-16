using LangLang.ViewModels.CourseViewModels;
using System.Windows;

namespace LangLang.Views.CourseViews
{
    /// <summary>
    /// Interaction logic for CurrentCourseView.xaml
    /// </summary>
    public partial class CurrentCourseView : Window
    {
        public CurrentCourseView(int courseId)
        {
            InitializeComponent();
            DataContext = new CurrentCourseViewModel(courseId, this);
        }
    }
}
