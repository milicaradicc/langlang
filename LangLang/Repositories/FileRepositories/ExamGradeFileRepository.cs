using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Models;

namespace LangLang.Repositories.FileRepositories
{
    public class ExamGradeFileRepository : IExamGradeRepository
    {
        private const string ExamGradeFileName = "examGrades.json";
        private const string ExamGradeDirectoryName = "data";

        private int _idCounter = 1;
        private Dictionary<int, ExamGrade> _examGrades = new();

        public List<ExamGrade> GetAll()
        {
            LoadData();
            return _examGrades.Values.ToList();
        }

        public ExamGrade? GetById(int id)
        {
            LoadData();
            _examGrades.TryGetValue(id, out var examGrade);
            return examGrade;
        }

        public int GenerateId()
        {
            LoadData();
            return _idCounter++;
        }

        public void Add(ExamGrade examGrade)
        {
            LoadData();
            if (examGrade.Id == 0)
                examGrade.Id = _idCounter++;
            _examGrades.Add(examGrade.Id, examGrade);
            SaveData();
        }

        public void Update(ExamGrade examGrade)
        {
            LoadData();
            _examGrades[examGrade.Id] = examGrade;
            SaveData();
        }

        public void Delete(int id)
        {
            LoadData();
            _examGrades.Remove(id);
            SaveData();
        }


        private void SaveData()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), ExamGradeDirectoryName, ExamGradeFileName);

            string json = JsonConvert.SerializeObject(_examGrades, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            });

            File.WriteAllText(filePath, json);
        }

        private void LoadData()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), ExamGradeDirectoryName, ExamGradeFileName);

            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            _examGrades = JsonConvert.DeserializeObject<Dictionary<int, ExamGrade>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }) ?? new Dictionary<int, ExamGrade>();

            if (_examGrades.Any())
                _idCounter = _examGrades.Keys.Max() + 1;
        }
    }
}
