using System.Collections.Generic;
using System.IO;
using System.Linq;
using LangLang.Models;
using Newtonsoft.Json;

namespace LangLang.Repositories.FileRepositories;

public class ExamFileRepository : IExamRepository
{
    private const string ExamFileName = "exams.json";
    private const string ExamDirectoryName = "data";

    private int _idCounter = 1;
    private Dictionary<int, Exam> _exams = new();

    public List<Exam> GetAll()
    {
        LoadData();
        return _exams.Values.ToList();
    }

    public Exam? GetById(int id)
    {
        LoadData();
        _exams.TryGetValue(id, out var exam);
        return exam;
    }

    public int GenerateId()
    {
        LoadData();
        return _idCounter++;
    }

    public Exam Add(Exam exam)
    {
        LoadData();
        if (exam.Id == 0)
            exam.Id = _idCounter++;
        _exams.Add(exam.Id, exam);
        SaveData();
        return exam;
    }

    public void Update(Exam exam)
    {
        LoadData();
        _exams[exam.Id] = exam;
        SaveData();
    }

    public void Delete(int id)
    {
        LoadData();
        _exams.Remove(id);
        SaveData();
    }

    private void SaveData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), ExamDirectoryName, ExamFileName);

        string json = JsonConvert.SerializeObject(_exams, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        });

        File.WriteAllText(filePath, json);
    }

    private void LoadData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), ExamDirectoryName, ExamFileName);

        if (!File.Exists(filePath))
            return;

        string json = File.ReadAllText(filePath);
        _exams = JsonConvert.DeserializeObject<Dictionary<int, Exam>>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        }) ?? new Dictionary<int, Exam>();

        if (_exams.Any())
            _idCounter = _exams.Keys.Max() + 1;
    }
}