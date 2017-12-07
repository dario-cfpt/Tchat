/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmLogin - This is first form of the app, the user can log in or create an account
 * Author : GENGA Dario
 * Project creation : 2017.08.31
 * Last update : 2017.10.2017
 * © 2017
 */

using System;
using System.Windows.Forms;

namespace Tchat
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }
        private string _username; // Contient le pseudo de l'utilisateur actuellement connecté

        public const string SERVER = "localhost";
        public const string DATABASE = "mytchatroomdb";
        public const string USER = "admin";
        public const string PASSWORD = "8185c8ac4656219f4aa5541915079f7b3743e1b5f48bacfcc3386af016b55320";

        public string Username
        {
            get { return this._username; }
            set { this._username = value; }
        }

        // Ouvre la fenêtre d'inscription lorsqu'on clique sur le lien
        private void lnkNoAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmRegister frmRegister = new FrmRegister();

            var dr = frmRegister.ShowDialog(); // Ouvre la fenêtre d'inscription

            if(dr == DialogResult.OK)
            {
                // Remplit automatiquement les textbox une fois que le compte a été créer
                tbxUserName.Text = frmRegister.Username;
                tbxPassword.Text = frmRegister.Password;
                btnLogin.Select(); // met le focus sur le bouton login
            }

        }

        // L'utilisateur tente de se connecter à la base
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = tbxUserName.Text;
            string password = tbxPassword.Text;

            DatabaseConnection dbConnect = new DatabaseConnection(SERVER, DATABASE, USER, PASSWORD);
            RequestsSQL requests = new RequestsSQL(dbConnect.Connection);

            ClientTchat client = new ClientTchat(username, password);

            if (requests.Login(username, password))
            {
                MessageBox.Show("Bonjour " + username + " !", "Connexion réussi !");

                Username = username; // On enregistrer le pseudo de l'utilisateur connecté

                FrmHome frmHome = new FrmHome(this);
                frmHome.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Le nom d'utilisateur ou le mot de passe est incorrect !", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Active ou désactive le bouton de login
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
        
        // Filtrage des caractères
        private void tbxUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsLetter(e.KeyChar)) && (!char.IsNumber(e.KeyChar)) && (!char.IsControl(e.KeyChar)))
                e.Handled = true;
        }
    }
}
