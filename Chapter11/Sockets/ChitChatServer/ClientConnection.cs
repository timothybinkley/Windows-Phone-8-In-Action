using System.Net.Sockets;

namespace ChitChatServer
{
    class ClientConnection
    {
        public const int BufferSize = 1024;
        public string UserName;
        public Socket Socket;
        public byte[] Buffer = new byte[BufferSize];
    }
}
