using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRHub
{
    public class SampleHub : Hub
    {
        private static Dictionary<string, string> userNames = new Dictionary<string, string>();

        public void Send(string name, string message)
        {
            if (message.StartsWith("@"))
            {
                var splits = message.Split(new char[] { ' ' });

                if(splits.Length > 0)
                {
                    var userName = splits[0].Substring(1);
                    var messageText = message.Substring(splits[0].Length + 1);

                    Clients.Client(userNames[userName]).broadcastMessage(name, messageText);
                    if (name != userName)
                    {
                        Clients.Client(userNames[name]).broadcastMessage(name, messageText);
                    }
                }
            }
            else
            {
                Clients.All.broadcastMessage(name, message);
            }
        }

        public void Register(string userName)
        {
            if (!userNames.ContainsKey(userName))
            {
                userNames.Add(userName, Context.ConnectionId);

                Clients.All.usersLoggedIn(userName);
            }
        }

        public void RequestUserNames(string userName)
        {
            var users = userNames.Keys.ToArray();

            Clients.Client(userNames[userName]).usersListSend(users);
        }


        public override Task OnConnected()
        {
            var id = Context.ConnectionId;

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var userName = string.Empty;

            foreach (var key in userNames.Keys)
            {
                if(userNames[key] == Context.ConnectionId)
                {
                    userName = key;
                }
            }

            userNames.Remove(userName);
            Clients.All.usersLoggedOut(userName);

            return base.OnDisconnected(stopCalled);
        }
    }
}