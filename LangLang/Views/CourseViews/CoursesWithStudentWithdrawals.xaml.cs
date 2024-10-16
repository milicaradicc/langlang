using LangLang.ViewModels.CourseViewModels;
using System.Windows;
using LangLang.Models;

namespace LangLang.Views.CourseViews
{
    /// <summary>
    /// Interaction logic for CoursesWithStudentWithdrawals.xaml
    /// </summary>
    public partial class CoursesWithStudentWithdrawalsView : Window
    {
        public CoursesWithStudentWithdrawalsView()
        {
            InitializeComponent();
            DataContext = ServiceProvider.GetRequiredService<CoursesWithStudentWithdrawalsViewModel>();
        }
    }
}
