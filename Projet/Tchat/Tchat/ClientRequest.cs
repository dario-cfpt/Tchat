/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : ClientRequest - Manage the requests between the client and the server
 * Author : GENGA Dario
 * Last update : 2017.12.17 (yyyy-MM-dd)
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
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
        /// Encrypts the received text into a SHA256 string 
        /// </summary>
        /// <param name="text">The text to encrypt</param>
        /// <returns>The encrypted text in SHA256</returns>
        private string GetHashSha256(string text)
        {
            string hashString = "";

            // Encode the text in UTF8 and get the bytes of each character in a table
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            
            // Hash our array of byte and stock it in a new array
            SHA256Managed sha256Managed = new SHA256Managed();
            byte[] hash = sha256Managed.ComputeHash(bytes);
            
            // Add ech byte of our array into our string
            foreach (byte b in hash)
            {
                hashString += String.Format("{0:x2}", b); // b is formatted in hexadecimal
            }

            return hashString;
        }

        /// <summary>
        /// Serialize a jagged byte array (byte[][]) into a byte array (byte[]) and return it
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
        /// Serialize any object into a byte array (byte[]) and return it
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Return the serialized jagged byte array</returns>
        private byte[] ConvertObjectToByteArray(object obj)
        {
            // Serialize the object to get a byte array
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            byte[] byteArray = ms.ToArray();

            return byteArray;
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
            password = GetHashSha256(password); // Encrypt the password

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
            password = GetHashSha256(password); // Encrypt the password

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

            if (buffer.All(singleByte => singleByte == 0))
            {
                return new string[0]; // Empty buffer received
            }
            else
            {            
                // We deserialize the buffer into a jagged byte array
                MemoryStream ms = new MemoryStream(buffer);
                BinaryFormatter bf = new BinaryFormatter();
                byte[][] result = (byte[][])bf.Deserialize(ms);

                // We recup the id stored in the jagged array and we return it
                string[] arrayImg = { Encoding.UTF8.GetString(result[0]), Encoding.UTF8.GetString(result[1]) };
                return arrayImg;

            }
        }

        /// <summary>
        /// Send the id of an image and recovers it by the server
        /// </summary>
        /// <param name="idImage">The image id</param>
        /// <returns>Return the image that corresponds to the id</returns>
        public Image SendImageIdToGetImage(string idImage)
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
            byte[][] buffer = {
                Encoding.UTF8.GetBytes(statut),
                Encoding.UTF8.GetBytes(username),
            };

            // Serialize the command and data and send it
            SerializeAndSendBuffer("/UpdateStatut", buffer);

            // Recup the response of the server and return it
            bool result = GetBooleanResult();
            return result;
        }

        /// <summary>
        /// Send the ID of the user sender and the user receiver to check if they don't already have a friend request
        /// </summary>
        /// <param name="idUserSender">The id of the user who send the request</param>
        /// <param name="idUserReceiver">The id of the user who recovers the request</param>
        /// <returns>Return true if they are already a friend request, else return false</returns>
        public bool SendIdUsersToCheckFriendRequest(string idUserSender, string idUserReceiver)
        {
            // Create a jagged byte array who contain the id of the users
            byte[][] buffer = {
                Encoding.UTF8.GetBytes(idUserSender),
                Encoding.UTF8.GetBytes(idUserReceiver)
            };

            // Serialize the command and data and send it
            SerializeAndSendBuffer("/CheckFriendRequest", buffer);

            // Recup the response of the server and return it
            bool result = GetBooleanResult();
            return result;
        }

        /// <summary>
        /// Send a name to check if an user exist with this name
        /// </summary>
        /// <param name="username">The name of the user to check</param>
        /// <returns>Return true if the user exist, else return false</returns>
        public bool SendUsernameToCheckIfUserExist(string username)
        {//TODO : test this method
            // Create a jagged byte array who contain the name of the user
            byte[][] buffer = { Encoding.UTF8.GetBytes(username) };

            // Serialize the command and data and send it
            SerializeAndSendBuffer("/CheckUser", buffer);

            // Recup the response of the server and return it
            bool result = GetBooleanResult();
            return result;
        }

        /// <summary>
        /// Send the name of the user sender and the user receiver to check if they aren't a duplicate friend request
        /// </summary>
        /// <param name="userSender">The name of the user who send the request</param>
        /// <param name="userReceiver">The name of the user who recovers the request</param>
        /// <returns>Return true if they are a duplicte friend request, else return false</returns>
        public bool SendUsernamesToCheckDuplicateFriendRequest(string userSender, string userReceiver)
        {
            // Create a jagged byte array who contain the name of the users
            byte[][] buffer = {
                Encoding.UTF8.GetBytes(userSender),
                Encoding.UTF8.GetBytes(userReceiver)
            };

            // Serialize the command and data and send it
            SerializeAndSendBuffer("/CheckDuplicateFriendRequest", buffer);

            // Recup the response of the server and return it
            bool result = GetBooleanResult();
            return result;
        }

        /// <summary>
        /// Send the ID of the user sender and the user receiver to create a friend request
        /// </summary>
        /// <param name="idUserSender">The id of the user who send the request</param>
        /// <param name="idUserReceiver">The id of the user who recovers the request</param>
        /// <param name="messageRequest">The message who accompanies the request</param>
        /// <returns>Return true if the resquest has been correctly sended, else return false</returns>
        public bool SendIdUsersToCreateFriendRequest(string idUserSender, string idUserReceiver, string messageRequest)
        {
            // Create a jagged byte array who contain the data
            byte[][] buffer = {
                Encoding.UTF8.GetBytes(idUserSender),
                Encoding.UTF8.GetBytes(idUserReceiver),
                Encoding.UTF8.GetBytes(messageRequest)
            };

            // Serialize the command and data and send it
            SerializeAndSendBuffer("/CreateFriendRequest", buffer);

            // Recup the response of the server and return it
            bool result = GetBooleanResult();
            return result;
        }


        /// <summary>
        /// Send the name of an user to get is password
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return the password of the user</returns>
        public string SendUsernameToGetPassword(string username)
        {
            // Create a jagged byte array who contain the username (we have to create a jaggend even if there is only one element)
            byte[][] user = { Encoding.UTF8.GetBytes(username) };

            SerializeAndSendBuffer("/GetPassword", user);

            // Receive the buffer by the server (but we have to deserialize it)
            byte[] buffer = new byte[128];
            Client.Receive(buffer);

            // Get the username of the user and return it
            string password = Encoding.UTF8.GetString(buffer);
            password = password.Replace("\0", ""); // this avoid to have \0 in the end of the string if the name lenght is inferior less than the buffer size 
            return password;
        }


        /// <summary>
        /// Send the name of an user to get is password
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>Return the centers of interests of the user</returns>
        public string SendUsernameToGetHobbies(string username)
        {
            // Create a jagged byte array who contain the username (we have to create a jaggend even if there is only one element)
            byte[][] user = { Encoding.UTF8.GetBytes(username) };

            SerializeAndSendBuffer("/GetHobbies", user);

            // Receive the buffer by the server
            byte[] buffer = new byte[128];
            Client.Receive(buffer);

            // Get the hobbies of the user and return it
            string hobbies = Encoding.UTF8.GetString(buffer);
            hobbies = hobbies.Replace("\0", ""); // this avoid to have \0 in the end of the string if the name lenght is inferior less than the buffer size 
            return hobbies;
        }

        /// <summary>
        /// Send the updated profil of an user
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <param name="email">The email of the user</param>
        /// <param name="phone">The phone of the user</param>
        /// <param name="description">The description of the profil of the user</param>
        /// <param name="hobbies">The centers of interests of the user</param>
        public void SendUpdatedProfilOfUser(string username, string email, string phone, string description, string hobbies)
        {
            // Create a jagged byte array who contain the profil
            byte[][] profil = {
                Encoding.UTF8.GetBytes(username),
                Encoding.UTF8.GetBytes(email),
                Encoding.UTF8.GetBytes(phone),
                Encoding.UTF8.GetBytes(description),
                Encoding.UTF8.GetBytes(hobbies)
            };

            SerializeAndSendBuffer("/UpdateProfil", profil);

            // We must receive the result before continuing (otherwise we will have interface problems with the profil)
            byte[] result = new byte[1];
            Client.Receive(result);
        }

        /// <summary>
        /// Send the updated image id of an user
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <param name="columnImg">The column of the image</param>
        /// <param name="idImg">The id of the image</param>
        public void SendUpdatedImageIdOfUser(string username, string columnImg, string idImg)
        {// TODO : test this method
            // Create a jagged byte array who contain the data
            byte[][] buffer = {
                Encoding.UTF8.GetBytes(username),
                Encoding.UTF8.GetBytes(columnImg),
                Encoding.UTF8.GetBytes(idImg)
            };

            SerializeAndSendBuffer("/UpdateIdImage", buffer);

            // We must receive the result before continuing (otherwise we will have interface problems with the profil)
            byte[] result = new byte[1];
            Client.Receive(result);

        }

        /// <summary>
        /// Send the updated profil of an user
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <param name="password">The password of the user</param>
        public void SendUpdatedPassword(string username, string password)
        {
            password = GetHashSha256(password); // Encrypt the password

            // Create a jagged byte array who contain the username and password
            byte[][] buffer = {
                Encoding.UTF8.GetBytes(username),
                Encoding.UTF8.GetBytes(password)
            };

            SerializeAndSendBuffer("/UpdatePassword", buffer);

            // We must receive the result before continuing (otherwise we will have interface problems with the profil)
            byte[] result = new byte[1];
            Client.Receive(result);
        }

        /// <summary>
        /// Send an image with is format and received is id
        /// </summary>
        /// <param name="img">The image to send</param>
        /// <returns>Return the id of the image after it has been added to the database</returns>
        public string SendImageAndGetId(Image img)
        {//TODO : define a maximum image size (it's actually 1048576 byte)
            // Create a jagged byte array who contain the image and the image format
            byte[][] byteImage = { ConvertObjectToByteArray(img) };

            // Send a jagged byte array who contains the data
            SerializeAndSendBuffer("/InsertImage", byteImage);

            // Receive the buffer by the server
            byte[] buffer = new byte[4];
            Client.Receive(buffer);

            // Get the image id and return it
            string imageId = Encoding.UTF8.GetString(buffer);
            imageId = imageId.Replace("\0", ""); // this avoid to have \0 in the end of the string if the name lenght is inferior less than the buffer size 
            return imageId;
        }

        #endregion DataMethods
    }
}
