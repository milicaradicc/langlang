using GalaSoft.MvvmLight;
using LangLang.Models;
using LangLang.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace LangLang.ViewModels.StudentViewModels
{
    class InboxViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService = ServiceProvider.GetRequiredService<IMessageService>();
        private readonly ObservableCollection<MessageViewModel> _messages;
        private readonly int _studentId;
        public InboxViewModel(int studentId)
        {
            _studentId = studentId;
            _messages = new ObservableCollection<MessageViewModel>(_messageService.GetUserMessages(_studentId).Select(message => new MessageViewModel(message)));
            MessagesCollectionView = CollectionViewSource.GetDefaultView(_messages);
        }
        public ICollectionView MessagesCollectionView { get; }

        public ObservableCollection<MessageViewModel> Messages => _messages;
        private void RefreshMessages()
        {
            Messages.Clear();
            _messageService.GetUserMessages(_studentId).Select(message => new MessageViewModel(message));
            MessagesCollectionView.Refresh();
        }
    }
}