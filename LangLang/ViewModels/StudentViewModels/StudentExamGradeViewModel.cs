using System;
using GalaSoft.MvvmLight;
using LangLang.Models;

namespace LangLang.ViewModels.StudentViewModels
{
    public class StudentExamGradeViewModel:ViewModelBase
    {
        private readonly Student _student;
        private readonly ExamGrade? _examGrade;
        public StudentExamGradeViewModel(Student student, ExamGrade? examGrade)
        {
            _student = student;
            _examGrade = examGrade;
        }

        public int StudentId => _student.Id;
        public String FirstName => _student.FirstName;
        public String LastName => _student.LastName;
        public String ReadingPoints => _examGrade == null ? "/" : _examGrade.ReadingPoints.ToString();
        public String WritingPoints => _examGrade == null ? "/" : _examGrade.WritingPoints.ToString();
        public String ListeningPoints => _examGrade == null ? "/" : _examGrade.ListeningPoints.ToString();
        public String TalkingPoints => _examGrade == null ? "/" : _examGrade.TalkingPoints.ToString();
    }
}
