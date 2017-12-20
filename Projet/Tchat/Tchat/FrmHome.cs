/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmHome - This is the main form of the application when the user is connected.
 * Author : GENGA Dario
 * Last update : 2017.12.17 (yyyy-MM-dd)
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Tchat
{
    /// <summary>
    /// This is the main form of the application when the user is connected.
    /// </summary>
    public partial class FrmHome : Form
    {
        private FrmLogin _frmLogin;
        private ClientTchat _client;
        
        private Image _imgAvatar; 
        private Image _imgBackground;
        private Control _btnNewHobbies;
        private List<string[]> _friendsList;

        private bool _inEdit = false;
        private bool _pwdHasChange = false;
        private bool _avatarHasChange = false; 
        private bool _backgroundHasChange = false;
        private bool _rePaint = false;
        private bool _exitApp = false;

        #region ConstString
        #region ConnectionStatut
        // The different statut of connection accepted in the base :
        private const string ONLINE = "En ligne";
        private const string ABSENT = "Absent";
        private const string DO_NOT_DISTURB = "Ne pas déranger";
        private const string INVISIBLE = "Invisible";
        private const string OFFLINE = "Hors-ligne";
        #endregion ConnectionStatut
        public const string EDIT_TAG = "edit"; //The tag of the(dynamic) components of the edition
        public const string COLUMN_AVATAR = "AVATAR_ID";
        public const string COLUMN_BACKGROUD = "BACKGROUND_ID";
        public const string PLACEHOLDER_HOBBIES = "Ajouter un centre d'intérêt...";
        public const string PLACEHOLDER_SEARCH_FRIENDS = "Rechercher un ami...";
        public const string PLACEHOLDER_SEARCH_ROOMS = "Rechercher un salon...";
        #endregion ConstString

        /// <summary>
        /// The home window, compound of multiple tab.
        /// </summary>
        /// <param name="frmLogin">The form that contains the client object and the name of the connected user</param>
        public FrmHome(FrmLogin frmLogin)
        {
            InitializeComponent();

            FrmLogin = frmLogin;

            Client = FrmLogin.Client;
            Username = FrmLogin.Username;
        }

        /// <summary>
        /// The form login
        /// </summary>
        private FrmLogin FrmLogin { get => _frmLogin; set => _frmLogin = value; }

        /// <summary>
        /// Containt all methods who manage the connection between the client and server
        /// </summary>
        private ClientTchat Client { get => _client; set => _client = value; }

        /// <summary>
        /// Contain the name of the user connected
        /// </summary>
        public string Username { get => FrmLogin.Username; set => FrmLogin.Username = value; }

        /// <summary>
        /// Contain the password of the user connected
        /// </summary>
        private string Password { get => FrmLogin.Password; set => FrmLogin.Password = value; }

        /// <summary>
        /// The actual avatar image of the user
        /// </summary>
        private Image ImgAvatar { get => _imgAvatar; set => _imgAvatar = value; }

        /// <summary>
        /// The actual background image of the user
        /// </summary>
        private Image ImgBackground { get => _imgBackground; set => _imgBackground = value; }

        /// <summary>
        /// Contains the add button of new hobbies
        /// </summary>
        private Control BtnNewHobbies { get => _btnNewHobbies; set => _btnNewHobbies = value; }

        /// <summary>
        /// The friend list of the user
        /// </summary>
        private List<string[]> FriendsList { get => _friendsList; set => _friendsList = value; }

        /// <summary>
        /// Indicate the edition mode is on (true = on, false = off)
        /// </summary>
        private bool InEdit { get => _inEdit; set => _inEdit = value; }

        /// <summary>
        /// Indicate if we have to change the password of the user (true = yes, false = no)
        /// </summary>
        private bool PwdHasChange { get => _pwdHasChange; set => _pwdHasChange = value; }

        /// <summary>
        /// Indicate if we have to change the avatar image of the user (true = yes, false = no)
        /// </summary>
        private bool AvatarHasChange { get => _avatarHasChange; set => _avatarHasChange = value; }

        /// <summary>
        /// Indicate if we have to change the background image of the user (true = yes, false = no)
        /// </summary>
        private bool BackgroundHasChange { get => _backgroundHasChange; set => _backgroundHasChange = value; }

        /// <summary>
        /// Indicate if we have to triggers the Paint event manually (true = yes, false = no)
        /// </summary>
        private bool RePaint { get => _rePaint; set => _rePaint = value; }

        /// <summary>
        /// Indicate if we have to exit the application (true = yes, false = no)
        /// </summary>
        private bool ExitApp { get => _exitApp; set => _exitApp = value; }


        #region FrmHomeEvents
        /// <summary>
        /// Initialise les différents composants de l'interface
        /// Initialize the differents components of the interface
        /// </summary>
        private void FrmHome_Load(object sender, EventArgs e)
        {   
            // Initialize the date and hour
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            tsDate.Text = dt.ToString("dddd d MMMM yyyy - HH:mm");
            
            // Round control of the interface
            Design.RoundControl(btnStatutAvatar, 2, 2, btnStatutAvatar.Width - 5, btnStatutAvatar.Height - 5);
            foreach (Control control in pnlStatus.Controls)
            {
                // We only round button
                if (control.GetType() == typeof(Button))
                {
                    Design.RoundControl(control, 2, 2, btnStatutAvatar.Width - 5, btnStatutAvatar.Height - 5);
                }
            }
            // Initialize the images of the differents tabs
            string[] arrayImg = Client.GetUserImagesIdByUsername(Username);
            string idAvatar = null;
            string idBackground = null;
            Image imgAvatar = null;
            Image imgBackground = null;

            idAvatar = arrayImg[0];
            idBackground = arrayImg[1];
            
            // Show avatar and background images
            if (idAvatar != null && idAvatar != "0")
            {
                imgAvatar = Client.GetImageById(idAvatar);
            }
            else
            {
                imgAvatar = Properties.Resources.default_avatar;
            }
            if (idBackground != null && idBackground != "0")
            {
                imgBackground = Client.GetImageById(idBackground);
            }

            InitPictureBox(pbxAvatar, imgAvatar, pbxBackground, imgBackground);
            
            // Initialize the profil of the user
            Dictionary<string, string> profil = Client.GetProfilByUsername(Username);// Recovers the profil of the user
            InitProfil(pbxAvatar2, imgAvatar, pbxBackground2, imgBackground, lblUsername, Username, rtbDescription, profil["description"], tbxEmail, profil["email"], tbxPhone, profil["phone"], dgvHobbies, profil["hobbies"]);

            lblHome.Text = "Bienvenue " + Username + " !";
            // Management of the transparency of components
            lblHome.Parent = pbxBackground;
            lnkEditProfil.Parent = pbxBackground;
            lnkEditProfil2.Parent = pbxBackground2;
            lblUsername.Parent = pbxBackground2;
            
            // Management of events & Placeholder
            tbxNewHobbie.LostFocus += TbxNewHobbie_LostFocus;
            Placeholder phNewHobby = new Placeholder(tbxNewHobbie, PLACEHOLDER_HOBBIES);
            Placeholder phSearchFriends = new Placeholder(tbxSearchFriend, PLACEHOLDER_SEARCH_FRIENDS);
            Placeholder phSearchFriends2 = new Placeholder(tbxSearchFriend2, PLACEHOLDER_SEARCH_FRIENDS);
            Placeholder phSearchRooms = new Placeholder(tbxSearchRoom, PLACEHOLDER_SEARCH_ROOMS);
            
            // Management of the friends list
            FriendsList = Client.GetFriendsListByUserId(Client.GetUserIdByUsername(Username));
            // REMARK : Création de la liste d'ami non satisfaisante (faut-il revoir l'interface ?)
            foreach (string[] friend in FriendsList)
            {
                Image friendAvatar = Client.GetFriendAvatarById(friend[0]); // Recovers the avatar of our friend
                // Creation of a row and data filling
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvFriendsList); // Don't forget to create cells !
                row.Cells[0].Value = friendAvatar; // Add the image
                row.Cells[1].Value = Client.GetUsernameByUserId(friend[0]); // Add the name of our friend
                row.Cells[2].Value = friend[1]; // Add the message

                dgvFriendsList.Rows.Add(row); // Add the row completed
            }
        }


        /// <summary>
        /// Ask the user if he really want to close the windows (and to disconnect)
        /// </summary>
        private void FrmHome_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO : Proposer à l'utilisateur de soit se déconnecter, soit de fermer totalement l'application
            FrmDisconnect disconnect = new FrmDisconnect(MessageBoxIcon.Warning);
            
            DialogResult dr = disconnect.ShowDialog();

            if (dr == DialogResult.Yes)
            {
                Client.UpdateStatutOfUser(OFFLINE, Username); // Disconnect the user
            }
            else if (dr == DialogResult.Cancel)
            {
                e.Cancel = true; // Cancel the closure
            }
            else
            {
                ExitApp = true; // We will quit the app when this form will be closed
            }
        }

        /// <summary>
        /// Redisplay the windows of login when we close the home window or exit the app
        /// </summary>
        private void FrmHome_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!ExitApp)
            {
                FrmLogin.Show(); // Show the login window
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion FrmHomeEvents

        #region UserStatus
        /// <summary>
        /// Show/Hide the panel who contain the buttons which can to change our statut
        /// </summary>
        private void btnStatutAvatar_Click(object sender, EventArgs e)
        {
            pnlStatus.Visible = !pnlStatus.Visible;
        }

        /// <summary>
        /// Change the statut of the user when a button is clicked
        /// </summary>
        private void btnOnline_Click(object sender, EventArgs e)
        {
            pnlStatus.Visible = false;
            string statut = "";

            Button btn = (Button)sender;
            
            // Look at the property of the clicked button to know the (new) statut
            // Then we recovers it and change the statut button color of the avatar
            switch (btn.Name)
            {
                case "btnOnline":
                    statut = ONLINE;
                    btnStatutAvatar.BackColor = Color.YellowGreen;
                    break;
                case "btnAbsent":
                    statut = ABSENT;
                    btnStatutAvatar.BackColor = Color.Orange;
                    break;
                case "btnDoNotDisturb":
                    statut = DO_NOT_DISTURB;
                    btnStatutAvatar.BackColor = Color.Red;
                    break;
                case "btnInvisible":
                    statut = INVISIBLE;
                    btnStatutAvatar.BackColor = Color.Gray;
                    break;
                default:
                    Console.WriteLine("La propriété Name du bouton qui vient de cliquer est inconnue !");
                    break;
            }
            // Update the statut
            Client.UpdateStatutOfUser(statut, Username);
        }
        #endregion UserStatus

        #region InitControls
        /// <summary>
        /// Initialize the PictureBox who contains the avatar and background images of the user
        /// </summary>
        /// <param name="avatar">The PictureBox who will contain the avatar</param>
        /// <param name="imgAvatar">The avatar image</param>
        /// <param name="background">The PictureBox who will contain the background</param>
        /// <param name="imgBackground">The background image</param>
        private void InitPictureBox(PictureBox avatar, Image imgAvatar, PictureBox background, Image imgBackground)
        {
            // Round the picturebox of the avatar
            Design.RoundControl(avatar, 0, 0, avatar.Width - 3, avatar.Height - 3);
            
            // Show the images
            avatar.Image = imgAvatar;
            background.Image = imgBackground;

            // Save the images
            ImgAvatar = imgAvatar;
            ImgBackground = imgBackground;
        }

        /// <summary>
        /// Initialise les données du profil d'utilisateur
        /// Initialize the data of the profil of the user
        /// </summary>
        /// <param name="avatar">The PictureBox who will contain the avatar</param>
        /// <param name="imgAvatar">The avatar image</param>
        /// <param name="background">The PictureBox who will contain the background</param>
        /// <param name="imgBackground">The background image</param>
        /// <param name="user">The Label who contain the username</param>
        /// <param name="username">The username</param>
        /// <param name="description">The RichTextBox who contain the description of the profil</param>
        /// <param name="userDescription">The description of the profil</param>
        /// <param name="email">The Textbox who contain the email</param>
        /// <param name="userEmail">The email</param>
        /// <param name="phone">The Textbox who contain the phone</param>
        /// <param name="userPhone">The phone</param>
        /// <param name="hobbies">The DataGridView who will contain the centers of interest of the user</param>
        /// <param name="userHobbies">The centers of interest of the user, separated by semicolons (;)</param>
        private void InitProfil(PictureBox avatar, Image imgAvatar,
                                PictureBox background, Image imgBackground,
                                Label user, string username,
                                RichTextBox description, string userDescription,
                                TextBox email, string userEmail,
                                TextBox phone, string userPhone,
                                DataGridView hobbies, string userHobbies)
        {
            // Initialize the PictureBox
            InitPictureBox(avatar, imgAvatar, background, imgBackground);
            
            // Initialize the differents components
            user.Text = username;
            description.Text = userDescription;
            email.Text = userEmail;
            phone.Text = userPhone;
            
            // Initialize the centers of interest
            UpdateHobbies(hobbies, userHobbies);
            
        }
        #endregion InitControls

        #region EditManagement
        #region MethodsEdit 
        /// <summary>
        /// (Des)Enabled the edition mode of the profil
        /// </summary>
        /// <param name="inEdit">If true the mode is enabled, else he is disabled</param>
        private void EditMode(bool inEdit)
        {
            // Management of the visibility of the non dynamic fields
            #region ControlsVisibility
            lblPwd.Visible = !lblPwd.Visible;
            tbxPwd.Visible = !tbxPwd.Visible;
            lblRooms.Visible = !lblRooms.Visible;
            lblFriends.Visible = !lblFriends.Visible;
            btnSave.Visible = !btnSave.Visible;
            btnCancel.Visible = !btnCancel.Visible;
            tbxNewHobbie.Visible = !tbxNewHobbie.Visible;
            tbxEmail.ReadOnly = !tbxEmail.ReadOnly;
            tbxPhone.ReadOnly = !tbxPhone.ReadOnly;
            rtbDescription.ReadOnly = !rtbDescription.ReadOnly;
            #endregion ControlsVisibility

            if (inEdit) // Edition mode ON
            {
                // We manage the visibility of links with fix value for avoid a bug when we added/removed a tab
                lnkEditProfil.Visible = false; 
                lnkEditProfil2.Visible = false;
                
                // We remove the tabs that do not match the profil tab
                tcWindows.TabPages.Remove(tpHome);
                tcWindows.TabPages.Remove(tpFriends);
                tcWindows.TabPages.Remove(tpRooms);
                tcWindows.TabPages.Remove(tpSettings);

                // Recovers the password of the user and show it on the textbox
                tbxPwd.Text = Password;
                
                // We add buttons that allow to edit the differents informations of the user
                Design.AddEditButtonForControl(tbxPwd, gbxInformation, "BtnPwd", "Wingdings", "!", BtnPwd_Click, EDIT_TAG);
                BtnNewHobbies = Design.AddEditButtonForControl(tbxNewHobbie, gbxInformation, "BtnNewHobbie", "Wingdings 2", "Ì", BtnNewHobbie_Click, EDIT_TAG);
                BtnNewHobbies.Enabled = false;
                
                // We add the button column
                if (!dgvHobbies.Columns.Contains("ColumnButton"))
                    dgvHobbies.Columns.Add(ColumnButton);

                // TODO : Lined border for the avatar
                //_rePaint = true;
                //pbxAvatar2.Refresh();
                
                // We add a bordure and we change the cursor of the PictureBox for indicate that the image can be changed
                RePaint = true;
                pbxBackground2.Refresh();
                pbxAvatar2.Cursor = Cursors.Hand;
                pbxBackground2.Cursor = Cursors.Hand;

            }
            else // Edition mode OFF
            {
                // We manage the visibility of links with fix value for avoid a bug when we added / removed a tab
                lnkEditProfil.Visible = true;
                lnkEditProfil2.Visible = true;

                // Management of tabs
                #region TabPages
                // Add the tabs of the application
                tcWindows.TabPages.Add(tpHome);
                // we must remove the tab of the profile then add it to have the tabs in the right order
                // but we should not remove it if it is the last tab present, we must first add another
                tcWindows.TabPages.Remove(tpProfil);
                tcWindows.TabPages.Add(tpProfil);
                tcWindows.TabPages.Add(tpFriends);
                tcWindows.TabPages.Add(tpRooms);
                tcWindows.TabPages.Add(tpSettings);
                #endregion TabPages

                DisposeEditsControls(); // Removes the dynamic components of the edition
                
                string hobbies = Client.GetHobbiesOfUser(Username);
                UpdateHobbies(dgvHobbies, hobbies); // Update the DataGridView with hobbies
                
                // (Re)Display the images for when the user cancel the modification of his profil
                pbxAvatar2.Image = ImgAvatar;
                pbxBackground2.Image = ImgBackground;

                pbxAvatar2.Cursor = Cursors.Arrow;
                pbxBackground2.Cursor = Cursors.Arrow;

                // Hide the confirm password
                tbxPwdConfirm.Visible = false;
                lblPwdConfirm.Visible = false;
            }
            InEdit = inEdit;
        }

        /// <summary>
        /// Update the DataGridView with the hobbies received in parameters
        /// </summary>
        /// <param name="hobbies">The DataGridView who has to be update</param>
        /// <param name="userHobbies">String who contains the centers of interests of the user, separated by semicolon (;)</param>
        private void UpdateHobbies(DataGridView hobbies, string userHobbies)
        {
            dgvHobbies.Rows.Clear(); // Delete old data

            if (userHobbies != null ||userHobbies == String.Empty)
            {
                // Create an array who contains the centers of interests of the user, separated by semicolon
                string[] arrHobbies = userHobbies.Split(';');

                // Add each hobby in our DataGridView
                foreach (string hobby in arrHobbies)
                {
                    dgvHobbies.Rows.Add(hobby);
                }
                dgvHobbies.Columns.Remove("ColumnButton"); // Remove the buttons column
            }
           
        }

        /// <summary>
        /// Enabled the edition of the password
        /// </summary>
        private void BtnPwd_Click(object sender, EventArgs e)
        {
            PwdHasChange = true; // To do at the beginning for avoid any problem with the TextChange event

            // Recup the button and makes it invisible
            Button btn = (Button)sender;
            btn.Visible = false;
            
            tbxPwd.ReadOnly = false; // Enable the edition of the field
            tbxPwd.Text = String.Empty; // Clear the TextBox to not keep the old password
            
            // Show Label and TextBox of confirmation
            tbxPwdConfirm.Visible = true;
            lblPwdConfirm.Visible = true;
            
            // Add a button which allows to cancel the modification of the password
            Design.AddEditButtonForControl(tbxPwdConfirm, gbxInformation, "BtnPwdConfirm", "Wingdings 2", "Ò", BtnPwdConfirm_Click, EDIT_TAG);
        }

        /// <summary>
        /// Cancel the edition of the password 
        /// </summary>
        private void BtnPwdConfirm_Click(object sender, EventArgs e)
        {
            PwdHasChange = false; // To do at the beginning for avoid any problem with the TextChange event
            
            // Recup the button and makes it invisible
            Button btn = (Button)sender;
            btn.Visible = false;

            tbxPwd.ReadOnly = true; // Disabled the edition of the field

            tbxPwdConfirm.Text = String.Empty; // Clear the field of confirmation

            // Recovers the password of the user and show it on the textbox
            tbxPwd.Text = Password;
            
            // Management of the visibility
            tbxPwdConfirm.Visible = false;
            lblPwdConfirm.Visible = false;
            gbxInformation.Controls.Find("BtnPwd", false)[0].Visible = true; // Recovers the button who enable the edition of the password and show it
        }

        /// <summary>
        /// Call the method who add a new center of interest in the list after a click
        /// </summary>
        private void BtnNewHobbie_Click(object sender, EventArgs e)
        {
            AddNewHobbie();
        }

        /// <summary>
        /// Add a new center of interest in the list
        /// </summary>
        private void AddNewHobbie()
        {
            if (tbxNewHobbie.Text != "")
            {
                dgvHobbies.Rows.Add(tbxNewHobbie.Text.Trim()); // Add a new center of interest in the DataGridView
                tbxNewHobbie.Text = String.Empty; // Clear the TextBox
            }
        }

        /// <summary>
        /// Delete a center of interest
        /// </summary>
        private void dgvHobbies_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If the button column is present AND if it is she who has just been clicked corresponds to the button column...

            if (dgvHobbies.Columns.Contains("ColumnButton") && e.ColumnIndex == dgvHobbies.Columns["ColumnButton"].Index)
            {
                dgvHobbies.Rows.RemoveAt(e.RowIndex); // ... then we delete the row
            }
        }

        /// <summary>
        /// Load an image for the PictureBox
        /// </summary>
        private void pbxAvatar2_Click(object sender, EventArgs e)
        {
            PictureBox pbx = (PictureBox)sender;

            if (InEdit)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Image Files (*.bmp, *.jpg, *.png, *.gif)|*.bmp;*.jpg;*.png;*.gif";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap btp = new Bitmap(ofd.FileName); // Create a bitmap from the file name
                    pbx.Image = btp; // Change the image of the picturebox with the bitmap

                    // Note : Don't use pbx.ImageLocation for changing the image !
                    // The "visual" image would change but it does not change the content of pbx.Image
                    // which would make the recovery of the image more "complex"
                    
                    // We indicate that a PictureBox has change his image
                    if (pbx.Name == pbxAvatar2.Name)
                        AvatarHasChange = true;
                    else
                        BackgroundHasChange = true;
                }

            }
        }
        #endregion MethodsEdit
        #region StateEdit
        /// <summary>
        /// Disabled the changement of TabPage when we are in edition mode
        /// </summary>
        private void tcWindows_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // NOTE :
            // The code below is no longer necessary because we delete the TabPages (tabs) "useless" when we are in edition mode
            // An alternative would be if we decide not to remove the tabs, we could then
            // notify the user with a message that they can not change tabs in edit mode
            
            // Deactivates the changement of tab when we are in edition mode
            if (InEdit) 
                e.Cancel = true; 
        }

        /// <summary>
        /// Enable the edition mode
        /// </summary>
        private void lnkEditProfil_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditMode(true);
        }

        /// <summary>
        /// Save or cancel the edition of the profil
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn == btnSave) // Saving the modifications
            {
                string idAvatar = null;
                string idBackground = null;
                string description = rtbDescription.Text;
                string password = tbxPwd.Text;
                string email = tbxEmail.Text;
                string phone = tbxPhone.Text;
                string hobbies = ""; // String that will store our hobbies, they will be separated by semicolons (;)

                foreach (DataGridViewRow rowHobby in dgvHobbies.Rows)
                {
                    // Add the value of the first cell of our row as well as a semicolons in our string of hobbies
                    hobbies += rowHobby.Cells[0].Value + ";"; 
                }
                if (hobbies.Length != 0)
                    hobbies = hobbies.Remove(hobbies.Length - 1); // Delete the last semicolons
                else
                    hobbies = null; // Set the string to null if there is not hobbies
                
                // Update the avatar and backgroud images
                if (AvatarHasChange)
                {
                    idAvatar = ManagesPicturebox(pbxAvatar2, COLUMN_AVATAR);
                }
                if (BackgroundHasChange)
                {
                    idBackground = ManagesPicturebox(pbxBackground2, COLUMN_BACKGROUD);
                }
                
                // Update the profil
                Client.UpdateProfilUser(Username, email, phone, description, hobbies);
                
                // We change the password if it has been changed
                if (PwdHasChange)
                {
                    Client.UpdatePasswordOfUser(Username, password);
                    Password = password;
                    PwdHasChange = false;
                }

                EditMode(false);
            }
            else // Cancel the modifications
            {
                DialogResult dr = MessageBox.Show("Souhaitez-vous vraiment annuler ? \nToutes les modifications non enregistrées seront perdues !", "Annuler la modification du profil ?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.OK)
                {
                    EditMode(false);
                }
            }
        }
        /// <summary>
        /// Add a new image to the database and update the image of the PictureBox of the home tab, as weel as the data associate of the image type
        /// </summary>
        /// <param name="pbx">The PictureBox of the tab profil who will contains the new image</param>
        /// <param name="columnImg">The column of the image in the database</param>
        /// <returns>Return the ID of our new image</returns>
        private string ManagesPicturebox(PictureBox pbx, string columnImg)
        {
            Image img = pbx.Image;
            string idImg = Client.InsertImage(img);

            // Performs the own processing for the type of image received
            switch (columnImg)
            {
                case "AVATAR_ID":
                    pbxAvatar.Image = img; // Change the avatar image of the tab home
                    ImgAvatar = img; // Save the image
                    AvatarHasChange = false;
                    break;
                case "BACKGROUND_ID":
                    pbxBackground.Image = img; // Change the background image of the tab home
                    ImgBackground = img; // Save the image
                    BackgroundHasChange = false;
                    break;
                default:
                    Console.WriteLine("Paramètre inconnu reçu lors de l'exécution de la methode UpdatePictureBox");
                    break;
            }
            // Update the id of the avatar of the user
            Client.UpdateIdImageOfUser(Username, columnImg, idImg);

            return idImg;
        }

        /// <summary>
        /// Removes all dynamic controls from the edit mode
        /// </summary>
        public void DisposeEditsControls()
        {
            List<Control> listCtrl = new List<Control>(); // Will contain the controls to delete
            
            foreach (Control control in gbxInformation.Controls)
            {
                // We have to do a cast of the tag instead of an .toString() to avoid a crash if the tag is null
                if ((string)control.Tag == EDIT_TAG)
                {
                    listCtrl.Add(control); // Add the control to the list if the tags match

                    /* Note : 
                     *          We don't delete the controls here
                     *          because the index of the foreach would be indirectly modified
                     *          which could prevent certain controls from being traveled (and therefore deleted)
                     */
                }
            }

            foreach (Control ctrl in listCtrl)
            {
                // Editing controls are removed from this list to make sure you delete all the controls
                gbxInformation.Controls.Remove(ctrl); 
            }
            
        }

        /// <summary>
        /// Enable or disable the edit record button after verifying that the passwords are the same or have not changed
        /// </summary>
        private void tbxPwd_TextChanged(object sender, EventArgs e)
        {
            // If the password confirm is identical to the passwaord, and if the lenght of the password is > at 0
            // Or if the password hasn't changed
            if ((tbxPwd.Text == tbxPwdConfirm.Text && tbxPwd.Text.Length > 0) || (PwdHasChange == false))
            {
                btnSave.Enabled = true; // Then we enable the edit record button
            }
            else
            {
                btnSave.Enabled = false; // Else we disable it
            }
        }
        #endregion StateEdit
        #endregion EditManagement

        #region ControlsEvents
        #region TextboxPlaceholder

        /// <summary>
        /// Enaled or disabled the add button of hobbies when the text change
        /// </summary>
        private void tbxNewHobbie_TextChanged(object sender, EventArgs e)
        {
            // Enaled or disabled the add button
            if (String.IsNullOrWhiteSpace(tbxNewHobbie.Text))
                BtnNewHobbies.Enabled = false;
            else
                BtnNewHobbies.Enabled = true;
        }

        /// <summary>
        /// Disabled the add button of hobbies when the textbox loses the focus
        /// </summary>
        private void TbxNewHobbie_LostFocus(object sender, EventArgs e)
        {
            // Disabled the add button of hobbies if the textbox is empty (or filled with whit spaces)
            // We test if the text correspond to the placeholder to be sure to disabled the button even if the event occurs too late
            if (String.IsNullOrWhiteSpace(tbxNewHobbie.Text) || tbxNewHobbie.Text == PLACEHOLDER_HOBBIES)
            {
                BtnNewHobbies.Enabled = false;
            }
        }

        #endregion TextboxPlaceholder

        /// <summary>
        /// Show the actual hour
        /// </summary>
        private void tmrDate_Tick(object sender, EventArgs e)
        {
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            tsDate.Text = dt.ToString("dddd d MMMM yyyy - HH:mm"); // ex : mercredi 11 octobre 2017 - 12:01

            if (Client.ClientSocket.Connected)
            {
                tsConnected.Text = "Connecté au serveur";
                tsConnected.ForeColor = Color.Green;
            }
            else
            {
                tsConnected.Text = "Déconnecté au serveur !";
                tsConnected.ForeColor = Color.Red;
                tmrTryConnect.Enabled = true;
            }
        }

        /// <summary>
        /// Character filtering
        /// </summary>
        private void tbxNewHobbie_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the user press enter AND if the center of interest isn't empty
            if (e.KeyChar == (char)13 && tbxNewHobbie.Text != "")
                AddNewHobbie(); // Then we add the new center of interest
            
            // Only letters and controls authorized
            if ((!char.IsLetter(e.KeyChar)) && (!char.IsControl(e.KeyChar)) && e.KeyChar == (char)20)
                e.Handled = true;
        }

        /// <summary>
        /// Redraw a PictureBox with a dashed border
        /// </summary>
        /// <param name="sender">The PictureBox that have called the method</param>
        /// <param name="e">The data of the events of the PictureBox</param>
        private void PaintPictureBox(object sender, PaintEventArgs e)
        {
            // We redraw the PictureBox only if this is a "manual" paint
            if (RePaint)
            {
                PictureBox pbx = (PictureBox)sender;
                Design.DrawDashedBorder(pbx, e);
                RePaint = false;
            }
        }

        #endregion ControlsEvents

        #region FriendsManagement

        /// <summary>
        /// Open the windows of the friendship request (FrmFriendInvitation)
        /// </summary>
        private void btnAddFriend_Click(object sender, EventArgs e)
        {
            FrmFriendInvitation frmFriendInvitation = new FrmFriendInvitation(Client, Username);
            frmFriendInvitation.ShowDialog();

            if (frmFriendInvitation.DialogResult == DialogResult.OK)
            {
                // Recovers the id of the users
                string idUserSender = Client.GetUserIdByUsername(Username);
                string idUserReceiver = Client.GetUserIdByUsername(frmFriendInvitation.FriendName);

                string messageRequest = frmFriendInvitation.MessageRequest;

                // Test if the user to whom we are about to send a request for friendship would not have already sent us a request
                if (Client.CheckIfFriendRequestAlreadyExist(idUserSender, idUserReceiver))
                {
                    MessageBox.Show("Vous avez déjà reçu une demande d'amitié de la part de " + frmFriendInvitation.FriendName + ".\n\nSouhaitez-vous l'acceptez maintenant (en cas de refus elle restera \"en attente\") ?", "Demande d'amitié en attente", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    // TODO : Permettre à l'utilisateur d'accepter une demande d'amitié si il y en a une déjà en cours
                }
                else if (Client.CreateFriendRequest(idUserSender, idUserReceiver, messageRequest))
                {
                    MessageBox.Show("Une demande d'amitié a été envoyé à " + frmFriendInvitation.FriendName, "Demande d'amitié envoyé", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Une erreure est survenue lors de l'envoie, veuillez réessayer plus tard. ", "Erreur lors de l'envoie", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }


        #endregion FriendsManagement

        private void tbxMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13 && !String.IsNullOrWhiteSpace(tbxMessage.Text))
            {
                // Add the message to the RichTextBox when the user press enter
                rtbConversation.Text += tbxMessage.Text + Environment.NewLine;
                tbxMessage.Text = String.Empty;
            }
        }

        /// <summary>
        /// Try to connect the user to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrTryConnect_Tick(object sender, EventArgs e)
        {
            Client = new ClientTchat(); // Try to connect the client to the server

            if (Client.ClientSocket.Connected)
            {
                tmrTryConnect.Enabled = false;
            }
        }
    }
}
