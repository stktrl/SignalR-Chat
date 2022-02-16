using System;
using System.Collections.Generic;

namespace SignalR_Chat.Models
{
    public class User
    {
        public int OrgID { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ConnectionID { get; set; }
        public DateTime LogOutDate { get; set; }
        public List<string> GroupsName { get; set; }

       

    }
}