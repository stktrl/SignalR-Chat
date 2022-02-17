using System;
using System.Collections.Generic;

namespace SignalR_Chat.Models
{
    public class Message
    { 
        public string Content { get; set; }
        public DateTime SendingDate{ get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public int OrgID { get; set; }
        public List<string> Mentions { get; set; }
        public MessageType Type { get; set; }

    }
    public enum MessageType
    {
        Group = 0,
        Direct = 1,
    }
}
