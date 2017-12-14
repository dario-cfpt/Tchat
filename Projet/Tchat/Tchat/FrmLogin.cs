/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmLogin - This is first form of the app, the user can log in or create an account
 * Author : GENGA Dario
 * Project creation : 2017.08.31
 * Last update : 2017.12.14 (yyyy-MM-dd)
 * © 2017
 */

using System;
using System.Windows.Forms;

namespace Tchat
{
    public partial class FrmLogin : Form
    {
        private string _username;
        private ClientTchat _client;

        public const string SERVER = "localhost";
        public const string DATABASE = "mytchatroomdb";
        public const string USER = "admin";
        public const string PASSWORD = "8185c8ac4656219f4aa5541915079f7b3743e1b5f48bacfcc3386af016b55320";

        public FrmLogin()
        {
            InitializeComponent();
            Client = new ClientTchat(); // Try to connect the client to the server
        }

        /// <summary>
        /// Contain the name of the user connected
        /// </summary>
        public string Username { get => _username;  set => _username = value; }

        /// <summary>
        /// Containt all methods who manage the connection between the client and server
        /// </summary>
        public ClientTchat Client { get => _client; set => _client = value; }
        
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
        /// Try to connect the user to the database
        /// </summary>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = tbxUserName.Text;
            string password = tbxPassword.Text;

            Client.CallSendLogin(username, password);

            if (Client.Logged)
            {
                // The user has connected successfully 
                MessageBox.Show("Bonjour " + username + " !", "Connexion réussi !");

                Username = username; // Save the username after the connection

                FrmHome frmHome = new FrmHome(this);
                frmHome.Show();
                Hide();
            }
            else
            {
                // Unknown user or incorrect password
                string message = "Le nom d'utilisateur ou le mot de passe est incorrect !";
                string title = "Erreur";

                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
