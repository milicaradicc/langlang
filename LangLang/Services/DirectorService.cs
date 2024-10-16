using LangLang.Repositories;
using System.Collections.Generic;
using System.Linq;
using LangLang.Models;

namespace LangLang.Services
{

    public class DirectorService : IDirectorService
    {
        private const int NumberOfTopStudents = 3;
        
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICourseGradeService _courseGradeService;
        
        public DirectorService(ICourseRepository courseRepository, IUserRepository userRepository, ICourseGradeService courseGradeService)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
            _courseGradeService = courseGradeService;
        }

        // knowledgePoints - if true they have priority over activity points
        public void NotifyBestStudents(int courseId, bool knowledgePoints)
        {
            double knowledgePointsMultiplier = knowledgePoints ? 1.5 : 1;
            double activityPointsMultiplier = knowledgePoints ? 1 : 1.5;
            double penaltyMultiplier = 2.0;

            Course course = _courseRepository.GetById(courseId)!;

            // student, calculatedPoints
            Dictionary<Student, double> rankedStudents = new();

            foreach (Student student in course.Students.Keys.Select(studentId => (_userRepository.GetById(studentId) as Student)!))
            {
                CourseGrade courseGrade = _courseGradeService.GetByStudentAndCourse(student.Id, courseId)!;
                double studentPoints = courseGrade.KnowledgeGrade * knowledgePointsMultiplier + courseGrade.ActivityGrade * activityPointsMultiplier - student.PenaltyPoints * penaltyMultiplier;
                rankedStudents.Add(student, studentPoints);
            }

            List<Student> bestStudents = rankedStudents.OrderByDescending(pair => pair.Value).Take(NumberOfTopStudents).Select(pair => pair.Key).ToList();

            foreach (Student student in bestStudents)
            {
                EmailService.SendMessage($"Congratulations {student.FirstName} {student.LastName}!",
                    $"You are one of the best students in the course {course.Language.Name} {course.Language.Level}.");
            }

            course.StudentsNotified = true;
            _courseRepository.Update(course);
        }
    }
}
