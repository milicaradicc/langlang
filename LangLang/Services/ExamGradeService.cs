using System.Collections.Generic;
using System.Linq;
using LangLang.Models;
using LangLang.Repositories;

namespace LangLang.Services
{
    internal class ExamGradeService : IExamGradeService
    {
        private readonly IExamGradeRepository _examGradeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IExamRepository _examRepository;
        
        public ExamGradeService(IExamGradeRepository examGradeRepository, IUserRepository userRepository, IExamRepository examRepository)
        {
            _examGradeRepository = examGradeRepository;
            _userRepository = userRepository;
            _examRepository = examRepository;
        }

        public List<ExamGrade> GetAll()
        {
            return _examGradeRepository.GetAll();
        }

        public ExamGrade? GetById(int id)
        {
            return _examGradeRepository.GetById(id);
        }

        public List<ExamGrade> GetByExamId(int examId)
        {
            return _examGradeRepository.GetAll()
                                        .Where(grade => grade.ExamId == examId)
                                        .ToList();
        }

        public int AddExamGrade(int studentId, int examId, int reading, int writing, int listening, int talking)
        {
            Student? student = _userRepository.GetById(studentId) as Student ?? throw new InvalidInputException("User doesn't exist.");
            Exam exam = _examRepository.GetById(examId) ?? throw new InvalidInputException("Exam doesn't exist.");

            if (student.ExamGradeIds.ContainsKey(examId))
            {
                Delete(student.ExamGradeIds[examId]);
            }

            ExamGrade examGrade = new(_examGradeRepository.GenerateId(), examId, studentId, reading, writing, listening, talking);
            _examGradeRepository.Add(examGrade);

            student.ExamGradeIds[examId] = examGrade.Id;
            student.LanguagePassFail[exam.Language.Id] = examGrade.Passed;

            _userRepository.Update(student);
            return examGrade.Id;
        }

        public void Delete(int id)
        {
            _examGradeRepository.Delete(id);
        }
    }
}
