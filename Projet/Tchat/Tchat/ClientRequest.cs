/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : ClientRequest - Manage the requests between the client and the server
 * Author : GENGA Dario
 * Last update : 2017.12.14 (yyyy-MM-dd)
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Tchat
{
    /// <summary>
    /// Manage the requests between the client and the server
    /// </summary>
    class ClientRequest
    {
        private Socket _client;
        
        /// <summary>
        /// Initialize the requests of the client for the server
        /// </summary>
        /// <param name="client">The socket of the client</param>
        public ClientRequest(Socket client)
        {
            Client = client;   
        }

        /// <summary>
        /// The socket of the client
        /// </summary>
        private Socket Client { get => _client; set => _client = value; }

        #region UtilityMethods
        /// <summary>
        /// Serialize a jagged byte array (byte[][]) into a byte array (byte[]) and send it
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Return the serialized jagged byte array</returns>
        private byte[] ConvertJaggedByteToByteArray(byte[][] data)
        {
            // Serialize the connection data to get a byte array
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, data);
            byte[] byteData = ms.ToArray();

            return byteData;
        }

        /// <summary>
        /// Serialize the buffer and send it to the server
        /// </summary>
        /// <param name="command">The name of the command that the server will execute</param>
        /// <param name="jaggedData">A jagged byte array of the data for the command</param>
        private void SerializeAndSendBuffer(string command, byte[][] jaggedData)
        {
            // Create another jagged byte array who contain the command to execute and the data to send
            byte[][] jaggedCommand = { Encoding.UTF8.GetBytes(command), ConvertJaggedByteToByteArray(jaggedData) };

            // Serialize the jagged command into a byte array and send it
            byte[] buffer = ConvertJaggedByteToByteArray(jaggedCommand);
            Client.Send(buffer);
        }

        /// <summary>
        /// Receive a boolean response of the server
        /// </summary>
        /// <returns>Return the result of the server</returns>
        private bool GetBooleanResult()
        {
            // We create a byte array who will receive the result of the connection attempt
            // The byte array has a size of 1 because we don't need more for a boolean
            byte[] byteResult = new byte[1];
            Client.Receive(byteResult);

            // Convert the result into a boolean. Then we save it.
            bool result = BitConverter.ToBoolean(byteResult, 0);

            return result;
        }
        #endregion UtilityMethods
        #region DataMethods
        /// <summary>
        /// Send the connection data to the server for connecting the user to the database
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Return the response of request by the server</returns>
        public bool SendLogin(string username, string password)
        {
            // Create a jagged byte array who contain the username and password of the user
            byte[][] login = { Encoding.UTF8.GetBytes(username), Encoding.UTF8.GetBytes(password) };

            // Serialize the command and data and send it
            SerializeAndSendBuffer("/login", login);

            // Recup the response of the server and save it
            bool accepted = GetBooleanResult();
            return accepted;
        }

        /// <summary>
        /// Send the name of an user to get his id
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return the id of the user</returns>
        public string SendUsernameToGetUserId(string username)
        {
            // Create a jagged byte array who contain the username (we have to create a jagged even if there is only one element)
            byte[][] user = { Encoding.UTF8.GetBytes(username) };

            // Serialize the command and data and send it
            SerializeAndSendBuffer("/GetUserId", user);

            // Receive the buffer by the server
            byte[] buffer = new byte[3];
            Client.Receive(buffer);

            // Get the id of the user and return it
            string id = Encoding.UTF8.GetString(buffer);
            id = id.Replace("\0", ""); // this avoid to have \0 in the end of the string if the id lenght is inferior less than the buffer size 
            return id;
        }

        /// <summary>
        /// Send the id of an user to get his username
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>Return the name of the user</returns>
        public string SendUserIdToGetUsername(string userId)
        {
            // Create a jagged byte array who contain the id (we have to create a jagged even if there is only one element)
            byte[][] id = { Encoding.UTF8.GetBytes(userId) };

            // Serialize the command and data and send it
            SerializeAndSendBuffer("/GetUsername", id);

            // Receive the buffer by the server
            byte[] buffer = new byte[20]; // The size of this buffer is the username maxlenght authorized in the database
            Client.Receive(buffer);

            // Get the username of the user and return it
            string username = Encoding.UTF8.GetString(buffer);
            username = username.Replace("\0", ""); // this avoid to have \0 in the end of the string if the name lenght is inferior less than the buffer size 
            return username;
        }

        /// <summary>
        /// Send the data for creating a new account to the database
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="password">The password of the user</param>
        /// <param name="email">The email of the user</param>
        /// <param name="phone">The phone of the user</param>
        /// <returns>Return the response of the server</returns>
        public bool SendNewAccount(string username, string password, string email, string phone)
        {
            // Create a jagged byte array who contain the data for the new account
            byte[][] account = {
                Encoding.UTF8.GetBytes(username),
                Encoding.UTF8.GetBytes(password),
                Encoding.UTF8.GetBytes(email),
                Encoding.UTF8.GetBytes(phone)
            };
            
            // Serialize the command and data and send it
            SerializeAndSendBuffer("/createAccount", account);
            
            // Recup the response of the server and return it
            bool created = GetBooleanResult();
            return created;
        }

        /// <summary>
        /// Send a username and get the id of avatar and backgroud images
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return an array of string who contain the id of the avatar image (first index) and the id of the background image (second index)</returns>
        public string[] SendUsernameToGetUserImagesId(string username)
        {
            // Create a jagged byte array who contain the username (we have to create a jagged even if there is only one element)
            byte[][] user = { Encoding.UTF8.GetBytes(username) };

            // Serialize the command and username and send it
            SerializeAndSendBuffer("/GetUserImagesId", user);

            // Receive the buffer by the server (but we have to deserialize it)
            byte[] buffer = new byte[1024];
            Client.Receive(buffer);

            // We deserialize the buffer into a jagged byte array
            MemoryStream ms = new MemoryStream(buffer);
            BinaryFormatter bf = new BinaryFormatter();
            byte[][] result = (byte[][])bf.Deserialize(ms);

            // We recup the id stored in the jagged array and we return it
            string[] arrayImg = { Encoding.UTF8.GetString(result[0]), Encoding.UTF8.GetString(result[1]) };
            return arrayImg;

        }

        /// <summary>
        /// Send the id of an image and recovers it by the server
        /// </summary>
        /// <param name="idImage">The image id</param>
        /// <returns>Return the image that corresponds to the id</returns>
        public Image SendImageId(string idImage)
        {
            // Create a jagged byte array who contain the id of the image (we have to create a jagged even if there is only one element)
            byte[][] id = { Encoding.UTF8.GetBytes(idImage) };

            // Serialize the command and id and send it
            SerializeAndSendBuffer("/GetImage", id);

            // Receive the buffer by the server (but we have to deserialize it)
            byte[] buffer = new byte[1000000];
            Client.Receive(buffer);

            // Deserialize the buffer into an image and return it
            MemoryStream ms = new MemoryStream(buffer);
            BinaryFormatter bf = new BinaryFormatter();
            Image img = (Image)bf.Deserialize(ms);
            return img;
        }

        /// <summary>
        /// Send the username to the server to recovers the profil of the user
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return a dictionary who contains the profil of the user</returns>
        public Dictionary<string, string> SendUsernameToGetProfil(string username)
        {
            // Create a jagged byte array who contain the username (we have to create a jaggend even if there is only one element)
            byte[][] user = { Encoding.UTF8.GetBytes(username) };

            // Serialize the command and username and send it
            SerializeAndSendBuffer("/GetProfil", user);

            // Receive the buffer by the server (but we have to deserialize it)
            byte[] buffer = new byte[8192];
            Client.Receive(buffer);

            // Deserialize the buffer into a Dictionnary and return it
            MemoryStream ms = new MemoryStream(buffer);
            BinaryFormatter bf = new BinaryFormatter();
            Dictionary<string, string> profil = (Dictionary<string, string>)bf.Deserialize(ms);
            return profil;
        }

        /// <summary>
        /// Send the id of an user to the server to recovers the friend list of the user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>Return a list who contains the friends list of the user</returns>
        public List<string[]> SendUserIdToGetFriendsList(string userId)
        {
            // Create a jagged byte array who contain the username (we have to create a jaggend even if there is only one element)
            byte[][] id = { Encoding.UTF8.GetBytes(userId) };

            // Serialize the command and id and send it
            SerializeAndSendBuffer("/GetFriendsList", id);

            // Receive the buffer by the server (but we have to deserialize it)
            byte[] buffer = new byte[1024];
            Client.Receive(buffer);

            // Deserialize the buffer into a List and return it
            MemoryStream ms = new MemoryStream(buffer);
            BinaryFormatter bf = new BinaryFormatter();
            List<string[]> profil = (List<string[]>)bf.Deserialize(ms);
            return profil;
        }

        /// <summary>
        /// Send the id of a friend and recovers his avatar
        /// </summary>
        /// <param name="idFriend">The friend id</param>
        /// <returns>Return the image that corresponds to the id</returns>
        public Image SendFriendIdToGetFriendAvatar(string idFriend)
        {
            // Create a jagged byte array who contain the id of the friend (we have to create a jagged even if there is only one element)
            byte[][] id = { Encoding.UTF8.GetBytes(idFriend) };

            // Serialize the command and id and send it
            SerializeAndSendBuffer("/GetFriendAvatar", id);

            // Receive the buffer by the server (but we have to deserialize it)
            byte[] buffer = new byte[1000000];
            Client.Receive(buffer);

            // Deserialize the buffer into an image and return it
            MemoryStream ms = new MemoryStream(buffer);
            BinaryFormatter bf = new BinaryFormatter();
            Image img = (Image)bf.Deserialize(ms);
            return img;
        }

        /// <summary>
        /// Send the new statut of a user, and his username, to update it
        /// </summary>
        /// <param name="statut">The new statut of the user</param>
        /// <param name="username">The name of the user</param>
        /// <returns>Return true if the statut has been correctly changed, else return false</returns>
        public bool SendNewStatutForUser(string statut, string username)
        {
            // Create a jagged byte array who contain the new statut and the username
            byte[][] jaggedData = {
                Encoding.UTF8.GetBytes(statut),
                Encoding.UTF8.GetBytes(username),
            };

            // Serialize the command and data and send it
            SerializeAndSendBuffer("/UpdateStatut", jaggedData);

            // Recup the response of the server and return it
            bool result = GetBooleanResult();
            return result;
        }
        #endregion DataMethods
    }
}
