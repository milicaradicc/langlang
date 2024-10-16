using GalaSoft.MvvmLight;
using LangLang.Models;

namespace LangLang.ViewModels.StudentViewModels
{
    public class StudentViewModel:ViewModelBase
    {
        private readonly Student _student;

        public StudentViewModel(Student student)
        {
            _student = student;
        }

        public int Id => _student.Id;
        public string FirstName => _student.FirstName;
        public string LastName => _student.LastName;
        public string Email => _student.Email;
        public Gender Gender => _student.Gender;
        public string Phone => _student.Phone;
        public Education? Education => _student.Education;
    }
}
