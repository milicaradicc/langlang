using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Repositories;

public interface ICourseRepository
{
    public List<Course> GetAll();
    public Course? GetById(int id);
    public Course Add(Course course);
    public void Update(Course course);
    public void Delete(int id);
}