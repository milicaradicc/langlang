using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Windows.Input;
using LangLang.Models;
using LangLang.Repositories;
using Microsoft.VisualBasic.Devices;

namespace LangLang.Services;

public class ExamService : IExamService
{

    private readonly IExamRepository _examRepository;
    private readonly IUserRepository _userRepository;
    private readonly IScheduleService _scheduleService;
    private readonly ILanguageService _languageService;
    private readonly IExamGradeRepository _examGradeRepository;
    private readonly IMessageService _messageService;
    
    public ExamService(IExamRepository examRepository, IUserRepository userRepository, IScheduleService scheduleService, ILanguageService languageService, IExamGradeRepository examGradeRepository, IMessageService messageService)
    {
        _examRepository = examRepository;
        _userRepository = userRepository;
        _scheduleService = scheduleService;
        _languageService = languageService;
        _examGradeRepository = examGradeRepository;
        _messageService = messageService;
    }

    public List<Exam> GetAll()
    {
        return _examRepository.GetAll();
    }

    public Exam? GetById(int id)
    {
        return _examRepository.GetById(id);
    }
    public List<Exam> GetAppliedExams(int studentId, int pageIndex = 1, int? amount = null)
    {
        Student student = (_userRepository.GetById(studentId) as Student)!;

        var appliedExamIds = student.AppliedExams;

        var appliedExams = _examRepository.GetAll()
            .Where(exam => appliedExamIds.Contains(exam.Id))
            .ToList();

        amount ??= appliedExams.Count;

        return appliedExams.Skip((pageIndex - 1) * amount.Value).Take(amount.Value).ToList();
    }


    /*
     1. student must have finished course for the language he wants to take exam in
     2. exam must have at least one available spot for student
     3. search date must be at least 30 days before the date the exam is held
     */
    public List<Exam> GetAvailableExams(int studentId, int pageIndex = 1, int? amount = null)
    {
        // Nakon što je učenik završio kurs, prikazuju mu se svi dostupni termini ispita koji se
        // odnose na jezik i nivo jezika koji je učenik obradio na kursu

        Student student = (_userRepository.GetById(studentId) as Student)!;

        List<Exam> exams = _examRepository.GetAll().Where(exam =>
                            exam.StudentIds.Count < exam.MaxStudents && IsNeededCourseFinished(exam, student) &&
                            (exam.Date.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days >= 30 &&
                            !IsAlreadyPassed(exam, student) &&
                            !student.AppliedExams.Contains(exam.Id)).ToList();

        amount ??= exams.Count;

        return exams.Skip((pageIndex - 1) * amount.Value).Take(amount.Value).ToList();
    }

    /*
    if language from that course is in the dict than student has finished that course
    if its in the dict and it has value true then student passed exam, if its false he didnt pass it yet
    */
    private bool IsNeededCourseFinished(Exam exam, Student student)
    {
        return student.LanguagePassFail.ContainsKey(exam.Language.Id) &&
               student.LanguagePassFail[exam.Language.Id] == false;
    }

    private bool IsAlreadyPassed(Exam exam, Student student)
    {
        foreach (int languageId in student.LanguagePassFail.Keys)
        {
            Language language = _languageService.GetById(languageId)!;
            if (language.Name == exam.Language.Name && language.Level >= exam.Language.Level &&
                student.LanguagePassFail[languageId])
                return true;
        }

        return false;
    }
    public Exam Add(string? languageName, LanguageLevel languageLevel, int maxStudents, DateOnly examDate, int? teacherId,
        TimeOnly examTime)
    {
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
        Language language = _languageService.GetLanguage(languageName, languageLevel) ??
                            throw new InvalidInputException("Language with the given level doesn't exist.");


        Exam addedExam = new(0,language, maxStudents, examDate, teacherId, examTime);
        
        addedExam = _examRepository.Add(addedExam);
        try
        {
            // If the exam can't be scheduled, delete it
            _scheduleService.Add(addedExam);
        }
        catch (InvalidInputException ex)
        {
            _examRepository.Delete(addedExam.Id);
            throw ex;
        }

        if (teacher != null)
        {
            teacher.ExamIds.Add(addedExam.Id);
            _userRepository.Update(teacher);
        }

        return addedExam;
    }

    public void Update(int id, int maxStudents, DateOnly date,
        int? teacherId, TimeOnly scheduledTime)

    {
        Exam exam = GetExamOrThrow(id);

        if ((exam.Date.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days < 14)
            throw new InvalidInputException("The exam can't be changed if it's less than 2 weeks from now.");

        Teacher? teacher = null;
        if (teacherId.HasValue)
            teacher = GetTeacherOrThrow(teacherId.Value);

        int? oldTeacherId = exam.TeacherId;
        
        exam.MaxStudents = maxStudents;
        exam.Date = date;
        exam.ScheduledTime = scheduledTime;
        exam.TeacherId = teacherId;

        // Validates if it can be added to the current schedule
        _scheduleService.Update(exam);

        Teacher? oldTeacher = null;
        if (oldTeacherId.HasValue)
            oldTeacher = GetTeacherOrThrow(oldTeacherId.Value);

        if (oldTeacher is not null)
        {
            oldTeacher.ExamIds.Remove(exam.Id);
            _userRepository.Update(oldTeacher);
        }

        if (teacher is not null)
        {
            teacher.ExamIds.Add(exam.Id);
            _userRepository.Update(teacher);
        }

        _examRepository.Update(exam);
    }

    public void Delete(int id)
    {
        Exam exam = GetExamOrThrow(id);
        Teacher? teacher = null;
        if (exam.TeacherId.HasValue)
            teacher = GetTeacherOrThrow(exam.TeacherId.Value);

        if (teacher is not null)
        {
            teacher.ExamIds.Remove(exam.Id);
            _userRepository.Update(teacher);
        }

        _scheduleService.Delete(id);
        
        foreach (Student? student in exam.StudentIds.Select(studentId => _userRepository.GetById(studentId) as Student))
        {
            student!.AppliedExams.Remove(exam.Id);
            _userRepository.Update(student);
        }

        _examRepository.Delete(id);
    }

    public void ConfirmExam(int examId)
    {
        Exam exam = GetExamOrThrow(examId);
        exam.Confirmed = true;
        _examRepository.Update(exam);
    }

    public List<Student> GetStudents(int examId)
    {
        Exam exam = _examRepository.GetById(examId)!;

        List<Student> students = exam.StudentIds.Select(studentId => (_userRepository.GetById(studentId) as Student)!).ToList();

        return students;
    }

    public List<Exam> GetStartableExams(int teacherId)
    {
        Teacher teacher = GetTeacherOrThrow(teacherId);
        List<Exam> startableExams = new();
        foreach (int examId in teacher.ExamIds)
        {
            Exam exam = GetExamOrThrow(examId);
            if ((exam.Date.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days <= 7 &&
                !exam.Confirmed)
            {
                startableExams.Add(exam);
            }
        }

        return startableExams;
    }

    public int GetCurrentExam(int teacherId)
    {
        Teacher teacher = GetTeacherOrThrow(teacherId);
        foreach (int examId in teacher.ExamIds)
        {

            Exam exam = GetExamOrThrow(examId);

            double timeDifference = (DateTime.Now - exam.Date.ToDateTime(exam.ScheduledTime)).TotalMinutes;

            if (timeDifference >= 0 && timeDifference < Exam.ExamDuration)
                return exam.Id;
        }

        throw new InvalidInputException("There are currently no exams");
    }

    public void FinishExam(int examId)
    {
        Exam exam = GetExamOrThrow(examId);
        CheckGrades(exam);

        exam.TeacherGraded = true;
        _examRepository.Update(exam);
    }

    private void CheckGrades(Exam exam)
    {
        foreach (int studentId in exam.StudentIds)
        {
            Student student = _userRepository.GetById(studentId) as Student ??
                              throw new InvalidInputException("Student doesn't exist.");

            if (!student.ExamGradeIds.ContainsKey(exam.Id))
            {
                throw new InvalidInputException("Not all students have been graded.");
            }
        }
    }
    public List<Exam> GetUngradedExams()
    {
        return _examRepository.GetAll().Where(exam => exam.TeacherGraded == true && exam.DirectorGraded == false).ToList();
    }
    public void SendGrades(int examId)
    {
        Exam exam = _examRepository.GetById(examId)!;
        exam.DirectorGraded = true;
        _examRepository.Update(exam);

        foreach (User user in _userRepository.GetAll())
        {
            if (user is Student student && student.AppliedExams.Contains(examId))
            {
                student.AppliedExams.Remove(examId);
                _userRepository.Update(student);
            }
        }
        SendEmail(examId);
    }
    private void SendEmail(int examId)
    {
        Exam exam = _examRepository.GetById(examId)!;
        foreach (ExamGrade examGrade in _examGradeRepository.GetAll().Where(eg => eg.ExamId == examId))
        {
            string passedText = examGrade.Passed
                ? $"Congratulations, you have passed {exam.Language} exam!\n"
                : $"Unfortunately, you have failed {exam.Language} exam.\n";

            string pointsText = "Here are your points:\n" +
                                $"\tReading: {examGrade.ReadingPoints} \n" +
                                $"\tListening: {examGrade.ListeningPoints} \n" +
                                $"\tTalking: {examGrade.TalkingPoints} \n" +
                                $"\tWriting: {examGrade.WritingPoints} \n";

            _messageService.Add(examGrade.StudentId, passedText+pointsText);
            EmailService.SendMessage("Exam results",passedText+pointsText);
        }
    }
    private Teacher GetTeacherOrThrow(int teacherId)
    {
        Teacher? teacher = _userRepository.GetById(teacherId) as Teacher;
        if (teacher == null)
            throw new InvalidInputException("User doesn't exist.");
        return teacher;
    }
    private Exam GetExamOrThrow(int examId)
    {
        Exam? exam = _examRepository.GetById(examId);
        if (exam == null)
            throw new InvalidInputException("Exam doesn't exist.");
        return exam;
    }
}