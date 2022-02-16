using Microsoft.AspNetCore.SignalR;
using SignalR_Chat.Datas;
using SignalR_Chat.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalR_Chat.Hubs
{
    public class MainHub : Hub
    {
        public async Task LogInAndGetUserID(int userID, int orgID, List<string> groupsName, DateTime logOutDate, string name)
        {
            User user = new User()
            {
                ConnectionID = Context.ConnectionId,
                Id = userID,
                OrgID = orgID,
                GroupsName = groupsName,
                LogOutDate = logOutDate,
                Name = name
            };
            ClientSource.Clients.Add(user);
            //ya kullanıcı bağlanırken grup isimlerini göndericek
            //ya da bağlandığında elasticten user indexi içerisinden
            //grup isimleri okunacak 
            if (groupsName.Count > 0)
            {   
                foreach (string groupName in groupsName)
                {
                    Group group = new Group()
                    {
                        GroupName = groupName,
                    };
                    GroupSource.Groups.Add(group);
                    await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
                    
                }
            }
            
        }
        public async Task SendDirectMessageAsync(string content,int receiverID)
        {
            var Sender = ClientSource.Clients.Find(c => c.ConnectionID == Context.ConnectionId);
            var Receiver = ClientSource.Clients.Find(c=>c.Id == receiverID);
            Message message = new Message()
            {
                SenderID = Sender.Id,
                OrgID=Sender.OrgID,
                SendingDate = DateTime.Now,
                Content = content,
                //Mentions
                ReceiverID= receiverID
            };
            //elastiğe yaz
            await Clients.Client(Receiver.ConnectionID).SendAsync("receivemessage",message);
        }
        public async Task SendGroupMessageAsync(string content,string groupName)
        {
            //message modeline group name ismi eklenebilir
            //ya da groupmessage adında ayrı bir model oluşturulup elasticsearch için indexleme yapılmalı
        }
    }
}
