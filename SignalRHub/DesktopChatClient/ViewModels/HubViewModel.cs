using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopChatClient.ViewModels
{
    public class HubViewModel : ViewModelBase
    {
        protected IHubProxy Proxy { get; set; }
        protected string url = "http://devbgsignalrdemo.azurewebsites.net/signalr";
        //protected string url = "http://localhost:54321/signalr";
        protected HubConnection Connection { get; set; }

        public HubViewModel()
        {
            {
                this.Connection = new HubConnection(url);
                this.Proxy = Connection.CreateHubProxy("SampleHub");

                try
                {
                    this.Connection.Start().Wait();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        internal void StopConnection()
        {
            if (this.Connection != null)
            {
                this.Connection.Stop();
            }
        }
    }
}
