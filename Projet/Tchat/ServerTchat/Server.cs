/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Class : Server - Manage the chat server (connection and data)
 * Author : GENGA Dario
 * Last update : 2017.12.14 (yyyy-MM-dd)
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
<<<<<<< HEAD
using System.Runtime.Serialization.Formatters.Binary;
=======
>>>>>>> f6cedd8d5f5bd9a4b353cd9d75a96269024a19ea
using System.Text;
using System.Threading;

namespace ServerTchat
{
    /// <summary>
    /// Manage the chat server (connection and data)
    /// </summary>
    class Server
    {
        private ManualResetEvent _connectDone = new ManualResetEvent(false);
        #region Database        
        private DatabaseConnection _dbConnect;
        private RequestsSQL _requestsSQL;
        public const string SERVER = "localhost";
        public const string DATABASE = "mytchatroomdb";
        public const string USER = "admin";
        public const string PASSWORD = "8185c8ac4656219f4aa5541915079f7b3743e1b5f48bacfcc3386af016b55320";
        #endregion Database
        private ServerRequest _svRequest;
        private const int PORT = 3001; // Port of the server
        private const int BACKLOG = 100; // The maximum length of the pending connections queue.

        /// <summary>
        /// Initialize the server and the database connection
        /// </summary>
        public Server()
        {
            DbConnect = new DatabaseConnection(SERVER, DATABASE, USER, PASSWORD);
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
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT); // Local endpoint of the server

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
<<<<<<< HEAD
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
=======
            StateObject state = new StateObject { WorkSocket = handler };
            handler.BeginReceive(state.Buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            string content = String.Empty;
>>>>>>> f6cedd8d5f5bd9a4b353cd9d75a96269024a19ea
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

<<<<<<< HEAD
            try
            {
                // Deserialize the buffer receive into a jagged byte array (byte[][])
                // The first element of the array is the command that will be executed later
                // The second element of the array is the data needed for this command (the data has to be a jagged byte array too !)
                MemoryStream ms = new MemoryStream(state.Buffer, 0, state.Buffer.Length);
                BinaryFormatter bf = new BinaryFormatter();
                byte[][] bufferReceive = (byte[][])bf.Deserialize(ms);

                // Deserialize the data of the array into another jagged byte array (byte[][])
                ms = new MemoryStream(bufferReceive[1], 0, bufferReceive[1].Length);
                bf = new BinaryFormatter();
                byte[][] dataReceive = (byte[][])bf.Deserialize(ms);

                // Recup the command string of the buffer and execute the command associate
                string command = Encoding.UTF8.GetString(bufferReceive[0]);
                switch (command)
                {
                    case "/login":
                        SvRequest.CommandLogin(handler, dataReceive);
                        break;
                    case "/createAccount":
                        SvRequest.CommandCreateAccount(handler, dataReceive);
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }

                // Wait for others data sended by the client
                // /!\ We have to send the state object to the AsyncCallback when we receive the data /!\
                handler.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                handler.Close();
                Console.WriteLine(ex);
            }

=======
            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                content = state.Sb.ToString();
                Console.WriteLine(content);
            }
            
>>>>>>> f6cedd8d5f5bd9a4b353cd9d75a96269024a19ea
        }

    }
}
