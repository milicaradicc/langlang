using LangLang.Models;
using System.Collections.Generic;

namespace LangLang.Repositories
{
    public interface ICourseGradeRepository
    {
        public List<CourseGrade> GetAll();
        public void Add(CourseGrade courseGrade);
        public void Update(CourseGrade courseGrade);
        public void Delete(int id);
        public int GenerateId();
        public CourseGrade? GetById(int id);
    }
}

