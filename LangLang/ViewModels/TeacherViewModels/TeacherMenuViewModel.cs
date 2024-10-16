using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;
using LangLang.Views.CourseViews;
using LangLang.Views.ExamViews;
using LangLang.Views.TeacherViews;

namespace LangLang.ViewModels.TeacherViewModels
{
    class TeacherMenuViewModel : ViewModelBase
    {
        private readonly IUserService _userService = ServiceProvider.GetRequiredService<IUserService>();

        private readonly Teacher _teacher = UserService.LoggedInUser as Teacher ??
                                            throw new InvalidOperationException("No one is logged in.");

        private readonly Window _teacherMenuWindow;

        public TeacherMenuViewModel(Window teacherMenuWindow)
        {
            _teacherMenuWindow = teacherMenuWindow;

            CourseCommand = new RelayCommand(Course);
            ExamCommand = new RelayCommand(Exam);
            LogOutCommand = new RelayCommand(LogOut);
            StartableExamsCommand = new RelayCommand(StartableExams);
            CurrentExamCommand = new RelayCommand(CurrentExam);
            StartableCoursesCommand = new RelayCommand(StartableCourses);
            ActiveCoursesCommand = new RelayCommand(ActiveCourses);
            CoursesWithWithdrawalsCommand = new RelayCommand(CoursesWithWithdrawals);
        }

        public ICommand CourseCommand { get; }

        private void Course()
        {
            var newWindow = new CourseListingView();
            newWindow.Show();
        }

        public ICommand ExamCommand { get; }

        private void Exam()
        {
            var newWindow = new ExamView();
            newWindow.Show();
        }

        public ICommand LogOutCommand { get; }

        private void LogOut()
        {
            _userService.Logout();
            new MainWindow().Show();
            _teacherMenuWindow.Close();
        }

        public ICommand StartableExamsCommand { get; }
        private void StartableExams()
        {
            var newWindow = new StartableExamsView();
            newWindow.Show();
        }

        public ICommand CurrentExamCommand { get; }
        private void CurrentExam()
        {
            try
            {
                var newWindow = new CurrentExamView();
                newWindow.Show();
            }
            catch (InvalidInputException exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public ICommand StartableCoursesCommand { get; }
        private void StartableCourses()
        {
            var newWindow = new StartableCoursesView();
            newWindow.Show();
        }

        public ICommand CoursesWithWithdrawalsCommand { get; }
        private void CoursesWithWithdrawals()
        {
            var newWindow = new CoursesWithStudentWithdrawalsView();
            newWindow.Show();
        }
        public ICommand ActiveCoursesCommand { get; }
        private void ActiveCourses()
        {
            var newWindow = new ActiveCoursesView(); 
            newWindow.Show();
        }
    }
}