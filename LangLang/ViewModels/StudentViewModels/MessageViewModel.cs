using GalaSoft.MvvmLight;
using LangLang.Models;
using System;

namespace LangLang.ViewModels.StudentViewModels
{
    public class MessageViewModel : ViewModelBase
    {
        private readonly Message _message;

        public MessageViewModel(Message message)
        {
            _message = message;
        }

        public int UserId => _message.UserId;
        public string Text => _message.Text;
        public DateTime DateTime => _message.DateTime;
    }
}