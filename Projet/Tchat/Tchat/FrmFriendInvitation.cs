/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmFriendInvitation 
 * Author : GENGA Dario
 * Last update : 2017.16.22 (yyyy-MM-dd)
 */

using System;
using System.Windows.Forms;

namespace Tchat
{
    public partial class FrmFriendInvitation : Form
    {
        private FrmLogin _frmLogin;
        private RequestsSQL _requestsSQL;
        private bool _cancel; // false = on autorise la fermeture de la form, true = on la refuse
        private string _errorMessage;
        private string _friendResquest;
        private string _messageRequest;

        public FrmFriendInvitation(FrmLogin frmLogin, RequestsSQL requestsSQL)
        {
            InitializeComponent();

            _frmLogin = frmLogin;
            _requestsSQL = requestsSQL;
            _cancel = false;
            _errorMessage = "";
            FriendRequest = "";
            MessageRequest = rtbMessage.Text;
        }

        /// <summary>
        /// Le nom de l'utilisateur à qui l'on souhaite envoyé une invitation pour devenir ami
        /// </summary>
        public string FriendRequest
        {
            get
            {
                return _friendResquest;
            }
            set
            {
                // Si aucun pseudo n'est spécifié, alors on ne fait aucun traitement
                if (!String.IsNullOrWhiteSpace(tbxFriend.Text))
                {
                    // On affiche un message d'erreur si on tente d'envoyer une invitation à nous-même
                    if (tbxFriend.Text == _frmLogin.Username)
                    {
                        _errorMessage = "Vous ne pouvez pas devenir ami avec vous-même !";
                        _cancel = true;
                    }
                    // On affiche un message d'erreur si on tente d'envoyé une invitation à un utilisateur qui n'existe pas
                    else if (!_requestsSQL.CheckIfUserExist(tbxFriend.Text))
                    {
                        _errorMessage = "Nom d'utilisateur inconnu !";
                        _cancel = true;
                    }
                    // Si il n'y a pas d'erreur alors on stocke le pseudo
                    else
                    {
                        _cancel = false;
                        _friendResquest = value;
                    }
                }
                else
                {
                    _cancel = false;
                }
            }
        }
        /// <summary>
        /// Le message qui accompagne l'invitation
        /// </summary>
        public string MessageRequest { get => _messageRequest; set => _messageRequest = value; }

        /// <summary>
        /// Active ou désactive le bouton selon le contenu du textbox
        /// </summary>
        private void tbxFriend_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbxFriend.Text))
            {
                btnOk.Enabled = false;
            }
            else
            {
                btnOk.Enabled = true;
            }
        }

        /// <summary>
        /// Vérifie le nom de l'utilisateur lors de la formeture de la page
        /// </summary>
        private void FrmFriendInvitation_FormClosing(object sender, FormClosingEventArgs e)
        {
            FriendRequest = tbxFriend.Text;

            // Test si on n'a pas déjà envoyé une demande d'amitié
            if (!_requestsSQL.CheckDuplicateFriendRequest(_frmLogin.Username, FriendRequest))
            {
                MessageRequest = rtbMessage.Text;
                lblError.Text = _errorMessage;
                lblError.Visible = _cancel;
            }
            else
            {
                // TODO : Fermer la fenêtre sans vérification si on tente de la fermer via le bouton annuler ou la croix rouge

                // On affiche un message à l'utilisateur pour le prévenir qu'il a déjà envoyé une demande d'amitié
                MessageBox.Show("Vous avez déjà envoyé une demande d'amitié à " + FriendRequest + " !", "Invitation déjà envoyé", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                _cancel = true;
            }

            e.Cancel = _cancel;
        }
    }
}
