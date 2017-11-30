/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmRegister - This form is used to create a new account
 * Author : GENGA Dario
 * Last update : 2017.09.28
 * Version : 0.2
 */

using System;
using System.Windows.Forms;

namespace Tchat
{
    public partial class FrmRegister : Form
    {
        public FrmRegister()
        {
            InitializeComponent();
        }
        // On a besoin de récupérer le nom d'utilisateur et le mot de passe après la création du compte
        private string _username;
        private string _password;

        public string Username
        {
            get { return _username; }
            set { this._username = value; }
        }

        public string Password
        {
            get {  return _password; }
            set { this._password = value; }
        }

        // Retourne à la fenêtre de connexion
        private void lnkAlreadyAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close(); // Ferme la fenêtre d'inscription
        }

        // L'utilisateur s'enregistre dans la base
        private void btnRegister_Click(object sender, EventArgs e)
        {
            Username = tbxUserName.Text;
            Password = tbxPassword.Text;
            string email = tbxEmail.Text;
            string phone = tbxPhone.Text;

            DatabaseConnection dbConnect = new DatabaseConnection(FrmLogin.SERVER, FrmLogin.DATABASE, FrmLogin.USER, FrmLogin.PASSWORD);

            RequestsSQL requestsSQL = new RequestsSQL(dbConnect.Connection);

            if (requestsSQL.CreateNewUser(Username, Password, email, phone))
            {
                MessageBox.Show("L'inscription a correctement fonctionné !", "Bienvenue " + Username, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Une erreure est survenue !" + Environment.NewLine + "Veuillez contactez l'administrateur", "Aïe... :( ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        // Filtrage des caractères
        private void tbxUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsLetter(e.KeyChar)) && (!char.IsNumber(e.KeyChar)) && (!char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        // Active ou désactive le bouton d'inscription
        private void tbxUserName_TextChanged(object sender, EventArgs e)
        {
            // Le nom d'utilisateur, le mot de passe (et sa confirmation) et l'email ne doivent pas être vide pour pouvoir s'inscrire
            // La confirmation du mot de passe doit également être identique au mot de passe
            if (tbxUserName.Text != "" && tbxPassword.Text != "" && tbxPasswordConfirm.Text != "" && tbxEmail.Text != "" && tbxPassword.Text == tbxPasswordConfirm.Text)
            {
                btnRegister.Enabled = true;
            }
            else
            {
                // TO DO : indiquer que la confirmation de mot de passe ne correspond pas au mdp
                // TO DO : gérer le format de l'email et du téléphone
                btnRegister.Enabled = false;
            }
        }
    }
}
