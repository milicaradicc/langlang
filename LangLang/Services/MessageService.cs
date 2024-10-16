using System.Collections.Generic;
using LangLang.Models;
using LangLang.Repositories;

namespace LangLang.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    
    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }
    
    public List<Message> GetAll()
    {
        return _messageRepository.GetAll();
    }

    public List<Message> GetUserMessages(int userId)
    {
        List<Message> messages = _messageRepository.GetUserMessages(userId);
        messages.Sort((a, b) => a.DateTime.CompareTo(b.DateTime));
        return messages;
    }

    public void Add(int userId, string text)
    {
        _messageRepository.Add(new Message(userId, text));
    }
}