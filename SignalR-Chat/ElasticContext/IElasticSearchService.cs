using SignalR_Chat.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalR_Chat.ElasticContext
{
    public interface IElasticSearchService
    {
        Task WriteMessageElastic(Message message);
        User GetUserInfo(int userID, int orgID);
        List<Message> GetUnreadMessages(DateTime logOutTime, int userID,int orgID);
        Task WriteUserElasticAsync(User user);
        List<User> GetUsersOfGroup(int groupID,int orgID);
        
    }
}
