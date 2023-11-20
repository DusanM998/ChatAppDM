using ChatAppWithDb.DbContexts;
using ChatAppWithDb.Models;
using ChatAppWithDb.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppWithDb.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;
        private readonly ApplicationDbContexts _db;
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
            int connectionId;
            if (int.TryParse(Context.ConnectionId, out connectionId))
            {
                var user = _chatService.GetUserByConnectionId(connectionId);
                _chatService.RemoveUserFromList(user);
                await DisplayOnlineUsers();
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddUserConnectionId(string name)
        {
            int connectionId;
            if (int.TryParse(Context.ConnectionId, out connectionId))
            {
                _chatService.AddUserConnectionId(name, connectionId);
                await DisplayOnlineUsers();
            }
            else
            {
                Console.WriteLine($"Nije moguce konvertovati ConnectionId {Context.ConnectionId} u int.");
            }
        }

        public async Task ReceiveMessage(Message message)
        {
            _db.Messages.Add(message);
            _db.SaveChanges();
            await Clients.Group("ChatAppDusan").SendAsync("NovaPoruka", message);
        }
        public async Task GetMessages()
        {
            var messages = _db.Messages.ToList();
            await Clients.Caller.SendAsync("Poruke", messages);
        }

        public async Task CreatePrivateChat(Message message)
        {
            string privateGroupName = GetPrivateGroupName(message.From, message.To);
            await Groups.AddToGroupAsync(Context.ConnectionId, privateGroupName);
            var toConnectionId = _chatService.GetConnectionIdByUser(message.To);

            await Groups.AddToGroupAsync(toConnectionId, privateGroupName);

            await Clients.Client(toConnectionId).SendAsync("OpenPrivateChat", message);
        }

        public async Task ReceivePrivateMessage(Message message)
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
