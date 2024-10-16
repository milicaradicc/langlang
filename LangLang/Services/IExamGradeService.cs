using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Models;

namespace LangLang.Services
{
    public interface IExamGradeService
    {
        public List<ExamGrade> GetAll();
        public ExamGrade? GetById(int id);
        public int AddExamGrade(int examId, int studentId, int readingPoints, int writingPoints, int listeningPoints, int talkingPoints);
        public void Delete(int id);
        public List<ExamGrade> GetByExamId(int examId);
    }
}
