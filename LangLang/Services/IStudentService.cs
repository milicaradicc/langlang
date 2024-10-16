using System;
using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Services;

public interface IStudentService
{
    public List<Student> GetAll();
    public void ApplyForCourse(int studentId, int courseId);
    public void WithdrawFromCourse(int studentId, int courseId);
    public void ApplyStudentExam(Student student, int examId);
    public void DropExam(Exam exam, Student student);
    public void ReportCheating(int studentId, int examId);

    public void CheckIfFirstInMonth();
    public void AddPenaltyPoint(int studentId, PenaltyPointReason penaltyPointReason, int courseId,
                                int teacherId, DateOnly datePenaltyPointGiven);
    public void ReviewTeacher(int id, int response);
    public void DropActiveCourse(int id, string responseText);
    public void PauseOtherApplications(int studentId, int courseId);
    public void ResumeApplications(int studentId);
}