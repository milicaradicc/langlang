using System.Windows;
using LangLang.ViewModels.UserViewModels;

namespace LangLang.Views.UserViews;

public partial class RegisterView : Window
{
    public RegisterView()
    {
        InitializeComponent();
        DataContext = new RegisterViewModel(this);
    }
}