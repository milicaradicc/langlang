using LangLang.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Repositories.FileRepositories
{
    public class CourseGradeFileRepository : ICourseGradeRepository
    {
        private const string CourseGradeFileName = "courseGrades.json";
        private const string CourseGradeDirectoryName = "data";

        private int _idCounter = 1;
        private Dictionary<int, CourseGrade> _courseGrades = new();

        public List<CourseGrade> GetAll()
        {
            LoadData();
            return _courseGrades.Values.ToList();
        }

        public CourseGrade? GetById(int id)
        {
            LoadData();
            _courseGrades.TryGetValue(id, out var courseGrade);
            return courseGrade;
        }

        public int GenerateId()
        {
            LoadData();
            return _idCounter++;
        }

        public void Add(CourseGrade courseGrade)
        {
            LoadData();
            if (courseGrade.Id == 0)
                courseGrade.Id = _idCounter++;
            _courseGrades.Add(courseGrade.Id, courseGrade);
            SaveData();
        }

        public void Update(CourseGrade courseGrade)
        {
            LoadData();
            _courseGrades[courseGrade.Id] = courseGrade;
            SaveData();
        }

        public void Delete(int id)
        {
            LoadData();
            _courseGrades.Remove(id);
            SaveData();
        }


        private void SaveData()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), CourseGradeDirectoryName, CourseGradeFileName);

            string json = JsonConvert.SerializeObject(_courseGrades, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            });

            File.WriteAllText(filePath, json);
        }

        private void LoadData()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), CourseGradeDirectoryName, CourseGradeFileName);

            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            _courseGrades = JsonConvert.DeserializeObject<Dictionary<int, CourseGrade>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }) ?? new Dictionary<int, CourseGrade>();

            if (_courseGrades.Any())
                _idCounter = _courseGrades.Keys.Max() + 1;
        }
    }
}
