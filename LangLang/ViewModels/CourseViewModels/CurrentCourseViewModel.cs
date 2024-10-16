using GalaSoft.MvvmLight;
using LangLang.Models;
using LangLang.Repositories;
using LangLang.Services;
using LangLang.ViewModels.StudentViewModels;
using LangLang.Views.CourseViews;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;

namespace LangLang.ViewModels.CourseViewModels
{
    class CurrentCourseViewModel : ViewModelBase
    {
        private readonly IUserRepository _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
        private readonly ICourseRepository _courseRepository = ServiceProvider.GetRequiredService<ICourseRepository>();
        private readonly ICourseGradeRepository _courseGradeRepository = ServiceProvider.GetRequiredService<ICourseGradeRepository>();
        
        private readonly IStudentService _studentService = ServiceProvider.GetRequiredService<IStudentService>();
        private readonly ITeacherService _teacherService = ServiceProvider.GetRequiredService<ITeacherService>();
        
        private readonly int _courseId;
        private readonly Window _currentWindow;

        public CurrentCourseViewModel(int courseId, Window currentWindow)
        {
            _courseId = courseId;
            RefreshStudents();
            PenalizeCommand = new RelayCommand(Penalize);
            EnterGradeCommand = new RelayCommand(EnterGrade);
            FinishCourseCommand = new RelayCommand(FinishCourse);
            _currentWindow = currentWindow;
        }

        public ObservableCollection<StudentCourseGradeViewModel> Students { get; set; } = new();
        public StudentCourseGradeViewModel? SelectedItem { get; set; }
        public PenaltyPointReason? SelectedPenaltyPointReason {  get; set; }

        public ICommand PenalizeCommand { get; set; }
        public ICommand FinishCourseCommand { get; set; }
        public ICommand EnterGradeCommand { get; set; }

        public IEnumerable<PenaltyPointReason?> PenaltyPointReasonValues => Enum.GetValues(typeof(PenaltyPointReason))
            .Cast<PenaltyPointReason?>();

        private void EnterGrade()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("No student selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newWindow = new AddCourseGradeView(SelectedItem.Id, _courseId);
            newWindow.ShowDialog();
            RefreshStudents();
        }

        private void FinishCourse()
        {
            try {
                _teacherService.FinishCourse(_courseId);
                MessageBox.Show("Successfully finished course.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _currentWindow.Close();
            }
            catch (InvalidInputException exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Penalize()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("No student selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SelectedPenaltyPointReason == null)
            {
                MessageBox.Show("Must input penalty reason.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string message =
                $"Are you sure you want to give penalty point to {SelectedItem.FirstName} {SelectedItem.LastName}. Reason : {SelectedPenaltyPointReason}?";
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(message, "Penalty point confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    _studentService.AddPenaltyPoint(SelectedItem.Id, (PenaltyPointReason)SelectedPenaltyPointReason, _courseId, UserService.LoggedInUser!.Id, DateOnly.FromDateTime(DateTime.Now));
                    MessageBox.Show("Successfully given penalty point.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
            Course? course = _courseRepository.GetById(_courseId);

            foreach (int studentId in course!.Students.Keys)
            {
                Student student = _userRepository.GetById(studentId) as Student ??
                                  throw new InvalidInputException("Student doesn't exist.");
                CourseGrade? courseGrade;
                if (student.CourseGradeIds.ContainsKey(_courseId))
                    courseGrade = _courseGradeRepository.GetById(student.CourseGradeIds[_courseId]) ??
                                throw new InvalidInputException("Course grade doesn't exist");
                else
                    courseGrade = null;

                Students.Add(new StudentCourseGradeViewModel(student, courseGrade!));
            }
        }
    }
}

