using SignalR_Chat.Models;
using System.Collections.Generic;

namespace SignalR_Chat.Datas
{
    public static class ClientSource
    {
        public static List<User> Clients { get; set; } = new List<User> ();
    }
}
