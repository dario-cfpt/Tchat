/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmRegister - This form is used to create a new account
 * Author : GENGA Dario
 * Last update : 2017.12.14 (yyyy-MM-dd)
 */

using System;
using System.Windows.Forms;

namespace Tchat
{
    public partial class FrmRegister : Form
    {
        private ClientTchat _client;
        private string _username; // This will serve for the autocompletion later
        private string _password; // This will serve for the autocompletion later

        /// <summary>
        /// The register form.
        /// </summary>
        /// <param name="client">ClientTchat object who will maintain only 1 connection of the client whith the server</param>
        public FrmRegister(ClientTchat client)
        {
            InitializeComponent();
            // We don't want to create multiple connection of the same user to the server, so we recup the current one !
            _client = client; 
        }
        
        /// <summary>
        /// Name of the user
        /// </summary>
        public string Username { get => _username; set => _username = value; }

        /// <summary>
        /// Password of the user
        /// </summary>
        public string Password { get => _password; set => _password = value; }
        
        /// <summary>
        /// The user registers to the database
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            Username = tbxUserName.Text;
            Password = tbxPassword.Text;
            string email = tbxEmail.Text;
            string phone = tbxPhone.Text;
            
            // We send the data to the server for creation a new account and recup the result of the creation
            bool created = _client.CreateNewAccount(Username, Password, email, phone);

            if (created)
            {
                MessageBox.Show("L'inscription a correctement fonctionné !", "Bienvenue " + Username, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Le nom d'utilisateur ou l'email est déjà utilisé !", "Echec de la création", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        /// <summary>
        /// Close the register windows (when we click the link), so we can return to the login windows
        /// </summary>
        private void lnkAlreadyAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close(); // Close the register windows
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
        /// Activate or deactivate the button for the register
        /// </summary>
        private void tbxUserName_TextChanged(object sender, EventArgs e)
        {
            // username, password (and confirm password) and email must be not empty
            // the confirm password must be indentical to the password
            if (tbxUserName.Text != "" && tbxPassword.Text != "" && tbxPasswordConfirm.Text != "" && tbxEmail.Text != "" && tbxPassword.Text == tbxPasswordConfirm.Text)
            {
                btnRegister.Enabled = true;
            }
            else
            {
                // TODO : indiquer que la confirmation de mot de passe ne correspond pas au mdp
                // TODO : gérer le format de l'email et du téléphone
                btnRegister.Enabled = false;
            }
        }

    }
}
