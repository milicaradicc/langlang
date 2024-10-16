using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Services;

public interface IMessageService
{
    public List<Message> GetAll();
    public List<Message> GetUserMessages(int userId);
    public void Add(int userId, string text);
}