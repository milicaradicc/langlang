using System;

namespace LangLang.Models;

public class Message
{
    public int UserId { get; }
    public string Text { get; }
    public DateTime DateTime { get; } = DateTime.Now;
    
    public Message(int userId, string text)
    {
        UserId = userId;
        Text = text;
    }
}