using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;
using LangLang.Views.StudentViews;
using LangLang.Views.TeacherViews;
using LangLang.Views.DirectorViews;
using LangLang.Views.UserViews;

namespace LangLang.ViewModels.UserViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IUserService _userService = ServiceProvider.GetRequiredService<IUserService>();
    private readonly IStudentService _studentService = ServiceProvider.GetRequiredService<IStudentService>();
    private readonly ICourseService _courseService = ServiceProvider.GetRequiredService<ICourseService>();

    private readonly Window _loginWindow;

    public string? Email { get; set; }
    public string? Password { get; set; }

    public ICommand LoginCommand { get; }
    public ICommand NavigateToRegisterCommand { get; }

    public MainViewModel(Window loginWindow)
    {
        _loginWindow = loginWindow;
        NavigateToRegisterCommand = new RelayCommand(NavigateToRegister);
        LoginCommand = new RelayCommand(Login);
    }

    private void NavigateToRegister()
    {
        new RegisterView().Show();
    }
    private void Login()
    {
        User? user = _userService.Login(Email!, Password!);

        switch (user)
        {
            case null:
                MessageBox.Show("Invalid email or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            case Student student:
                _studentService.CheckIfFirstInMonth();
                ReviewTeacher(student);

                new StudentView().Show();
                break;
            case Director:
                new DirectorMainMenu().Show();
                break;
            case Teacher:
                new TeacherMenu().Show();
                break;
        }

        _loginWindow.Close();
        Application.Current.MainWindow?.Close();
    }

    // If current course is finished, show a dialog to rate the teacher
    private void ReviewTeacher(Student student)
    {
        if (student.ActiveCourseId is null || !_courseService.GetById(student.ActiveCourseId!.Value)!.IsFinished) return;
        
        var dialog = new TeacherReviewModal();
        if (dialog.ShowDialog()!.Value)
            _studentService.ReviewTeacher(student.Id, dialog.Response);
    }
}