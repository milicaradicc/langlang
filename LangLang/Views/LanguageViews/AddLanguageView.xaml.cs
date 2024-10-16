using System.Windows;
using LangLang.ViewModels.LanguageViewModels;

namespace LangLang.Views.LanguageViews
{
    /// <summary>
    /// Interaction logic for AddLanguageView.xaml
    /// </summary>
    public partial class AddLanguageView : Window
    {
        public AddLanguageView()
        {
            DataContext = new AddLanguageViewModel(this);
            InitializeComponent();
        }
    }
}
