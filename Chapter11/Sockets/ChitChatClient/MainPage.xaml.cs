using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ChitChatClient.Resources;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Text;

namespace ChitChatClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string CONNECT = "CONNECT";
        const string CHAT = "CHAT";
        const int PORT = 22222;
        const int MAX_BUFFER_SIZE = 1024;

        IPEndPoint endpoint;
        Socket socket;
        string user;

        readonly ObservableCollection<string> messages = new ObservableCollection<string>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            messageList.ItemsSource = messages;
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        
        private void connect_Click(object sender, EventArgs e)
        {
            Disconnect();
            IPAddress address;
            if (IPAddress.TryParse(serverIpAddress.Text, out address))
            {
                endpoint = new IPEndPoint(address, PORT);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                user = userName.Text;
                Connect();
            }
        }

        private void send_Click(object sender, EventArgs e)
        {
            if (socket.Connected && !string.IsNullOrEmpty(messageText.Text))
            {
                byte[] buffer = FormatPayload(CHAT, messageText.Text);

                var sendArgs = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };
                sendArgs.SetBuffer(buffer, 0, buffer.Length);
                sendArgs.Completed += send_Completed;
                socket.SendAsync(sendArgs);
            }
        }

        void send_Completed(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= send_Completed;
            if (e.SocketError != SocketError.Success)
                MessageBox.Show("Your message could not be sent");
            else
                Dispatcher.BeginInvoke(() => messageText.Text = string.Empty);
        }

        private byte[] FormatPayload(string command, string text)
        {
            string message = string.Format("{0};{1};{2}", command,
                     user, text);
            return Encoding.UTF8.GetBytes(message);
        }

        private void Disconnect()
        {
            endpoint = null;
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
        }

        private void Connect()
        {
            byte[] buffer = FormatPayload(CONNECT, string.Empty);
            var connectArgs = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };
            connectArgs.Completed += connect_Completed;
            connectArgs.SetBuffer(buffer, 0, buffer.Length);
            socket.ConnectAsync(connectArgs);
        }

        void connect_Completed(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= connect_Completed;
            if (e.SocketError != SocketError.Success)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Unable to connect to the chat server.", "Error", MessageBoxButton.OK);
                    Disconnect();
                });
            }
            else
            {
                var receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.RemoteEndPoint = endpoint;
                receiveArgs.SetBuffer(new byte[MAX_BUFFER_SIZE], 0, MAX_BUFFER_SIZE);
                receiveArgs.Completed += receive_Completed;
                socket.ReceiveAsync(receiveArgs);
            }
        }

        void receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                Dispatcher.BeginInvoke(() =>
                    MessageBox.Show("An error occured during server communication. " + e.SocketError));
            }
            else
            {
                string message = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);
                if (!string.IsNullOrEmpty(message))
                {
                    string[] msgParts = message.Split(';');
                    string newMessage;
                    if (msgParts[0] == CONNECT)
                        newMessage = string.Format("[{0} connected]", msgParts[1]);
                    else
                        newMessage = string.Format("{0}: {1}",
                            msgParts[1] == user ? "Me" : msgParts[1], msgParts[2]);
                    Dispatcher.BeginInvoke(() => messages.Add(newMessage));
                }

                // listen for another message, reusing the event args.
                if (socket != null)
                    socket.ReceiveAsync(e);
            }
        }
    }
}