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
using LangLang.ViewModels.ExamViewModels;

namespace LangLang.ViewModels.StudentViewModels;

public class StudentExamViewModel : ViewModelBase
{
    private readonly ILanguageService _languageService;
    private readonly IStudentService _studentService;
    private readonly IExamService _examService;
    private readonly Student _student = UserService.LoggedInUser as Student ??
                              throw new InvalidOperationException("No one is logged in.");
    
    private string? _languageNameSelected;
    private string? _languageLevelSelected;
    private DateTime _dateSelected;
    private string? _selectedSortingWay;
    private string? _selectedPropertyName;
    
    private int _currentPage = 1;
    private const int ItemsPerPage = 5;
    private int _totalPages;
    private readonly int _totalCourses;
    
    public StudentExamViewModel(ILanguageService languageService, IStudentService studentService, IExamService examService)
    {
        _languageService = languageService;
        _studentService = studentService;
        _examService = examService;
        
        _totalCourses = _examService.GetAvailableExams(_student.Id).Count;
        CalculateTotalPages();
        
        AvailableExams = new ObservableCollection<ExamViewModel>(_examService.GetAvailableExams(_student.Id, _currentPage, ItemsPerPage).Select(exam => new ExamViewModel(exam)));
        ExamCollectionView = CollectionViewSource.GetDefaultView(AvailableExams);
        ExamCollectionView.Filter = FilterExams;
        
        ResetFiltersCommand = new RelayCommand(ResetFilters);
        ApplyForExamCommand = new RelayCommand(Apply);
        PreviousPageCommand = new RelayCommand(PreviousPage);
        NextPageCommand = new RelayCommand(NextPage);
    }
    
    public ObservableCollection<ExamViewModel> AvailableExams { get; }
    public ExamViewModel? SelectedItem { get; set; }
    public ICollectionView ExamCollectionView { get; set; }
    public IEnumerable<LanguageLevel> LanguageLevelValues => Enum.GetValues(typeof(LanguageLevel)).Cast<LanguageLevel>();
    public IEnumerable<string> LanguageNames => _languageService.GetAllNames();
    public static IEnumerable<String> SortingWays => new List<String> { "ascending", "descending" };
    public static IEnumerable<String> PropertyNames => new List<String> { "LanguageName","LanguageLevel", "ExamDate" };
    
    public ICommand ResetFiltersCommand { get; }
    public ICommand ApplyForExamCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }


    public string? LanguageNameSelected
    {
        get => _languageNameSelected;
        set
        {
            _languageNameSelected = value;
            ExamCollectionView.Refresh();
        }
    }
    
    public string? LanguageLevelSelected
    {
        get => _languageLevelSelected;
        set
        {
            _languageLevelSelected = value;
            ExamCollectionView.Refresh();
        }
    }
    
    public DateTime DateSelected
    {
        get => _dateSelected;
        set
        {
            _dateSelected = value;
            ExamCollectionView.Refresh();
        }
    }
    
    public string? SelectedSortingWay
    {
        get => _selectedSortingWay;
        set
        {
            _selectedSortingWay = value;
            ExamCollectionView.SortDescriptions.Clear();
            if (value!.Equals("ascending"))
            {
                ExamCollectionView.SortDescriptions.Add(new SortDescription(_selectedPropertyName, ListSortDirection.Ascending));
                return;
            }
            ExamCollectionView.SortDescriptions.Add(new SortDescription(_selectedPropertyName, ListSortDirection.Descending));
        }
    }
    public string? SelectedPropertyName
    {
        get => _selectedPropertyName;
        set
        {
            _selectedPropertyName = value;
            ExamCollectionView.SortDescriptions.Clear();
            if (value!.Equals("ascending"))
            {
                ExamCollectionView.SortDescriptions.Add(new SortDescription(_selectedPropertyName, ListSortDirection.Ascending));
                return;
            }
            ExamCollectionView.SortDescriptions.Add(new SortDescription(_selectedPropertyName, ListSortDirection.Descending));
        }
    }
    
    private bool FilterExams(object obj)
    {
        if (obj is ExamViewModel examViewModel)
        {
            return examViewModel.FilterLanguageName(LanguageNameSelected) &&
                   examViewModel.FilterLevel(LanguageLevelSelected) &&
                   examViewModel.FilterDateHeld(DateSelected);
        }
        return false;
    }
    
    private void ResetFilters()
    {
        LanguageNameSelected = null!;
        LanguageLevelSelected = null!;
        DateSelected = DateTime.MinValue;
    }

    private void Apply()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("No exam selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            Exam exam = _examService.GetById(SelectedItem.Id)!;
            _studentService.ApplyStudentExam(_student, exam.Id);
            RefreshExams();
            MessageBox.Show("You have applied for the exam.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception err)
        {
            MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void PreviousPage()
    {
        if (_currentPage < 2) { return; }
        _currentPage--;
        RefreshExams();
    }

    private void NextPage()
    {
        if (_currentPage + 1 > _totalPages) { return; }
        _currentPage++;
        RefreshExams();
    }
    
    private void CalculateTotalPages()
    {
        _totalPages = (int)Math.Ceiling((double)_totalCourses / ItemsPerPage);
    }
    
    private void RefreshExams()
    {
        AvailableExams.Clear();
        _examService.GetAvailableExams(_student.Id, _currentPage, ItemsPerPage)
            .ForEach(exam => AvailableExams.Add(new ExamViewModel(exam)));
        ExamCollectionView.Refresh();
    }
}