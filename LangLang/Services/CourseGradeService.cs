using LangLang.Models;
using LangLang.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace LangLang.Services
{
    internal class CourseGradeService : ICourseGradeService
    {
        private readonly ICourseGradeRepository _courseGradeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;

        public CourseGradeService(ICourseGradeRepository courseGradeRepository, IUserRepository userRepository, ICourseRepository courseRepository)
        {
            _courseGradeRepository = courseGradeRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
        }
        
        public List<CourseGrade> GetAll()
        {
            return _courseGradeRepository.GetAll();
        }

        public CourseGrade? GetById(int id)
        {
            return _courseGradeRepository.GetById(id);
        }
        
        public CourseGrade? GetByStudentAndCourse(int studentId, int courseId)
        {
            return GetAll().FirstOrDefault(courseGrade => courseGrade.StudentId == studentId && courseGrade.CourseId == courseId);
        }
        public List<CourseGrade> GetByCourseId(int courseId)
        {
            return GetAll().Where(courseGrade => courseGrade.CourseId == courseId).ToList();
        }


        public int Add(int courseId, int studentId, int knowledgeGrade, int activityGrade)
        {
            _ = _userRepository.GetById(studentId) as Student ??
                              throw new InvalidInputException("User doesn't exist.");
            _ = _courseRepository.GetById(courseId) ?? throw new InvalidInputException("Course doesn't exist.");

            CourseGrade courseGrade = new(courseId, studentId, knowledgeGrade, activityGrade) { Id = _courseGradeRepository.GenerateId() };

            _courseGradeRepository.Add(courseGrade);

            return courseGrade.Id;
        }
        public void AddCourseGrade(int studentId, int courseId, int knowledgeGrade, int activityGrade)
        {
            int courseGradeId = Add(courseId, studentId, knowledgeGrade, activityGrade);

            Student? student = _userRepository.GetById(studentId) as Student;
            if (student == null)
                throw new InvalidInputException("User doesn't exist.");
            _ = _courseRepository.GetById(courseId) ?? throw new InvalidInputException("Course doesn't exist.");

            if (student.CourseGradeIds.ContainsKey(courseId))
                Delete(student.CourseGradeIds[courseId]);

            student.CourseGradeIds[courseId] = courseGradeId;

            _userRepository.Update(student);
        }

        public void Delete(int id)
        {
            _courseGradeRepository.Delete(id);
        }
    }
}
