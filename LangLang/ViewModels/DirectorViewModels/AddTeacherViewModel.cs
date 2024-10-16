using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;
using LangLang.Views.LanguageViews;

namespace LangLang.ViewModels.DirectorViewModels
{
    internal class AddTeacherViewModel : ViewModelBase
    {
        private readonly IUserService _userService = ServiceProvider.GetRequiredService<IUserService>();
        private readonly ILanguageService _languageService = ServiceProvider.GetRequiredService<ILanguageService>();

        private readonly ListBox _qualificationsListBox;
        private readonly Window _addTeacherWindow;

        public AddTeacherViewModel(ListBox qualificationsListBox, Window addTeacherWindow)
        {
            _addTeacherWindow = addTeacherWindow;

            AddLanguageCommand = new RelayCommand(OpenAddLanguageWindow);
            AddTeacherCommand = new RelayCommand(AddTeacher);
            QualificationCollectionView = CollectionViewSource.GetDefaultView(_languageService.GetAll());
            _qualificationsListBox = qualificationsListBox;
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Gender Gender { get; set; }
        public string? Phone { get; set; }
        public ICollectionView QualificationCollectionView { get; set; }

        public IEnumerable<Gender> GenderValues => Enum.GetValues(typeof(Gender)).Cast<Gender>();

        public ICommand AddLanguageCommand { get; }
        public ICommand AddTeacherCommand { get; }

        private void OpenAddLanguageWindow()
        {
            new AddLanguageView().ShowDialog();
            UpdateQualifications();
        }

        private void AddTeacher()
        {
            try
            {
                List<Language> languages = _qualificationsListBox.SelectedItems.Cast<Language>().ToList();
                _userService.Add(FirstName, LastName, Email, Password, Gender, Phone, languages: languages);

                MessageBox.Show("Teacher added successfully.", "Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                _addTeacherWindow.Close();
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

        private void UpdateQualifications()
        {
            QualificationCollectionView = CollectionViewSource.GetDefaultView(_languageService.GetAll());
            _qualificationsListBox.ItemsSource = QualificationCollectionView;
            QualificationCollectionView.Refresh();
        }
    }
}