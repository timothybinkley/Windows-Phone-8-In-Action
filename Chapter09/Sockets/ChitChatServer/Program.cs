using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChitChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Exception ex = e.ExceptionObject as Exception;
                if (ex != null)
                    System.Diagnostics.Debug.WriteLine("Unhandled execption {0}\r\n{1}", ex.Message, ex.StackTrace);
            };
            TcpServer server = new TcpServer();
            server.Run();
        }
    }
}
