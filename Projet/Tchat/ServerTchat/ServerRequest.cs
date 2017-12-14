/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Class : ServerRequest - Manage the requests of the chat server with the database
 * Author : GENGA Dario
 * Last update : 2017.12.14 (yyyy-MM-dd)
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ServerTchat
{
    /// <summary>
    /// Manage the requests of the chat server with the database
    /// </summary>
    class ServerRequest
    {
        private RequestsSQL _requestsSQL;
        
        /// <summary>
        /// Initialize the requests SQL for the database
        /// </summary>
        /// <param name="requestsSQL"></param>
        public ServerRequest(RequestsSQL requestsSQL)
        {
            RequestsSQL = requestsSQL;
        }

        private RequestsSQL RequestsSQL { get => _requestsSQL; set => _requestsSQL = value; }

        /// <summary>
        /// Serialize an object into a byte array, then send it to the client
        /// </summary>
        /// <param name="handler">The socket of the client</param>
        /// <param name="obj">The object that we want to serialize into a byte array</param>
        private void SerializeObjectIntoByteArrayAndSendIt(Socket handler, object obj)
        {
            // Serialize the object
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            
            // Convert it to a byte array and send it
            byte[] buffer = ms.ToArray();
            handler.Send(buffer);
        }

        /// <summary>
        /// Try to connect the user to the database
        /// <para>The result is sended to the client</para>
        /// </summary>
        /// <param name="handler">The socket of the client who is trying to connect the database</param>
        /// <param name="data">Jagged byte array of the data. The first element contain the username and the second the password</param>
        public void CommandLogin(Socket handler, byte[][] data)
        {
            // Recovers username and password
            string username = Encoding.UTF8.GetString(data[0]);
            string password = Encoding.UTF8.GetString(data[1]);

            // Try to connect the user to the database
            bool accepted = RequestsSQL.Login(username, password);

            // Convert the result of the connection into a byte array and send it to the client
            byte[] buffer = BitConverter.GetBytes(accepted);
            handler.Send(buffer);
            
        }

        /// <summary>
        /// Try to create a new account for the database
        /// <para>The result is sended to the client</para>
        /// </summary>
        /// <param name="handler">The socket of the client who is trying to connect the database</param>
        /// <param name="data">Jagged byte array of the data for the new account</param>
        public void CommandCreateAccount(Socket handler, byte[][] data)
        {
            // Recovers data of the new account
            string username = Encoding.UTF8.GetString(data[0]);
            string password = Encoding.UTF8.GetString(data[1]);
            string email = Encoding.UTF8.GetString(data[2]);
            string phone = Encoding.UTF8.GetString(data[3]);

            // Try to create the new account
            bool created = RequestsSQL.CreateNewUser(username, password, email, phone);

            // Convert the result of the creation into a byte array and send it to the client
            byte[] buffer = BitConverter.GetBytes(created);
            handler.Send(buffer);

        }

        /// <summary>
        /// Update the statut of an user
        /// <para>The result is sended to the client</para>
        /// </summary>
        /// <param name="handler">The socket of the cliente</param>
        /// <param name="data">Jagged byte array who contains the statut and username</param>
        public void CommandUpdateStatut(Socket handler, byte[][] data)
        {
            // Recovers data of the new 
            string statut = Encoding.UTF8.GetString(data[0]);
            string username = Encoding.UTF8.GetString(data[1]);

            // Try to create the new account
            bool created = RequestsSQL.UpdateStatutUser(statut, username);

            // Convert the result of the creation into a byte array and send it to the client
            byte[] buffer = BitConverter.GetBytes(created);
            handler.Send(buffer);
        }

        /// <summary>
        /// Recovers the username by his id
        /// </summary>
        /// <param name="handler">The socket of the client</param>
        /// <param name="data">Jagged byte array who contain the id</param>
        public void CommandGetUsernameByUserId(Socket handler, byte[][] data)
        {
            // Recovers the id
            string id = Encoding.UTF8.GetString(data[0]);

            // Recovers the name of the user
            string username = RequestsSQL.GetUsernameById(id);

            // Convert the username into a byte array and send it to the client
            byte[] buffer = Encoding.UTF8.GetBytes(username);
            handler.Send(buffer);
        }

        /// <summary>
        /// Recovers the user id by his username
        /// </summary>
        /// <param name="handler">The socket of the client</param>
        /// <param name="data">Jagged byte array who contain the username</param>
        public void CommandGetUserIdByUsername(Socket handler, byte[][] data)
        {
            // Recovers the username
            string username = Encoding.UTF8.GetString(data[0]);

            // Recovers the id of the user
            string id = RequestsSQL.GetUserIdByUsername(username);

            // Convert the id into a byte array and send it to the client
            byte[] buffer = Encoding.UTF8.GetBytes(id);
            handler.Send(buffer);
        }
        
        /// <summary>
        /// Recovers the images id of the avatar and backgroud images by an username
        /// </summary>
        /// <param name="handler">The socket of the client</param>
        /// <param name="data">Jagged byte array who contain the username</param>
        public void CommandGetUserImagesIdByUsername(Socket handler, byte[][] data)
        {
            // Recovers the username
            string username = Encoding.UTF8.GetString(data[0]);

            // Recovers the result of the request into a string array
            string[] arrayImg = RequestsSQL.GetUserImagesIdByUsername(username);

            // Create a jagged byte array of the result
            byte[][] jaggedResult = { Encoding.UTF8.GetBytes(arrayImg[0]), Encoding.UTF8.GetBytes(arrayImg[1]) };

            // Convert the result into a byte array and send it to the client (who will have to deserialize it)
            SerializeObjectIntoByteArrayAndSendIt(handler, jaggedResult);

        }

        /// <summary>
        /// Recovers an image by his id
        /// </summary>
        /// <param name="handler">The socket of the client</param>
        /// <param name="data">Jagged byte array who contain the id of the image</param>
        public void CommandGetImageById(Socket handler, byte[][] data)
        {
            // Recovers the id
            string id = Encoding.UTF8.GetString(data[0]);

            // Create an image by his id
            Image img = RequestsSQL.CreateImageById(id);

            // Save the image into a byte array and send it to the client
            SerializeObjectIntoByteArrayAndSendIt(handler, img);
        }

        /// <summary>
        /// Recovers the profil of an user by his name
        /// </summary>
        /// <param name="handler">The socket of the client</param>
        /// <param name="data">Jagged byte array who contain the username </param>
        public void CommandGetProfilByUsername(Socket handler, byte[][] data)
        {
            // Recovers the username
            string username = Encoding.UTF8.GetString(data[0]);

            // Create a Dictionary for the profil by the username
            Dictionary<string, string> profil = RequestsSQL.GetProfilByUsername(username);

            // Serialize the profil into a byte array and send it to the client
            SerializeObjectIntoByteArrayAndSendIt(handler, profil);

        }

        /// <summary>
        /// Recovers the friends list of an user by his id
        /// </summary>
        /// <param name="handler">The socket of the client</param>
        /// <param name="data">Jagged byte array who contain the id</param>
        public void CommandGetFriendsListByUserId(Socket handler, byte[][] data)
        {
            // Recovers the username
            string id = Encoding.UTF8.GetString(data[0]);

            // Create a list for the friends list by the username
            List<string[]> friendsList = RequestsSQL.GetFriendsList(id);

            // Serialize the profil into a byte array and send it to the client
            SerializeObjectIntoByteArrayAndSendIt(handler, friendsList);

        }

        /// <summary>
        /// Recovers friend avatar by his friend id
        /// </summary>
        /// <param name="handler">The socket of the client</param>
        /// <param name="data">Jagged byte array who contain the id of the friend</param>
        public void CommandGetFriendAvatarByFriendId(Socket handler, byte[][] data)
        {
            // Recovers the id
            string id = Encoding.UTF8.GetString(data[0]);

            // Recovers the avatar of a friend by his id
            Image img = RequestsSQL.GetFriendAvatarByFriendId(id);

            // Save the image into a byte array and send it to the client
            SerializeObjectIntoByteArrayAndSendIt(handler, img);
        }
    }
}
