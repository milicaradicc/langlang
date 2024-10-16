using System;
using GalaSoft.MvvmLight;
using LangLang.Models;

namespace LangLang.ViewModels.ExamViewModels
{
    public class ExamViewModel : ViewModelBase
    {
        private readonly Exam _exam;

        public ExamViewModel(Exam exam)
        {
            _exam = exam;
        }

        public int Id => _exam.Id;
        public string Language => _exam.Language.Name;
        public LanguageLevel LanguageLevel => _exam.Language.Level;
        public string MaxStudents => _exam.MaxStudents.ToString();
        public DateOnly ExamDate => _exam.Date;
        public static int Duration => Exam.ExamDuration;
        public TimeOnly ExamTime => _exam.ScheduledTime;

        public bool FilterLevel(string? level)
        {
            if (level == null)
            {
                return true;
            }

            return _exam.Language.Level == (LanguageLevel)Enum.Parse(typeof(LanguageLevel), level);
        }

        public bool FilterLanguageName(string? name)
        {
            if (name == null)
            {
                return true;
            }

            return _exam.Language.Name == name;
        }

        public bool FilterDateHeld(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return true;
            }

            DateOnly chosenDate = new(date.Year, date.Month, date.Day);
            return chosenDate == _exam.Date;
        }

        public bool FilterTeacherId(int id)
        {
            return id == -1 || _exam.TeacherId == id;
        }
    }
}