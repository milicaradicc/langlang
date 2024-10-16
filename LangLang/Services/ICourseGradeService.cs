using LangLang.Models;
using System.Collections.Generic;

namespace LangLang.Services
{
    public interface ICourseGradeService
    {
        public List<CourseGrade> GetAll();
        public CourseGrade? GetById(int id);
        public CourseGrade? GetByStudentAndCourse(int studentId, int courseId);
        public List<CourseGrade> GetByCourseId(int courseId);
        public int Add(int courseId, int studentId, int knowledgeGrade, int activityGrade);
        public void AddCourseGrade(int studentId, int courseId, int knowledgeGrade, int activityGrade);

        public void Delete(int id);
    }
}
