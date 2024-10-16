using LangLang.ViewModels.CourseViewModels;
using System.Windows;
using LangLang.Models;

namespace LangLang.Views.CourseViews
{
    /// <summary>
    /// Interaction logic for StartableCoursesView.xaml
    /// </summary>
    public partial class StartableCoursesView : Window
    {
        public StartableCoursesView()
        {
            InitializeComponent();
            DataContext = ServiceProvider.GetRequiredService<StartableCoursesViewModel>();
        }
    }
}
