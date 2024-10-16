using System.Windows;
using LangLang.ViewModels.TeacherViewModels;

namespace LangLang.Views.TeacherViews
{
    public partial class TeacherMenu : Window
    {
        public TeacherMenu()
        {
            InitializeComponent();
            DataContext = new TeacherMenuViewModel(this);
        }
    }
}
