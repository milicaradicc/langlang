using System.Collections.Generic;
using System.IO;
using System.Linq;
using LangLang.Models;
using Newtonsoft.Json;

namespace LangLang.Repositories.FileRepositories;

public class LanguageFileRepository : ILanguageRepository
{
    private const string LanguageFileName = "languages.json";
    private const string LanguageDirectoryName = "data";

    private Dictionary<int, Language> _languages = new();
    private int _idCounter = 1;
    public List<Language> GetAll()
    {
        LoadData();
        return _languages.Values.ToList();
    }

    public Language? GetById(int id)
    {
        LoadData();
        _languages.TryGetValue(id, out var language);
        return language;
    }
    public void Add(Language language)
    {
        LoadData();
        language.Id = _idCounter++;
        if (_languages.Values.Any(lang => lang.Name == language.Name && lang.Level == language.Level))
            throw new InvalidInputException("Language already exists.");

        _languages.Add(language.Id, language);

        SaveData();
    }

    private void SaveData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), LanguageDirectoryName, LanguageFileName);

        string json = JsonConvert.SerializeObject(_languages, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        });

        File.WriteAllText(filePath, json);
    }

    private void LoadData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), LanguageDirectoryName, LanguageFileName);

        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        _languages = JsonConvert.DeserializeObject<Dictionary<int, Language>>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        }) ?? new Dictionary<int, Language>();
        if (_languages.Any())
            _idCounter = _languages.Keys.Max() + 1;
    }
}