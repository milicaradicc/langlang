using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;

namespace LangLang.ViewModels.ExamViewModels
{
    class AddExamViewModel : ViewModelBase
    {
        private readonly ILanguageService _languageService = ServiceProvider.GetRequiredService<ILanguageService>();
        private readonly IExamService _examService = ServiceProvider.GetRequiredService<IExamService>();
        private readonly ITeacherService _teacherService = ServiceProvider.GetRequiredService<ITeacherService>();

        private DateTime _dateSelected;

        private readonly Exam? _exam;

        private readonly User _loggedIn = UserService.LoggedInUser!;

        private readonly Window _addExamWindow;

        public AddExamViewModel(Exam? exam, Window addExamWindow)
        {
            _exam = exam;
            _addExamWindow = addExamWindow;

            // Edit
            if (_exam is not null)
            {
                Name = _exam.Language.Name;
                LanguageLevel = _exam.Language.Level;
                MaxStudents = _exam.MaxStudents;
                ExamDate = _exam.Date;
                HourSelected = _exam.ScheduledTime.Hour;
                MinuteSelected = _exam.ScheduledTime.Minute;
            }

            EnterExamCommand = new RelayCommand(AddExam);
        }

        public string? Name { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
        public IEnumerable<string> LanguageNames => _languageService.GetAllNames();
        public int MaxStudents { get; set; }
        public DateOnly ExamDate { get; set; }

        public DateTime DateSelected
        {
            get => _dateSelected;
            set
            {
                _dateSelected = value;
                ExamDate = new DateOnly(value.Year, value.Month, value.Day);
                RaisePropertyChanged();
            }
        }

        public int HourSelected { get; set; }
        public int MinuteSelected { get; set; }

        public static IEnumerable<LanguageLevel> LanguageLevelValues =>
            Enum.GetValues(typeof(LanguageLevel)).Cast<LanguageLevel>();

        public static List<int> Hours => new()
            { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };

        public static List<int> Minutes => new() { 0, 15, 30, 45 };

        public ICommand EnterExamCommand { get; }

        private void AddExam()
        {
            try
            {
                if (_exam is null)
                {
                    int? teacherId;
                    if (_loggedIn is Director)
                    {
                        Exam exam = _examService.Add(Name, LanguageLevel, MaxStudents, ExamDate, _loggedIn.Id, new TimeOnly(HourSelected, MinuteSelected));
                        teacherId = _teacherService.SmartPickExam(exam);
                        exam.TeacherId = teacherId;
                        Language language = _languageService.GetLanguage(exam.Language.Name, exam.Language.Level) ??
                    throw new InvalidInputException("Language with the given level doesn't exist.");
                        _examService.Update(exam.Id, exam.MaxStudents, exam.Date, exam.TeacherId, exam.ScheduledTime);
                    }
                    else
                    {
                        Exam exam = _examService.Add(Name, LanguageLevel, MaxStudents, ExamDate, _loggedIn.Id, new TimeOnly(HourSelected, MinuteSelected));
                    }

                    MessageBox.Show("Exam added successfully.", "Success", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    Language language = _languageService.GetLanguage(Name!, LanguageLevel) ??
                    throw new InvalidInputException("Language with the given level doesn't exist.");

                    _examService.Update(_exam.Id, MaxStudents, ExamDate, _loggedIn.Id,
                        new TimeOnly(HourSelected, MinuteSelected));

                    MessageBox.Show("Exam edited successfully.", "Success", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                _addExamWindow.Close();
            }
            catch (InvalidInputException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}