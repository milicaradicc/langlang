using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;
using LangLang.ViewModels.StudentViewModels;
using LangLang.Views.ExamViews;

namespace LangLang.ViewModels.ExamViewModels
{
    public class CurrentExamViewModel:ViewModelBase
    {
        private readonly Teacher _teacher = UserService.LoggedInUser as Teacher ??
                                            throw new InvalidOperationException("No one is logged in.");
        
        private readonly IExamService _examService = ServiceProvider.GetRequiredService<IExamService>();
        private readonly IStudentService _studentService = ServiceProvider.GetRequiredService<IStudentService>();
        private readonly IUserService _userService = ServiceProvider.GetRequiredService<IUserService>();
        private readonly IExamGradeService _examGradeService = ServiceProvider.GetRequiredService<IExamGradeService>();
        
        private readonly int _examId;
        private readonly Window _currentWindow;

        public CurrentExamViewModel(Window currentWindow)
        {
            _examId = _examService.GetCurrentExam(_teacher.Id);
            RefreshStudents();
            AddExamGradeCommand = new RelayCommand(AddExamGrade);
            FinishExamCommand = new RelayCommand(FinishExam);
            ReportCheatingCommand = new RelayCommand(ReportCheating);
            _currentWindow=currentWindow;
        }

        public ObservableCollection<StudentExamGradeViewModel> Students { get; set; } = new();
        public StudentExamGradeViewModel? SelectedItem { get; set; }

        public ICommand AddExamGradeCommand { get; set; }
        public ICommand FinishExamCommand { get; set; }
        public ICommand ReportCheatingCommand { get; set; }

        private void AddExamGrade()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("No student selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newWindow = new AddExamGradeView(SelectedItem.StudentId, _examId);
            newWindow.ShowDialog();
            RefreshStudents();
        }

        private void FinishExam()
        {
            try
            {
                _examService.FinishExam(_examId);
                MessageBox.Show("Successfully finished exam.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _currentWindow.Close();
            }
            catch (InvalidInputException exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReportCheating()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("No student selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string message =
                $"Are you sure you want to report {SelectedItem.FirstName} {SelectedItem.LastName} for cheating?";
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(message, "Cheating Report Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    _studentService.ReportCheating(SelectedItem.StudentId,_examId);   
                    MessageBox.Show("Successfully reported cheating.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshStudents();
                }
                catch (InvalidInputException exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshStudents()
        {
            Students.Clear();
            Exam exam = _examService.GetById(_examId)!;

            foreach (int studentId in exam.StudentIds)
            {
                Student student = _userService.GetById(studentId) as Student ??
                                  throw new InvalidInputException("Student doesn't exist.");
                ExamGrade? examGrade;
                if (student.ExamGradeIds.ContainsKey(_examId))
                    examGrade = _examGradeService.GetById(student.ExamGradeIds[_examId]) ??
                                throw new InvalidInputException("Exam grade doesn't exist");
                else
                    examGrade = null;

                Students.Add(new StudentExamGradeViewModel(student,examGrade));
            }
        }
    }
}
