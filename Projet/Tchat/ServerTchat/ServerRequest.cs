/* Project name : ServerTchat
 * Description : Chat server application console for the project "Tchat"
 * Class : ServerRequest - Manage the requests of the chat server with the database
 * Author : GENGA Dario
 * Last update : 2017.12.14 (yyyy-MM-dd)
 */
using System;
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
            byte[] result = BitConverter.GetBytes(accepted);
            handler.Send(result);
            
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
            byte[] result = BitConverter.GetBytes(created);
            handler.Send(result);

        }

        /// <summary>
        /// Recovers the images id of the avatar and backgroud images by an username
        /// </summary>
        /// <param name="handler">The socket of the user</param>
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
            byte[] result = ConvertJaggedByteToByteArray(jaggedResult);
            handler.Send(result);
        }
    }
}
