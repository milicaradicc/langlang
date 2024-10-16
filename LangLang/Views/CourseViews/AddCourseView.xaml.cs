using System.Windows;
using LangLang.Models;
using LangLang.ViewModels.CourseViewModels;

namespace LangLang.Views.CourseViews
{
    /// <summary>
    /// Interaction logic for AddCourseView.xaml
    /// </summary>
    public partial class AddCourseView : Window
    {
        public AddCourseView()
        {
            InitializeComponent();
            DataContext = ServiceProvider.GetRequiredService<AddCourseViewModel>();
        }
    }
}