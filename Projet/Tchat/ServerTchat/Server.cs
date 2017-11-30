using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ServerTchat
{
    class Server
    {
        private TcpListener _tcpListener;

        public Server()
        {
            // Do nothing
        }

        public void Start(IPEndPoint ipEndPoint)
        {
            _tcpListener = new TcpListener(ipEndPoint);
            _tcpListener.Start();

            // Wait a client to connect
            _tcpListener.BeginAcceptTcpClient(HandleClient, null);
        }

        public void Stop()
        {
            _tcpListener.Stop();
        }

        public void HandleClient(IAsyncResult res)
        {
            try
            {
                while (true)
                {
                    // the client is connected, we recover him
                    TcpClient client = _tcpListener.EndAcceptTcpClient(res);

                    // Wait another client to connect
                    _tcpListener.BeginAcceptTcpClient(HandleClient, null);

                    // start the diaglog with the client
                    ProcessClient(client);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Server error : {0}", ex.Message);
            }
        }

        public void ProcessClient(TcpClient client)
        {
            // The connection is established, we can start the dialog between the client and server
            try
            {
                StreamReader reader = new StreamReader(client.GetStream());

                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
