using System.Collections.Generic;
using LangLang.Models;

namespace LangLang.Repositories;

public interface IMessageRepository
{
    public List<Message> GetAll();
    public List<Message> GetUserMessages(int userId);
    public void Add(Message message);
}