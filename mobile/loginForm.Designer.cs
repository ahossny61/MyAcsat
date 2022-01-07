namespace mobile
{
    partial class Form1
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
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.login_password = new System.Windows.Forms.TextBox();
            this.login_username = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_importDB = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 25.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(218, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(536, 61);
            this.label3.TabIndex = 16;
            this.label3.Text = "مؤسسة اليسر";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Highlight;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Chiller", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.button1.Location = new System.Drawing.Point(176, 274);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 43);
            this.button1.TabIndex = 15;
            this.button1.Text = "login";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            this.button1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button1_KeyDown);
            // 
            // login_password
            // 
            this.login_password.BackColor = System.Drawing.SystemColors.Window;
            this.login_password.Font = new System.Drawing.Font("Bodoni MT", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.login_password.ForeColor = System.Drawing.Color.Gray;
            this.login_password.Location = new System.Drawing.Point(93, 198);
            this.login_password.Name = "login_password";
            this.login_password.PasswordChar = '*';
            this.login_password.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.login_password.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.login_password.Size = new System.Drawing.Size(324, 35);
            this.login_password.TabIndex = 12;
            this.login_password.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.login_password.KeyDown += new System.Windows.Forms.KeyEventHandler(this.login_password_KeyDown);
            // 
            // login_username
            // 
            this.login_username.BackColor = System.Drawing.SystemColors.Window;
            this.login_username.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.login_username.Font = new System.Drawing.Font("Comic Sans MS", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.login_username.ForeColor = System.Drawing.Color.Gray;
            this.login_username.Location = new System.Drawing.Point(93, 107);
            this.login_username.Name = "login_username";
            this.login_username.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.login_username.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.login_username.Size = new System.Drawing.Size(324, 40);
            this.login_username.TabIndex = 11;
            this.login_username.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.login_username.KeyDown += new System.Windows.Forms.KeyEventHandler(this.login_username_KeyDown);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.btn_importDB);
            this.panel1.Controls.Add(this.login_username);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.login_password);
            this.panel1.Location = new System.Drawing.Point(241, 128);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(492, 387);
            this.panel1.TabIndex = 17;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(0, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(972, 550);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btn_importDB
            // 
            this.btn_importDB.BackColor = System.Drawing.SystemColors.Highlight;
            this.btn_importDB.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_importDB.Font = new System.Drawing.Font("Chiller", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_importDB.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btn_importDB.Location = new System.Drawing.Point(176, 332);
            this.btn_importDB.Name = "btn_importDB";
            this.btn_importDB.Size = new System.Drawing.Size(140, 43);
            this.btn_importDB.TabIndex = 16;
            this.btn_importDB.Text = "Import DB";
            this.btn_importDB.UseVisualStyleBackColor = false;
            this.btn_importDB.Click += new System.EventHandler(this.btn_importDB_Click);
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(975, 552);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "تسجيل الدخول";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox login_password;
        private System.Windows.Forms.TextBox login_username;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_importDB;
    }
}

