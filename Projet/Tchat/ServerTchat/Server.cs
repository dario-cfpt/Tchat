/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Class : Server - Manage the chat server (connection and data)
 * Author : GENGA Dario
 * Last update : 2017.12.17 (yyyy-MM-dd)
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace ServerTchat
{
    /// <summary>
    /// Manage the chat server (connection and data)
    /// </summary>
    class Server
    {//TODO : prevent all client when we the server is disconnected
        private ManualResetEvent _connectDone = new ManualResetEvent(false);
        #region Database        
        private DatabaseConnection _dbConnect;
        private RequestsSQL _requestsSQL;
        public const string SERVER = "localhost";
        public const string PORT = "3306";
        public const string DATABASE = "mytchatroomdb";
        public const string USER = "admin";
        public const string PASSWORD = "8185c8ac4656219f4aa5541915079f7b3743e1b5f48bacfcc3386af016b55320";
        #endregion Database
        private ServerRequest _svRequest;
        private const int APP_PORT = 3001; // Port of the app server
        private const int BACKLOG = 100; // The maximum length of the pending connections queue.

        /// <summary>
        /// Initialize the server and the database connection
        /// </summary>
        public Server()
        {
            DbConnect = new DatabaseConnection(SERVER, PORT, DATABASE, USER, PASSWORD);
            RequestsSQL = new RequestsSQL(DbConnect.Connection);
            SvRequest = new ServerRequest(RequestsSQL);
            Start();
        }

        /// <summary>
        /// Signal of the thread for the connection
        /// </summary>
        public ManualResetEvent ConnectDone { get => _connectDone; set => _connectDone = value; }

        /// <summary>
        /// The connection to the MySQL database
        /// </summary>
        private DatabaseConnection DbConnect { get => _dbConnect; set => _dbConnect = value; }

        /// <summary>
        /// The SQL resquests for the database
        /// </summary>
        private RequestsSQL RequestsSQL { get => _requestsSQL; set => _requestsSQL = value; }

        /// <summary>
        /// The requests between the server and the database
        /// </summary>
        private ServerRequest SvRequest { get => _svRequest; set => _svRequest = value; }

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
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, APP_PORT); // Local endpoint of the server

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a TCP/IP socket

            try
            {
                socket.Bind(localEndPoint); // Associate the socket with the local endpoint
                socket.Listen(BACKLOG); // Start listening the socket
                while (true)
                {
                    ConnectDone.Reset(); // Reset the event of the connection, so we can listen a new connection

                    Console.WriteLine("Waiting for a connection...");
                    socket.BeginAccept(new AsyncCallback(AcceptCallback), socket); // Start an asynchronous Socket to listen for connections

                    ConnectDone.WaitOne(); // Wait for a connection...
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Accept the asynchronous connection when the client try to connect to the server
        /// </summary>
        /// <param name="ar">The socket of the client who is trying to connect to the server</param>
        public void AcceptCallback(IAsyncResult ar)
        {
            Console.WriteLine("New asynchronous connection !");
            ConnectDone.Set(); // Signal the main thread to continue.

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.WorkSocket = handler;

            // Wait for data sended by the client
            // /!\ We have to send the state object to the AsyncCallback when we receive the data /!\
            handler.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, new AsyncCallback(ReadCallback), state);

        }

        /// <summary>
        /// Read the data sended by the client (state object) and execute a command from the data
        /// </summary>
        /// <param name="ar">The state object who containt the data sended by the user</param>
        public void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

            try
            {
                // Deserialize the buffer receive into a jagged byte array (byte[][])
                // The first element of the array is the command that will be executed later
                // The second element of the array is the data needed for this command (the data has to be a jagged byte array too !)
                MemoryStream ms = new MemoryStream(state.Buffer);
                BinaryFormatter bf = new BinaryFormatter();
                byte[][] bufferReceive = (byte[][])bf.Deserialize(ms);

                // Deserialize the data of the array into another jagged byte array (byte[][])
                ms = new MemoryStream(bufferReceive[1]);
                bf = new BinaryFormatter();
                byte[][] receivedData = (byte[][])bf.Deserialize(ms);
                
                // Recup the command string of the buffer and execute the command associate
                string command = Encoding.UTF8.GetString(bufferReceive[0]);
                switch (command)
                {
                    case "/login":
                        SvRequest.CommandLogin(handler, receivedData);
                        break;
                    case "/createAccount":
                        SvRequest.CommandCreateAccount(handler, receivedData);
                        break;
                    case "/GetUsername":
                        SvRequest.CommandGetUsernameByUserId(handler, receivedData);
                        break;
                    case "/GetUserId":
                        SvRequest.CommandGetUserIdByUsername(handler, receivedData);
                        break;
                    case "/GetPassword":
                        SvRequest.CommandGetUserPasswordByUsername(handler, receivedData);
                        break;
                    case "/GetHobbies":
                        SvRequest.CommandGetUserHobbiesByUsername(handler, receivedData);
                        break;
                    case "/GetUserImagesId":
                        SvRequest.CommandGetUserImagesIdByUsername(handler, receivedData);
                        break;
                    case "/GetImage":
                        SvRequest.CommandGetImageById(handler, receivedData);
                        break;
                    case "/GetProfil":
                        SvRequest.CommandGetProfilByUsername(handler, receivedData);
                        break;
                    case "/GetFriendsList":
                        SvRequest.CommandGetFriendsListByUserId(handler, receivedData);
                        break;
                    case "/GetFriendAvatar":
                        SvRequest.CommandGetFriendAvatarByFriendId(handler, receivedData);
                        break;
                    case "/UpdateStatut":
                        SvRequest.CommandUpdateStatut(handler, receivedData);
                        break;
                    case "/UpdateProfil":
                        SvRequest.CommandUpdateProfil(handler, receivedData);
                        break;
                    case "/UpdatePassword":
                        SvRequest.CommandUpdatePassword(handler, receivedData);
                        break;
                    case "/UpdateIdImage":
                        SvRequest.CommandUpdateIdImageOfUser(handler, receivedData);
                        break;
                    case "/InsertImage":
                        SvRequest.CommandInsertImage(handler, receivedData);
                        break;
                    case "/CheckUser":
                        SvRequest.CommandCheckIfUserExist(handler, receivedData);
                        break;
                    case "/CheckFriendRequest":
                        SvRequest.CommandCheckFriendRequest(handler, receivedData);
                        break;  
                    case "/CheckDuplicateFriendRequest":
                        SvRequest.CommandCheckDuplicateFriendRequest(handler, receivedData);
                        break;
                    case "/CreateFriendRequest":
                        SvRequest.CommandCreateFriendRequest(handler, receivedData);
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        handler.Send(new byte[0]);
                        break;
                }

                // Wait for others data sended by the client
                // /!\ We have to send the state object to the AsyncCallback when we receive the data /!\
                handler.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                state.WorkSocket.Close();
                Console.WriteLine(ex);
            }

        }

    }
}
