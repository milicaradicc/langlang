using System.Collections.Generic;
using System.Linq;
using LangLang.Models;

namespace LangLang.Repositories.PostgresRepositories;

public class CoursePostgresRepository : ICourseRepository
{
    private readonly DatabaseContext _dbContext;
    
    public CoursePostgresRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public List<Course> GetAll()
    {
        return _dbContext.Courses.ToList();
    }

    public Course? GetById(int id)
    {
        return _dbContext.Courses.Find(id);
    }

    public Course Add(Course course)
    {
        var addedCourse = _dbContext.Courses.Add(course);
        _dbContext.SaveChanges();
        return addedCourse.Entity;
    }

    public void Update(Course course)
    {
        _dbContext.Courses.Update(course);
        _dbContext.SaveChanges();
    }

    public void Delete(int id)
    {
        Course? course = _dbContext.Courses.Find(id);
        if (course == null) return;
        
        _dbContext.Courses.Remove(course);
        _dbContext.SaveChanges();
    }
}