using Microsoft.Extensions.Configuration;
using Nest;
using SignalR_Chat.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace SignalR_Chat.ElasticContext
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IConfiguration _configuration;
        public ElasticSearchService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private ElasticClient CreateClient(string indexName)
        {

            return new ElasticClient(new ConnectionSettings(new Uri(_configuration.GetSection("ElasticsearchOptions").GetSection("Host").Value)).DefaultIndex(indexName));
        }
        public async Task WriteMessageElastic(Message message)
        {
            var client  = CreateClient("ix_messages");
            var indexResponse = await client.IndexDocumentAsync<Message>(message);
            if (!indexResponse.IsValid)
            {
                throw new ArgumentException("Indexleme Hatalı!");
            }    
        }

        public User GetUserInfo(int userID, int orgID)
        {
            var client = CreateClient("ix_chatusers");
            var response = client.Search<User>(s=>s
                .Query(q=>q
                .Term(t=>t.userid,userID)&&
                q.Term(t=>t.orgid,orgID)
                ));

            User userInfo = new User();

            foreach (var item in response.Documents)
            {
                 userInfo = item;
            }
            return userInfo;
        }

        public List<Message> GetUnreadMessages(DateTime logOutTime, int userID, int orgID)
        {
            var client = CreateClient("ix_messages");
            var response = client.Search<Message>(s => s
                .Query(q => q
                    .Bool(b => b
                       .Filter(f=>f
                        .Term(t=>t.ReceiverID,userID)&&
                        q.Term(t=>t.OrgID,orgID)&&
                        q.DateRange(r=>r
                            .Field(f=>f.SendingDate)
                                .GreaterThanOrEquals(logOutTime)
                        )))));

            return new List<Message>(response.Documents);
        }

        public async Task WriteUserElasticAsync(User user)
        {
            var client = CreateClient("ix_chatusers");
            var indexResponse = await client.UpdateAsync<User>(string.Format("{0}{1}",user.userid,user.orgid),u=>u.Doc(user));
            if (!indexResponse.IsValid)
            {
                throw new ArgumentException("Indexleme Hatalı!");
            }
        }

        public List<User> GetUsersOfGroup(int groupID, int orgID)
        {
            var client = CreateClient("ix_chatusers");
            var response = client.Search<User>(s => s
               .Query(q => q
               .Term(t => t.groupids,groupID) &&
               q.Term(t => t.orgid, orgID)
               ));

            return new List<User>(response.Documents);
        }
    }
}
