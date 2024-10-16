using System.Windows;
using LangLang.ViewModels.StudentViewModels;

namespace LangLang.Views.StudentViews;

public partial class StudentView : Window
{
    public StudentView()
    {
        InitializeComponent();
        DataContext = new StudentMenuViewModel(this);
    }
}