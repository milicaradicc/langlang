using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;

namespace LangLang.ViewModels.DirectorViewModels
{
    internal class EditTeacherViewModel : ViewModelBase
    {
        private readonly IUserService _userService = ServiceProvider.GetRequiredService<IUserService>();

        private readonly Teacher _teacher;

        private readonly Window _editTeacherWindow;

        public EditTeacherViewModel(Teacher teacher, Window editTeacherWindow)
        {
            _editTeacherWindow = editTeacherWindow;
            _teacher = teacher;

            FirstName = _teacher.FirstName;
            LastName = _teacher.LastName;
            Email = _teacher.Email;
            Password = _teacher.Password;
            Gender = _teacher.Gender;
            Phone = _teacher.Phone;
            Qualifications = _teacher.Qualifications.ConvertAll(qualification => qualification.ToString());

            SaveEditCommand = new RelayCommand(Edit);
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public List<string> Qualifications { get; set; }

        public IEnumerable<Gender> GenderValues => Enum.GetValues(typeof(Gender)).Cast<Gender>();

        public ICommand SaveEditCommand { get; }

        private void Edit()
        {
            try
            {
                _userService.Update(_teacher.Id, FirstName, LastName, Password, Gender, Phone);

                MessageBox.Show("Teacher edited successfully.", "Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                _editTeacherWindow.Close();
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
}