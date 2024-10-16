using LangLang.Models;
using LangLang.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using LangLang.Views.CourseViews;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;

namespace LangLang.ViewModels.CourseViewModels
{
    class StartableCoursesViewModel : ViewModelBase
    {
        private readonly ICourseService _courseService;
        private readonly Teacher _teacher = UserService.LoggedInUser as Teacher ??
                                            throw new InvalidOperationException("No one is logged in.");

        public StartableCoursesViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            
            StartableCourses = new ObservableCollection<CourseViewModel>();
            RefreshStartableCourses();
            StartCourseCommand = new RelayCommand(StartCourse);
        }

        public ObservableCollection<CourseViewModel> StartableCourses { get; set; }
        public CourseViewModel? SelectedItem { get; set; }
        public ICommand StartCourseCommand { get; set; }

        private void StartCourse()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("No course selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newWindow = new StartCourseView(SelectedItem.Id);

            newWindow.ShowDialog();
            RefreshStartableCourses();
        }

        private void RefreshStartableCourses()
        {
            StartableCourses.Clear();
            _courseService.GetStartableCourses(_teacher.Id).ForEach(course => StartableCourses.Add(new CourseViewModel(course)));
        }
    }

}

