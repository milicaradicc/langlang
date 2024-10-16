using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Models;
using Newtonsoft.Json;

namespace LangLang.Repositories.FileRepositories;
public class PenaltyPointFileRepository : IPenaltyPointRepository
{
    private const string PenaltyPointFileName = "penalty_points.json";
    private const string PenaltyPointDirectoryName = "data";

    private int _idCounter = 1;
    private Dictionary<int, PenaltyPoint> _penaltyPoints = new();

    public List<PenaltyPoint> GetAll()
    {
        LoadData();
        return _penaltyPoints.Values.ToList();
    }
    public PenaltyPoint? GetById(int id)
    {
        LoadData();
        _penaltyPoints.TryGetValue(id, out var point);
        return point;
    }

    public int GenerateId()
    {
        LoadData();
        return _idCounter++;
    }

    public void Add(PenaltyPoint penaltyPoint)
    {
        LoadData();
        if (penaltyPoint.Id == 0)
            penaltyPoint.Id = _idCounter++;
        _penaltyPoints.Add(penaltyPoint.Id, penaltyPoint);
        SaveData();
    }
    public void Update(PenaltyPoint penaltyPoint)
    {
        LoadData();
        _penaltyPoints[penaltyPoint.Id] = penaltyPoint;
        SaveData();
    }

    public void Delete(int id)
    {
        LoadData();
        PenaltyPoint? point = GetById(id);
        if (point != null)
        {
            point.Deleted = true;
            SaveData();
        }

    }
    private void SaveData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), PenaltyPointDirectoryName, PenaltyPointFileName);

        string json = JsonConvert.SerializeObject(_penaltyPoints, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        });

        File.WriteAllText(filePath, json);
    }

    private void LoadData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), PenaltyPointDirectoryName, PenaltyPointFileName);

        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        _penaltyPoints = JsonConvert.DeserializeObject<Dictionary<int, PenaltyPoint>>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        }) ?? new Dictionary<int, PenaltyPoint>();

        if (_penaltyPoints.Any())
            _idCounter = _penaltyPoints.Keys.Max() + 1;
    }
}


