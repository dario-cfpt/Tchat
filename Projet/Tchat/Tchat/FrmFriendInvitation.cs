/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmFriendInvitation - This windows allows to create friend request
 * Author : GENGA Dario
 * Last update : 2017.12.17 (yyyy-MM-dd)
 */
using System;
using System.Windows.Forms;

namespace Tchat
{
    /// <summary>
    /// This windows allows to create friend request
    /// </summary>
    public partial class FrmFriendInvitation : Form
    {
        private ClientTchat _client;
        private string _username;

        private string _friendName;
        private string _messageRequest;
        private string _errorMessage;

        private bool _checkRequest;
        private bool _cancel;


        /// <summary>
        /// The friend invitation window
        /// </summary>
        /// <param name="client">The methods and connections of the client for the server</param>
        /// <param name="username">The name of the user connected</param>
        public FrmFriendInvitation(ClientTchat client, string username)
        {
            InitializeComponent();
            
            Client = client;
            Username = username;

            CheckRequest = false;
            Cancel = false;
            
            FriendName = "";
            MessageRequest = rtbMessage.Text;
            ErrorMessage = "";

        }

        /// <summary>
        /// Contains the methods of the client for the server
        /// </summary>
        private ClientTchat Client { get => _client; set => _client = value; }

        /// <summary>
        /// The name of the user connected
        /// </summary>
        private string Username { get => _username; set => _username = value; }

        /// <summary>
        /// The name of the user to whom we want to send an invitation to become a friend
        /// <para>When we set this we have to do multiple test to detect potential errors</para>
        /// </summary>
        public string FriendName
        {
            get =>_friendName;
            set
            {
                // If no username is specified then no treatment is done
                if (!String.IsNullOrWhiteSpace(tbxFriend.Text))
                {
                    // We get an error message if we try to send an invitation to ourselves
                    if (tbxFriend.Text == Username)
                    {
                        ErrorMessage = "Vous ne pouvez pas devenir ami avec vous-même !";
                        Cancel = true;
                    }

                    // We get an error message if we try to send an invitation to a user who does not exist
                    else if (!Client.CheckIfUserExist(tbxFriend.Text))
                    {
                        ErrorMessage = "Nom d'utilisateur inconnu !";
                        Cancel = true;
                    }
                    // If there is no error then we store the username
                    else
                    {
                        Cancel = false;
                        _friendName = value;
                    }
                }
                else
                {
                    Cancel = false;
                }
            }
        }

        /// <summary>
        /// The message that accompanies the invitation
        /// </summary>
        public string MessageRequest { get => _messageRequest; set => _messageRequest = value; }
        
        /// <summary>
        /// The message error if we can't create a friend request
        /// </summary>
        private string ErrorMessage { get => _errorMessage; set => _errorMessage = value; }

        /// <summary>
        /// If true then we have to check the request before closing the form, if false then we close it without check
        /// </summary>
        private bool CheckRequest { get => _checkRequest; set => _checkRequest = value; }

        /// <summary>
        /// Indicate if we authorize the form to close or not (false = authorized, true = not authorized
        /// </summary>
        private bool Cancel { get => _cancel; set => _cancel = value; }


        /// <summary>
        /// Check the user's name when closing the page
        /// </summary>
        private void FrmFriendInvitation_FormClosing(object sender, FormClosingEventArgs e)
        {
            // We check the request if the user has clicked on the send button
            if (CheckRequest)
            {
                FriendName = tbxFriend.Text;

                // If there was an error while changing the friend name, then Cancel would be true (so we would not need to do the last test)
                if (!Cancel)
                {
                    // If Cancel is false then we test if we don't already have sended a friend request
                    if (!Client.CheckDuplicateFriendRequest(Username, FriendName))
                    {
                        MessageRequest = rtbMessage.Text;
                    }
                    else
                    {
                        ErrorMessage = "Vous avez déjà envoyé une demande d'amitié à " + FriendName + " !";

                        // A message is displayed to the user to inform him that he has already sent a friend request
                        MessageBox.Show(ErrorMessage, "Invitation déjà envoyé", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        lblError.Text = ErrorMessage;
                        lblError.Visible = true;
                        Cancel = true;
                    }
                }
                else
                {
                    lblError.Text = ErrorMessage;
                    lblError.Visible = true;
                }

                CheckRequest = false;
                e.Cancel = Cancel;
            }
            // Else the form will be closed without check (and without sending)
        }

        /// <summary>
        /// Toggle the button according to the content of the TextBox
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
        /// Set to true the CheckRequest when the user click on the button
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            CheckRequest = true;
        }

    }
}
