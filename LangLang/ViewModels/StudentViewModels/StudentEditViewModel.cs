using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;

namespace LangLang.ViewModels.StudentViewModels;

public class StudentEditViewModel : ViewModelBase
{
    private readonly IUserService _userService = ServiceProvider.GetRequiredService<IUserService>();

    private readonly Student _student =
        UserService.LoggedInUser as Student ?? throw new InvalidInputException("No one is logged in.");

    private readonly Window _editWindow;

    public StudentEditViewModel(Window editWindow)
    {
        _editWindow = editWindow;

        FirstName = _student.FirstName;
        LastName = _student.LastName;
        Password = _student.Password;
        Phone = _student.Phone;
        Gender = _student.Gender;
        Education = _student.Education;

        SaveCommand = new RelayCommand(Save);
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public Gender Gender { get; set; }
    public Education? Education { get; set; }

    public Array GenderValues => Enum.GetValues(typeof(Gender));
    public Array EducationValues => Enum.GetValues(typeof(Education));

    public ICommand SaveCommand { get; }

    private void Save()
    {
        try
        {
            _userService.Update(_student.Id, FirstName, LastName, Password, Gender, Phone, Education);
            _editWindow.Close();
            MessageBox.Show("Information successfully edited.", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
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