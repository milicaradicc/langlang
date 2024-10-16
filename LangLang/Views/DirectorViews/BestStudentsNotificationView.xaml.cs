using System.Windows;
using LangLang.Models;
using LangLang.ViewModels.DirectorViewModels;

namespace LangLang.Views.DirectorViews;

public partial class BestStudentsNotificationView : Window
{
    public BestStudentsNotificationView()
    {
        InitializeComponent();
        DataContext = ServiceProvider.GetRequiredService<BestStudentsNotificationViewModel>();
    }
}