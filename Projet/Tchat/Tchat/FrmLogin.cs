/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmLogin - This is first form of the app, the user can log in or create an account with a link
 * Author : GENGA Dario
 * Project creation : 2017.08.31
 * Last update : 2017.12.17 (yyyy-MM-dd)
 * © 2017
 */

using System;
using System.Windows.Forms;

namespace Tchat
{
    /// <summary>
    /// This is first form of the app, the user can log in or create an account with a link
    /// </summary>
    public partial class FrmLogin : Form
    {
        private ClientTchat _client;
        private string _username;
        private string _password;

        /// <summary>
        /// Login Window. Create the connection with the server
        /// </summary>
        public FrmLogin()
        {
            //TODO : icon for the app
            InitializeComponent();
            Client = new ClientTchat(); // Try to connect the client to the server
        }

        /// <summary>
        /// Containt all methods who manage the connection between the client and server
        /// </summary>
        public ClientTchat Client { get => _client; set => _client = value; }

        /// <summary>
        /// Contain the name of the user connected
        /// </summary>
        public string Username { get => _username;  set => _username = value; }

        /// <summary>
        /// Contain the password of the user connected
        /// </summary>
        public string Password { get => _password; set => _password = value; }


        /// <summary>
        /// Try to connect the user to the database
        /// </summary>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = tbxUserName.Text;
            string password = tbxPassword.Text;

            Client.TryLogin(username, password);

            if (Client.Logged)
            {
                // The user has connected successfully 
                MessageBox.Show("Bonjour " + username + " !", "Connexion réussi !");

                Username = username; // Save the username after the connection
                Password = password; // Save the password after the connection

                FrmHome frmHome = new FrmHome(this);
                frmHome.Show();
                Hide();
            }
            else
            {
                string title = "Erreur";
                string message = null;
                
                if (!Client.ClientSocket.Connected)
                {
                    message = "Vous n'êtes pas connectez au serveur !"; // Connection with the server error
                    tmrTryConnect.Enabled = true;
                }
                else
                {
                    message = "Le nom d'utilisateur ou le mot de passe est incorrect !"; // Login error
                }
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Open the registration page when we click on the link
        /// </summary>
        private void lnkNoAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmRegister frmRegister = new FrmRegister(_client);

            var dr = frmRegister.ShowDialog(); // Open the registration page

            if (dr == DialogResult.OK)
            {
                // Complete automaticaly the textbox when the account has been created
                tbxUserName.Text = frmRegister.Username;
                tbxPassword.Text = frmRegister.Password;
                
                btnLogin.Select();
            }

        }
        
        /// <summary>
        /// Activate or deactivate the button for the connection
        /// </summary>
        private void tbxUserName_TextChanged(object sender, EventArgs e)
        {
            if (tbxUserName.Text != "" && tbxPassword.Text != "")
            {
                btnLogin.Enabled = true;
            }
            else
            {
                btnLogin.Enabled = false;
            }
        }
        
        /// <summary>
        /// Filtering characters
        /// </summary>
        private void tbxUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Authorize only letters, numbers and controls
            if ((!char.IsLetter(e.KeyChar)) && (!char.IsNumber(e.KeyChar)) && (!char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        /// <summary>
        /// Shutdown and close the socket before leaving the app
        /// </summary>
        private void FrmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            // REMARK : The code below seems to have no effect (an error is still detected in the server), 
            //          send a command that will tell the server to close the connection might be better
            if (Client.ClientSocket.Connected)
            {
                Client.ClientSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                Client.ClientSocket.Close();
            }
        }

        /// <summary>
        /// Try to connect the user to the server
        /// </summary>
        private void tmrTryConnect_Tick(object sender, EventArgs e)
        {
            Client = new ClientTchat(); // Try to connect the client to the server

            if (Client.ClientSocket.Connected)
            {
                MessageBox.Show("Vous êtes connectez au serveur !");
                tmrTryConnect.Enabled = false;
            }
        }
    }
}
