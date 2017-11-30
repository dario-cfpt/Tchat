/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Class : Server - Manage the chat server (connection and data)
 * Author : GENGA Dario
 * Last update : 2017.11.30 (yyyy-MM-dd)
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerTchat
{
    class Server
    {
        private ManualResetEvent _eventConnect;
        private const int PORT = 3001;
        private const int BACKLOG = 100;

        /// <summary>
        /// Instantiate the server
        /// </summary>
        public Server()
        {
            _eventConnect = new ManualResetEvent(false);
            Start();
        }

        /// <summary>
        /// Get the local IP address of the machine
        /// <para>Source : https://stackoverflow.com/questions/6803073/get-local-ip-address/6803109#6803109 </para>
        /// </summary>
        /// <returns>Return the local IP address of the machine</returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// Start the server
        /// </summary>
        private void Start()
        {
            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress()); // Local IP of the server
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT); // Local endpoint of the server

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a TCP/IP socket

            try
            {
                socket.Bind(localEndPoint); // Associate the socket with the local endpoint
                socket.Listen(BACKLOG);
                while (true)
                {
                    _eventConnect.Reset(); // Reset the event of the connection, so we can listen a new connection

                    Console.WriteLine("Waiting for the connection...");
                    socket.BeginAccept(new AsyncCallback(AcceptCallback), socket); // Start an asynchronous socket to listen for connections

                    _eventConnect.WaitOne(); // Wait for a connection...
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Accept the asynchronous connection
        /// </summary>
        /// <param name="ar">The state of the asynchronous operation</param>
        public void AcceptCallback(IAsyncResult ar)
        {
            _eventConnect.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // TODO : commencer à traiter les données
        }



    }
}
