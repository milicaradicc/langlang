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

public class AppliedExamListingViewModel : ViewModelBase
{
    private readonly ILanguageService _languageService;
    private readonly IStudentService _studentService;
    private readonly IExamService _examService;
    
    private string? _languageNameSelected;
    private string? _languageLevelSelected;
    private DateTime _dateSelected;
    private string? _selectedSortingWay;
    private string? _selectedPropertyName;
    
    private readonly Student _student = UserService.LoggedInUser as Student ?? throw new InvalidInputException("No one is logged in.");
    
    private int _currentPage = 1;
    private const int ItemsPerPage = 5;
    private int _totalPages;
    private readonly int _totalCourses;
    
    public AppliedExamListingViewModel(ILanguageService languageService, IStudentService studentService, IExamService examService)
    {
        _languageService = languageService;
        _studentService = studentService;
        _examService = examService;
        
        _totalCourses = _examService.GetAppliedExams(_student.Id).Count;
        CalculateTotalPages();
        
        AppliedExams = new ObservableCollection<ExamViewModel>(_examService.GetAppliedExams(_student.Id, _currentPage, ItemsPerPage).Select(exam => new ExamViewModel(exam)));
        ExamCollectionView = CollectionViewSource.GetDefaultView(AppliedExams);
        ExamCollectionView.Filter = FilterExams;
        ResetFiltersCommand = new RelayCommand(ResetFilters);

        DropExamCommand = new RelayCommand(Drop);
    }

    public ObservableCollection<ExamViewModel> AppliedExams { get; }
    public ExamViewModel? SelectedItem { get; set; }
    public ICollectionView ExamCollectionView { get; set; }
    public IEnumerable<LanguageLevel> LanguageLevelValues => Enum.GetValues(typeof(LanguageLevel)).Cast<LanguageLevel>();
    public IEnumerable<string> LanguageNames => _languageService.GetAllNames();
    public static IEnumerable<String> SortingWays => new List<String> { "ascending", "descending" };
    public static IEnumerable<String> PropertyNames => new List<String> { "LanguageName","LanguageLevel", "ExamDate" };
    
    public ICommand ResetFiltersCommand { get; }
    public ICommand DropExamCommand { get; }


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

    private void ResetFilters()
    {
        LanguageNameSelected = null!;
        LanguageLevelSelected = null!;
        DateSelected = DateTime.MinValue;
    }

    private void Drop()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("No exam selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        try
        {
            Exam exam = _examService.GetById(SelectedItem.Id)!;
            _studentService.DropExam(exam, _student);
            RefreshExams();
            MessageBox.Show("Exam droped successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        } catch(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        AppliedExams.Clear();
        _examService.GetAppliedExams(_student.Id, _currentPage, ItemsPerPage).ForEach(exam => AppliedExams.Add(new ExamViewModel(exam)));
        ExamCollectionView.Refresh();
    }
}