using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;

namespace LangLang.ViewModels.LanguageViewModels
{
    internal class AddLanguageViewModel : ViewModelBase
    {
        private readonly ILanguageService _languageService = ServiceProvider.GetRequiredService<ILanguageService>();

        private readonly Window _addLanguageWindow;

        public AddLanguageViewModel(Window addLanguageWindow)
        {
            _addLanguageWindow = addLanguageWindow;
            AddLanguageCommand = new RelayCommand(AddLanguage);
        }

        public string? LanguageName { get; set; }
        public LanguageLevel SelectedLanguageLevel { get; set; }
        public IEnumerable<string> LanguageLevelValues => Enum.GetNames(typeof(LanguageLevel));

        public ICommand AddLanguageCommand { get; }

        private void AddLanguage()
        {
            try
            {
                if (LanguageName is null)
                    throw new InvalidInputException("Language name cannot be empty.");
                
                _languageService.Add(LanguageName, SelectedLanguageLevel);

                MessageBox.Show("Language added successfully.", "Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                _addLanguageWindow.Close();
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