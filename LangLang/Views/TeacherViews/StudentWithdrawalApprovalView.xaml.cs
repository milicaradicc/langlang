using LangLang.ViewModels.TeacherViewModels;
using System.Windows;

namespace LangLang.Views.TeacherViews
{
    /// <summary>
    /// Interaction logic for StudentWithdrawalApprovalView.xaml
    /// </summary>
    public partial class StudentWithdrawalApprovalView : Window
    {
        public StudentWithdrawalApprovalView(int courseId)
        {
            InitializeComponent();
            DataContext = new StudentWithdrawalApprovalViewModel(courseId);
        }
    }
}
