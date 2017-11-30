namespace Tchat
{
    partial class FrmRegister
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxUserName = new System.Windows.Forms.TextBox();
            this.tbxPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxPasswordConfirm = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxEmail = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxPhone = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.lnkAlreadyAccount = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(125, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "INSCRIPTION";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Pseudo* :";
            // 
            // tbxUserName
            // 
            this.tbxUserName.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.tbxUserName.Location = new System.Drawing.Point(203, 75);
            this.tbxUserName.Name = "tbxUserName";
            this.tbxUserName.ShortcutsEnabled = false;
            this.tbxUserName.Size = new System.Drawing.Size(205, 20);
            this.tbxUserName.TabIndex = 2;
            this.tbxUserName.TextChanged += new System.EventHandler(this.tbxUserName_TextChanged);
            this.tbxUserName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxUserName_KeyPress);
            // 
            // tbxPassword
            // 
            this.tbxPassword.Location = new System.Drawing.Point(203, 115);
            this.tbxPassword.Name = "tbxPassword";
            this.tbxPassword.ShortcutsEnabled = false;
            this.tbxPassword.Size = new System.Drawing.Size(205, 20);
            this.tbxPassword.TabIndex = 4;
            this.tbxPassword.UseSystemPasswordChar = true;
            this.tbxPassword.TextChanged += new System.EventHandler(this.tbxUserName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Mot de passe* :";
            // 
            // tbxPasswordConfirm
            // 
            this.tbxPasswordConfirm.Location = new System.Drawing.Point(203, 151);
            this.tbxPasswordConfirm.Name = "tbxPasswordConfirm";
            this.tbxPasswordConfirm.ShortcutsEnabled = false;
            this.tbxPasswordConfirm.Size = new System.Drawing.Size(205, 20);
            this.tbxPasswordConfirm.TabIndex = 6;
            this.tbxPasswordConfirm.UseSystemPasswordChar = true;
            this.tbxPasswordConfirm.TextChanged += new System.EventHandler(this.tbxUserName_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Confirmation du mot de passe* :";
            // 
            // tbxEmail
            // 
            this.tbxEmail.Location = new System.Drawing.Point(203, 185);
            this.tbxEmail.Name = "tbxEmail";
            this.tbxEmail.ShortcutsEnabled = false;
            this.tbxEmail.Size = new System.Drawing.Size(205, 20);
            this.tbxEmail.TabIndex = 8;
            this.tbxEmail.TextChanged += new System.EventHandler(this.tbxUserName_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 192);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Email* :";
            // 
            // tbxPhone
            // 
            this.tbxPhone.Location = new System.Drawing.Point(203, 225);
            this.tbxPhone.Name = "tbxPhone";
            this.tbxPhone.ShortcutsEnabled = false;
            this.tbxPhone.Size = new System.Drawing.Size(205, 20);
            this.tbxPhone.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 228);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Téléphone :";
            // 
            // btnRegister
            // 
            this.btnRegister.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnRegister.Enabled = false;
            this.btnRegister.Location = new System.Drawing.Point(312, 294);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(96, 23);
            this.btnRegister.TabIndex = 12;
            this.btnRegister.Text = "Inscription";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // lnkAlreadyAccount
            // 
            this.lnkAlreadyAccount.AutoSize = true;
            this.lnkAlreadyAccount.Location = new System.Drawing.Point(41, 299);
            this.lnkAlreadyAccount.Name = "lnkAlreadyAccount";
            this.lnkAlreadyAccount.Size = new System.Drawing.Size(142, 13);
            this.lnkAlreadyAccount.TabIndex = 13;
            this.lnkAlreadyAccount.TabStop = true;
            this.lnkAlreadyAccount.Text = "Vous avez déjà un compte ?";
            this.lnkAlreadyAccount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAlreadyAccount_LinkClicked);
            // 
            // FrmRegister
            // 
            this.AcceptButton = this.btnRegister;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 326);
            this.Controls.Add(this.lnkAlreadyAccount);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.tbxPhone);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbxEmail);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbxPasswordConfirm);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbxPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbxUserName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FrmRegister";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inscription";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxUserName;
        private System.Windows.Forms.TextBox tbxPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxPasswordConfirm;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxEmail;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxPhone;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.LinkLabel lnkAlreadyAccount;
    }
}