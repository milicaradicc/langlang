using LangLang.ViewModels.CourseViewModels;
using System.Windows;
using LangLang.Models;

namespace LangLang.Views.CourseViews
{
    /// <summary>
    /// Interaction logic for ActiveCoursesView.xaml
    /// </summary>
    public partial class ActiveCoursesView : Window
    {
        public ActiveCoursesView()
        {
            InitializeComponent();
            DataContext = ServiceProvider.GetRequiredService<ActiveCoursesViewModel>();
        }
    }
}
