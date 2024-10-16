using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LangLang.ViewModels.DirectorViewModels;

namespace LangLang.Views.DirectorViews
{
    /// <summary>
    /// Interaction logic for AddTeacherView.xaml
    /// </summary>
    public partial class AddTeacherView : Window
    {
        public AddTeacherView()
        {
            InitializeComponent();
            DataContext = new AddTeacherViewModel(QualificationsListBox, this);
        }
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                if (string.IsNullOrEmpty(box.Text))
                    box.Background = (ImageBrush)FindResource("watermark");
                else
                    box.Background = null;
            }
        }
    }

}
