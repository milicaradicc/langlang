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
using LangLang.ViewModels.CourseViewModels;

namespace LangLang.ViewModels.StudentViewModels;

public class StudentCourseViewModel : ViewModelBase
{
    private readonly Student _student = UserService.LoggedInUser as Student ??
                                        throw new InvalidOperationException("No one is logged in.");

    private readonly ILanguageService _languageService = ServiceProvider.GetRequiredService<ILanguageService>();
    private readonly IStudentService _studentService = ServiceProvider.GetRequiredService<IStudentService>();
    private readonly ICourseService _couseService = ServiceProvider.GetRequiredService<ICourseService>();

    private string? _selectedLanguageName;
    private string? _selectedLanguageLevel;
    private DateTime _selectedDate;
    private string? _selectedDuration;
    private string? _selectedFormat;
    private string? _selectedSortingWay;
    private string? _selectedPropertyName;

    private readonly bool _applied;
    
    private int _currentPage = 1;
    private const int ItemsPerPage = 5;
    private int _totalPages;
    private readonly int _totalCourses;

    public StudentCourseViewModel(bool applied = false)
    {
        _applied = applied;
        
        _totalCourses = (applied
            ? _couseService.GetAppliedCourses(_student.Id)
            : _couseService.GetAvailableCourses(_student.Id)).Count;
        CalculateTotalPages();
        
        AvailableCourses = new ObservableCollection<CourseViewModel>(
            (applied
                ? _couseService.GetAppliedCourses(_student.Id, _currentPage, ItemsPerPage)
                : _couseService.GetAvailableCourses(_student.Id, _currentPage, ItemsPerPage))
            .Select(course => new CourseViewModel(course)));
        CoursesCollectionView = CollectionViewSource.GetDefaultView(AvailableCourses);
        CoursesCollectionView.Filter = FilterCourses;

        ResetFiltersCommand = new RelayCommand(ResetFilters);
        ApplyForCourseCommand = new RelayCommand(ApplyForCourse);
        WithdrawFromCourseCommand = new RelayCommand(WithdrawFromCourse);
        PreviousPageCommand = new RelayCommand(PreviousPage);
        NextPageCommand = new RelayCommand(NextPage);
    }

    public ICommand ResetFiltersCommand { get; }
    public ICommand ApplyForCourseCommand { get; }
    public ICommand WithdrawFromCourseCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }

    public CourseViewModel? SelectedCourse { get; set; }
    public ObservableCollection<CourseViewModel> AvailableCourses { get; }
    public ICollectionView CoursesCollectionView { get; }
    public IEnumerable<string> LanguageNameValues => _languageService.GetAllNames();
    public IEnumerable<string> LanguageLevelValues => Enum.GetNames(typeof(LanguageLevel));
    public IEnumerable<string> FormatValues => new List<string> { "online", "in-person" };
    public static IEnumerable<String> SortingWays => new List<String> { "ascending", "descending" };
    public static IEnumerable<String> PropertyNames => new List<String> { "LanguageName","LanguageLevel", "StartDate" };

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
            CoursesCollectionView.SortDescriptions.Clear();
            if (value!.Equals("ascending"))
            {
                CoursesCollectionView.SortDescriptions.Add(new SortDescription(_selectedPropertyName, ListSortDirection.Ascending));
                return;
            }
            CoursesCollectionView.SortDescriptions.Add(new SortDescription(_selectedPropertyName, ListSortDirection.Descending));
        }
    }
    public string? SelectedPropertyName
    {
        get => _selectedPropertyName;
        set
        {
            _selectedPropertyName = value;
            CoursesCollectionView.SortDescriptions.Clear();
            if (value!.Equals("ascending"))
            {
                CoursesCollectionView.SortDescriptions.Add(new SortDescription(_selectedPropertyName, ListSortDirection.Ascending));
                return;
            }
            CoursesCollectionView.SortDescriptions.Add(new SortDescription(_selectedPropertyName, ListSortDirection.Descending));
        }
    }

    private bool FilterCourses(object obj)
    {
        if (obj is CourseViewModel courseViewModel)
        {
            return courseViewModel.FilterLanguageName(SelectedLanguageName) &&
                   courseViewModel.FilterLanguageLevel(SelectedLanguageLevel) &&
                   courseViewModel.FilterStartDate(SelectedDate) &&
                   courseViewModel.FilterDuration(SelectedDuration) &&
                   courseViewModel.FilterFormat(SelectedFormat);
        }

        return false;
    }

    private void PreviousPage()
    {
        if (_currentPage < 2) { return; }
        _currentPage--;
        RefreshCourses(_applied);
    }

    private void NextPage()
    {
        if (_currentPage + 1 > _totalPages) { return; }
        _currentPage++;
        RefreshCourses(_applied);
    }
    
    private void CalculateTotalPages()
    {
        _totalPages = (int)Math.Ceiling((double)_totalCourses / ItemsPerPage);
    }

    private void ResetFilters()
    {
        SelectedLanguageLevel = null!;
        SelectedLanguageName = null!;
        SelectedDate = DateTime.MinValue;
        SelectedDuration = null!;
        SelectedFormat = null!;
    }

    private void ApplyForCourse()
    {
        if (SelectedCourse == null)
            MessageBox.Show("Please select a course.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        try
        {
            _studentService.ApplyForCourse(_student.Id, SelectedCourse!.Id);
            MessageBox.Show("You have successfully applied for the course.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
            RefreshCourses(_applied);
        }
        catch (InvalidInputException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void WithdrawFromCourse()
    {
        if (SelectedCourse == null)
            MessageBox.Show("Please select a course.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        try
        {
            _studentService.WithdrawFromCourse(_student.Id, SelectedCourse!.Id);
            MessageBox.Show("You have successfully withdrawn from the course.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
            RefreshCourses(_applied);
        }
        catch (InvalidInputException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RefreshCourses(bool applied)
    {
        AvailableCourses.Clear();
        (applied ? _couseService.GetAppliedCourses(_student.Id, _currentPage, ItemsPerPage) : _couseService.GetAvailableCourses(_student.Id, _currentPage, ItemsPerPage))
            .ForEach(course => AvailableCourses.Add(new CourseViewModel(course)));
        CoursesCollectionView.Refresh();
    }
}