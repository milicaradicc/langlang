using System.Windows;
using LangLang.Models;
using LangLang.ViewModels.StudentViewModels;

namespace LangLang.Views.StudentViews;

public partial class StudentExamView : Window
{
    public StudentExamView()
    {
        InitializeComponent();
        DataContext = ServiceProvider.GetRequiredService<StudentExamViewModel>();
    }
}