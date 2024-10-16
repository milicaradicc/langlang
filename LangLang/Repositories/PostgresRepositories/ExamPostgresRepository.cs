using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Models;

namespace LangLang.Repositories.PostgresRepositories
{
    internal class ExamPostgresRepository:IExamRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ExamPostgresRepository(DatabaseContext databaseContext)
        {
            _databaseContext= databaseContext;
        }

        public List<Exam> GetAll()
        {
            return _databaseContext.Exams.ToList();
        }

        public Exam? GetById(int id)
        {
            return _databaseContext.Exams.Find(id);
        }

        public int GenerateId()
        {
            throw new NotImplementedException();
        }

        public Exam Add(Exam exam)
        {
            var addedExam = _databaseContext.Exams.Add(exam);
            _databaseContext.SaveChanges();
            return addedExam.Entity;
        }

        public void Update(Exam exam)
        {
            _databaseContext.Exams.Update(exam);
            _databaseContext.SaveChanges();
        }

        public void Delete(int id)
        {
            Exam? exam = _databaseContext.Exams.Find(id);
            if (exam == null)
                return;
            _databaseContext.Exams.Remove(exam);
            _databaseContext.SaveChanges();
        }
    }
}
