using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChitChatServer
{
    class TcpServer
    {
        const string CONNECT = "CONNECT";
        const string CHAT = "CHAT";
        const int PORT = 22222;
        const int MAX_PENDING_REQUEST = 10;

        ManualResetEvent blocker = new ManualResetEvent(false);
        Socket listener;
        ConcurrentBag<ClientConnection> clientConnections = new ConcurrentBag<ClientConnection>();

        public void Run()
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endpoint = new IPEndPoint(IPAddress.Any, PORT);
            listener.Bind(endpoint);
            listener.Listen(MAX_PENDING_REQUEST);

            while (true)
            {
                // listen for a connection
                blocker.Reset();
                Console.WriteLine("Listening for new connections.");
                listener.BeginAccept(new AsyncCallback(AcceptCallback), null);
                blocker.WaitOne(); // wait until the a connection has been made, then start listening again
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            ClientConnection connection = new ClientConnection();
            connection.Socket = listener.EndAccept(ar);
            Receive(connection);

            blocker.Set(); // signal the loop to start listening for another connection.
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            ClientConnection connection = (ClientConnection)ar.AsyncState;
            try
            {
                int bytesRead = connection.Socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(connection.Buffer, 0, bytesRead);
                    string[] messageParts = message.Split(';');

                    if (messageParts.Length > 1)
                    {
                        string command = messageParts[0];
                        string deviceName = messageParts[1];
                        Console.WriteLine("{0} received from device: {1}", command, deviceName);

                        if (command == CONNECT)
                        {
                            connection.UserName = deviceName;
                            clientConnections.Add(connection);
                        }
                        Send(message);
                    }

                    Receive(connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured when attempting to receive from {0} : {1}", connection.UserName, ex.Message);
            }
        }

        private void Receive(ClientConnection connection)
        {
            try
            {
                connection.Socket.BeginReceive(connection.Buffer, 0, ClientConnection.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured when attempting to receive from {0} : {1}", connection.UserName, ex.Message);
            }
        }

        private void Send(String data)
        {
            foreach (var connection in clientConnections)
            {
                if (connection.Socket.Connected)
                {
                    Console.WriteLine("Sending {0}", data);
                    byte[] byteData = Encoding.UTF8.GetBytes(data);
                    connection.Socket.SendBufferSize = byteData.Length;
                    try
                    {
                        connection.Socket.BeginSend(byteData, 0, byteData.Length, 0,
                            new AsyncCallback(SendCallback), connection.Socket);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occured attempting to send to {0} : {1}", connection.UserName, ex.Message);
                    }
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int bytesSent = socket.EndSend(ar);
        }
    }
}
