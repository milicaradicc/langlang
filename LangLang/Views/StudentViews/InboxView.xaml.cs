using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LangLang.ViewModels.StudentViewModels;

namespace LangLang.Views.StudentViews
{
    /// <summary>
    /// Interaction logic for InboxView.xaml
    /// </summary>
    public partial class InboxView : Window
    {
        public InboxView(int studentId)
        {
            InitializeComponent();
            DataContext = new InboxViewModel(studentId);
        }
    }
}
