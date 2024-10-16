using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LangLang.Models;

public class Student : User
{

    public Student(string? firstName, string? lastName, string? email, string? password, Gender gender, string? phone, Education? education)
        : base(firstName, lastName, email, password, gender, phone)
    {
        Education = education ?? throw new ArgumentNullException(nameof(education));
        PenaltyPoints = 0;
        ActiveCourseId = null;
    }
    
    [JsonConstructor]
    public Student(string firstName, string lastName, string email, string password, Gender gender, string phone, Education education, int penaltyPoints, int? activeCourseId, Dictionary<int, bool> languagePassFail, List<int> appliedCourses, List<int> appliedExams, Dictionary<int, int> examGradeIds, Dictionary<int, int> courseGradeIds)
        : base(firstName, lastName, email, password, gender, phone)
    {
        Education = education;
        PenaltyPoints = penaltyPoints;
        ActiveCourseId = activeCourseId;
        LanguagePassFail = languagePassFail;
        AppliedCourses = appliedCourses;
        AppliedExams = appliedExams;
        ExamGradeIds = examGradeIds;
        CourseGradeIds = courseGradeIds;
    }

    public Student()
    {
    }

    public Education? Education { get; set; }
    public int PenaltyPoints { get; set; }
    public int? ActiveCourseId { get; set; } = null;

    // obradjeniJezici / zavrseniJezici
    // dict jezik-bool, kada se zavrsi dodaj sa false, kada polozi ispit promeni na true
    public Dictionary<int, bool> LanguagePassFail { get; set; } = new();
    public List<int> AppliedCourses { get; set; } = new();
    public List<int> AppliedExams { get; set; } = new();

    public Dictionary<int, int> ExamGradeIds { get; set; } = new();
    public Dictionary<int, int> CourseGradeIds { get; set; } = new();
    
    public void AddCourse(int courseId)
    {
        if (AppliedCourses.Contains(courseId))
            throw new InvalidInputException("You have already applied to this course.");
        
        AppliedCourses.Add(courseId);
    }
    
    public void RemoveCourse(int courseId)
    {
        if (!AppliedCourses.Contains(courseId))
            throw new InvalidInputException("You haven't applied to this course.");
        
        AppliedCourses.Remove(courseId);
    }

    public void SetActiveCourse(int courseId)
    {
        if (ActiveCourseId is not null)
            throw new InvalidInputException("You are already enrolled in a course.");

        ActiveCourseId = courseId;
    }

    public void DropActiveCourse()
    {
        if (ActiveCourseId is null)
            throw new InvalidInputException("You are not enrolled in any courses.");

        ActiveCourseId = null;
    }
}