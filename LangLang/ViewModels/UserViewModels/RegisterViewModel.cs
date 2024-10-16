using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;
using LangLang.Views.StudentViews;

namespace LangLang.ViewModels.UserViewModels;

public class RegisterViewModel : ViewModelBase
{
    private readonly IUserService _userService = ServiceProvider.GetRequiredService<IUserService>();

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public Gender Gender { get; set; }
    public string? Phone { get; set; }
    public Education Education { get; set; }

    public IEnumerable<Gender> GenderValues => Enum.GetValues(typeof(Gender)).Cast<Gender>();
    public IEnumerable<Education> EducationValues => Enum.GetValues(typeof(Education)).Cast<Education>();

    public ICommand RegisterCommand { get; }

    private readonly Window _registerWindow;

    public RegisterViewModel(Window registerWindow)
    {
        _registerWindow = registerWindow;
        RegisterCommand = new RelayCommand(Register);
    }

    private void Register()
    {
        try
        {
            _userService.Add(FirstName!, LastName!, Email!, Password!, Gender, Phone!, Education);

            MessageBox.Show("User registered successfully.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);

            _userService.Login(Email!, Password!);
            
            new StudentView().Show();
            _registerWindow.Close();
            Application.Current.MainWindow?.Close();
        }
        catch (InvalidInputException exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (ArgumentNullException exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}