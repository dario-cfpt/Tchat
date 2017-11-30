/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : ClientTchat - Manage the connection between the client and the server
 * Author : GENGA Dario
 * Last update : 2017.11.30 (yyyy-MM-dd)
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Tchat
{
    class ClientTchat
    {
        private ManualResetEvent _eventConnect;
        private const string SERVER_HOSTNAME = "CFPI-R113PC01";
        private const int PORT = 3001;

        public ClientTchat()
        {
            _eventConnect = new ManualResetEvent(false);
            Start();
        }

        private void Start()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(SERVER_HOSTNAME);
                IPAddress ipAddress = ipHostInfo.AddressList[1]; // Recup the ip address of the server (to check if error encountered)
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);
                
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a TCP/IP socket

                socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), socket); // Connect to the remote endpoint
                
                _eventConnect.WaitOne(); // Wait for the connection with the server

                // Shutdown and close the socket
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Established the connection between the client and the server
        /// </summary>
        /// <param name="ar">The state of the asynchronous operation</param>
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                _eventConnect.Set();

                // TODO : gérer la connection entre le client et le serveur 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
