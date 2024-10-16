using GalaSoft.MvvmLight;
using LangLang.Models;
using System;

namespace LangLang.ViewModels.StudentViewModels
{
    public class StudentCourseGradeViewModel : ViewModelBase
    {
        private readonly Student _student;
        private readonly CourseGrade _courseGrade;
        public StudentCourseGradeViewModel(Student student, CourseGrade courseGrade)
        {
            _student = student;
            _courseGrade = courseGrade;
        }

        public int Id => _student.Id;
        public String FirstName => _student.FirstName;
        public String LastName => _student.LastName;
        public Gender Gender => _student.Gender;
        public Education? Education => _student.Education;
        public String KnowledgeGrade => _courseGrade == null ? "/" : _courseGrade.KnowledgeGrade.ToString();
        public String ActivityGrade => _courseGrade == null ? "/" : _courseGrade.ActivityGrade.ToString();
    }
}
