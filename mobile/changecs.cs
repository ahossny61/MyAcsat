using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace mobile
{
    public partial class changecs : Form
    {
        MySqlConnection conn;
        string connStr = @"server=127.0.0.1;database= mobile_shop;uid=root;password=rootroot;";
        public changecs()
        {
            InitializeComponent();
            conn = new MySqlConnection(connStr);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string old_pass = login_Oldpassword.Text;
            string use = login_username.Text;
            string pass = login_password.Text;
            MySqlDataAdapter ad = new MySqlDataAdapter("select * from setting ", conn);
            DataTable tb = new DataTable();
            ad.Fill(tb);


            if (tb.Rows.Count > 0)
            {
                if (old_pass == tb.Rows[0]["password"].ToString())
                {
                    try
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("update setting set userName = '" + use+"' , password = '"+pass+"' where id=1", conn);
                      //  cmd.Parameters.AddWithValue("@s1", user);
                        //cmd.Parameters.AddWithValue("@s2", pass);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم تعديل البيانات بنجاح");
                        new Form1().Show();
                        this.Hide();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else {
                    MessageBox.Show("الرقم السرى القديم غير صحيح");
                    }
                }
            }

        private void changecs_Load(object sender, EventArgs e)
        {

        }

        private void login_Oldpassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void changecs_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
    }

