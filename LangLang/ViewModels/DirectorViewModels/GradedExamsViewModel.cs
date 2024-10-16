using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;
using LangLang.ViewModels.ExamViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace LangLang.ViewModels.DirectorViewModels;

public class GradedExamsViewModel
{

    private readonly IExamService _examService;

    public GradedExamsViewModel(IExamService examService)
    {
        _examService = examService;
        
        UngradedExams = new ObservableCollection<ExamViewModel>(_examService.GetUngradedExams().Select(exam => new ExamViewModel(exam)));
        ExamCollectionView = CollectionViewSource.GetDefaultView(UngradedExams);

        SendGrades = new RelayCommand(Send);
    }
    public ObservableCollection<ExamViewModel> UngradedExams { get; }
    public ExamViewModel? SelectedItem { get; set; }
    public ICollectionView ExamCollectionView { get; set; }
    public ICommand SendGrades { get; }

    private void Send()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("No exam selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        try
        {
            _examService.SendGrades(SelectedItem.Id);
            UpdateExamList();
            MessageBox.Show("Grades sent successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void UpdateExamList()
    {
        UngradedExams.Clear();
        _examService.GetUngradedExams().ForEach(exam => UngradedExams.Add(new ExamViewModel(exam)));
        ExamCollectionView.Refresh();
    }

}