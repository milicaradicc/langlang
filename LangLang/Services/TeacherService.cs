using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Models;
using LangLang.Repositories;

namespace LangLang.Services;

public class TeacherService : ITeacherService
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IExamRepository _examRepository;
    private readonly IExamService _examService;
    private readonly IScheduleService _scheduleService;
    private readonly IStudentService _studentService;
    private readonly IMessageService _messageService;
    private readonly ILanguageService _languageService; 

    public TeacherService(IUserRepository userRepository, ICourseRepository courseRepository, IExamRepository examRepository, IExamService examService, IScheduleService scheduleService, IStudentService studentService, IMessageService messageService, ILanguageService languageService)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _examRepository = examRepository;
        _examService = examService;
        _scheduleService = scheduleService;
        _studentService = studentService;
        _messageService = messageService;
        _languageService = languageService;
    }

    public List<Teacher> GetAll()
    {
        return _userRepository.GetAll().OfType<Teacher>().ToList();
    }

    public List<Teacher> GetPage(int pageIndex = 1, int? amount = null, string propertyName = "",
        string sortingWay = "ascending")
    {
        List<Teacher> teachers = _userRepository.GetAll().OfType<Teacher>().Where(teacher => !teacher.Deleted).ToList();

        amount ??= teachers.Count;
        switch (propertyName)
        {
            case "Name":
                teachers = sortingWay == "ascending"
                    ? teachers.OrderBy(teacher => teacher.FirstName + teacher.LastName).ToList()
                    : teachers.OrderByDescending(teacher => teacher.FirstName + teacher.LastName).ToList();
                break;
            case "DateAdded":
                teachers = sortingWay == "ascending"
                    ? teachers.OrderBy(teacher => teacher.DateCreated).ToList()
                    : teachers.OrderByDescending(teacher => teacher.DateCreated).ToList();
                break;
            default:
                break;
        }
        return teachers.Skip((pageIndex - 1) * amount.Value).Take(amount.Value).ToList();
    }

    public int Count()
    {
        return _userRepository.GetAll().OfType<Teacher>().Where(teacher => !teacher.Deleted).ToList().Count;
    }

    public List<Course> GetCourses(int teacherId, int pageIndex = 1, int? amount = null, string propertyName = "", string sortingWay = "ascending")
    {
        List<Course> courses = _courseRepository.GetAll().Where(course => course.TeacherId == teacherId).ToList();
        amount ??= courses.Count;
        switch (propertyName)
        {
            case "LanguageName":
                courses = sortingWay == "ascending" ? courses.OrderBy(course => course.Language.Name).ToList() :
                                                      courses.OrderByDescending(course => course.Language.Name).ToList();
                break;
            case "LanguageLevel":
                courses = sortingWay == "ascending" ? courses.OrderBy(course => course.Language.Level).ToList() :
                                                      courses.OrderByDescending(course => course.Language.Level).ToList();
                break;
            case "StartDate":
                courses = sortingWay == "ascending" ? courses.OrderBy(course => course.StartDate).ToList() :
                                                      courses.OrderByDescending(course => course.StartDate).ToList();
                break;
            default:
                break;
        }
        return courses.Skip((pageIndex - 1) * amount.Value).Take(amount.Value).ToList();
    }

    public List<Exam> GetExams(int teacherId, int pageIndex = 1, int? amount = null, string propertyName = "", string sortingWay = "ascending")
    {
        List<Exam> exams = _examRepository.GetAll().Where(exam => exam.TeacherId == teacherId).ToList();
        amount ??= exams.Count;

        switch (propertyName)
        {
            case "Language":
                exams = sortingWay == "ascending" ? exams.OrderBy(exam => exam.Language.Name).ToList() :
                                                      exams.OrderByDescending(exam => exam.Language.Name).ToList();
                break;
            case "LanguageLevel":
                exams = sortingWay == "ascending" ? exams.OrderBy(exam => exam.Language.Level).ToList() :
                                                      exams.OrderByDescending(exam => exam.Language.Level).ToList();
                break;
            case "ExamDate":
                exams = sortingWay == "ascending" ? exams.OrderBy(exam => exam.Date).ToList() :
                                                      exams.OrderByDescending(exam => exam.Date).ToList();
                break;
            default:
                break;
        }
        return exams.Skip((pageIndex - 1) * amount.Value).Take(amount.Value).ToList();
    }


    public int GetCourseCount(int teacherId)
    {
        return _courseRepository.GetAll().Count(course => course.TeacherId == teacherId);
    }
    public int GetExamCount(int teacherId)
    {
        return _examRepository.GetAll().Count(exam => exam.TeacherId == teacherId);
    }

    public List<Teacher> GetAvailableTeachers(Course course)
    {
        List<Teacher> availableTeachers = new();
        foreach (Teacher teacher_ in GetAll())
        {
            Course tempCourse = new(course.Language, course.Duration, course.Held, true,
                course.MaxStudents, course.CreatorId, course.ScheduledTime, course.StartDate,
                course.AreApplicationsClosed, teacher_.Id);

            if (_scheduleService.ValidateScheduleItem(tempCourse, true))
            {
                availableTeachers.Add(teacher_);
            }
        }

        return availableTeachers;
    }
    public List<Teacher> GetAvailableTeachers(Exam exam)
    {
        List<Teacher> availableTeachers = new();
        foreach (Teacher teacher_ in GetAll())
        {
            Exam tempExam = new(exam.Language, exam.MaxStudents, exam.Date, teacher_.Id, exam.ScheduledTime);

            if (_scheduleService.ValidateScheduleItem(tempExam, true))
            {
                availableTeachers.Add(teacher_);
            }
        }

        return availableTeachers;
    }

    public void RejectStudentApplication(int studentId, int courseId)
    {
        Course course = _courseRepository.GetById(courseId)!;

        if (!course.Students.ContainsKey(studentId))
            throw new InvalidInputException("Student hasn't applied to this course.");
        
        // course.RemoveStudent(studentId);
        course.Students[studentId] = ApplicationStatus.Denied;
        _courseRepository.Update(course);
    }
    
    public void ConfirmCourse(int courseId)
    {
        Course course = _courseRepository.GetById(courseId) ?? throw new InvalidInputException("Course doesn't exist.");
        course.Confirmed = true;
        
        foreach ((int studentId, ApplicationStatus applicationStatus) in course.Students)
        {
            Student student = _userRepository.GetById(studentId) as Student ??
                              throw new InvalidInputException("Student doesn't exist.");

            switch (applicationStatus)
            {
                // All paused and denied applications are removed
                case ApplicationStatus.Paused:
                case ApplicationStatus.Denied:
                    course.RemoveStudent(studentId);
                    student.RemoveCourse(courseId);
                    _userRepository.Update(student);
                    if (applicationStatus == ApplicationStatus.Denied)
                        _messageService.Add(studentId, $"Your application for the course {course.Language.Name} has been denied.");
                    break;
                default:
                    student.SetActiveCourse(courseId);
                    student.RemoveCourse(courseId);
                    _studentService.PauseOtherApplications(studentId, courseId);
                    _userRepository.Update(student);
                    _messageService.Add(studentId, $"Your application for the course {course.Language.Name} has been accepted.");
                    break;
            }
        }
        
        _courseRepository.Update(course);
    }
    
    private void CheckGrades(int courseId)
    {
        Course course = _courseRepository.GetById(courseId) ?? throw new InvalidInputException("Course doesn't exist.");

        foreach (int studentId in course.Students.Keys)
        {
            Student student = _userRepository.GetById(studentId) as Student ??
                              throw new InvalidInputException("Student doesn't exist.");

            if (!student.CourseGradeIds.ContainsKey(courseId))
            {
                throw new InvalidInputException("Not all students have been graded.");
            }
        }
    }
    
    public void FinishCourse(int courseId)
    {
        Course course = _courseRepository.GetById(courseId) ?? throw new InvalidInputException("Course doesn't exist.");
        CheckGrades(courseId);
        course.IsFinished = true;
        _courseRepository.Update(course);
        
        foreach (int studentId in course.Students.Keys)
        {
            Student student = (_userRepository.GetById(studentId) as Student)!;
            student.LanguagePassFail[course.Language.Id] = false;
            // Course is dropped whe student reviews the teacher
            // student.DropActiveCourse();
            _studentService.ResumeApplications(studentId);
            _userRepository.Update(student);
        }
    }
    // get all available teachers and sort them based on ranking
    // pick the first one as the best choice

    public int? SmartPick(ScheduleItem scheduleItem)
    {
         if(scheduleItem is Exam exam){
            return  SmartPickExam(exam);
         }else if(scheduleItem is Course course) 
            {
            return SmartPickCourse(course);
         }
        return null;
    }
    public int? SmartPickCourse(Course course)
    {
        List<Teacher> availableTeachers = GetAvailableTeachers(course)
             .Where(teacher => teacher.Qualifications.Contains(course.Language))
             .OrderByDescending(teacher => teacher.Rating)
             .ToList();

        if (!availableTeachers.Any())
            throw new InvalidInputException("There are no available substitute teachers");

        course.TeacherId = availableTeachers.First().Id;
        availableTeachers.First().CourseIds.Add(course.Id);
        _courseRepository.Update(course);
        return course.TeacherId;
    }
    public int? SmartPickExam(Exam exam)
    {
        List<Teacher> availableTeachers = GetAvailableTeachers(exam)
                    .Where(teacher => teacher.Qualifications.Contains(exam.Language))
                    .OrderByDescending(teacher => teacher.Rating)
                    .ToList();

        if (!availableTeachers.Any())
        {
            _examService.Delete(exam.Id);
            throw new InvalidInputException("There are no available substitute teachers");
        }

        exam.TeacherId = availableTeachers.First().Id;
        availableTeachers.First().ExamIds.Add(exam.Id);
        Language language = _languageService.GetLanguage(exam.Language.Name, exam.Language.Level) ??
                    throw new InvalidInputException("Language with the given level doesn't exist.");

        _examService.Update(exam.Id ,exam.MaxStudents, exam.Date, exam.TeacherId, exam.ScheduledTime);
        return exam.TeacherId;
    }
}