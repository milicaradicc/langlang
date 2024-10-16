using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Models;
using LangLang.Repositories;

namespace LangLang.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILanguageService _languageService;
    private readonly IScheduleService _scheduleService;

    public CourseService(ICourseRepository courseRepository, IUserRepository userRepository, ILanguageService languageService, IScheduleService scheduleService)
    {
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _languageService = languageService;
        _scheduleService = scheduleService;
    }

    public List<Course> GetAll()
    {
        return _courseRepository.GetAll();
    }

    public Course? GetById(int id)
    {
        return _courseRepository.GetById(id);
    }

    /// <summary>
    /// Get students with pending applications
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of students with pending applications</returns>
    public List<Student> GetStudents(int courseId)
    {
        Course course = GetById(courseId)!;
        List<Student> students = course.Students.Where(student => student.Value == ApplicationStatus.Pending)
            .Select(student => (_userRepository.GetById(student.Key) as Student)!).ToList();
        return students;
    }
    public List<Course> GetStartableCourses(int teacherId)
    {
        return GetCourses(teacherId, course =>
            (course.StartDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days <= 7 && !course.Confirmed);
    }

    public List<Course> GetActiveCourses(int teacherId)
    {
        return GetCourses(teacherId, course =>
            (DateTime.Now - course.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays >= 0 && course.Confirmed && !course.IsFinished);
    }

    private List<Course> GetCourses(int teacherId, Func<Course, bool> filter)
    {
        Teacher teacher = GetTeacherOrThrow(teacherId);
        List<Course> filteredCourses = new();

        foreach (int courseId in teacher.CourseIds)
        {
            Course course = _courseRepository.GetById(courseId) ?? throw new InvalidInputException("Course doesn't exist.");
            if (filter(course))
            {
                filteredCourses.Add(course);
            }
        }

        return filteredCourses;
    }

    public List<Course> GetFinishedCourses()
    {
        return GetAll().Where(course => course.IsFinished && !course.StudentsNotified).ToList();
    }

    public List<Course> GetCoursesWithWithdrawals(int teacherId)
    {
        Teacher teacher = GetTeacherOrThrow(teacherId);
        List<Course> coursesWithWithdrawals = new();

        foreach (int courseId in teacher.CourseIds)
        {
            Course course = _courseRepository.GetById(courseId) ?? throw new InvalidInputException("Course doesn't exist.");
            if (course.DropOutRequests.Any())
                coursesWithWithdrawals.Add(course);
        }
        return coursesWithWithdrawals;
    }
    public List<Course> GetAvailableCourses(int studentId, int pageIndex = 1, int? amount = null)
    {
        List<Course> courses = _courseRepository.GetAll().Where(course =>
            (course.Students.Count < course.MaxStudents || course.IsOnline) &&
            (course.StartDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days >= 7 &&
            !course.Students.ContainsKey(studentId)).ToList();

        amount ??= courses.Count;

        return courses.Skip((pageIndex - 1) * amount.Value).Take(amount.Value).ToList();
    }

    public List<Course> GetAppliedCourses(int studentId, int pageIndex = 1, int? amount = null)
    {
        Student? student = _userRepository.GetById(studentId) as Student;
        List<Course> courses = student!.AppliedCourses.Select(courseId => _courseRepository.GetById(courseId)!).ToList();
        amount ??= courses.Count;
        return courses.Skip((pageIndex - 1) * amount.Value).Take(amount.Value).ToList();
    }


    public Course Add(string languageName, LanguageLevel languageLevel, int duration, List<Weekday> held, bool isOnline,
        int maxStudents, int? creatorId, TimeOnly scheduledTime, DateOnly startDate, bool areApplicationsClosed,
        int? teacherId)
    {
        Language language = _languageService.GetLanguage(languageName, languageLevel) ??
                            throw new InvalidInputException("Language with the given level doesn't exist.");
        Teacher? teacher = null;
        User user = _userRepository.GetById(teacherId!.Value)!;

        // zbog smart picka, ako je id direktora onda ce se promeniti u narenih par funkcija na validan id nastavnika
        if (user is Teacher)
        {
            teacher = (Teacher)user;
        }
        else if (user is not Director)
        {
            throw new InvalidInputException("User doesn't exist.");
        }

        startDate = SetValidStartDate(startDate, held);
        Course course = new(language, duration, held, isOnline, maxStudents, creatorId, scheduledTime, startDate,
            areApplicationsClosed, teacherId);

        // To auto generate the ID
        Course addedCourse = _courseRepository.Add(course);
        try
        {
            // If the course can't be scheduled, delete it
            _scheduleService.Add(addedCourse);
        }
        catch (InvalidInputException ex)
        {
            _courseRepository.Delete(addedCourse.Id);
            throw ex;
        }

        if (teacher != null)
        {
            teacher.CourseIds.Add(course.Id);
            _userRepository.Update(teacher);
        }
        return course;
    }

    public void Update(int id, int duration, List<Weekday> held,
        bool isOnline, int maxStudents, TimeOnly scheduledTime, DateOnly startDate,
        bool areApplicationsClosed, int? teacherId)
    {
        Course course = _courseRepository.GetById(id) ?? throw new InvalidInputException("Course doesn't exist.");
        Teacher? teacher = teacherId.HasValue ? GetTeacherOrThrow(teacherId.Value) : null;

        if ((course.StartDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days < 7)
            throw new InvalidInputException("The course can't be changed if it's less than 1 week from now.");

        int? oldTeacherId = course.TeacherId;

        startDate = SetValidStartDate(startDate, held);
        course.Duration = duration;
        course.Held = held;
        course.IsOnline = isOnline;
        course.MaxStudents = maxStudents;
        course.StartDate = startDate;
        course.ScheduledTime = scheduledTime;
        course.AreApplicationsClosed = areApplicationsClosed;
        course.TeacherId = teacherId;

        _scheduleService.Update(course);


        Teacher? oldTeacher = oldTeacherId.HasValue ? _userRepository.GetById(oldTeacherId.Value) as Teacher : null;
        if (oldTeacher is not null)
        {
            oldTeacher.CourseIds.Remove(course.Id);
            _userRepository.Update(oldTeacher);
        }

        if (teacher is not null)
        {
            teacher.CourseIds.Add(course.Id);
            _userRepository.Update(teacher);
        }

        _courseRepository.Update(course);
    }

    public void Delete(int id)
    {
        Course course = _courseRepository.GetById(id) ?? throw new InvalidInputException("Course doesn't exist.");
        Teacher? teacher = course.TeacherId.HasValue ? GetTeacherOrThrow(course.TeacherId.Value) : null;
                                                                           
        if (teacher is not null)
        {
            teacher.CourseIds.Remove(id);
            _userRepository.Update(teacher);
        }

        foreach (Student student in course.Students.Keys.Select(studentId => (_userRepository.GetById(studentId) as Student)!))
        {
            student.RemoveCourse(course.Id);
            _userRepository.Update(student);
        }

        _scheduleService.Delete(id);
        _courseRepository.Delete(id);
    }

    private static DateOnly SetValidStartDate(DateOnly startDate, List<Weekday> held)
    {
        int a = (int)held[0] + 1;
        int b = (int)startDate.DayOfWeek;
        int difference = a - b;
        return startDate.AddDays((difference < 0 ? difference + 7 : difference) % 7);
    }
    private Teacher GetTeacherOrThrow(int teacherId)
    {
        Teacher? teacher = _userRepository.GetById(teacherId) as Teacher;
        if (teacher == null)
            throw new InvalidInputException("User doesn't exist.");
        return teacher;
    }
}