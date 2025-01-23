using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Librarian.Models;
using Librarian.Services;
using Librarian.ViewModels.Base;

namespace Librarian.ViewModels;

public partial class ChatViewModel : ViewModelBase, IQueryAttributable
{
    private readonly INavigationService _navigationService;
    private readonly IChatService _chatService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EmptyChatLabel))]
    private string _title = string.Empty;

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(IsChatEmpty))]
    private ObservableCollection<Message> _messages = [];

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(EntryBoxIcon))]
    private string _question = string.Empty;
    
    public Guid ChatId { get; set; }
    public ImageSource EntryBoxIcon => 
        Question == string.Empty ? ImageSource.FromFile("mic_icon") : ImageSource.FromFile("send_icon");
    public bool IsChatEmpty => !Messages.Any();
    public string EmptyChatLabel => $"Hello! Please ask me anything about {Title}.";

    public ChatViewModel(INavigationService navigationService, IChatService chatService)
    {
        _navigationService = navigationService;
        _chatService = chatService;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await _navigationService.GoBackAsync();
    }

    protected override async Task OnAppearingAsync()
    {
        var chat = await _chatService.GetCurrentChat(ChatId);
        Title = chat.Title;
        Messages = chat.Messages.ToObservableCollection();
        await base.OnAppearingAsync();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(ChatId), out var chatId))
        {
            ChatId = (Guid)chatId;
        }
    }
}