using System;
using System.Linq;
using GalaSoft.MvvmLight;
using LangLang.Models;

namespace LangLang.ViewModels.TeacherViewModels
{
    public class TeacherViewModel : ViewModelBase
    {
        private readonly Teacher _teacher;

        public TeacherViewModel(Teacher teacher)
        {
            _teacher = teacher;
        }

        public int Id => _teacher.Id;
        public string FirstName => _teacher.FirstName;
        public string LastName => _teacher.LastName;
        public string Email => _teacher.Email;
        public string Password => _teacher.Password;
        public Gender Gender => _teacher.Gender;
        public string Phone => _teacher.Phone;

        public string Qualifications => string.Join(", ", _teacher.Qualifications);

        public string DateAdded => _teacher.DateCreated.ToString();

        public bool FilterLanguageLevel(string? languageLevel)
        {
            return languageLevel==null || _teacher.Qualifications.Any(language => language.Level == (LanguageLevel)Enum.Parse(typeof(LanguageLevel), languageLevel));
        }

        public bool FilterLanguageName(string? languageName)
        {
            return languageName==null || _teacher.Qualifications.Any(language => language.Name.Equals(languageName));
        }

        public bool FilterDateCreated(DateTime dateCreated)
        {
            return dateCreated == DateTime.MinValue || _teacher.DateCreated == DateOnly.FromDateTime(dateCreated.Date);
        }
    }
}
