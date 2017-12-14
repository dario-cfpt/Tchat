/* Project name : ServerTchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : RequestsSql - Contains all queries SQL of the app
 * Author : GENGA Dario
 * Last update : 2017.12.14 (yyyy-MM-dd)
 */

// TODO : faire des commentaires en anglais dans ce fichier
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ServerTchat
{
    /// <summary>
    /// Contains all queries SQL of the app
    /// </summary>
    public class RequestsSQL
    {
        private MySqlConnection _connection;
        // TODO : faire des enum ici
        // Les différents statuts de connection accepté dans la base :
        public const string ONLINE = "En ligne";
        public const string ABSENT = "Absent";
        public const string DO_NOT_DISTURB = "Ne pas déranger";
        public const string INVISIBLE = "Invisible";
        public const string OFFLINE = "Hors-ligne";
        // Les différents status entre deux utilisateurs accepté dans la base :
        public const string FRIEND = "Ami";
        public const string FRIEND_REQUEST = "En attente";
        public const string MUTE = "Muet";
        public const string BLOCKED = "Bloqué";
        
        public RequestsSQL(MySqlConnection connection)
        {
            Connection = connection;
        }
        public MySqlConnection Connection { get => _connection; set => _connection = value; }

        /// <summary>
        /// Encrypte le texte reçu en une chaîne SHA256
        /// </summary>
        /// <param name="text">Le texte à encrypté</param>
        /// <returns>Le texte encrypté en SHA256</returns>
        private string GetHashSha256(string text)
        {
            string hashString = ""; // chaîne qui stockera le text encrypter en SHA256

            // Encode le texte en UTF8 et récupères les bytes de chaques caractères dans un tableau
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            // Hash notre tableau de byte et stocke le tout dans un nouveau tableau
            SHA256Managed sha256Managed = new SHA256Managed();
            byte[] hash = sha256Managed.ComputeHash(bytes);


            // Parcour notre tableau hasher et ajoute chaque byte dans notre chaîne
            foreach (byte b in hash)
            {
                hashString += String.Format("{0:x2}", b); // b est formater en hexadécimal
            }

            return hashString;
        }

        /// <summary>
        /// Convertie une image en tableau de byte
        /// </summary>
        /// <param name="img">L'image à convertir</param>
        /// <param name="imgFormat">Le format de l'image dans lequelle on convertie l'image</param>
        /// <returns>Retourne un tableau de byte quî représente l'image</returns>
        private byte[] ConvertImageToByteArray(Image img, ImageFormat imgFormat)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, imgFormat); // Enregistre l'image dans le flux au format souhaité
            byte[] byteArray = ms.ToArray(); // Transforme l'image en tableau de byte
            return byteArray;
        }

        /// <summary>
        /// Récupère le dernier ID ajouté dans la base
        /// </summary>
        /// <returns>Retourne le dernier ID ajouté dans la base</returns>
        private string GetLastInsertId()
        {
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT LAST_INSERT_ID()";

            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            string lastInsertId = reader[0].ToString();
            reader.Close();

            return lastInsertId;
        }

        #region UsersRequests
        #region SelectRequests
        #region Checks
        /// <summary>
        /// Vérifie si un utilisateur existe
        /// </summary>
        /// <param name="username">Le pseudo à vérifier</param>
        /// <param name="email">L'email à vérifier</param>
        /// <returns>Retourne false si le pseudo ou l'email n'existent pas. Si ils existent (ou si une erreur est survenue) on retourne false </returns>
        public bool CheckIfExist(string username, string email)
        {
            // Vérifie que les champs reçus ne sont pas vide
            if (username == "" || email == "")
            {
                Console.WriteLine("Le nom d'utilisateur ou l'email ne peut être null !");
                return true;
            }

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT USERNAME, EMAIL FROM mytchatroomdb.users WHERE USERNAME = @username AND EMAIL = @email;";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@email", email);

            try
            {
                Console.WriteLine(command.CommandText);

                MySqlDataReader reader = command.ExecuteReader(); // Récupère le résultat de la requete
                reader.Read();

                if (reader.HasRows)
                {
                    string user = reader["USERNAME"].ToString();

                    if (user == username)
                    {
                        Console.WriteLine("Le pseudo est déjà utilisé !");
                    }
                    else
                    {
                        Console.WriteLine("L'email est déjà utilisé !");
                    }
                    reader.Close(); // ne pas oublier de fermer la connection
                    return true; // L'utilisateur existe déjà
                }
                else
                {
                    Console.WriteLine("L'utilisateur n'existe pas !");
                    reader.Close(); // ne pas oublier de fermer la connection
                    return false; // L'utilisateur n'existe pas
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return false;
            }
        }

        /// <summary>
        /// Vérifie si le pseudo existe
        /// </summary>
        /// <param name="username">Le pseudo de l'utilisateur que l'on recherche</param>
        /// <returns>Retourne true si l'utilisateur existe. Si il n'existe pas (ou si une erreur est survenue) alors on retourne false</returns>
        public bool CheckIfUserExist(string username)
        {
            // Vérifie que les champs reçus ne sont pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return false;
            }

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT USERNAME FROM mytchatroomdb.users WHERE USERNAME = @username";
            command.Parameters.AddWithValue("@username", username);

            try
            {
                Console.WriteLine(command.CommandText);

                MySqlDataReader reader = command.ExecuteReader(); // Récupère le résultat de la requete
                reader.Read();

                if (reader.HasRows)
                {
                    Console.WriteLine("L'utilisateur existe !");
                    reader.Close();
                    return true;
                }
                else
                {
                    Console.WriteLine("L'utilisateur n'existe pas !");
                    reader.Close(); // ne pas oublier de fermer la connection
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return false;
            }
        }

        /// <summary>
        /// Vérifie si une invitation d'amitié existe déjà entre 2 utilisateurs
        /// </summary>
        /// <param name="userSender">Le pseudo de l'utilisateur qui envoie la demande d'amitié</param>
        /// <param name="userReceiver">Le pseudo de l'utilisateur qui reçoit la demande d'amitié</param>
        /// <returns>Retourne false si l'invitation n'existe pas pour le moment, sinon retourne true si elle existe déjà (ou si une erreur est survenue)</returns>
        public bool CheckDuplicateFriendRequest(string userSender, string userReceiver)
        {
            // Vérifie que les champs reçus ne sont pas vide
            if (String.IsNullOrEmpty(userSender) || String.IsNullOrEmpty(userReceiver))
            {
                Console.WriteLine("Les pseudos des utilisateurs ne peuvent être null !");
                return true;
            }
            else
            {
                // Transforme les pseudos en ID
                userSender = GetUserIdByUsername(userSender);
                userReceiver = GetUserIdByUsername(userReceiver);
            }

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM mytchatroomdb.users_has_friends WHERE USERS_ID = @userSender AND USERS_FRIEND_ID = @userReceiver;";
            command.Parameters.AddWithValue("@userSender", userSender);
            command.Parameters.AddWithValue("@userReceiver", userReceiver);

            try
            {
                Console.WriteLine(command.CommandText);

                MySqlDataReader reader = command.ExecuteReader(); // Récupère le résultat de la requete
                reader.Read();

                if (reader.HasRows)
                {
                    Console.WriteLine("Attention, l'invitation existe déjà !");
                    reader.Close();
                    return true;
                }
                else
                {
                    Console.WriteLine("C'est bon, l'invitation n'existe pas encore.");
                    reader.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return true;
            }
        }

        /// <summary>
        /// Vérifie si l'invitation d'amitié existe déjà en regardant si l'utilisateur qui envoie l'invitation d'amitié à un autre utilisateur ne l'aurait pas déjà reçu entre temps
        /// </summary>
        /// <param name="idUserSender">L'utilisateur qui envoie l'invitation (celui qui en a peut-être déjà reçu une par l'autre utilisateur)</param>
        /// <param name="idUserReceiver">L'utilisateur qui reçoit l'invitation (celui qui en a peut-être déjà envoyé une à l'autre utilisateur)</param>
        /// <returns>Retourne false si l'invitation n'existe pas pour le moment, sinon retourne true si elle existe déjà (ou si une erreur est survenue)</returns>
        public bool CheckIfFriendRequestAlreadyExist(string idUserSender, string idUserReceiver)
        {
            // Vérifie que les champs reçus ne sont pas vide
            if (String.IsNullOrEmpty(idUserSender) || String.IsNullOrEmpty(idUserReceiver))
            {
                Console.WriteLine("Les ID des utilisateurs ne peuvent être null !");
                return true;
            }

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM mytchatroomdb.users_has_friends WHERE USERS_ID = @idUserReceiver AND USERS_FRIEND_ID = @idUserSender;";
            command.Parameters.AddWithValue("@idUserSender", idUserSender);
            command.Parameters.AddWithValue("@idUserReceiver", idUserReceiver);

            try
            {
                Console.WriteLine(command.CommandText);

                MySqlDataReader reader = command.ExecuteReader(); // Récupère le résultat de la requete
                reader.Read();

                if (reader.HasRows)
                {
                    Console.WriteLine("Attention, l'invitation existe déjà !");
                    reader.Close();
                    return true;
                }
                else
                {
                    Console.WriteLine("C'est bon, l'invitation n'existe pas encore.");
                    reader.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return true;
            }
        }
        #endregion Checks

        /// <summary>
        /// L'utilisateur tente de se connecté à la base via son pseudo et son mot de passe
        /// </summary>
        /// <param name="username">Pseudo de l'utilisateur</param>
        /// <param name="password">Mot de passe de l'utilisateur</param>
        /// <returns>Retourne true si le mot de passe correspond à un utilisateur de la base, sinon retourne false</returns>
        public bool Login(string username, string password)
        {
            // Vérifie que les champs reçus ne sont pas vide
            if (username == "" || password == "")
            {
                Console.WriteLine("Le nom d'utilisateur ou le mot de passe ne peut être null !");
                return false;
            }

            // Encrypte le mot de passe en SHA256
            password = GetHashSha256(password);

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM mytchatroomdb.users WHERE username = @username AND pssw = @password";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            try
            {
                Console.WriteLine(command.CommandText);
                MySqlDataReader reader = command.ExecuteReader(); // Récupère le résultat de la requete
                
                if (reader.HasRows)
                {
                    Console.WriteLine("L'utilisateur s'est correctement connecté !");
                    reader.Close();
                    return UpdateStatutUser(ONLINE, username); // Connexion réussi
                }
                else
                {
                    Console.WriteLine("Le nom d'utilisateur ou le mot de passe est incorrect !");
                    reader.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return false;
            }
        }

        #region Getter
        /// <summary>
        /// Récupère l'ID de l'utilisateur à l'aide son pseudo
        /// </summary>
        /// <param name="username">Pseudo de l'utilisateur</param>
        /// <returns>Retourne l'ID de l'utilisateur. Si le pseudo n'existe pas ou si une erreure est survenue, alors on retourne null</returns>
        public string GetUserIdByUsername(string username)
        {
            // Vérifie que les champs reçus ne sont pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return null;
            }

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT ID FROM mytchatroomdb.users WHERE USERNAME = @username";
            command.Parameters.AddWithValue("@username", username);

            try
            {
                Console.WriteLine(command.CommandText);

                MySqlDataReader reader = command.ExecuteReader(); // Récupère le résultat de la requete
                reader.Read();

                if (!reader.IsDBNull(0))
                {
                    string idUser = reader.GetString(0);
                    reader.Close();
                    Console.WriteLine("L'id de l'utlisateur a correctement été récupéré !");
                    return idUser;
                }
                else
                {
                    Console.WriteLine("L'utilisateur n'existe pas !");
                    reader.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null;
            }
        }

        /// <summary>
        /// Récupère le oseudo de l'utilisateur à l'aide son ID
        /// </summary>
        /// <param name="username">L'ID de l'utilisateur</param>
        /// <returns>Retourne le peusdo de l'utilisateur. Si l'ID n'existe pas ou si une erreure est survenue, alors on retourne null</returns>
        public string GetUsernameById(string userId)
        {
            // Vérifie que les champs reçus ne sont pas vide
            if (userId == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return null;
            }

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT USERNAME FROM mytchatroomdb.users WHERE ID = @userId";
            command.Parameters.AddWithValue("@userId", userId);

            try
            {
                Console.WriteLine(command.CommandText);

                MySqlDataReader reader = command.ExecuteReader(); // Récupère le résultat de la requete
                reader.Read();

                if (!reader.IsDBNull(0))
                {
                    string idUser = reader.GetString(0);
                    reader.Close();
                    Console.WriteLine("Le nom de l'utlisateur a correctement été récupéré !");
                    return idUser;
                }
                else
                {
                    Console.WriteLine("L'utilisateur n'existe pas !");
                    reader.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null;
            }
        }

        /// <summary>
        /// Récupère l'ID de l'avatar et de l'image de background de l'utilisateur à l'aide de son pseudo
        /// </summary>
        /// <param name="username">Le pseudo de l'utilisateur</param>
        /// <returns>Retourne un tableau de string qui contient l'ID de l'avatar et l'image de background de l'utilisateur si il n'y a eu aucun soucis, sinon retourne null en cas d'erreur</returns>
        public string[] GetUserImagesIdByUsername(string username)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return null;
            }

            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "SELECT AVATAR_ID, BACKGROUND_ID FROM mytchatroomdb.users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);
            try
            {
                Console.WriteLine(command.CommandText);
                MySqlDataReader reader = command.ExecuteReader(); // Exécute la requête et récupère son résultat
                reader.Read();
                string avatarId = null;
                string backgroundId = null;
                // On ne récupère les id que si il y en a
                if (!reader.IsDBNull(0))
                {
                    avatarId = reader.GetString(0); // récupère l'id de l'avatar
                }
                if (!reader.IsDBNull(1))
                {
                    backgroundId = reader.GetString(1); // Récupère l'id du background
                }

                string[] arrayImgId = new string[] { avatarId, backgroundId }; // On stocke les ID dans un tableau de string

                reader.Close();
                return arrayImgId; // On retourne le tableau des id
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null;
            }
        }
        
        /// <summary>
        /// Récupère les données de profil de l'utilisateur à l'aide de son pseudo
        /// </summary>
        /// <param name="username">Le pseudo de l'utilisateur</param>
        /// <returns>Retourne un dictionnaire qui contient les données de profil récupéré. En cas d'erreur retourne null</returns>
        public Dictionary<string, string> GetProfilByUsername(string username)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return null;
            }

            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "SELECT EMAIL, PHONE, DESCRIPTION, HOBBIES FROM mytchatroomdb.users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);
            try
            {
                Console.WriteLine(command.CommandText);
                MySqlDataReader reader = command.ExecuteReader(); // Exécute la requête et récupère son résultat
                reader.Read();
                string email = null;
                string phone = null;
                string description = null;
                string hobbies = null;

                // On ne récupère les champs que si il y en a
                if (!reader.IsDBNull(0))
                {
                    email = reader.GetString(0); // récupère l'email de l'utilisateur
                }
                if (!reader.IsDBNull(1))
                {
                    phone = reader.GetString(1); // Récupère le numéro de téléphone de l'utilisateur
                }
                if (!reader.IsDBNull(2))
                {
                    description = reader.GetString(2); // Récupère la description de l'utilisateur
                }
                if (!reader.IsDBNull(3))
                {
                    hobbies = reader.GetString(3); // Récupère la liste des hobbies de l'utilisateur
                }

                Dictionary<string, string> profil = new Dictionary<string, string>()
                {
                    { "email", email },
                    { "phone", phone },
                    { "description", description },
                    { "hobbies", hobbies }
                };

                reader.Close();
                return profil; // On retourne le dictionnaire
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null;
            }

        }

        /// <summary>
        /// Récupère les hobbies de l'utilisateur à l'aide de son pseudo
        /// </summary>
        /// <param name="username">Le pseudo de l'utilisateur</param>
        /// <returns>Retourne un string qui contient les données de profil récupéré. En cas d'erreur retourne null</returns>
        public string GetUserHobbiesByUsername(string username)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return null;
            }

            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "SELECT HOBBIES FROM mytchatroomdb.users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);

            try
            {
                Console.WriteLine(command.CommandText);
                MySqlDataReader reader = command.ExecuteReader(); // Exécute la requête et récupère son résultat
                reader.Read();
                string hobbies = null;

                if (!reader.IsDBNull(0))
                {
                    hobbies = reader.GetString(0); // Récupère la liste des hobbies de l'utilisateur
                }
                reader.Close();
                return hobbies; // On retourne les hobby
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null;
            }

        }

        /// <summary>
        /// Récupère le mot de passe de l'utilisateur via son pseudo
        /// </summary>
        /// <param name="username">Pseudo de l'utilisateur</param>
        /// <returns>Retourne le mot de passe de l'utilisateur. Si une erreur est survenue ou si le pseudo n'existe pas return null</returns>
        public string GetUserPasswordByUsername(string username)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return null;
            }

            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "SELECT PSSW FROM mytchatroomdb.users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);

            try
            {
                Console.WriteLine(command.CommandText);

                MySqlDataReader reader = command.ExecuteReader(); // Exécute la requête et récupère son résultat
                reader.Read();
                string password = null;

                if (!reader.IsDBNull(0))
                {
                    password = reader.GetString(0);
                    Console.WriteLine("Le mot de passe a correctement été récupérer !");
                    reader.Close();
                    return password; // On retourne le mot de passe
                }
                else
                {
                    Console.WriteLine("Aucun mot de passe n'a été trouver, le pseudo de l'utilisateur est probablement incorrect : ");
                    reader.Close();
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null;
            }
        }
        
        /// <summary>
        /// Récupère la liste d'amis d'un utilisateur à l'aide de son ID
        /// </summary>
        /// <param name="idUser">L'ID de l'utilisateur</param>
        /// <returns>Retourne les ID et les messages (s'il y en a)</returns>
        public List<string[]> GetFriendsList(string idUser)
        {
            if (String.IsNullOrEmpty(idUser))
            {
                Console.WriteLine("L'id de l'utilisateur ne peut pas être vide !");
                return null;
            }

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT USERS_FRIEND_ID, MESSAGE FROM mytchatroomdb.users_has_friends WHERE USERS_ID = @idUser";
            command.Parameters.AddWithValue("@idUser", idUser);

            try
            {
                Console.WriteLine(command.CommandText);

                MySqlDataReader reader = command.ExecuteReader(); // Exécute la requête et récupère son résultat
                List<string[]> result = new List<string[]>();

                // Parcour chaque ligne récupérer avec la requête
                while (reader.Read())
                {
                    string idFriend = reader.GetString(0);
                    string message = reader.GetString(1);
                    result.Add(new string[] { idFriend, message }); // On ajoute les données dans la liste
                }
                reader.Close();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null;
            }

        }
        #endregion Getter
        #endregion SelectRequests
        #region InsertRequests
        /// <summary>
        /// Ajoute un nouvel utilisateur dans la table USERS
        /// </summary>
        /// <param name="username">Le pseudo de l'utilisateur (obligatoire)</param>
        /// <param name="password">Le mot de passe de l'utilisateur (obligatoire)</param>
        /// <param name="email">L'email de l'utilisateur (obligatoire)</param>
        /// <param name="phone">Le numéro de téléphone de l'utilisateur (par défaut null)</param>
        /// <returns>Retourne true si la création a correctemment fonctionnement, sinon retourne false</returns>
        public bool CreateNewUser(string username, string password, string email, string phone = null)
        {
            // Vérifie que les champs reçus ne sont pas vide
            if (username == "" || password == "" || email == "")
            {
                Console.WriteLine("Le nom d'utilisateur, le mot de passe ou l'email ne peut être null !");
                return false;
            }

            // Vérifie si le pseudo ou l'email sont déjà utilisé
            if (CheckIfExist(username, email))
            {
                return false; // Annule la création si c'est le cas
            }

            // Encrypte le mot de passe en SHA256
            password = GetHashSha256(password);

            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "INSERT INTO users (`USERNAME`, `PSSW`,`EMAIL`, `PHONE`, `AVATAR_ID`) VALUES(@username, @password, @email, @phone, 1)";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@phone", phone);
            try
            {
                Console.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
                Console.WriteLine("L'utilisateur a correctement été créé !");
                return true; // Création réussi
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return false; // Création raté
            }
        }

        /// <summary>
        /// Crée une invitation pour devenir ami entre 2 utilisateurs
        /// </summary>
        /// <param name="idUserSender">L'ID de l'utilisateur qui envoie l'invitation</param>
        /// <param name="idUserReceiver">L'ID de l'utilisateur qui reçoit l'invitation</param>
        /// <param name="message">Le message qui accompagne l'invitation</param>
        /// <returns>Retourne true si l'inviation a correctement été crée, sinon retourne false</returns>
        public bool CreateFriendRequest(string idUserSender, string idUserReceiver, string message)
        {
            // Vérifie que les champs reçus ne sont pas vide
            if (String.IsNullOrEmpty(idUserReceiver) || String.IsNullOrEmpty(idUserReceiver))
            {
                Console.WriteLine("L'ID des utilisateurs ne peuvent etre vide !");
                return false;
            }

            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "INSERT INTO users_has_friends (`USERS_ID`, `USERS_FRIEND_ID`,`STATUT`, `MESSAGE`) VALUES(@idUserSender, @idUserReceiver, 'En attente', @message)";
            command.Parameters.AddWithValue("@idUserSender", idUserSender);
            command.Parameters.AddWithValue("@idUserReceiver", idUserReceiver);
            command.Parameters.AddWithValue("@message", message);

            try
            {
                Console.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
                Console.WriteLine("L'invitation a correctement été créé !");
                return true; // Création réussi
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return false; // Création raté
            }
        }
        #endregion InsertRequests
        #region UpdateRequests
        /// <summary>
        /// Met à jour le statut de l'utilisateur
        /// </summary>
        /// <param name="statut">Le nouveau statut de l'utilisateur</param>
        /// <param name="username">Le pseudo de l'utilisateur à qui on change le statut</param>
        /// <returns>retourne true si le statut a correctement été changé, sinon retourne false en cas d'erreurs</returns>
        public bool UpdateStatutUser(string statut, string username)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return false;
            }
            // On vérifie que le statut reçu est accepté parmit ceux de la base
            switch (statut)
            {
                case ONLINE:
                case ABSENT:
                case DO_NOT_DISTURB:
                case INVISIBLE:
                case OFFLINE:
                    // Le champ reçu est correct, on ne fait donc rien pour l'instant
                    break;
                default:
                    // On n'a pas reçu un statut accepté par la base, on retourne donc une erreure
                    Console.WriteLine(statut + " n'est pas accepté par la base, vérifier l'orthographe !");
                    return false;
            }

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "UPDATE mytchatroomdb.users SET STATUT = @statut WHERE USERNAME = @username";
            command.Parameters.AddWithValue("@statut", statut);
            command.Parameters.AddWithValue("@username", username);

            try
            {
                Console.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
                Console.WriteLine("L'utilisateur " + username + " a correctement changé son statut en " + statut);
                return true; // La modification du statut a correctement fonctionné
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return false;
            }
        }

        /// <summary>
        /// Modifie le mot de passe de l'utilisateur
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Retourne true si le mot de passe a correctement été modifié, sinon retourne false en cas d'erreur</returns>
        public bool UpdatePasswordUser(string username, string password)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return false;
            }

            // Encrypte le mot de passe en SHA256
            password = GetHashSha256(password);

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "UPDATE mytchatroomdb.users SET PSSW = @password WHERE USERNAME = @username";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            try
            {
                Console.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
                Console.WriteLine("Le mot de passe de l'utilisateur \"" + username + "\" a correctement été modifié");
                return true; // La mot de passe a correctement fonctionné
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue lors de la modification du mdp : " + ex);
                return false;
            }
        }

        /// <summary>
        /// Met à jour le profil de l'utilisateur
        /// </summary>
        /// <param name="username">Le pseudo de l'utilisateur</param>
        /// <param name="password">Le (nouveau) mot de passe</param>
        /// <param name="email">Le (nouvel) email</param>
        /// <param name="phone">Le (nouveau) numéro de téléphone</param>
        /// <param name="description">La (nouvelle) description</param>
        /// <param name="hobbies">Les (nouveaux) centres d'intérêt</param>
        /// <returns>Returne true si la mise à jour du profil a correctement fonctionné, retourne sinon false</returns>
        public bool UpdateProfilUser(string username, string email, string phone, string description, string hobbies)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return false;
            }

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "UPDATE mytchatroomdb.users SET EMAIL = @email, PHONE = @phone, DESCRIPTION = @description, HOBBIES = @hobbies WHERE USERNAME = @username";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@hobbies", hobbies);

            try
            {
                Console.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
                Console.WriteLine("Le profil de l'utilisateur \"" + username + "\" a correctement été modifié");
                return true; // La mise à jour du profil a correctement fonctionné
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue lors de la mise à jour d'un profil : " + ex);
                return false;
            }
        }

        /// <summary>
        /// Met à jour l'ID l'image (de profil ou de fond) de l'utilisateur
        /// </summary>
        /// <param name="username">Le pseudo de l'utilisateur</param>
        /// <param name="columnImg">La colonne de l'image à mettre à jour (AVATAR_ID | BACKGROUND_ID)</param>
        /// <param name="idImage">Le nouvel ID de l'image</param>
        /// <returns>Retourne true si la modification de l'image a correctement fonctionné. Sinon retourne false</returns>
        public bool UpdateIdImageOfUser(string username, string columnImg, string idImage)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (username == "")
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return false;
            }

            MySqlCommand command = Connection.CreateCommand();
            // On définit la table via une concaténation du nom de la table au niveau du .CommandText et non via l'ajout d'un paramètre !
            command.CommandText = "UPDATE mytchatroomdb.users SET " + columnImg + " = @idImage WHERE USERNAME = @username";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@idImage", idImage);

            try
            {
                Console.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
                Console.WriteLine("L'ID de l'image a correctement été modifié");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue lors de la mise à jour d'un profil : " + ex);
                return false;
            }
        }

        #endregion UpdateRequests
        #endregion UsersRequests
        #region ImagesRequests
        #region SelectRequests
        /// <summary>
        /// Crée une image à partir d'un ID provenant de la base
        /// </summary>
        /// <param name="imageId">Id de l'image que l'on souhaite crée</param>
        /// <returns>Retourne l'image crée, en cas d'erreur retourne null</returns>
        public Image CreateImageById(string imageId)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (imageId == "")
            {
                Console.WriteLine("L'ID de l'image ne peut être null !");
                return null;
            }

            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "SELECT IMAGE FROM mytchatroomdb.images WHERE id = @imageId";
            command.Parameters.AddWithValue("@imageId", imageId);

            try
            {
                Console.WriteLine(command.CommandText);

                // Exécute la requête avant de stocker le résultat dans un tableau de byte
                byte[] blob = (byte[])command.ExecuteScalar(); // ExecuteScalar() ne retourne que la première ligne de résultat de la requête

                MemoryStream ms = new MemoryStream(blob); // Crée un flux de données (Stream) qui va stocker notre tableau de byte
                Image img = Image.FromStream(ms); // Crée notre image à partir du flux de données ms

                Console.WriteLine("L'image correctement été récupéré");
                return img; // Retourne notre image
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null;
            }
        }
        /// <summary>
        /// Récupère l'image d'avatar de notre ami à l'aide de son ID
        /// </summary>
        /// <param name="friendId">L'id de notre ami</param>
        /// <returns>Retourne l'avatar de notre ami. Si l'ID de notre ami est inconnu (ou si une erreur est survenue), alors on retourne null</returns>
        public Image GetFriendAvatarByFriendId(string friendId)
        {
            // On vérifie que le nom d'utilisateur n'est pas vide
            if (String.IsNullOrEmpty(friendId))
            {
                Console.WriteLine("Le nom d'utilisateur ne peut être null !");
                return null;
            }

            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "SELECT AVATAR_ID FROM mytchatroomdb.users WHERE ID = @friendId";
            command.Parameters.AddWithValue("@friendId", friendId);
            try
            {
                Console.WriteLine(command.CommandText);
                MySqlDataReader reader = command.ExecuteReader(); // Exécute la requête et récupère son résultat
                reader.Read();
                string avatarId = null;
                // On ne récupère les id que si il y en a
                if (!reader.IsDBNull(0))
                {
                    avatarId = reader.GetString(0); // récupère l'id de l'avatar
                    reader.Close();
                    Image friendAvatar = CreateImageById(avatarId); // On crée notre image à partir de notre ID
                    return friendAvatar; // On retourne l'avatar de notre ami
                }
                else
                {
                    Console.WriteLine("ID utilisateur inconnu");
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null;
            }
        }
        #endregion SelectRequests
        #region InsertRequests
        /// <summary>
        /// Ajoute une image dans la base
        /// </summary>
        /// <param name="img">L'image à ajouter</param>
        /// <param name="imgFormat">Le format de l'image que l'on ajoute</param>
        /// <returns>Retourne l'ID de l'image que l'on vient d'ajouté. Si une erreure est survenue on retourne null</returns>
        public string InsertImage(Image img, ImageFormat imgFormat)
        {
            byte[] blob = ConvertImageToByteArray(img, imgFormat);

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "INSERT INTO images (`IMAGE`) VALUES(@image)";
            command.Parameters.AddWithValue("@image", blob);

            try
            {
                Console.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
                Console.WriteLine("L'image a correctement été ajouté !");

                return GetLastInsertId(); // On retourne l'ID correspond à l'image que l'on vient d'ajouté
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreure est survenue : " + ex);
                return null; // Echec de l'ajout
            }
        }

        #endregion InsertRequests
        #region UpdateRequests
        #endregion UpdateRequests
        #endregion ImagesRequests

    }
}
