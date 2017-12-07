/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmHome - This is the main form of the application when the user is connected.
 * Author : GENGA Dario
 * Last update : 2017.11.23 (yyyy-MM-dd)
 */

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Net;
//using Rtb = RicherTextBox.RicherTextBox; // https://www.codeproject.com/Articles/24443/RicherTextBox

namespace Tchat
{
    public partial class FrmHome : Form
    {
        private FrmLogin _frmLogin;
        private DatabaseConnection _dbConnect = new DatabaseConnection(FrmLogin.SERVER, FrmLogin.DATABASE, FrmLogin.USER, FrmLogin.PASSWORD);
        private RequestsSQL _requestsSQL;
        private Control _btnNewHobbies; // Contiendra le bouton d'ajout de nouvel hobby

        // On stocke l'image de l'avatar et du backjground pour pouvoir la remettre en cas d'annulation sans refaire de requête
        private Image _imgAvatar; 
        private Image _imgBackground;

        private List<string[]> _friendsList;

        private bool _inEdit = false; // Indique si on est en mode d'edition
        private bool _pwdHasChange = false; // Si cette variable est à true, alors on doit modifier le mdp de l'utilisateur
        private bool _avatarHasChange = false; // Si cette variable est à true, alors on doit modifier l'avatar de l'utilisateur
        private bool _backgroundHasChange = false; // Si cette variable est à true, alors on doit modifier le background de l'utilisateur
        private bool _rePaint = false; // Indique si on déclanche l'event Paint manuellement
        //private Rtb _rtb = new Rtb();

        public const string EDIT_TAG = "edit"; // Le tag des composants (dynamiques) de l'édition
        public const string COLUMN_AVATAR = "AVATAR_ID";
        public const string COLUMN_BACKGROUD = "BACKGROUND_ID";
        public const string PLACEHOLDER_HOBBIES = "Ajouter un centre d'intérêt...";
        public const string PLACEHOLDER_SEARCH_FRIENDS = "Rechercher un ami...";
        public const string PLACEHOLDER_SEARCH_ROOMS = "Rechercher un salon...";


        /// <summary>
        /// Constructeur qui permettra de stocker la page de login afin d'y retourner lorsqu'on fermera la page d'accueil
        /// </summary>
        public FrmHome(FrmLogin frmLogin)
        {
            InitializeComponent(); // ne pas oublier de remettre InitializeComponent() ici

            // Si on ne stocke pas la page de login via le constructeur alors on ne pourra pas l'afficher plus tard
            this._frmLogin = frmLogin;
            _requestsSQL = new RequestsSQL(_dbConnect.Connection);
        }

        #region FrmHomeEvents
        /// <summary>
        /// Initialise les différents composants de l'interface
        /// </summary>
        private void FrmHome_Load(object sender, EventArgs e)
        {
            // Initialise la date et l'heure
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            tsDate.Text = dt.ToString("dddd d MMMM yyyy - HH:mm");

            // On arrondit les boutons de l'interface
            Design.RoundControl(btnStatutAvatar, 2, 2, btnStatutAvatar.Width - 5, btnStatutAvatar.Height - 5);
            foreach (Control control in pnlStatus.Controls)
            {
                // Si le contrôle qu'on est en train de parcourir dans notre panel est un boutton, alors on l'arrondit
                // On ne veut pas arrondir les labels présent dans le panel
                if (control.GetType() == typeof(Button))
                {
                    Design.RoundControl(control, 2, 2, btnStatutAvatar.Width - 5, btnStatutAvatar.Height - 5);
                }
            }
            // Initialisation des images des différents onglets
            string[] arrayImg = _requestsSQL.GetUserImagesIdByUsername(_frmLogin.Username);
            string idAvatar = arrayImg[0];
            string idBackground = arrayImg[1];
            Image imgAvatar = null;
            Image imgBackground = null;
            // On affiche les images de profil et de fond
            if (idAvatar != null)
            {
                imgAvatar = _requestsSQL.CreateImageById(idAvatar); // Récupère l'avatar de l'utilisateur
            }
            if (idBackground != null)
            {
                imgBackground = _requestsSQL.CreateImageById(idBackground); // Récupère le background de l'utilisateur
            }

            InitPictureBox(pbxAvatar, imgAvatar, pbxBackground, imgBackground);
            
            // Initialisation du profil de l'utilisateur
            Dictionary<string, string> profil = _requestsSQL.GetProfilByUsername(_frmLogin.Username); // Récupère les informations du profil de l'utilisateur
            InitProfil(pbxAvatar2, imgAvatar, pbxBackground2, imgBackground, lblUsername, _frmLogin.Username, rtbDescription, profil["description"], tbxEmail, profil["email"], tbxPhone, profil["phone"], dgvHobbies, profil["hobbies"]);

            lblHome.Text = "Bienvenue " + _frmLogin.Username + " !";
            // Gestion de la transparence des composants
            lblHome.Parent = pbxBackground;
            lnkEditProfil.Parent = pbxBackground;
            lnkEditProfil2.Parent = pbxBackground2;
            lblUsername.Parent = pbxBackground2;
            
            // Gestions d'événements & Placeholder
            tbxNewHobbie.LostFocus += TbxNewHobbie_LostFocus;
            Placeholder phNewHobby = new Placeholder(tbxNewHobbie, PLACEHOLDER_HOBBIES);
            Placeholder phSearchFriends = new Placeholder(tbxSearchFriend, PLACEHOLDER_SEARCH_FRIENDS);
            Placeholder phSearchFriends2 = new Placeholder(tbxSearchFriend2, PLACEHOLDER_SEARCH_FRIENDS);
            Placeholder phSearchRooms = new Placeholder(tbxSearchRoom, PLACEHOLDER_SEARCH_ROOMS);

            // Gestion de la liste d'ami
            _friendsList = _requestsSQL.GetFriendsList(_requestsSQL.GetUserIdByUsername(_frmLogin.Username));
            // REMARK : Création de la liste d'ami non satisfaisante (faut-il revoir l'interface ?)
            foreach (string[] friend in _friendsList)
            {
                Image friendAvatar = _requestsSQL.GetFriendAvatarByFriendId(friend[0]); // On récupère l'avatar de notre ami

                // Création d'une ligne et remplissage des données
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvFriendsList); // Ne pas oublié de créé les cellules
                row.Cells[0].Value = friendAvatar; // Ajout de l'image
                row.Cells[1].Value = _requestsSQL.GetUsernameById(friend[0]); // Ajout du nom de l'ami
                row.Cells[2].Value = friend[1]; // Ajout du message

                dgvFriendsList.Rows.Add(row); // On ajoute la ligne
            }


            // TODO : connexion avec le serveur
            //IPAddress address = IPAddress.Parse("10.134.98.135");

            //_client = new ClientTchat(address, 3001);
        }


        /// <summary>
        /// Demande à l'utilisateur s'il est veut réellement fermer la fenêtre (ainsi que se déconnecter)
        /// </summary>
        private void FrmHome_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO : Proposer à l'utilisateur de soit se déconnecter, soit de fermer totalement l'application

            DialogResult dr = MessageBox.Show("Êtes-vous sûr de vouloir quitter My TchatRoom ?" + Environment.NewLine + "Vous serez déconnecté.", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (dr == DialogResult.OK)
            {
                _requestsSQL.UpdateStatutUser(RequestsSQL.OFFLINE, _frmLogin.Username); // Déconnexion de l'utilisateur
            }
            else
            {
                e.Cancel = true; // Annule la fermeture de la fenêtre
            }
        }

        /// <summary>
        /// Réafiche la page de login lorsqu'on ferme la page d'accueil
        /// </summary>
        private void FrmHome_FormClosed(object sender, FormClosedEventArgs e)
        {
            _frmLogin.Show();
        }
        #endregion FrmHomeEvents

        #region UserStatus
        /// <summary>
        /// Affiche/Masque le panel qui contient les boutons qui permettent de changer notre statut
        /// </summary>
        private void btnStatutAvatar_Click(object sender, EventArgs e)
        {
            pnlStatus.Visible = !pnlStatus.Visible;
        }
        /// <summary>
        /// Change le statut de l'utilisateur selon le bouton cliqué
        /// </summary>
        private void btnOnline_Click(object sender, EventArgs e)
        {
            pnlStatus.Visible = false;
            string statut = "";

            Button btn = (Button)sender; // Récupère le bouton qui vient de cliquer
            // Regarde quel est le bouton qui vient d'être cliquer selon sa propriété Name afin de connaître le (nouveau) statut
            // On récupère donc le statut correspondant au bouton et on change la couleur du bouton de statut de l'avatar
            switch (btn.Name)
            {
                case "btnOnline":
                    statut = RequestsSQL.ONLINE;
                    btnStatutAvatar.BackColor = Color.YellowGreen;
                    break;
                case "btnAbsent":
                    statut = RequestsSQL.ABSENT;
                    btnStatutAvatar.BackColor = Color.Orange;
                    break;
                case "btnDoNotDisturb":
                    statut = RequestsSQL.DO_NOT_DISTURB;
                    btnStatutAvatar.BackColor = Color.Red;
                    break;
                case "btnInvisible":
                    statut = RequestsSQL.INVISIBLE;
                    btnStatutAvatar.BackColor = Color.Gray;
                    break;
                default:
                    Console.WriteLine("La propriété Name du bouton qui vient de cliquer est inconnue !");
                    break;
            }
            // Met à jour le statut
            _requestsSQL.UpdateStatutUser(statut, _frmLogin.Username);
        }
        #endregion UserStatus

        #region InitControls
        /// <summary>
        /// Initialise les PictureBox qui contiennent les images de l'avatar et du fond de l'utilisateur
        /// </summary>
        /// <param name="avatar">Le PictureBox qui contiendra l'avatar</param>
        /// <param name="imgAvatar">L'image de l'avatar</param>
        /// <param name="background">Le PictureBox qui contiendra le fond</param>
        /// <param name="imgBackground">L'image de fond</param>
        private void InitPictureBox(PictureBox avatar, Image imgAvatar, PictureBox background, Image imgBackground)
        {
            // On arrondit le PictureBox l'avatar
            Design.RoundControl(avatar, 0, 0, avatar.Width - 3, avatar.Height - 3);

            // On affiche les images de profil et de fond
            avatar.Image = imgAvatar;
            background.Image = imgBackground;

            // On stocke les images
            _imgAvatar = imgAvatar;
            _imgBackground = imgBackground;
        }

        /// <summary>
        /// Initialise les données du profil d'utilisateur
        /// </summary>
        /// <param name="avatar">Le PictureBox qui contiendra l'avatar</param>
        /// <param name="imgAvatar">L'image de l'avatar</param>
        /// <param name="background">Le PictureBox qui contiendra le fond</param>
        /// <param name="imgBackground">L'image de fond</param>
        /// <param name="user">Le Label qui contiendra le pseudo</param>
        /// <param name="username">Le pseudo</param>
        /// <param name="description">Le RichTextBox qui contiendra la description de profil</param>
        /// <param name="userDescription">La description de profil</param>
        /// <param name="email">Le TextBox qui contiendra l'email</param>
        /// <param name="userEmail">L'email</param>
        /// <param name="phone">Le TextBox qui contiendra le téléphone</param>
        /// <param name="userPhone">Le téléphone</param>
        /// <param name="hobbies">Le DataGridView qui contiendra les centres d'intérêts</param>
        /// <param name="userHobbies">Les centres d'intérêts de l'utilisateur, ils doivent être séparé par des points virgules (;)</param>
        private void InitProfil(PictureBox avatar, Image imgAvatar,
                                PictureBox background, Image imgBackground,
                                Label user, string username,
                                RichTextBox description, string userDescription,
                                TextBox email, string userEmail,
                                TextBox phone, string userPhone,
                                DataGridView hobbies, string userHobbies)
        {
            // Initialisation des PictureBox
            InitPictureBox(avatar, imgAvatar, background, imgBackground);

            // Initialisation des différents composants
            user.Text = username;
            description.Text = userDescription;
            email.Text = userEmail;
            phone.Text = userPhone;

            // Initialisation de la liste des centres d'intérêt
            UpdateHobbies(hobbies, userHobbies);
            
        }
        #endregion InitControls

        #region EditManagement
        #region MethodsEdit 
        /// <summary>
        /// Active ou désactive le mode d'édition du profil
        /// </summary>
        /// <param name="inEdit">Si true alors on active l'édition, sinon on la désactive</param>
        private void EditMode(bool inEdit)
        {
            // Gestion de la visibilité des champs non-dynamique
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

            if (inEdit) // Mode edition on
            {
                // On gère la visibilité des liens avec des valeures fixe pour éviter un bug
                // il y avait une chance que les liens restaient visible à cause de l'ajout/du retrait d'onglet
                lnkEditProfil.Visible = false; 
                lnkEditProfil2.Visible = false;

                // On retire les pages d'onglet qui ne correspondent pas à celle du profil
                tcWindows.TabPages.Remove(tpHome);
                tcWindows.TabPages.Remove(tpFriends);
                tcWindows.TabPages.Remove(tpRooms);
                tcWindows.TabPages.Remove(tpSettings);

                // On récupère le mot de passe de l'utilisateur et on l'affiche dans le textbox
                tbxPwd.Text = _requestsSQL.GetUserPasswordByUsername(_frmLogin.Username);

                // On ajoute des boutons qui permettront d'éditer les différentes informations de l'utilisateur
                Design.AddEditButtonForControl(tbxPwd, gbxInformation, "BtnPwd", "Wingdings", "!", BtnPwd_Click, EDIT_TAG);
                _btnNewHobbies = Design.AddEditButtonForControl(tbxNewHobbie, gbxInformation, "BtnNewHobbie", "Wingdings 2", "Ì", BtnNewHobbie_Click, EDIT_TAG);
                _btnNewHobbies.Enabled = false;

                // On ajoute la colonne des boutons
                if (!dgvHobbies.Columns.Contains("ColumnButton"))
                    dgvHobbies.Columns.Add(ColumnButton);

                // TODO : Bordure en traitillé pour l'avatar
                //_rePaint = true;
                //pbxAvatar2.Refresh();

                // Ajout d'une bordure et changement du curseur sur les picturebox
                // pour indiqué que l'on peut changer l'image en cliquant dessus
                _rePaint = true;
                pbxBackground2.Refresh();
                pbxAvatar2.Cursor = Cursors.Hand;
                pbxBackground2.Cursor = Cursors.Hand;

            }
            else // Mode edition off
            {
                // On gère la visibilité des liens avec des valeures fixe pour éviter un bug
                // il y avait une chance que les liens restaient visible à cause de l'ajout/du retrait d'onglet
                lnkEditProfil.Visible = true;
                lnkEditProfil2.Visible = true;

                // Gestion des onglets
                #region TabPages
                // Ajoute les pages d'onglet de l'application
                tcWindows.TabPages.Add(tpHome);
                // on doit retirer l'onglet du profil puis le rajouter pour avoir les onglets dans le bonne ordre 
                // mais on ne doit pas le retirer si il s'agit du dernier onglet présent, il faut d'abord en ajouter un autre
                tcWindows.TabPages.Remove(tpProfil);
                tcWindows.TabPages.Add(tpProfil);
                tcWindows.TabPages.Add(tpFriends);
                tcWindows.TabPages.Add(tpRooms);
                tcWindows.TabPages.Add(tpSettings);
                #endregion TabPages

                DisposeEditsControls(); // Supprime les composants dynamiques de l'édition

                string hobbies = _requestsSQL.GetUserHobbiesByUsername(_frmLogin.Username);
                UpdateHobbies(dgvHobbies, hobbies); // Met à jour le DataGridView des hobbies

                // On (ré)affiche la bonne image pour si jamais l'utilisateur avait annulé sa modification du profil
                // TODO : les images sont mal enregistrer (en local)
                pbxAvatar2.Image = _imgAvatar;
                pbxBackground2.Image = _imgBackground;

                pbxAvatar2.Cursor = Cursors.Arrow;
                pbxBackground2.Cursor = Cursors.Arrow;
            }
            _inEdit = inEdit;
        }

        /// <summary>
        /// Met à jour le DataGridView des hobbies reçus en paramètres
        /// </summary>
        /// <param name="hobbies">Le DataGridView qui doit être mis à jour</param>
        /// <param name="userHobbies">Chaîne qui contient les hobbies de l'utilisateur, séparé par des points virgules (;)</param>
        private void UpdateHobbies(DataGridView hobbies, string userHobbies)
        {
            dgvHobbies.Rows.Clear(); // Efface les précédentes données

            if (userHobbies != null ||userHobbies == String.Empty)
            {
                // Crée un tableau qui contient les centres d'intérêt, séparé par des points virgules
                string[] arrHobbies = userHobbies.Split(';');
                // On ajoute chaque hobby du tableau dans notre DataGridView
                foreach (string hobby in arrHobbies)
                {
                    dgvHobbies.Rows.Add(hobby);
                }
                dgvHobbies.Columns.Remove("ColumnButton"); // On retire la colonne des boutons
            }
           
        }

        /// <summary>
        /// Active l'édition du mot de passe
        /// </summary>
        private void BtnPwd_Click(object sender, EventArgs e)
        {
            _pwdHasChange = true; // à faire au tout début pour éviter tout problème avec le TextChange

            // Récupère le bouton et le rend invisible
            Button btn = (Button)sender;
            btn.Visible = false;
            
            tbxPwd.ReadOnly = false; // Active l'édition du champs
            tbxPwd.Text = String.Empty; // Efface la TextBox pour ne pas garder le mdp encrypter qui s'y trouvait

            // Affiche le Label et la TextBox de confirmation
            tbxPwdConfirm.Visible = true;
            lblPwdConfirm.Visible = true;

            // Ajoute un bouton qui permet d'annuler la modification du mot de passe
            Design.AddEditButtonForControl(tbxPwdConfirm, gbxInformation, "BtnPwdConfirm", "Wingdings 2", "Ò", BtnPwdConfirm_Click, EDIT_TAG);
        }

        /// <summary>
        /// Annule la modification du mot de passe
        /// </summary>
        private void BtnPwdConfirm_Click(object sender, EventArgs e)
        {
            _pwdHasChange = false; // à faire au tout début pour éviter tout problème avec le TextChange

            // Récupère le bouton et le rend invisible
            Button btn = (Button)sender;
            btn.Visible = false;

            tbxPwd.ReadOnly = true; // Désactive l'édition du champs
            
            tbxPwdConfirm.Text = String.Empty;
            // On récupère le mot de passe de l'utilisateur et on l'affiche dans le textbox
            tbxPwd.Text = _requestsSQL.GetUserPasswordByUsername(_frmLogin.Username);

            // Gestion de l'affichage
            tbxPwdConfirm.Visible = false;
            lblPwdConfirm.Visible = false;
            gbxInformation.Controls.Find("BtnPwd", false)[0].Visible = true; // Récupère le bouton qui active l'edition du mot de passe et l'affiche
        }

        /// <summary>
        /// Ajoute un nouveau centre d'intérêt dans la liste après un clique sur le bouton
        /// </summary>
        private void BtnNewHobbie_Click(object sender, EventArgs e)
        {
            AddNewHobbie();
        }

        /// <summary>
        /// Ajoute un nouveau centre d'intérêt dans la liste
        /// </summary>
        private void AddNewHobbie()
        {
            if (tbxNewHobbie.Text != "")
            {
                dgvHobbies.Rows.Add(tbxNewHobbie.Text.Trim()); // Ajoute le nouveau centre d'intérêt dans le DataGridView
                tbxNewHobbie.Text = String.Empty; // Vide le TextBox
            }
        }

        /// <summary>
        /// Supprime le centre d'intérêt lorsque l'utilisateur clique sur la ligne du DataGridView correspondante.
        /// </summary>
        private void dgvHobbies_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Si la colonne des boutons est présente ET si c'est elle qui vient d'être cliquée correspond à celle des boutons...
            if (dgvHobbies.Columns.Contains("ColumnButton") && e.ColumnIndex == dgvHobbies.Columns["ColumnButton"].Index)
            {
                dgvHobbies.Rows.RemoveAt(e.RowIndex); // ... alors on supprime la ligne
            }
        }

        /// <summary>
        /// Charge une image pour les pictures box
        /// </summary>
        private void pbxAvatar2_Click(object sender, EventArgs e)
        {
            PictureBox pbx = (PictureBox)sender;

            if (_inEdit)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Image Files (*.bmp, *.jpg, *.png, *.gif)|*.bmp;*.jpg;*.png;*.gif";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap btp = new Bitmap(ofd.FileName); // Crée un bitmap à partir du nom du fichier choisi
                    pbx.Image = btp; // On change l'image du pbx avec le bitmap
                    pbx.Tag = btp.RawFormat; // Stocke le format de l'image dans le Tag pour plus tard

                    // Note : ne pas utiliser pbx.ImageLocation pour changer l'image !
                    // L'image "visuel" du picturebox changerait bel et bien mais cela ne change pas le contenu de pbx.Image
                    // ce qui rendrait la récupération de l'image plus "complexe"

                    // On indique qu'un picturebox a changé d'image
                    if (pbx.Name == pbxAvatar2.Name)
                        _avatarHasChange = true;
                    else
                        _backgroundHasChange = true;
                }

            }
        }
        #endregion MethodsEdit
        #region StateEdit
        /// <summary>
        /// Désactive le changement de TabPage lorsqu'on est en édition
        /// </summary>
        private void tcWindows_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // NOTE :
            // Le code ci-dessous n'est plus nécessaire car on supprime les TabPages (onglets) "inutiles" lorsqu'on est en mode d'édition
            // Une alternative serait que si on décide de ne pas retirer les onglets on pourrait alors
            // prévenir l'utilisateur avec un message lui indiquant qu'il ne peut pas changer d'onglet en mode édition

            // On désactive le changement d'onglet lorsqu'on est en mode d'édition
            if (_inEdit) 
                e.Cancel = true; 
        }

        /// <summary>
        /// Active le mode édition du profil de l'utilisateur
        /// </summary>
        private void lnkEditProfil_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditMode(true);
        }

        /// <summary>
        /// Sauvegarde ou annule l'edition du profil
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn == btnSave) // Sauvegarde des modifications
            {
                string idAvatar = null;
                string idBackground = null;
                string description = rtbDescription.Text;
                string password = tbxPwd.Text;
                string email = tbxEmail.Text;
                string phone = tbxPhone.Text;
                string hobbies = ""; // Chaîne qui stockera nos hobbies, ils seront séparé par des points virgules (;)

                // Parcour chaque ligne de notre DataGridView qui contient nos centres d'intérêt
                foreach (DataGridViewRow rowHobby in dgvHobbies.Rows)
                {
                    // Ajoute la Value de la première cellule de notre ligne ainsi qu'un point de virgule dans notre chaîne d'hobbies
                    hobbies += rowHobby.Cells[0].Value + ";"; 
                }
                if (hobbies.Length != 0)
                    hobbies = hobbies.Remove(hobbies.Length - 1); // Supprime le dernier point virgule 
                else
                    hobbies = null; // On met null si il n'y a pas d'hobby pour pas en avoir 1 "vide"/"sans nom"

                // Mise à jours des images de profil et de fond
                if (_avatarHasChange)
                {
                    idAvatar = ManagesPicturebox(pbxAvatar2, COLUMN_AVATAR);
                }
                if (_backgroundHasChange)
                {
                    idBackground = ManagesPicturebox(pbxBackground2, COLUMN_BACKGROUD);
                }

                // Mise à jour du profil
                _requestsSQL.UpdateProfilUser(_frmLogin.Username, email, phone, description, hobbies);

                // On modifie le mdp de l'utilisateur si il a changés
                if (_pwdHasChange)
                {
                    _requestsSQL.UpdatePasswordUser(_frmLogin.Username, password);
                    _pwdHasChange = false;
                }

                EditMode(false);
            }
            else // Annulation des modifications
            {
                DialogResult dr = MessageBox.Show("Souhaitez-vous vraiment annuler ? \nToutes les modifications non enregistrées seront perdues !", "Annuler la modification du profil ?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.OK)
                {
                    EditMode(false);
                }
            }
        }

        /// <summary>
        /// Ajoute une nouvelle image dans la base et met à jour celle du PictureBox de l'onglet accueil ainsi que les données associées au type d'image
        /// </summary>
        /// <param name="pbx">Le PictureBox de l'onglet profil qui contient la nouvelle image</param>
        /// <param name="columnImg">La colonne de l'image dans la base</param>
        /// <returns>Retourne l'ID de notre nouvelle image</returns>
        private string ManagesPicturebox(PictureBox pbx, string columnImg)
        {
            Image img = pbx.Image;
            ImageFormat imgFormat = (ImageFormat)pbx.Tag;
            string idImg = _requestsSQL.InsertImage(img, imgFormat); // Ajoute l'image dans la base et récupère son ID

            // Effectue les traitements propres au type d'image reçu
            switch (columnImg)
            {
                case "avatar":
                    pbxAvatar.Image = img; // Change l'image du PictureBox dans l'onglet Accueil
                    _imgAvatar = img; // Stocke l'image
                    break;
                case "background":
                    pbxBackground.Image = img; // Change l'image du PictureBox dans l'onglet Accueil
                    _imgBackground = img; // Stocke l'image
                    break;
                default:
                    Console.WriteLine("Paramètre inconnu reçu lors de l'exécution de la methode UpdatePictureBox");
                    break;
            }
            // Met à jour l'ID de l'avatar de l'utilisateur
            _requestsSQL.UpdateIdImageOfUser(_frmLogin.Username, columnImg, idImg);

            return idImg; // Retourne l'ID de l'image que l'on vient d'ajouter
        }

        /// <summary>
        /// Supprime tout les contrôle dynamiques du mode d'édition
        /// </summary>
        public void DisposeEditsControls()
        {
            List<Control> listCtrl = new List<Control>(); // contiendra les contrôles à supprimer

            // Parcour chaque contrôle du groupbox des 
            foreach (Control control in gbxInformation.Controls)
            {
                // Il faut faire un cast du Tag au lieu d'un .toString() pour éviter un crash si le tag est null
                if ((string)control.Tag == EDIT_TAG)
                {
                    listCtrl.Add(control); // On ajoute le contrôle dans la liste si les tags correspondent

                    /* Note : 
                     *          On ne supprime pas les contrôles ici 
                     *          car l'index que parcourerait le foreach serait indirectement modifier
                     *          se qui pourrait empêcher certains contrôles d'être parcouru (et donc supprimer)
                     */
                }
            }

            foreach (Control ctrl in listCtrl)
            {
                // On supprime les contrôles de l'édition depuis la liste pour être sûr de bien tous les supprimer
                gbxInformation.Controls.Remove(ctrl); 
            }
            
        }

        /// <summary>
        /// Active ou désactive le bouton d'enregistrement de l'édition après avoir vérifier que les mots de passe sont identique ou qu'il n'a pas changé
        /// </summary>
        private void tbxPwd_TextChanged(object sender, EventArgs e)
        {
            // Si la confirmation du mdp est identique au mdp et si la longueur du mdp est > à 0
            // Ou si le mot de passe n'a pas changer
            // Alors on active le bouton d'enregistrement
            if ((tbxPwd.Text == tbxPwdConfirm.Text && tbxPwd.Text.Length > 0) || (_pwdHasChange == false))
            {
                btnSave.Enabled = true;
            }
            else
            {
                // Sinon on le désactive
                btnSave.Enabled = false;
            }
        }
        #endregion StateEdit
        #endregion EditManagement

        #region ControlsEvents
        #region TextboxPlaceholder

        /// <summary>
        /// (Dé)active le bouton d'ajout d'hobby lorsque le texte change
        /// </summary>
        private void tbxNewHobbie_TextChanged(object sender, EventArgs e)
        {
            // Active ou désactive le bouton d'ajout
            if (String.IsNullOrWhiteSpace(tbxNewHobbie.Text))
                _btnNewHobbies.Enabled = false;
            else
                _btnNewHobbies.Enabled = true;
        }

        /// <summary>
        /// Désactive le bouton d'ajout d'hobby lorsque le textbox perd le focus
        /// </summary>
        private void TbxNewHobbie_LostFocus(object sender, EventArgs e)
        {
            // Désactive le bouton d'ajout d'hobby si le textbox est vide (ou remplit d'espaces blancs)
            // On test si le texte correspond à celui du placeholder pour être sûr de désactiver le bouton même si l'event se produit trop tard
            if (String.IsNullOrWhiteSpace(tbxNewHobbie.Text) || tbxNewHobbie.Text == PLACEHOLDER_HOBBIES)
            {
                _btnNewHobbies.Enabled = false;
            }
        }

        #endregion TextboxPlaceholder

        /// <summary>
        /// Affichage de l'heure actuel
        /// </summary>
        private void tmrDate_Tick(object sender, EventArgs e)
        {
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            tsDate.Text = dt.ToString("dddd d MMMM yyyy - HH:m"); // ex : mercredi 11 octobre 2017 - 12:01
        }

        /// <summary>
        /// Filtrage des caractères
        /// </summary>
        private void tbxNewHobbie_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si l'utilisateur appuie sur Enter ET si le centre d'intérêt n'est pas vide
            if (e.KeyChar == (char)13 && tbxNewHobbie.Text != "")
                AddNewHobbie(); // Alors il ajoute le nouveau centre d'intérêt

            // N'autorise que les lettres et touche de controle
            if ((!char.IsLetter(e.KeyChar)) && (!char.IsControl(e.KeyChar)) && e.KeyChar == (char)20)
                e.Handled = true;
        }

        /// <summary>
        /// Redessine une PictureBox avec une bordure en traitillé (dashed)
        /// </summary>
        /// <param name="sender">Le PictureBox qui a appelé la méthode</param>
        /// <param name="e">Les données de l'événément du PictureBox</param>
        private void PaintPictureBox(object sender, PaintEventArgs e)
        {
            // Si il s'agit d'un Paint "manuelle"
            if (_rePaint)
            {
                PictureBox pbx = (PictureBox)sender;
                Design.DrawDashedBorder(pbx, e);
                _rePaint = false;
            }
        }

        #endregion ControlsEvents

        #region FriendsManagement

        /// <summary>
        /// Ouvre la fenêtre de requête d'amitié (FrmFriendInvitation)
        /// </summary>
        private void btnAddFriend_Click(object sender, EventArgs e)
        {
            FrmFriendInvitation frmFriendInvitation = new FrmFriendInvitation(_frmLogin, _requestsSQL);
            frmFriendInvitation.ShowDialog();

            if (frmFriendInvitation.DialogResult == DialogResult.OK)
            {
                // On récupère les ID des utilisateurs
                string idUserSender = _requestsSQL.GetUserIdByUsername(_frmLogin.Username);
                string idUserReceiver = _requestsSQL.GetUserIdByUsername(frmFriendInvitation.FriendRequest);

                string messageRequest = frmFriendInvitation.MessageRequest;

                // Test si l'utilisateur à qui on s'aprête à envoyé une demande d'amitié ne nous aurait il pas déjà envoyé une demande.
                if (_requestsSQL.CheckIfFriendRequestAlreadyExist(idUserSender, idUserReceiver))
                {
                    MessageBox.Show("Vous avez déjà reçu une demande d'amitié de la part de " + frmFriendInvitation.FriendRequest + ".\n\nSouhaitez-vous l'acceptez maintenant (en cas de refus elle restera \"en attente\") ?", "Demande d'amitié en attente", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    // TODO : Permettre à l'utilisateur d'accepter une demande d'amitié si il y en a une déjà en cours
                }
                else if (_requestsSQL.CreateFriendRequest(idUserSender, idUserReceiver, messageRequest))
                {
                    MessageBox.Show("Une demande d'amitié a été envoyé à " + frmFriendInvitation.FriendRequest, "Demande d'amitié envoyé", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (e.KeyChar == (char)13)
            {
                rtbConversation.Text += tbxMessage.Text + Environment.NewLine;
                tbxMessage.Text = String.Empty;
            }
        }
    }
}
