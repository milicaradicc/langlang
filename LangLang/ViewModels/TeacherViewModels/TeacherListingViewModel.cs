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
using LangLang.Views.DirectorViews;

namespace LangLang.ViewModels.TeacherViewModels
{
    public class TeacherListingViewModel : ViewModelBase
    {
        private readonly IUserService _userService = ServiceProvider.GetRequiredService<IUserService>();
        private readonly ITeacherService _teacherService = ServiceProvider.GetRequiredService<ITeacherService>();
        private readonly ILanguageService _languageService = ServiceProvider.GetRequiredService<ILanguageService>();

        private string? _selectedLanguageName;
        private string? _selectedLanguageLevel;
        private DateTime _selectedDateCreated;
        private string? _selectedSortingWay;
        private string? _selectedPropertyName;

        private int _currentPage;
        private readonly int _itemsPerPage = 2;
        private int _totalPages;
        private readonly int _totalTeachers;

        private readonly ObservableCollection<TeacherViewModel> _teachers;
        private readonly Window _teacherListingWindow;

        public TeacherListingViewModel(Window teacherListingWindow)
        {
            _currentPage = 1;
            _totalTeachers = _teacherService.Count();
            CalculateTotalPages();

            _teacherListingWindow = teacherListingWindow;

            _teachers = new ObservableCollection<TeacherViewModel>(_teacherService.GetPage(_currentPage,_itemsPerPage)
                .Select(teacher => new TeacherViewModel(teacher)));
            TeachersCollectionView = CollectionViewSource.GetDefaultView(_teachers);

            EditCommand = new RelayCommand(EditTeacher);
            AddCommand = new RelayCommand(AddTeacher);
            DeleteCommand = new RelayCommand(DeleteTeacher);
            LogOutCommand = new RelayCommand(LogOut);

            TeachersCollectionView.Filter = FilterTeachers;

            PreviousPageCommand = new RelayCommand(PreviousPage);
            NextPageCommand = new RelayCommand(NextPage);
        }

        public ICollectionView TeachersCollectionView { get; }
        public IEnumerable<TeacherViewModel> Teachers => _teachers;
        public IEnumerable<String> LanguageNameValues => _languageService.GetAllNames();
        public static IEnumerable<String> LanguageLevelValues => Enum.GetNames(typeof(LanguageLevel));
        public static IEnumerable<String> SortingWays => new List<String> { "ascending", "descending" };
        public static IEnumerable<String> PropertyNames => new List<String> { "Name", "DateAdded" };
        public ICommand EditCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand LogOutCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public TeacherViewModel? SelectedItem { get; set; }

        public string? SelectedLanguageName
        {
            get => _selectedLanguageName;
            set
            {
                _selectedLanguageName = value;
                TeachersCollectionView.Refresh();
            }
        }

        public string? SelectedLanguageLevel
        {
            get => _selectedLanguageLevel;
            set
            {
                _selectedLanguageLevel = value;
                TeachersCollectionView.Refresh();
            }
        }

        public DateTime SelectedDateCreated
        {
            get => _selectedDateCreated;
            set
            {
                _selectedDateCreated = value;
                TeachersCollectionView.Refresh();
            }
        }

        public string? SelectedSortingWay
        {
            get => _selectedSortingWay;
            set
            {
                _selectedSortingWay = value;
                SortTeachers();
            }
        }
        public string? SelectedPropertyName
        {
            get => _selectedPropertyName;
            set
            {
                _selectedPropertyName = value;
                SortTeachers();
            }
        }

        private void SortTeachers()
        {
            if (SelectedPropertyName == null || SelectedSortingWay == null)
            {
                RefreshTeachers();
                return;
            }

            RefreshTeachers(SelectedPropertyName, SelectedSortingWay);
        }

        private void NextPage()
        {
            if (_currentPage + 1 > _totalPages) { return; }
            _currentPage++;
            RefreshTeachers(SelectedPropertyName!, SelectedSortingWay!);
        }

        private void PreviousPage()
        {
            if (_currentPage < 2) { return; }
            _currentPage--;
            RefreshTeachers(SelectedPropertyName!, SelectedSortingWay!);
        }

        private void RefreshTeachers(string propertyName = "", string sortingWay = "ascending")
        {
            _teachers.Clear();
            _teacherService.GetPage(_currentPage, _itemsPerPage, propertyName, sortingWay).ForEach(teacher => _teachers.Add(new TeacherViewModel(teacher)));
            TeachersCollectionView.Refresh();
        }

        private bool FilterTeachers(object obj)
        {
            if (obj is TeacherViewModel teacherViewModel)
            {
                return teacherViewModel.FilterLanguageName(SelectedLanguageName) &&
                       teacherViewModel.FilterLanguageLevel(SelectedLanguageLevel) &&
                       teacherViewModel.FilterDateCreated(SelectedDateCreated);
            }

            return false;
        }

        private void EditTeacher()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("No teacher selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_userService.GetById(SelectedItem!.Id) is not Teacher teacher)
            {
                MessageBox.Show("Teacher not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newWindow = new EditTeacherView(teacher);

            newWindow.ShowDialog();
            UpdateTeacherList();
        }

        private void AddTeacher()
        {
            var newWindow = new AddTeacherView();
            newWindow.ShowDialog();
            UpdateTeacherList();
        }

        // delete all courses and exams that the teacher created
        // if they are on active course it can not be deleted
        // if they are on courses or exams that director chose, just remove them
        private void DeleteTeacher()
        {
            try
            {
                if (SelectedItem == null)
                    throw new Exception("No teacher selected");

                _userService.Delete(SelectedItem.Id);

                _teachers.Remove(SelectedItem);
                TeachersCollectionView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogOut()
        {
            _userService.Logout();
            new MainWindow().Show();
            _teacherListingWindow.Close();
        }

        private void UpdateTeacherList()
        {
            _teachers.Clear();
            _teacherService.GetAll().ForEach(teacher => _teachers.Add(new TeacherViewModel(teacher)));
            TeachersCollectionView.Refresh();
        }

        private void CalculateTotalPages()
        {
            _totalPages = (int)Math.Ceiling((double)_totalTeachers / _itemsPerPage);
        }
    }

}