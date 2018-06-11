using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRHub
{
    public class SampleHub : Hub
    {
        private static Dictionary<string, string> userNames = new Dictionary<string, string>();
        private static Timer adminNotificationTimer;

        static SampleHub()
        {
            adminNotificationTimer = new Timer();
            adminNotificationTimer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;

            adminNotificationTimer.Elapsed += AdminNotificationTimer_Elapsed;
            adminNotificationTimer.Start();
        }

        private static void AdminNotificationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (userNames.Keys.Contains("admin"))
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<SampleHub>();

                context.Clients.Client(userNames["admin"]).broadcastMessage("systemInfo", "ServiceUp");
            }
        }

        public void Send(string name, string message)
        {
            if (message.StartsWith("@"))
            {
                var splits = message.Split(new char[] { ' ' });

                if(splits.Length > 0)
                {
                    var userName = splits[0].Substring(1);
                    var messageText = message.Substring(splits[0].Length + 1);

                    if (userNames.Keys.Contains(userName))
                    {
                        Clients.Client(userNames[userName]).broadcastMessage(name, messageText);
                        if (name != userName)
                        {
                            Clients.Client(userNames[name]).broadcastMessage(name, messageText);
                        }
                    }
                    else
                    {
                        Clients.Client(userNames[name]).broadcastMessage("systemInfo", "User not logged in: " + userName);
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

            }
            else
            {
                userNames[userName] = Context.ConnectionId;
            }

            Clients.All.usersLoggedIn(userName);
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