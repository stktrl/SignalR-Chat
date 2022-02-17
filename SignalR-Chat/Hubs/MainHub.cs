using Microsoft.AspNetCore.SignalR;
using SignalR_Chat.Datas;
using SignalR_Chat.ElasticContext;
using SignalR_Chat.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalR_Chat.Hubs
{
    public class MainHub : Hub
    {
        private IElasticSearchService _elasticSearchService;
        public MainHub(IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userInfo = ClientSource.Clients.Find(c => c.connectionid == Context.ConnectionId);
            ClientSource.Clients.Remove(userInfo);
            userInfo.logoutdate=DateTime.Now;
            _elasticSearchService.WriteUserElasticAsync(userInfo);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task LogInAndGetUserIDAsync(int userID, int orgID, string name)
        {
            User UserInfo = _elasticSearchService.GetUserInfo(userID, orgID);
            UserInfo.connectionid = Context.ConnectionId;
            
            var unreadMessages = _elasticSearchService.GetUnreadMessages( UserInfo.logoutdate,userID, orgID);
            ClientSource.Clients.Add(UserInfo);
            foreach (var item in unreadMessages)//test için normalde zaten liste obje dönecek
            {
                await Clients.Caller.SendAsync("receivemessage", item.Content);
            }
           //client tarafta okunmamış mesajlar fonksiyonu oluşturulacak.
                    
        }
        //grup eklemesi veya oluşturulması durumunda tetikelenecek bir fonksiyon yazılacak ve grup idleri belirlenip userlara atanacak
        public async Task SendDirectMessageAsync(string content,int receiverID)
        {
            var Sender = ClientSource.Clients.Find(c => c.connectionid == Context.ConnectionId);
            var Receiver = ClientSource.Clients.Find(c=>c.userid == receiverID);
            Message message = new Message()
            {
                SenderID = Sender.userid,
                OrgID=Sender.orgid,
                SendingDate = DateTime.Now,
                Content = content,
                //Mentions
                ReceiverID= receiverID,
                Type=MessageType.Direct,
        
                
            };
            _elasticSearchService.WriteMessageElastic(message);
            if (Receiver!=null)
            {
                await Clients.Client(Receiver.connectionid).SendAsync("receivemessage",message.Content);// obje dönülmeli
            }
            
        }
        public async Task SendGroupMessageAsync(string content,int[] receiverIDs)
        {
            var Sender = ClientSource.Clients.Find(c => c.connectionid == Context.ConnectionId);
            Message message = new Message()
            {
                SenderID= Sender.userid,
                OrgID=Sender.orgid,
                SendingDate= DateTime.Now,
                Content= content,
                //Mentions
                Type=MessageType.Group
            };
            foreach (var receiverID in receiverIDs)
            {
                var Receiver = ClientSource.Clients.Find(c => c.userid == receiverID);
                message.ReceiverID = receiverID;
                _elasticSearchService.WriteMessageElastic(message);
                await Clients.Client(Receiver.connectionid).SendAsync("receiveGroupMessage", message);
            }

            
        }
    }
}
