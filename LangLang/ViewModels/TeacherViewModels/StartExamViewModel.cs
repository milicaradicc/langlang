using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using LangLang.ViewModels.StudentViewModels;
using LangLang.Services;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;

namespace LangLang.ViewModels.TeacherViewModels
{
    public class StartExamViewModel:ViewModelBase
    {
        private readonly IExamService _examService = ServiceProvider.GetRequiredService<IExamService>();
        
        private readonly int _examId;
        private readonly Window _startExamWindow;
        
        public StartExamViewModel(int examId, Window startExamWindow)
        {
            _examId = examId;
            _startExamWindow = startExamWindow;
            Students = new ObservableCollection<StudentViewModel>(_examService.GetStudents(_examId)
                .Select(student => new StudentViewModel(student)));
            ConfirmCommand = new RelayCommand(Confirm);
        }

        public ObservableCollection<StudentViewModel> Students { get; set; }
        public ICommand ConfirmCommand { get; set; }

        private void Confirm()
        {
            _examService.ConfirmExam(_examId);
            MessageBox.Show("Exam started successfully.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
            _startExamWindow.Close();
        }
    }
}
