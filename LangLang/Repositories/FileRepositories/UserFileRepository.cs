using System.Collections.Generic;
using System.IO;
using System.Linq;
using LangLang.Models;
using Newtonsoft.Json;

namespace LangLang.Repositories.FileRepositories;

public class UserFileRepository : IUserRepository
{
    private const string UserFileName = "users.json";
    private const string UserDirectoryName = "data";

    private int _idCounter = 1;
    private Dictionary<int, User> _users = new();

    public List<User> GetAll()
    {
        LoadData();
        return _users.Values.ToList();
    }

    public User? GetById(int id)
    {
        LoadData();
        _users.TryGetValue(id, out var user);
        return user;
    }

    public void Add(User user)
    {
        LoadData();
        user.Id = _idCounter++;
        _users.Add(user.Id, user);
        SaveData();
    }

    public void Update(User user)
    {
        LoadData();
        _users[user.Id] = user;
        SaveData();
    }

    public void Delete(int id)
    {
        LoadData();
        User? user = GetById(id);
        if (user != null)
        {
            user.Deleted = true;
            SaveData();
        }
    }

    private void SaveData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), UserDirectoryName, UserFileName);

        string json = JsonConvert.SerializeObject(_users, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        });

        File.WriteAllText(filePath, json);
    }

    private void LoadData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), UserDirectoryName, UserFileName);

        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        _users = JsonConvert.DeserializeObject<Dictionary<int, User>>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        }) ?? new Dictionary<int, User>();

        if (_users.Any())
            _idCounter = _users.Keys.Max() + 1;
    }
}