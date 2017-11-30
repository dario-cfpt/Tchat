using System;
using System.Net;

namespace ServerTchat
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();

            server.Start(new IPEndPoint(IPAddress.Loopback, 3001));
            Console.ReadLine();
            server.Stop();

        }
    }
}
