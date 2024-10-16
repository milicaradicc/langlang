using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Models;
using LangLang.Repositories;

namespace LangLang.Services;

public class StudentService : IStudentService
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IExamRepository _examRepository;
    private readonly ILanguageRepository _languageRepository;

    private readonly IUserService _userService;
    private readonly IExamGradeService _examGradeService;
    private readonly ICourseGradeService _courseGradeService;
    private readonly IPenaltyPointService _penaltyPointService;
    
    public StudentService(IUserRepository userRepository, ICourseRepository courseRepository, ILanguageRepository languageRepository, IExamRepository examRepository, IUserService userService, IExamGradeService examGradeService, ICourseGradeService courseGradeService, IPenaltyPointService penaltyPointService)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _examRepository = examRepository;
        _languageRepository = languageRepository;
        _userService = userService;
        _examGradeService = examGradeService;
        _courseGradeService = courseGradeService;
        _penaltyPointService = penaltyPointService;
    }


    public List<Student> GetAll()
    {
        return _userRepository.GetAll().OfType<Student>().ToList();
    }

    /*
     *  if teacher has graded the exam but director has not sent the email,
     *  then student can not apply for new exams
     */
    public void ApplyStudentExam(Student student, int examId)
    {
        if (student.AppliedExams.Contains(examId))
        {
            throw new InvalidInputException("You already applied");
        }

        if (_examRepository.GetById(examId) == null)
        {
            throw new InvalidInputException("Exam not found.");
        }

        foreach (int id in student.AppliedExams)
        {
            Exam exam = _examRepository.GetById(id)!;
            if (exam.TeacherGraded == true && exam.DirectorGraded == false)
            {
                throw new InvalidInputException("Cant apply for exam while waiting for results.");
            }
        }

        Exam appliedExam = _examRepository.GetById(examId)!;
        appliedExam.StudentIds.Add(student.Id);
        _examRepository.Update(appliedExam);
        student.AppliedExams.Add(examId);
        _userRepository.Update(student);
    }

    public void DropExam(Exam exam, Student student)
    {
        if (_examRepository.GetById(exam.Id) == null)
        {
            throw new InvalidInputException("Exam not found.");
        }

        if (!student.AppliedExams.Contains(exam.Id))
        {
            throw new InvalidInputException("Exam does not exist");
        }

        if ((exam.Date.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days < 10)
            throw new InvalidInputException("The exam can't be dropped if it's less than 10 days from now.");

        student.AppliedExams.Remove(exam.Id);
        _userRepository.Update(student);
    }

    public void ApplyForCourse(int studentId, int courseId)
    {
        Student student = GetStudentOrThrow(studentId);

        if (student.ActiveCourseId is not null)
            throw new InvalidInputException("You are already enrolled in a course.");

        Course course = _courseRepository.GetById(courseId) ??
                        throw new InvalidInputException("Course doesn't exist.");

        student.AddCourse(course.Id);
        course.AddStudent(student.Id);

        _userRepository.Update(student);
        _courseRepository.Update(course);
    }

    public void WithdrawFromCourse(int studentId, int courseId)
    {
        Student student = GetStudentOrThrow(studentId);

        Course course = _courseRepository.GetById(courseId) ??
                        throw new InvalidInputException("Course doesn't exist.");

        if ((course.StartDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days < 7)
            throw new InvalidInputException("The course can't be withdrawn from if it's less than 1 week from now.");

        student.RemoveCourse(course.Id);
        course.RemoveStudent(student.Id);

        _userRepository.Update(student);
        _courseRepository.Update(course);
    }

    public void DropActiveCourse(int studentId, string reason)
    {
        Student student = GetStudentOrThrow(studentId);

        if (student.ActiveCourseId is null)
            throw new InvalidInputException("You are not enrolled in a course.");

        Course course = _courseRepository.GetById(student.ActiveCourseId!.Value) ??
                        throw new InvalidInputException("Course doesn't exist.");

        if ((DateTime.Now - course.StartDate.ToDateTime(TimeOnly.MinValue)).Days < 7)
            throw new InvalidInputException("The course can't be dropped if it started less than a week ago.");

        course.AddDropOutRequest(studentId, reason);
        _courseRepository.Update(course);
    }

    public void ReportCheating(int studentId, int examId)
    {
        Student student = GetStudentOrThrow(studentId);

        Exam exam = _examRepository.GetById(examId) ?? throw new InvalidInputException("Exam doesn't exist.");

        exam.RemoveStudent(studentId);
        _examRepository.Update(exam);
        _userService.Delete(studentId);
    }

    public void CheckIfFirstInMonth()
    {
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        int dayOfMonth = currentDate.Day;

        if (dayOfMonth != 10 || UserService.LoggedInUser is not Student student || student.PenaltyPoints <= 0)
        {
            return;
        }

        --student.PenaltyPoints;
        _userRepository.Update(student);
        RemoveStudentPenaltyPoint(student.Id);
    }

    private void RemoveStudentPenaltyPoint(int studentId)
    {
        List<PenaltyPoint> penaltyPoints = _penaltyPointService.GetAll();                                             

        foreach (PenaltyPoint point in penaltyPoints)
        {
            if (point.StudentId == studentId && !point.Deleted)
            {
                _penaltyPointService.Delete(point.Id);
                break;
            }
        }
    }

    public void AddPenaltyPoint(int studentId, PenaltyPointReason penaltyPointReason, int courseId,
        int teacherId, DateOnly datePenaltyPointGiven)
    {
        Student student = GetStudentOrThrow(studentId);
        _ = _courseRepository.GetById(courseId) ?? throw new InvalidInputException("Course doesn't exist.");
        ++student.PenaltyPoints;
        _userRepository.Update(student);
        _penaltyPointService.Add(penaltyPointReason, student.Id, courseId, teacherId, datePenaltyPointGiven);
        if (student.PenaltyPoints == 3)
        {
            _userService.Delete(student.Id);
        }
    }

    /// <summary>
    /// Reviews the teacher after the student has finished the course and removes the active course from the student
    /// </summary>
    public void ReviewTeacher(int studentId, int rating)
    {
        Student? student = _userRepository.GetById(studentId) as Student;

        Course course = _courseRepository.GetById(student!.ActiveCourseId!.Value)!;
        
        Teacher teacher = (_userRepository.GetById(course.TeacherId!.Value) as Teacher)!;

        teacher.AddReview(rating);
        _userRepository.Update(teacher);

        student.DropActiveCourse();
        _userRepository.Update(student);

        // Update the logged in student
        if (UserService.LoggedInUser?.Id == studentId)
            _userService.Login(student.Email, student.Password);
    }

    /// <summary>
    /// Pause all student applications except the one with the passed ID
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="courseId">Course ID which not to pause</param>
    public void PauseOtherApplications(int studentId, int courseId)
    {
        Student student = GetStudentOrThrow(studentId);

        foreach (Course course in student.AppliedCourses.Select(id => _courseRepository.GetById(id)!))
        {
            if (course.Id == courseId)
                continue;

            if (!course.Students.ContainsKey(studentId))
                throw new InvalidInputException($"Student hasn't applied to the course with ID {course.Id}.");

            course.Students[studentId] = ApplicationStatus.Paused;
            _courseRepository.Update(course);
        }
    }

    public void ResumeApplications(int studentId)
    {
        Student student = GetStudentOrThrow(studentId);

        foreach (Course course in student.AppliedCourses.Select(courseId => _courseRepository.GetById(courseId)!))
        {
            if (!course.Students.ContainsKey(studentId))
                throw new InvalidInputException($"Student hasn't applied to the course with ID {course.Id}.");

            course.Students[studentId] = ApplicationStatus.Pending;
            _courseRepository.Update(course);
        }
    }
    private Student GetStudentOrThrow(int studentId)
    {
        Student? student = _userRepository.GetById(studentId) as Student;
        if (student == null)
            throw new InvalidInputException("User doesn't exist.");
        return student;
    }
}