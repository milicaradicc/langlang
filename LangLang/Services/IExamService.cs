using System;
using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Services;

public interface IExamService
{
    public List<Exam> GetAll();
    public Exam? GetById(int id);
    public List<Exam> GetAvailableExams(int studentId, int pageIndex = 1, int? amount = null);
    public List<Exam> GetAppliedExams(int studentId, int pageIndex = 1, int? amount = null);
    public Exam Add(string? languageName, LanguageLevel languageLevel, int maxStudents, DateOnly examDate, int? teacherId, TimeOnly examTime);
    public void Update(int id, int maxStudents, DateOnly date, int? teacherId, TimeOnly time);
    public void Delete(int id);
    public List<Student> GetStudents(int examId);
    public List<Exam> GetStartableExams(int teacherId);
    public void ConfirmExam(int examId);
    public int GetCurrentExam(int teacherId);
    public void FinishExam(int examId);
    public List<Exam> GetUngradedExams();
    public void SendGrades(int examId);
}