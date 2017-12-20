/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : ClientTchat - Manage the connection between the client and the server
 * Author : GENGA Dario
 * Last update : 2017.12.17 (yyyy-MM-dd)
 */
using System;
using System.Collections.Generic;
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

        private Socket _clientSocket;
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
        public Socket ClientSocket { get => _clientSocket; set => _clientSocket = value; }

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
                
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create a TCP/IP socket

                ClientSocket.Connect(remoteEP); // Connect the Socket to the remote endpoint

                ClRequest = new ClientRequest(ClientSocket); // Create the request for the client

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
        public void TryLogin(string username, string password)
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
        /// Call the method that send a name to check if an user exist with this name
        /// </summary>
        /// <param name="username">The name of the user to check</param>
        /// <returns>Return true if the user exist, else return false</returns>
        public bool CheckIfUserExist(string username)
        {
            if (Connected)
            {
                return ClRequest.SendUsernameToCheckIfUserExist(username); // Save the result of the check
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return false;
            }
        }

        /// <summary>
        /// Call the method that send the ID of the user sender and the user receiver to check if they don't already have a friend request
        /// </summary>
        /// <param name="idUserSender">The id of the user who send the request</param>
        /// <param name="idUserReceiver">The id of the user who recovers the request</param>
        /// <returns>Return true if they are already a friend request, else return false</returns>
        public bool CheckIfFriendRequestAlreadyExist(string idUserSender, string idUserReceiver)
        {
            if (Connected)
            {
                return ClRequest.SendIdUsersToCheckFriendRequest(idUserSender, idUserReceiver); // Save the result of the check
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return false;
            }
        }

        /// <summary>
        /// Call the method that send the name of the user sender and the user receiver to check if they aren't a duplicate friend request
        /// </summary>
        /// <param name="userSender">The name of the user who send the request</param>
        /// <param name="userReceiver">The name of the user who recovers the request</param>
        /// <returns>Return true if they are a duplicte friend request, else return false</returns>
        public bool CheckDuplicateFriendRequest(string userSender, string userReceiver)
        {
            if (Connected)
            {
                return ClRequest.SendUsernamesToCheckDuplicateFriendRequest(userSender, userReceiver); // Save the result of the check
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return false;
            }
        }

        /// <summary>
        /// Call the method that send the ID of the user sender and the user receiver to create a friend request
        /// </summary>
        /// <param name="idUserSender">The id of the user who send the request</param>
        /// <param name="idUserReceiver">The id of the user who recovers the request</param>
        /// <param name="messageRequest">The message who accompanies the request</param>
        /// <returns>Return true if the resquest has been correctly sended, else return false</returns>
        public bool CreateFriendRequest(string idUserSender, string idUserReceiver, string messageRequest)
        {
            if (Connected)
            {
                return ClRequest.SendIdUsersToCreateFriendRequest(idUserSender, idUserReceiver, messageRequest); // Save the result of the creation
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return false;
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
        public bool CreateNewAccount(string username, string password, string email, string phone)
        {
            if (Connected)
            {
                return ClRequest.SendNewAccount(username, password, email, phone);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return false;
            }
        }

        /// <summary>
        /// Call the method who will recup the id of an user by his username
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return the id of the user</returns>
        public string GetUserIdByUsername(string username)
        {
            if (Connected)
            {
                return ClRequest.SendUsernameToGetUserId(username);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }

        /// <summary>
        /// Call the method who will recup the username of an user by his id
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>Return the username of the user</returns>
        public string GetUsernameByUserId(string id)
        {
            if (Connected)
            {
                return ClRequest.SendUserIdToGetUsername(id);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }

        /// <summary>
        /// Call the method who will recup a username and get the id of avatar and backgroud images
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return an array of string who contain the id of the avatar image (first index) and the id of the background image (second index)</returns>
        public string[] GetUserImagesIdByUsername(string username)
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

        /// <summary>
        /// Call the method who create an image by his id
        /// </summary>
        /// <param name="id">The id of the image</param>
        /// <returns>Return the image that corresponds to the id</returns>
        public Image GetImageById(string id)
        {
            if (Connected)
            {
                return ClRequest.SendImageIdToGetImage(id);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }

        /// <summary>
        /// Call the method that return a profil of a user
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return a dictionary who contains the profil of the user</returns>
        public Dictionary<string, string> GetProfilByUsername(string username)
        {
            if (Connected)
            {
                return ClRequest.SendUsernameToGetProfil(username);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }

        /// <summary>
        /// Call the method that return the friend list of an user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>Return a list who contains the friend list of the user</returns>
        public List<string[]> GetFriendsListByUserId(string userId)
        {
            if (Connected)
            {
                return ClRequest.SendUserIdToGetFriendsList(userId);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }

        /// <summary>
        /// Call the method who recovers the image of a friend by his id
        /// </summary>
        /// <param name="id">The id of the friend</param>
        /// <returns>Return the image that corresponds to the id</returns>
        public Image GetFriendAvatarById(string id)
        {
            if (Connected)
            {
                return ClRequest.SendFriendIdToGetFriendAvatar(id);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }

        /// <summary>
        /// Call the method who will update the statut of an user
        /// </summary>
        /// <param name="statut">The new statut for the user</param>
        /// <param name="username">The name of the user</param>
        /// <returns>Return false if an error has been encountered, else return true</returns>
        public bool UpdateStatutOfUser(string statut, string username)
        {
            if (Connected)
            {
                return ClRequest.SendNewStatutForUser(statut, username);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return false;
            }
        }

        /// <summary>
        /// Call the method who recovers the password of an user
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return the password of the user</returns>
        public string GetPasswordOfUser(string username)
        {
            if (Connected)
            {
                return ClRequest.SendUsernameToGetPassword(username);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }

        /// <summary>
        /// Call the method who recovers the centers of interests of an user
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return the password of the user</returns>
        public string GetHobbiesOfUser(string username)
        {
            if (Connected)
            {
                return ClRequest.SendUsernameToGetHobbies(username);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }

        /// <summary>
        /// Call the method who will send the data to update the profil of an user
        /// </summary>
        /// <param name="password">The name of the user</param>
        /// <param name="email">The email of the user</param>
        /// <param name="phone">The phone of the user</param>
        /// <param name="description">The description of the user</param>
        /// <param name="hobbies">The centers of interests of the user</param>
        public void UpdateProfilUser(string username, string email, string phone, string description, string hobbies)
        {
            if (Connected)
            {
                ClRequest.SendUpdatedProfilOfUser(username, email, phone, description, hobbies);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
            }
        }

        /// <summary>
        /// Call the method who will send the data to update the image of an user
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <param name="columnImg">The column of the image</param>
        /// <param name="idImg">The id of the image</param>
        public void UpdateIdImageOfUser(string username, string columnImg, string idImg)
        {
            if (Connected)
            {
                ClRequest.SendUpdatedImageIdOfUser(username, columnImg, idImg);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
            }
        }

        /// <summary>
        /// Call the method who will send updated password of an user
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <param name="password">The password of the user</param>
        public void UpdatePasswordOfUser(string username, string password)
        {
            if (Connected)
            {
                ClRequest.SendUpdatedPassword(username, password);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
            }
        }

        /// <summary>
        /// Call the method who send an image to the server and recovers his id
        /// </summary>
        /// <param name="img">The image to send</param>
        /// <returns>Return the password of the user</returns>
        public string InsertImage(Image img)
        {
            if (Connected)
            {
                return ClRequest.SendImageAndGetId(img);
            }
            else
            {
                Console.WriteLine("The socket must be connected to the server !");
                return null;
            }
        }
    }
}
