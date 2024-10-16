using System.Windows;
using LangLang.ViewModels.TeacherViewModels;

namespace LangLang.Views.DirectorViews
{
    /// <summary>
    /// Interaction logic for TeacherListingView.xaml
    /// </summary>
    public partial class TeacherListingView : Window
    {
        public TeacherListingView()
        {
            DataContext = new TeacherListingViewModel(this);

            InitializeComponent();
        }
    }
}
