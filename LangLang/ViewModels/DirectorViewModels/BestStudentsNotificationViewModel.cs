using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;
using LangLang.ViewModels.CourseViewModels;

namespace LangLang.ViewModels.DirectorViewModels;

public class BestStudentsNotificationViewModel : ViewModelBase
{
    private readonly ICourseService _courseService;
    private readonly IDirectorService _directorService;
    
    public ObservableCollection<CourseViewModel> FinishedCourses { get; }
    public CourseViewModel? SelectedCourse { get; set; }
    public bool KnowledgePoints { get; set; }
    
    public RelayCommand NotifyStudentsCommand { get; }
    
    public BestStudentsNotificationViewModel(ICourseService courseService, IDirectorService directorService)
    {
        _courseService = courseService;
        _directorService = directorService;
        
        FinishedCourses = new ObservableCollection<CourseViewModel>(_courseService.GetFinishedCourses().Select(course => new CourseViewModel(course)));
        NotifyStudentsCommand = new RelayCommand(NotifyStudents);
    }
    
    private void NotifyStudents()
    {
        try
        {
            if (SelectedCourse is null)
                throw new InvalidInputException("No course selected.");
            
            _directorService.NotifyBestStudents(SelectedCourse.Id, KnowledgePoints);
            FinishedCourses.Remove(SelectedCourse);
            
            MessageBox.Show("Best students notified successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (InvalidInputException e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}