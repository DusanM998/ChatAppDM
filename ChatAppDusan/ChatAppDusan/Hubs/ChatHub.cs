using ChatAppDusan.Dtos;
using ChatAppDusan.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppDusan.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;
        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "ChatAppDusan");
            await Clients.Caller.SendAsync("KorisnikPovezan");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChatAppDusan");
            var user = _chatService.GetUserByConnectionId(Context.ConnectionId);
            _chatService.RemoveUserFromList(user);
            await DisplayOnlineUsers();

            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddUserConnectionId(string name)
        {
            _chatService.AddUserConnectionId(name, Context.ConnectionId);
            await DisplayOnlineUsers();
        }

        public async Task ReceiveMessage(MessageDto message)
        {
            await Clients.Group("ChatAppDusan").SendAsync("NovaPoruka", message);
        }

        public async Task CreatePrivateChat(MessageDto message)
        {
            string privateGroupName = GetPrivateGroupName(message.From, message.To);
            await Groups.AddToGroupAsync(Context.ConnectionId, privateGroupName);
            var toConnectionId = _chatService.GetConnectionIdByUser(message.To);

            await Groups.AddToGroupAsync(toConnectionId, privateGroupName);

            await Clients.Client(toConnectionId).SendAsync("OpenPrivateChat", message);
        }

        public async Task ReceivePrivateMessage(MessageDto message)
        {
            string privateGroupName = GetPrivateGroupName(message.From, message.To);
            await Clients.Group(privateGroupName).SendAsync("NewPrivateMessage", message);
        }

        public async Task RemovePrivateChat(string from, string to)
        {
            string privateGroupName = GetPrivateGroupName(from, to);
            await Clients.Group(privateGroupName).SendAsync("ClosePrivateChat");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, privateGroupName);
            var toConnectionId = _chatService.GetConnectionIdByUser(to);
            await Groups.RemoveFromGroupAsync(toConnectionId, privateGroupName);
        }

        private async Task DisplayOnlineUsers()
        {
            var onlineUsers = _chatService.GetOnlineUsers();
            await Clients.Groups("ChatAppDusan").SendAsync("OnlineUsers", onlineUsers);
        }

        private string GetPrivateGroupName(string from, string to)
        {
            var stringCompare = string.CompareOrdinal(from, to) < 0;
            return stringCompare ? $"{from}-{to}" : $"{to}-{from}";
        }
    }
}
