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
    class EditCourseViewModel : ViewModelBase
    {
        private readonly ILanguageService _languageService = ServiceProvider.GetRequiredService<ILanguageService>();
        private readonly ICourseService _courseService = ServiceProvider.GetRequiredService<ICourseService>();

        private readonly Teacher _teacher = UserService.LoggedInUser as Teacher ??
                                            throw new InvalidOperationException("No one is logged in.");
        private int _hours;
        private int _minutes;
        private readonly int _id;
        private readonly bool _areApplicationsClosed;
        private readonly List<string> _hoursValues = Enumerable.Range(0, 24).Select(hour => hour.ToString("00")).ToList();
        private readonly List<string> _minutesValues = Enumerable.Range(0, 60)
                                         .Where(minute => minute % 15 == 0)
                                         .Select(minute => minute.ToString("00"))
                                         .ToList();

        public EditCourseViewModel(Course? course)
        {
            _id = course!.Id;
            _areApplicationsClosed = course.AreApplicationsClosed;
            MaxStudents = course.MaxStudents;
            StartDate = new DateTime(course.StartDate.Year, course.StartDate.Month, course.StartDate.Day);
            Duration = course.Duration;
            Format = course.IsOnline ? "online" : "in-person";
            ScheduledTime = course.ScheduledTime;
            Minutes = course.ScheduledTime.Minute.ToString("00");
            Hours = course.ScheduledTime.Hour.ToString("00");
            SelectedWeekdays = new bool[7];
            course.Held.ForEach(day => SelectedWeekdays[(int)day] = true);
            EditCourseCommand = new RelayCommand(EditCourse);
        }

        public bool MaxStudentsEnabled => Format != null && Format.Equals("in-person");
        public int MaxStudents { get; set; }
        public DateTime StartDate { get; set; }
        public string Format { get; set; }
        public int Duration { get; set; }
        public TimeOnly ScheduledTime { get; set; }
        public List<Weekday>? Held { get; set; }
        public string Hours {
            get => _hours.ToString("00");
            set => _hours = int.Parse(value); 
        }
        public string Minutes {
            get => _minutes.ToString("00");
            set => _minutes = int.Parse(value);
        }
        public bool[] SelectedWeekdays { get; set; }

        public ICommand EditCourseCommand { get; }
        public static IEnumerable<string?> FormatValues => new List<string?> { "online", "in-person" };
        public static IEnumerable<Weekday> WeekdayValues => Enum.GetValues(typeof(Weekday)).Cast<Weekday>();
        public IEnumerable<string?> HourValues => _hoursValues;
        public IEnumerable<string?> MinuteValues => _minutesValues;

        private void EditCourse()
        {
            if ((!Format.Equals("online") && MaxStudents <= 0)|| Duration <= 0 || StartDate == default 
                || _hours < 0 || _minutes < 0)
            {
                MessageBox.Show("Fields cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                List<Weekday> Held = Enum.GetValues(typeof(Weekday))
                    .Cast<Weekday>()
                    .Where(day => SelectedWeekdays[(int)day])
                    .ToList();
                ScheduledTime = new TimeOnly().AddHours(_hours).AddMinutes(_minutes);
                bool isOnline = Format.Equals("online");
                DateOnly startDate = new(StartDate.Year, StartDate.Month, StartDate.Day);

                _courseService.Update(_id, Duration, Held, isOnline,
                    MaxStudents, ScheduledTime, startDate, _areApplicationsClosed, _teacher.Id);

                MessageBox.Show("Course edited successfully.", "Success", MessageBoxButton.OK,
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