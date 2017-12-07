/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : ClientTchat - Manage the connection between the client and the server
 * Author : GENGA Dario
 * Last update : 2017.11.30 (yyyy-MM-dd)
 */
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Tchat
{
    class ClientTchat
    {
        private ManualResetEvent _connectDone;
        private ManualResetEvent _sendDone;
        private const string SERVER_HOSTNAME = "CFPI-R107PC42";
        private const int PORT = 3001;

        public ClientTchat(string username, string password)
        {
            _connectDone = new ManualResetEvent(false);
            _sendDone = new ManualResetEvent(false);
            Start(username, password);
        }

        /// <summary>
        /// Recup the IP Address of the server by his hostname
        /// </summary>
        /// <param name="hostname">The hostname of the server</param>
        /// <returns>Return the IP address of the server</returns>
        public string GetServerIpAdress(string hostname)
        {
            var host = Dns.GetHostEntry(hostname);
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
        /// Start the connection with the server
        /// </summary>
        private void Start(string username, string password)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(GetServerIpAdress(SERVER_HOSTNAME)); // Recup the ip address of the server
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);
                
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a TCP/IP socket

                socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), socket); // Connect to the remote endpoint
                
                _connectDone.WaitOne(); // Wait for the connection with the server

                SendLogin(socket, username, password);
                
                // Shutdown and close the socket
                //socket.Shutdown(SocketShutdown.Both);
                //socket.Close();

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

                _connectDone.Set();
                Console.WriteLine("Connection with the server established !");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SendLogin(Socket client, string username, string password)
        {
            byte[][] login = { Encoding.ASCII.GetBytes(username), Encoding.ASCII.GetBytes(password) };


            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, login);
            byte[] byteData = ms.ToArray();
            

            // Convert the string data to byte data using ASCII encoding.
            //byte[] byteData = Encoding.ASCII.GetBytes(username);


            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, login.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                _sendDone.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
