using System.Collections.Generic;
using System.Linq;
using LangLang.Models;

namespace LangLang.Repositories.PostgresRepositories;

public class LanguagePostgresRepository : ILanguageRepository
{
    private readonly DatabaseContext _dbContext;
    
    public LanguagePostgresRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public List<Language> GetAll()
    {
        return _dbContext.Languages.ToList();
    }

    public Language? GetById(int id)
    {
        return _dbContext.Languages.Find(id);
    }

    public void Add(Language language)
    {
        _dbContext.Languages.Add(language);
        _dbContext.SaveChanges();
    }
}