using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;

namespace LangLang.ViewModels.ExamViewModels
{
    internal class AddExamGradeViewModel : ViewModelBase
    {
        private readonly int _studentId;
        private readonly int _examId;
        private readonly Window _currentWindow;

        private readonly IExamGradeService _examGradeService = ServiceProvider.GetRequiredService<IExamGradeService>();

        public AddExamGradeViewModel(int studentId, int examId, Window currentWindow)
        {
            _studentId = studentId;
            _examId = examId;
            AddExamGradeCommand = new RelayCommand(AddExamGrade);
            _currentWindow = currentWindow;
        }

        public int ReadingPoints { get; set; }
        public int WritingPoints { get; set; }
        public int ListeningPoints { get; set; }
        public int TalkingPoints { get; set; }

        public ICommand AddExamGradeCommand { get; set; }

        private void AddExamGrade()
        {
            try
            {
                _examGradeService.AddExamGrade(_studentId, _examId, ReadingPoints, WritingPoints, ListeningPoints, TalkingPoints);
                MessageBox.Show("Grade added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _currentWindow.Close();
            }
            catch (InvalidInputException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
