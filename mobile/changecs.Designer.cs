namespace mobile
{
    partial class changecs
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
            this.label1 = new System.Windows.Forms.Label();
            this.login_Oldpassword = new System.Windows.Forms.TextBox();
            this.login_username = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.login_password = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.AliceBlue;
            this.label3.Location = new System.Drawing.Point(362, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(217, 91);
            this.label3.TabIndex = 14;
            this.label3.Text = "حسابى";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(4)))), ((int)(((byte)(74)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Chiller", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.button1.Location = new System.Drawing.Point(311, 420);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(324, 43);
            this.button1.TabIndex = 13;
            this.button1.Text = "حفظ التغيرات";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Chiller", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label1.Location = new System.Drawing.Point(305, 210);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(205, 34);
            this.label1.TabIndex = 11;
            this.label1.Text = "اسم المستخدم الجديد";
            // 
            // login_Oldpassword
            // 
            this.login_Oldpassword.BackColor = System.Drawing.Color.Gainsboro;
            this.login_Oldpassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.login_Oldpassword.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.login_Oldpassword.ForeColor = System.Drawing.Color.Gray;
            this.login_Oldpassword.Location = new System.Drawing.Point(309, 158);
            this.login_Oldpassword.Name = "login_Oldpassword";
            this.login_Oldpassword.PasswordChar = '*';
            this.login_Oldpassword.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.login_Oldpassword.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.login_Oldpassword.Size = new System.Drawing.Size(324, 49);
            this.login_Oldpassword.TabIndex = 9;
            this.login_Oldpassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // login_username
            // 
            this.login_username.BackColor = System.Drawing.Color.Gainsboro;
            this.login_username.Font = new System.Drawing.Font("Bodoni MT", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.login_username.ForeColor = System.Drawing.Color.Gray;
            this.login_username.Location = new System.Drawing.Point(309, 247);
            this.login_username.Name = "login_username";
            this.login_username.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.login_username.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.login_username.Size = new System.Drawing.Size(324, 43);
            this.login_username.TabIndex = 10;
            this.login_username.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.login_username.TextChanged += new System.EventHandler(this.login_Oldpassword_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Chiller", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Gainsboro;
            this.label2.Location = new System.Drawing.Point(305, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 34);
            this.label2.TabIndex = 12;
            this.label2.Text = "الرقم السرى القديم";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Chiller", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Gainsboro;
            this.label4.Location = new System.Drawing.Point(313, 299);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 34);
            this.label4.TabIndex = 16;
            this.label4.Text = "الرقم السرى الجديد";
            // 
            // login_password
            // 
            this.login_password.BackColor = System.Drawing.Color.Gainsboro;
            this.login_password.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.login_password.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.login_password.ForeColor = System.Drawing.Color.Gray;
            this.login_password.Location = new System.Drawing.Point(309, 335);
            this.login_password.Name = "login_password";
            this.login_password.PasswordChar = '*';
            this.login_password.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.login_password.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.login_password.Size = new System.Drawing.Size(324, 49);
            this.login_password.TabIndex = 15;
            this.login_password.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // changecs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(962, 517);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.login_password);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.login_username);
            this.Controls.Add(this.login_Oldpassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "changecs";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "تعديل الحساب";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.changecs_FormClosing);
            this.Load += new System.EventHandler(this.changecs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox login_Oldpassword;
        private System.Windows.Forms.TextBox login_username;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox login_password;
    }
}