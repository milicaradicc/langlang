using System;
using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Services;

public interface ICourseService
{
    public List<Course> GetAll();
    public Course? GetById(int id);
    public List<Student> GetStudents(int courseId);
    public List<Course> GetStartableCourses(int teacherId);
    public List<Course> GetActiveCourses(int teacherId);
    public List<Course> GetFinishedCourses();
    public List<Course> GetCoursesWithWithdrawals(int teacherId);
    public List<Course> GetAppliedCourses(int studentId, int pageIndex = 1, int? amount = null);
    List<Course> GetAvailableCourses(int studentId, int pageIndex = 1, int? amount = null);

    public Course Add(string languageName, LanguageLevel languageLevel, int duration, List<Weekday> held,
        bool isOnline, int maxStudents, int? creatorId, TimeOnly scheduledTime, DateOnly startDate,
        bool areApplicationsClosed, int? teacherId);

    public void Update(int id, int duration, List<Weekday> held,
        bool isOnline, int maxStudents, TimeOnly scheduledTime, DateOnly startDate,
        bool areApplicationsClosed, int? teacherId);

    public void Delete(int id);
}