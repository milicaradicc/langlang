using System.Collections.Generic;
using System.IO;
using System.Linq;
using LangLang.Models;
using Newtonsoft.Json;

namespace LangLang.Repositories.FileRepositories;

public class CourseFileRepository : ICourseRepository
{
    private const string CourseFileName = "courses.json";
    private const string CourseDirectoryName = "data";

    private int _idCounter = 1;
    private Dictionary<int, Course> _courses = new();

    public List<Course> GetAll()
    {
        LoadData();
        return _courses.Values.ToList();
    }

    public Course? GetById(int id)
    {
        LoadData();
        _courses.TryGetValue(id, out var course);
        return course;
    }

    public int GenerateId()
    {
        LoadData();
        return _idCounter++;
    }

    public Course Add(Course course)
    {
        LoadData();
        if (course.Id == 0)
            course.Id = _idCounter++;
        _courses.Add(course.Id, course);
        SaveData();
        return course;
    }

    public void Update(Course course)
    {
        LoadData();
        _courses[course.Id] = course;
        SaveData();
    }

    public void Delete(int id)
    {
        LoadData();
        _courses.Remove(id);
        SaveData();
    }

    private void SaveData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), CourseDirectoryName, CourseFileName);

        string json = JsonConvert.SerializeObject(_courses, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        });

        File.WriteAllText(filePath, json);
    }

    private void LoadData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), CourseDirectoryName, CourseFileName);

        if (!File.Exists(filePath))
            return;

        string json = File.ReadAllText(filePath);
        _courses = JsonConvert.DeserializeObject<Dictionary<int, Course>>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        }) ?? new Dictionary<int, Course>();

        if (_courses.Any())
            _idCounter = _courses.Keys.Max() + 1;
    }
}