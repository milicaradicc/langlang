using System.Windows;
using LangLang.ViewModels.StudentViewModels;

namespace LangLang.Views.StudentViews;

public partial class StudentCourseView : Window
{
    public StudentCourseView(bool applied = false)
    {
        InitializeComponent();
        DataContext = new StudentCourseViewModel(applied);
    }
}