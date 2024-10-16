using System.Windows;
using LangLang.Models;
using LangLang.ViewModels.DirectorViewModels;

namespace LangLang.Views.DirectorViews
{
    /// <summary>
    /// Interaction logic for EditTeacherView.xaml
    /// </summary>
    public partial class EditTeacherView : Window
    {
        public EditTeacherView(Teacher teacher)
        {
            DataContext = new EditTeacherViewModel(teacher, this);
            InitializeComponent();
        }
    }
}
