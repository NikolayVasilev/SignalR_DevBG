using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopChatClient.ViewModels
{
    public class ChatViewModel : HubViewModel
    {
        private string mCurrentMessage;
        public string CurrentMessage
        {
            get { return mCurrentMessage; }
            set
            {
                if (value != null || value != mCurrentMessage)
                {
                    mCurrentMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string mUserName;
        public string UserName
        {
            get { return mUserName; }
            set
            {
                if (value != null || value != mUserName)
                {
                    mUserName = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<MessageViewModel> mMessages;
        public ObservableCollection<MessageViewModel> Messages
        {
            get { return mMessages; }
            set
            {
                if (value != null || value != mMessages)
                {
                    mMessages = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<string> mUsers;
        public ObservableCollection<string> Users
        {
            get { return mUsers; }
            set
            {
                if (value != null || value != mUsers)
                {
                    mUsers = value;
                    OnPropertyChanged();
                }
            }
        }

        public ChatViewModel(string userName)
        {
            this.UserName = userName;
            this.Proxy.Invoke("Register", this.UserName);

            this.Messages = new ObservableCollection<MessageViewModel>();
            this.Users = new ObservableCollection<string>();

            this.Proxy.On("broadcastMessage", (string from, string message) => this.OnBroadCastMessage(from, message));
            this.Proxy.On("usersLoggedIn", (string user) => this.OnUserLoggedIn(user));
            this.Proxy.On("usersLoggedOut", (string user) => this.OnUserLoggedOut(user));

            this.Proxy.On("usersListSend", (string[] users) => this.OnUsersListSent(users));

            this.Proxy.Invoke("RequestUserNames", this.UserName);
        }

        internal void CloseConnection()
        {
            this.StopConnection();
        }

        private void OnUsersListSent(string[] userNames)
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                foreach (var userName in userNames)
                {
                    if (!this.Users.Contains(userName))
                    {
                        this.Users.Add(userName);
                    }
                }
            }));

        }

        private void OnUserLoggedOut(string userName)
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.Users.Remove(userName);
            }));
        }

        private void OnUserLoggedIn(string userName)
        {
            if (!this.Users.Contains(userName))
            {
                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.Users.Add(userName);
                }));
            }
        }

        internal void OnBroadCastMessage(string from, string message)
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.Messages.Add(new MessageViewModel() { From = from, Message = message });
            }));
        }

        internal void SendCurrentMessage()
        {
            if (!string.IsNullOrEmpty(this.CurrentMessage))
            {
                this.Proxy.Invoke("Send", this.UserName, this.CurrentMessage).ContinueWith((t) => { this.CurrentMessage = string.Empty; });
            }
        }
    }
}
