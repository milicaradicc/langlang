using System.Windows;
using LangLang.Models;
using LangLang.ViewModels.ExamViewModels;

namespace LangLang.Views.ExamViews
{
    /// <summary>
    /// Interaction logic for ExamView.xaml
    /// </summary>
    public partial class ExamView : Window
    {
        public ExamView()
        {
            DataContext = ServiceProvider.GetRequiredService<ExamListingViewModel>();
            InitializeComponent();
        }
    }
}
