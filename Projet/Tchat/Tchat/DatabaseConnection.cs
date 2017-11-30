/* Project name : Tchat
 * Class : DatabaseConnection - Contains all method which manage the connection to the database
 * Author : GENGA Dario
 * Last update : 2017.09.28
 * Version : 0.1
 */

using System;
using MySql.Data.MySqlClient;
using System.Data;

namespace Tchat
{
    class DatabaseConnection
    {
        private string _server;
        private string _database;
        private string _user;
        private string _pwd;
        private MySqlConnection _connection;

        /// <summary>
        /// Définit les paramètres de connexion du serveur MySQL
        /// </summary>
        /// <param name="server">Le serveur MySQL</param>
        /// <param name="database">La base de données MySQL</param>
        /// <param name="user">L'utilisateur de la base</param>
        /// <param name="pwd">Le mot de passe de l'utilisateur</param>
        public DatabaseConnection(string server, string database, string user, string pwd)
        {
            Server = server;
            Database = database;
            User = user;
            Pwd = pwd;

            TryConnexion();
        }

        public string Server
        {
            get
            {
                return this._server;
            }
            set
            {
                if (value != "")
                {
                    this._server = value;
                }
                else
                {
                    Console.WriteLine("Server can't be null !");
                }
            }
        }
        public string Database
        {
            get
            {
                return this._database;
            }
            set
            {
                if (value != "")
                {
                    this._database = value;
                }
                else
                {
                    Console.WriteLine("Database's name can't be null !");
                }
            }
        }
        public string User
        {
            get
            {
                return this._user;
            }
            set
            {
                if (value != "")
                {
                    this._user = value;
                }
                else
                {
                    Console.WriteLine("Username can't be null !");
                }
            }
        }
        public string Pwd
        {
            get
            {
                return this._pwd;
            }
            set
            {
                this._pwd = value;
            }
        }

        public MySqlConnection Connection
        {
            get
            {
                return this._connection;
            }
            set
            {
                this._connection = value;
            }
        }

        /// <summary>
        /// Tente une connexion à la base de données
        /// Si la connexion a réussi on la laisse ouverte
        /// </summary>
        /// <returns>retourne true si la connexion à réussi, sinon retourne false</returns>
        public bool TryConnexion()
        {
            string ConnectionString = "SERVER=" + Server + ";DATABASE=" + Database + ";UID=" + User + ";PASSWORD=" + Pwd + ";";
            Connection = new MySqlConnection(ConnectionString);

            try
            {
                return OpenConnexion();
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
        public bool OpenConnexion()
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
