using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Repositories;

public interface ILanguageRepository
{
    public List<Language> GetAll();
    public Language? GetById(int id);
    public void Add(Language language);
}