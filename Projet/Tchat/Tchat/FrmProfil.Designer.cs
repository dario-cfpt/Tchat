namespace Tchat
{
    partial class FrmProfil
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblProfilUsername = new System.Windows.Forms.Label();
            this.pbxProfilAvatar = new System.Windows.Forms.PictureBox();
            this.pbxProfilBackground = new System.Windows.Forms.PictureBox();
            this.rtbProfilDescription = new System.Windows.Forms.RichTextBox();
            this.lbxProfilHobbies = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxProfilPhone = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxProfilEmail = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbxProfilAvatar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxProfilBackground)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblProfilUsername
            // 
            this.lblProfilUsername.AutoSize = true;
            this.lblProfilUsername.Location = new System.Drawing.Point(466, 175);
            this.lblProfilUsername.Name = "lblProfilUsername";
            this.lblProfilUsername.Size = new System.Drawing.Size(53, 13);
            this.lblProfilUsername.TabIndex = 22;
            this.lblProfilUsername.Text = "Utilisateur";
            // 
            // pbxProfilAvatar
            // 
            this.pbxProfilAvatar.Image = global::Tchat.Properties.Resources.default_avatar;
            this.pbxProfilAvatar.Location = new System.Drawing.Point(431, 44);
            this.pbxProfilAvatar.Name = "pbxProfilAvatar";
            this.pbxProfilAvatar.Size = new System.Drawing.Size(128, 128);
            this.pbxProfilAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxProfilAvatar.TabIndex = 21;
            this.pbxProfilAvatar.TabStop = false;
            // 
            // pbxProfilBackground
            // 
            this.pbxProfilBackground.Location = new System.Drawing.Point(12, 12);
            this.pbxProfilBackground.Name = "pbxProfilBackground";
            this.pbxProfilBackground.Size = new System.Drawing.Size(956, 205);
            this.pbxProfilBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxProfilBackground.TabIndex = 20;
            this.pbxProfilBackground.TabStop = false;
            // 
            // rtbProfilDescription
            // 
            this.rtbProfilDescription.AcceptsTab = true;
            this.rtbProfilDescription.EnableAutoDragDrop = true;
            this.rtbProfilDescription.Location = new System.Drawing.Point(12, 247);
            this.rtbProfilDescription.Name = "rtbProfilDescription";
            this.rtbProfilDescription.Size = new System.Drawing.Size(956, 102);
            this.rtbProfilDescription.TabIndex = 19;
            this.rtbProfilDescription.Text = "";
            // 
            // lbxProfilHobbies
            // 
            this.lbxProfilHobbies.FormattingEnabled = true;
            this.lbxProfilHobbies.Location = new System.Drawing.Point(161, 108);
            this.lbxProfilHobbies.Name = "lbxProfilHobbies";
            this.lbxProfilHobbies.Size = new System.Drawing.Size(219, 56);
            this.lbxProfilHobbies.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Centres d\'intérêt :";
            // 
            // tbxProfilPhone
            // 
            this.tbxProfilPhone.Location = new System.Drawing.Point(161, 82);
            this.tbxProfilPhone.Name = "tbxProfilPhone";
            this.tbxProfilPhone.Size = new System.Drawing.Size(219, 20);
            this.tbxProfilPhone.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Téléphone :";
            // 
            // tbxProfilEmail
            // 
            this.tbxProfilEmail.Location = new System.Drawing.Point(161, 56);
            this.tbxProfilEmail.Name = "tbxProfilEmail";
            this.tbxProfilEmail.Size = new System.Drawing.Size(219, 20);
            this.tbxProfilEmail.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Email :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Location = new System.Drawing.Point(12, 355);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(956, 245);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Information sur :";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.tbxProfilEmail);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.tbxProfilPhone);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.lbxProfilHobbies);
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(944, 237);
            this.panel1.TabIndex = 10;
            // 
            // FrmProfil
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 612);
            this.Controls.Add(this.lblProfilUsername);
            this.Controls.Add(this.pbxProfilAvatar);
            this.Controls.Add(this.pbxProfilBackground);
            this.Controls.Add(this.rtbProfilDescription);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FrmProfil";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "My TchatRoom - Profil";
            ((System.ComponentModel.ISupportInitialize)(this.pbxProfilAvatar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxProfilBackground)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProfilUsername;
        private System.Windows.Forms.PictureBox pbxProfilAvatar;
        private System.Windows.Forms.PictureBox pbxProfilBackground;
        private System.Windows.Forms.RichTextBox rtbProfilDescription;
        private System.Windows.Forms.ListBox lbxProfilHobbies;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxProfilPhone;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxProfilEmail;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel1;
    }
}