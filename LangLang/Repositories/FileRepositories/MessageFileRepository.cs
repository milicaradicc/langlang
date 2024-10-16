using System.Collections.Generic;
using System.IO;
using System.Linq;
using LangLang.Models;
using Newtonsoft.Json;

namespace LangLang.Repositories.FileRepositories;

public class MessageFileRepository : IMessageRepository
{
    private const string MessageFileName = "messages.json";
    private const string MessageDirectoryName = "data";

    private List<Message> _messages = new();

    public List<Message> GetAll()
    {
        LoadData();
        return _messages;
    }

    public List<Message> GetUserMessages(int userId)
    {
        LoadData();
        return _messages.Where(message => message.UserId == userId).ToList();
    }

    public void Add(Message message)
    {
        LoadData();
        _messages.Add(message);
        SaveData();
    }

    private void SaveData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), MessageDirectoryName, MessageFileName);

        string json = JsonConvert.SerializeObject(_messages, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        });

        File.WriteAllText(filePath, json);
    }

    private void LoadData()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), MessageDirectoryName, MessageFileName);

        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        _messages = JsonConvert.DeserializeObject<List<Message>>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        }) ?? new List<Message>();
    }
}