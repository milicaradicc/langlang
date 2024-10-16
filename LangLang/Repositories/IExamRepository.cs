using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Repositories;

public interface IExamRepository
{
    public List<Exam> GetAll();
    public Exam? GetById(int id);
    public Exam Add(Exam exam);
    public void Update(Exam exam);
    public void Delete(int id);
}