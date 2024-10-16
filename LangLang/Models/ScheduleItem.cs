using System;
using LangLang.FormTable;

namespace LangLang.Models
{
    public abstract class ScheduleItem
    {
        private Language _language;
        private int _maxStudents;
        private DateOnly _date;

        private readonly bool _loadingFromDatabase;
        
        protected ScheduleItem()
        {
            // Only Entity Framework uses empty constructor
            _loadingFromDatabase = true;
        }

        protected ScheduleItem(Language language, int maxStudents, DateOnly date, int? teacherId, TimeOnly time)
        {
            Language = language;
            TeacherId = teacherId;
            ScheduledTime = time;
            MaxStudents = maxStudents;
            Date = date;
            Confirmed = false;
        }

        // Constructor without date validation for deserializing
        protected ScheduleItem(int id, Language language, int maxStudents, DateOnly date, int? teacherId, TimeOnly time)
        {
            Id = id;
            Language = language;
            TeacherId = teacherId;
            MaxStudents = maxStudents;
            _date = date;
            ScheduledTime = time;
        }

        [TableItem(1)]
        public int Id { get; set; }

        [TableItem(8)]
        public bool Confirmed { get; set; }

        [TableItem(2)]
        public Language Language
        {
            get => _language;
            set
            {
                ValidateLanguage(value);
                _language = value;
            }
        }

        [TableItem(7)]
        public int MaxStudents
        {
            get => _maxStudents;
            protected set
            {
                ValidateMaxStudents(value);
                _maxStudents = value;
            }
        }

        [TableItem(5)]
        public int? TeacherId { get; set; }

        [TableItem(3)]
        public DateOnly Date
        {
            get => _date;
            protected set
            {
                ValidateDate(value);
                _date = value;
            }
        }

        [TableItem(4)]
        public TimeOnly ScheduledTime { get; set; }

        [TableItem(6)]
        public bool IsOnline
        {
            get
            {
                if (this is Course course)
                    return course.IsOnline;
                
                return false;
            }
        }

        private static void ValidateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));
        }

        private static void ValidateMaxStudents(int maxStudents)
        {
            if (maxStudents < 0)
                throw new InvalidInputException("Number of max students can not be negative.");
        }

        private void ValidateDate(DateOnly date)
        {
            if (_loadingFromDatabase) return;
            if (date < DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidInputException("Date must be after today.");
        }
    }
}