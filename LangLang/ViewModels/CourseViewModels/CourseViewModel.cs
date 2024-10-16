using System;
using System.Linq;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using LangLang.Models;
using LangLang.Services;

namespace LangLang.ViewModels.CourseViewModels
{
    public class CourseViewModel : ViewModelBase
    {
        private readonly IUserService _userService = ServiceProvider.GetRequiredService<IUserService>();

        private readonly Course _course;
        private readonly Teacher? _teacher;

        public CourseViewModel(Course course)
        {
            _course = course;
            _teacher = course.TeacherId.HasValue ? _userService.GetById(course.TeacherId.Value) as Teacher : null;
        }

        public SolidColorBrush BackgroundColor
        {
            get
            {
                return _course.TeacherId == null ? Brushes.LightGray : Brushes.Transparent;
            }
        }
        public int Id => _course.Id;
        public string LanguageName => _course.Language.Name;
        public LanguageLevel LanguageLevel => _course.Language.Level;
        public string Duration => _course.Duration + " weeks";
        public string Held => string.Join(", ", _course.Held);
        public string IsOnline => _course.IsOnline ? "online" : "in-person";
        public string Applications => _course.AreApplicationsClosed ? "closed" : "opened";
        public int MaxStudents => _course.MaxStudents;
        public TimeOnly ScheduledTime => _course.ScheduledTime;
        public DateOnly StartDate => _course.StartDate;
        public string TeachersName => _teacher != null ? $"{_teacher.FirstName} {_teacher.LastName}" : string.Empty;

        public string Students => string.Join(", ", _course.Students.Keys.Select(studentId =>
        {
            User? user = _userService.GetById(studentId);
            return user != null ? $"{user.FirstName} {user.LastName}" : "error";
        }));

        public bool FilterLanguageLevel(string? languageLevel)
        {
            return languageLevel == null || _course.Language.Level.ToString().Equals(languageLevel);
        }

        public bool FilterLanguageName(string? languageName)
        {
            return languageName == null || _course.Language.Name.Equals(languageName);
        }

        public bool FilterTeacher(int teacherId)
        {
            return teacherId == -1 || _course.TeacherId == teacherId;
        }


        public bool FilterStartDate(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return true;
            }

            DateOnly chosenDate = new DateOnly(date.Year, date.Month, date.Day);
            return chosenDate == _course.StartDate;
        }

        public bool FilterDuration(string? duration)
        {
            if (duration == null)
                return true;

            int.TryParse(duration.Split(" ")[0], out int result);
            return _course.Duration == result;
        }

        public bool FilterFormat(string? format)
        {
            return format == null || format.Equals("online") == _course.IsOnline;
        }
    }
}