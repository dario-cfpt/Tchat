/* Project name : ServerTchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : DatabaseConnection - Connection to the MySQL database
 * Author : GENGA Dario
 * Last update : 2017.12.16 (yyyy-MM-dd)
 */

using System;
using MySql.Data.MySqlClient;
using System.Data;

namespace ServerTchat
{
    /// <summary>
    /// Connection to the MySQL database
    /// </summary>
    class DatabaseConnection
    {
        private MySqlConnection _connection;
        private string _server;
        private string _port;
        private string _database;
        private string _user;
        private string _pwd;

        private const string DEFAULT_PORT = "3306";

        /// <summary>
        /// Create the connection with the MySQL database (default 3306 port used)
        /// </summary>
        /// <param name="server">The hostname or ip adress of the server with the MySQL database</param>
        /// <param name="database">The name of the MySQL database</param>
        /// <param name="user">The name of the user for the database</param>
        /// <param name="pwd">The password of the user for the database</param>
        public DatabaseConnection(string server, string database, string user, string pwd) : this(server, DEFAULT_PORT, database, user, pwd)
        {
            // Do nothing...
        }

        /// <summary>
        /// Create the connection with the MySQL database
        /// </summary>
        /// <param name="server">The hostname or ip adress of the server with the MySQL database</param>
        /// <param name="port">The port of the MySQL server</param>
        /// <param name="database">The name of the MySQL database</param>
        /// <param name="user">The name of the user for the database</param>
        /// <param name="pwd">The password of the user for the database</param>
        public DatabaseConnection(string server, string port, string database, string user, string pwd)
        {
            Server = server;
            Port = port;
            Database = database;
            User = user;
            Pwd = pwd;

            TryConnection(Server, Port, Database, User, Pwd);
        }

        /// <summary>
        /// The hostname or ip adress of the server with the MySQL database
        /// </summary>
        private string Server
        {
            get => _server;
            set
            {
                if (value != "")
                {
                    _server = value;
                }
                else
                {
                    Console.WriteLine("Server can't be null !");
                }
            }
        }

        /// <summary>
        /// The hostname or ip adress of the server with the MySQL database
        /// </summary>
        private string Port
        {
            get => _port;
            set
            {
                if (value != "")
                {
                    _port = value;
                }
                else
                {
                    Console.WriteLine("port can't be null !");
                }
            }
        }

        /// <summary>
        /// The name of the MySQL database
        /// </summary>
        private string Database
        {
            get => _database;
            set
            {
                if (value != "")
                {
                    _database = value;
                }
                else
                {
                    Console.WriteLine("Database's name can't be null !");
                }
            }
        }

        /// <summary>
        /// The name of the user for the database
        /// </summary>
        private string User
        {
            get => _user;
            set
            {
                if (value != "")
                {
                    _user = value;
                }
                else
                {
                    Console.WriteLine("Username can't be null !");
                }
            }
        }
        /// <summary>
        /// The password of the user for the database
        /// </summary>
        private string Pwd { get => _pwd; set => _pwd = value; }

        /// <summary>
        /// The connection to the MySQL database
        /// </summary>
        public MySqlConnection Connection { get => _connection; set => _connection = value; }

        /// <summary>
        /// Tente une connexion à la base de données
        /// Si la connexion a réussi on la laisse ouverte
        /// </summary>
        /// <returns>retourne true si la connexion à réussi, sinon retourne false</returns>
        public bool TryConnection(string server, string port, string database, string user, string password)
        {
            string connectionString = string.Format("SERVER={0};PORT={1};DATABASE={2};UID={3};PASSWORD={4}", server, port, database, user, password);

            try
            {
                Connection = new MySqlConnection(connectionString);
                return OpenConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connecting to MySQL has failed : " + ex);
                return false;
            }
        }

        /// <summary>
        /// Ouvre la connexion à la base de données
        /// </summary>
        /// <returns>Retourne true si la connexion à réussi, sinon retourne false</returns>
        public bool OpenConnection()
        {
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Console.WriteLine("Connecting to MySQL...");
                    Connection.Open();
                    Console.WriteLine("Connection is now open !");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection errror : " + ex);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Connection already open !");
                return true;
            }
        }
    }
}
