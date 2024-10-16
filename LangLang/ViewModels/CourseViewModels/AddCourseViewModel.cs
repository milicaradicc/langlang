using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LangLang.Models;
using LangLang.Services;

namespace LangLang.ViewModels.CourseViewModels
{
    class AddCourseViewModel : ViewModelBase
    {
        private readonly ILanguageService _languageService;
        private readonly ICourseService _courseService;
        private readonly ITeacherService _teacherService;

        // the one that is not null is logged in, do this in order to not repeat the code
        private readonly User loggedInUser;
        private readonly Teacher _teacher;
        private readonly Director _director;

        private readonly List<string> _hours = Enumerable.Range(0, 24).Select(hour => hour.ToString("00")).ToList();
        private readonly List<string> _minutes = Enumerable.Range(0, 60)
                                         .Where(minute => minute % 15 == 0)
                                         .Select(minute => minute.ToString("00"))
                                         .ToList();

        public AddCourseViewModel(ILanguageService languageService, ICourseService courseService, ITeacherService teacherService)
        {
            _languageService = languageService;
            _courseService = courseService;
            _teacherService = teacherService;
            
            loggedInUser = UserService.LoggedInUser ??
            throw new InvalidOperationException("No one is logged in.");

            _teacher = loggedInUser as Teacher;
            _director = loggedInUser as Director;
            
            
            SelectedWeekdays = new bool[7];
            AddCourseCommand = new RelayCommand(AddCourse);
        }

        public bool? MaxStudentsEnabled => Format != null && Format.Equals("in-person");
        public string? LanguageName { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
        public int MaxStudents { get; set; }
        public DateTime StartDate { get; set; }
        public string? Format { get; set; }
        public int Duration { get; set; }
        public TimeOnly ScheduledTime { get; set; }
        public List<Weekday>? Held { get; set; }
        public int CreatorId { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool[] SelectedWeekdays {  get; set; }
        public ICommand? AddCourseCommand { get; }

        public static IEnumerable<LanguageLevel> LanguageLevelValues =>
            Enum.GetValues(typeof(LanguageLevel)).Cast<LanguageLevel>();
        public IEnumerable<string?> LanguageNameValues => _languageService.GetAllNames();
        public static IEnumerable<string?> FormatValues => new List<string> { "online", "in-person" };
        public IEnumerable<string?> HourValues => _hours;
        public IEnumerable<string?> MinuteValues => _minutes;

        private void AddCourse()
        {
            if (string.IsNullOrEmpty(LanguageName) ||
                (Format != null && (!Format.Equals("online") && MaxStudents <= 0) || Duration <= 0 || StartDate == default || Hours < 0 || Minutes < 0))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {   
                List<Weekday> Held = Enum.GetValues(typeof(Weekday))
                    .Cast<Weekday>()
                    .Where(day => SelectedWeekdays[(int)day])
                    .ToList();
                Language? language = IsValidLanguage(LanguageName, LanguageLevel);
                ScheduledTime = new TimeOnly().AddHours(Hours).AddMinutes(Minutes);
                bool isOnline = Format != null && Format.Equals("online");
                DateOnly startDate = new(StartDate.Year, StartDate.Month, StartDate.Day);

                int? teacherId;
                int? creatorId;
                if (_teacher != null)
                {
                    teacherId = _teacher.Id;
                    creatorId = _teacher.Id;
                    Course course = _courseService.Add(LanguageName, LanguageLevel, Duration, Held, isOnline, MaxStudents,
                        creatorId, ScheduledTime, startDate, false, teacherId);
                }
                else
                {
                    Course course = _courseService.Add(LanguageName, LanguageLevel, Duration, Held, isOnline, MaxStudents,
                    _director.Id, ScheduledTime, startDate, false, _director.Id);
                    teacherId = _teacherService.SmartPick(course);
                    course.TeacherId = teacherId;
                    _courseService.Update(course.Id, course.Duration, course.Held, course.IsOnline, course.MaxStudents, course.ScheduledTime, course.StartDate, course.AreApplicationsClosed, course.TeacherId);
                }
                        
                MessageBox.Show("Course added successfully.", "Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (InvalidInputException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Language? IsValidLanguage(string languageName, LanguageLevel level)
        {
            return _languageService.GetLanguage(languageName, level) 
                ?? throw new InvalidInputException("Language doesn't exist.");
        }

    }
}