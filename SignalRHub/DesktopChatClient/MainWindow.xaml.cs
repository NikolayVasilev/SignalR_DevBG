using DesktopChatClient.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var loginWindow = new LoginWindow();

            var result = loginWindow.ShowDialog();

            if(result != null && !result.Value)
            {
                Application.Current.Shutdown();
            }


            this.ViewModel = new ChatViewModel(loginWindow.ViewModel.UserName);
            this.DataContext = this.ViewModel;

            this.ViewModel.Messages.CollectionChanged += Messages_CollectionChanged;
        }

        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.MessageBoard.ScrollIntoView(this.ViewModel.Messages.LastOrDefault());
        }

        public ChatViewModel ViewModel { get; }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.SendCurrentMessage();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                this.ViewModel.SendCurrentMessage();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.CloseConnection();
            }

            base.OnClosing(e);
        }
    }
}
