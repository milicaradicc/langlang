using System.Windows;
using LangLang.Models;
using LangLang.ViewModels.TeacherViewModels;

namespace LangLang.Views.TeacherViews
{
    /// <summary>
    /// Interaction logic for StartableExamsView.xaml
    /// </summary>
    public partial class StartableExamsView : Window
    {
        public StartableExamsView()
        {
            InitializeComponent();
            DataContext = ServiceProvider.GetRequiredService<StartableExamsViewModel>();
        }
    }
}
