using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;
using LangLang.Views.CourseViews;

namespace LangLang.ViewModels.CourseViewModels
{
    public class CourseListingViewModel : ViewModelBase
    {
        private readonly ITeacherService _teacherService;
        private readonly ILanguageService _languageService;
        private readonly ICourseService _courseService;

        private readonly Teacher _teacher = UserService.LoggedInUser as Teacher ??
                                            throw new InvalidOperationException("No one is logged in.");

        private readonly ObservableCollection<CourseViewModel> _courses;

        private string? _selectedLanguageName;
        private string? _selectedLanguageLevel;
        private DateTime _selectedDate;
        private string? _selectedDuration;
        private string? _selectedFormat;
        private string? _selectedSortingWay;
        private string? _selectedPropertyName;
        
        private int _currentPage;
        private  readonly int _itemsPerPage = 2;
        private int _totalPages;
        private int _totalCourses;
        
        public CourseListingViewModel(ITeacherService teacherService, ILanguageService languageService, ICourseService courseService)
        {
            _teacherService = teacherService;
            _languageService = languageService;
            _courseService = courseService;
            
            _currentPage = 1;
            _totalCourses = _teacherService.GetCourseCount(_teacher.Id);
            CalculateTotalPages();
            _courses = new ObservableCollection<CourseViewModel>(_teacherService.GetCourses(_teacher.Id, _currentPage, _itemsPerPage)
                .Select(course => new CourseViewModel(course)));
            CoursesCollectionView = CollectionViewSource.GetDefaultView(_courses);
            CoursesCollectionView.Filter = FilterCourses;
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            ResetFiltersCommand = new RelayCommand(ResetFilters);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            NextPageCommand = new RelayCommand(NextPage);
        }


        public CourseViewModel? SelectedItem { get; set; }

        public ICollectionView CoursesCollectionView { get; }
        public IEnumerable<String> LanguageNameValues => _languageService.GetAllNames();
        public static IEnumerable<String> LanguageLevelValues => Enum.GetNames(typeof(LanguageLevel));
        public static IEnumerable<String> FormatValues => new List<String> { "online", "in-person" };
        public static IEnumerable<String> SortingWays => new List<String> { "ascending", "descending" };
        public static IEnumerable<String> PropertyNames => new List<String> { "LanguageName","LanguageLevel", "StartDate" };
        public IEnumerable<CourseViewModel> Courses => _courses;

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public ICommand ResetFiltersCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public string? SelectedLanguageName
        {
            get => _selectedLanguageName;
            set
            {
                _selectedLanguageName = value;
                CoursesCollectionView.Refresh();
            }
        }

        public string? SelectedLanguageLevel
        {
            get => _selectedLanguageLevel;
            set
            {
                _selectedLanguageLevel = value;
                CoursesCollectionView.Refresh();
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                CoursesCollectionView.Refresh();
            }
        }

        public string? SelectedDuration
        {
            get => _selectedDuration;
            set
            {
                _selectedDuration = value;
                CoursesCollectionView.Refresh();
            }
        }

        public string? SelectedFormat
        {
            get => _selectedFormat;
            set
            {
                _selectedFormat = value;
                CoursesCollectionView.Refresh();
            }
        }

        public string? SelectedSortingWay
        {
            get => _selectedSortingWay;
            set
            {
                _selectedSortingWay = value;
                SortCourses();
            }
        }
        public string? SelectedPropertyName
        {
            get => _selectedPropertyName;
            set
            {
                _selectedPropertyName = value;
                SortCourses();
            }
        }
        private void SortCourses()
        {
            if (SelectedPropertyName == null || SelectedSortingWay == null)
            {
                RefreshCourses();
                return;
            }

            RefreshCourses(SelectedPropertyName, SelectedSortingWay);
        }
        private void Add()
        {
            var newWindow = new AddCourseView();
            newWindow.ShowDialog();
            _totalCourses = _teacherService.GetCourseCount(_teacher.Id);
            CalculateTotalPages();
            RefreshCourses();
        }

        private void Edit()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a course to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            new EditCourseView(_courseService.GetById(SelectedItem.Id)).ShowDialog();
            RefreshCourses();
        }

        private void Delete()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select an Course to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            Course course = _courseService.GetById(SelectedItem.Id) ?? throw new InvalidOperationException("Course doesn't exist.");
            _courseService.Delete(course.Id);
            _totalCourses--;
            CalculateTotalPages();
            RefreshCourses();

            MessageBox.Show("Course deleted successfully.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void CalculateTotalPages()
        {
            _totalPages = (int)Math.Ceiling((double)_totalCourses / _itemsPerPage);
        }
        private void NextPage()
        {
            if (_currentPage + 1 > _totalPages) { return; }
            _currentPage++;
            RefreshCourses(SelectedPropertyName!, SelectedSortingWay!);
        }

        private void PreviousPage()
        {
            if (_currentPage < 2) { return; }
            _currentPage--;
            RefreshCourses(SelectedPropertyName!, SelectedSortingWay!);
        }

        private bool FilterCourses(object obj)
        {
            if (obj is CourseViewModel courseViewModel)
            {
                return courseViewModel.FilterLanguageName(SelectedLanguageName) &&
                       courseViewModel.FilterLanguageLevel(SelectedLanguageLevel) &&
                       courseViewModel.FilterStartDate(SelectedDate) &&
                       courseViewModel.FilterDuration(SelectedDuration) &&
                       courseViewModel.FilterFormat(SelectedFormat) &&
                       courseViewModel.FilterTeacher(_teacher.Id);
            }
            return false;
        }

        private void RefreshCourses(string propertyName = "", string sortingWay = "ascending")
        {
            _courses.Clear();
            _teacherService.GetCourses(_teacher.Id, _currentPage, _itemsPerPage, propertyName, sortingWay).ForEach(course => _courses.Add(new CourseViewModel(course)));
            CoursesCollectionView.Refresh();
        }

        private void ResetFilters()
        {
            SelectedLanguageLevel = null!;
            SelectedLanguageName = null!;
            SelectedDate = DateTime.MinValue;
            SelectedDuration = null!;
            SelectedFormat = null!;
        }
    }
}
