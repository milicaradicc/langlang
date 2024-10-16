using LangLang.Models;
using LangLang.Services;
using LangLang.Views.CourseViews;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Data;
using System.Linq;
using System.ComponentModel;

namespace LangLang.ViewModels.CourseViewModels
{
    class ActiveCoursesViewModel : ViewModelBase
    {
        private readonly ICourseService _courseService;
        private readonly Teacher _teacher = UserService.LoggedInUser as Teacher ??
                                            throw new InvalidOperationException("No one is logged in.");
        private readonly ObservableCollection<CourseViewModel> _activeCourses;

        public ActiveCoursesViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            
            _activeCourses = new ObservableCollection<CourseViewModel>(_courseService.GetActiveCourses(_teacher.Id).Select(course => new CourseViewModel(course)));
            ActiveCoursesCollectionView = CollectionViewSource.GetDefaultView(_activeCourses);
            SeeStudentsListCommand = new RelayCommand(SeeStudentsList);
        }
        public ICollectionView ActiveCoursesCollectionView { get; }

        public ObservableCollection<CourseViewModel> ActiveCourses => _activeCourses;
        public CourseViewModel? SelectedItem { get; set; }
        public ICommand SeeStudentsListCommand { get; set; }

        private void SeeStudentsList()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("No course selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newWindow = new CurrentCourseView(SelectedItem.Id);

            newWindow.ShowDialog();
            RefreshActiveCourses();
        }

        private void RefreshActiveCourses()
        {
            ActiveCourses.Clear();
            _courseService.GetActiveCourses(_teacher.Id).ForEach(course => ActiveCourses.Add(new CourseViewModel(course)));
            ActiveCoursesCollectionView.Refresh();
        }
    }
}

