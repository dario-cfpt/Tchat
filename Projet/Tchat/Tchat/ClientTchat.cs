/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : ClientTchat - Manage the connection between the client and the server
 * Author : GENGA Dario
 * Last update : 2017.12.14 (yyyy-MM-dd)
 */
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace Tchat
{
    /// <summary>
    /// Manage the connection between the client and the server
    /// </summary>
    public class ClientTchat
    {
        private const string SERVER_HOSTNAME = "CFPI-R113PC12";
        private const int PORT = 3001;

        private Socket _client;
        private ClientRequest _clRequest;
        private bool _connected = false;
        private bool _logged = false;

        /// <summary>
        /// Manage the chat client (connection and data)
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        public ClientTchat()
        {
            StartConnection();
        }

        /// <summary>
        /// The socket of the client
        /// </summary>
        private Socket Client { get => _client; set => _client = value; }

        /// <summary>
        /// The requests between the client and the server
        /// </summary>
        private ClientRequest ClRequest { get => _clRequest; set => _clRequest = value; }

        /// <summary>
        /// The state of the connection of the client with the server
        /// <para>Is true when the user is connected</para>
        /// </summary>
        private bool Connected { get => _connected; set => _connected = value; }

        /// <summary>
        /// Indicate if the user is connected to the database (true = connected, false = not connected)
        /// </summary>
        public bool Logged { get => _logged; set => _logged = value; }

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
        /// Start the connection with the server and try to connect the user to the database
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        /// </summary>
        private void StartConnection()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(GetServerIpAdress(SERVER_HOSTNAME)); // Recup the ip address of the server
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);
                
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a TCP/IP socket

                Client.Connect(remoteEP); // Connect the Socket to the remote endpoint

                ClRequest = new ClientRequest(Client); // Create the request for the client

                Connected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Call the method who send the connection data to the server for connecting the user to the database
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        public void CallSendLogin(string username, string password)
        {
            if (Connected)
            {
                Logged = ClRequest.SendLogin(username, password); // Save the result of the connection
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
            }
        }
        
        /// <summary>
        /// Call the method who send the data for creating a new account to the database
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="password">The password of the user</param>
        /// <param name="email">The email of the user</param>
        /// <param name="phone">The phone of the user</param>
        /// <returns>Return true if the account was correctly created, if not return false</returns>
        public bool CallSendNewAccount(string username, string password, string email, string phone)
        {
            if (Connected)
            {
                return ClRequest.SendNewAccount(username, password, email, phone); // Return the response of the server
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return false;
            }
        }

        /// <summary>
        /// Call the method who will recup a username and get the id of avatar and backgroud images
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return an array of string who contain the id of the avatar image (first index) and the id of the background image (second index)</returns>
        public string[] CallSendUsernameToGetUserImagesId(string username)
        {
            if (Connected)
            {
                return ClRequest.SendUsernameToGetUserImagesId(username);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }

        public Image CallSendIdImage(string id)
        {
            if (Connected)
            {
                return ClRequest.SendImageId(id);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }
    }
}
