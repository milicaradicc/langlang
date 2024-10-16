using System.Windows;
using LangLang.ViewModels.DirectorViewModels;


namespace LangLang.Views.DirectorViews
{
    /// <summary>
    /// Interaction logic for DirectorMainMenu.xaml
    /// </summary>
    public partial class DirectorMainMenu : Window
    {
        public DirectorMainMenu()
        {
            InitializeComponent();
            DataContext = new DirectorMenuViewModel(this);
        }
    }
}
