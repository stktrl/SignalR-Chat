using System;
using System.Collections.Generic;

namespace SignalR_Chat.Models
{
    public class User
    {
        public int orgid { get; set; }
        public int userid { get; set; }
        public string name { get; set; }
        public string connectionid { get; set; }
        public DateTime logoutdate { get; set; }
        public List<int> groupids { get; set; }
    }
}