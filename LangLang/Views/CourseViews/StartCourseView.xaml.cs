using System.Windows;
using LangLang.ViewModels.CourseViewModels;

namespace LangLang.Views.CourseViews
{
    /// <summary>
    /// Interaction logic for StartCourseView.xaml
    /// </summary>
    public partial class StartCourseView : Window
    {
        public StartCourseView(int courseId)
        {
            InitializeComponent();
            DataContext = new StartCourseViewModel(courseId, this);
        }
    }
}
