using System.Windows;
using LangLang.Models;
using LangLang.ViewModels.CourseViewModels;


namespace LangLang.Views.CourseViews
{
    /// <summary>
    /// Interaction logic for CoursesListingDirectorView.xaml
    /// </summary>
    public partial class CoursesListingDirectorView : Window
    {
        public CoursesListingDirectorView()
        {
            InitializeComponent();
            DataContext = ServiceProvider.GetRequiredService<CourseListingDirectorViewModel>(); 
        }
    }
}
