using MySql.Data.MySqlClient;
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
using System.Management;
using MySql.Data.MySqlClient;


namespace mobile
{
    public partial class Form1 : Form
    {
        int r = 0;
        Random rand = new Random();
        public static int tries = 0;
        string user, pass;
        MySqlConnection conn;
        string connStr = @"server=127.0.0.1;database= mobile_shop;uid=root;password=rootroot;";


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            string manuf = "", product = "", serial = "";
            manuf = systemInfo.Manufacturer;
            product = systemInfo.Product;
            serial = systemInfo.SerialNumber;
            //manuf == "Dell Inc." && product == "0MYF02" && serial == "/5J794X1/CN1296135A05C8/"
            if ((manuf == "Hewlett-Packard" && product == "1850" && serial == "CZC339B5FW"))
            {
                r = rand.Next(0, 62);
                panel1.BackgroundImage = Image.FromFile("l" + r + ".jpeg");
                pictureBox1.Image = Image.FromFile("l" + r + ".jpeg");
                conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();
                    /* SqlCommand c = new SqlCommand("create database mobile1 ", conn);
                     c.ExecuteNonQuery();
                     c = new SqlCommand("use mobile1  ", conn);
                     c.ExecuteNonQuery();
                     conn.Close();*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            else
            {
                MessageBox.Show("لقد تم تغير الجهاز");
                Application.Exit();
            }



        }


        private void button1_Click_1(object sender, EventArgs e)
        {
           
            try
            {
                user = login_username.Text;
                pass = login_password.Text;
                // MessageBox.Show(user + " " + pass);
                MySqlDataAdapter ad = new MySqlDataAdapter("select * from setting ", conn);
                DataTable tb = new DataTable();
                ad.Fill(tb);
                if (tb.Rows.Count > 0)
                {
                    if (user == tb.Rows[0]["userName"].ToString() && pass == tb.Rows[0]["password"].ToString())
                    {
                        
                        mainForm f = new mainForm();
                        f.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("رقم غير صحيح");
                    }
                }
                else
                {
                    MessageBox.Show(" لا يوجد اى حساب ");
                }
                conn.Close();
            }
            catch (SqlException ex)
            {

                MessageBox.Show(ex.Message);

            }
        }

        private void login_username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                this.SelectNextControl((Control)sender, false, true, true, true);
            }
            else if (e.KeyCode == Keys.Down)
            {
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void login_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                this.SelectNextControl((Control)sender, false, true, true, true);
            }
            else if (e.KeyCode == Keys.Down)
            {
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                this.SelectNextControl((Control)sender, false, true, true, true);
            }
            else if (e.KeyCode == Keys.Down)
            {
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btn_importDB_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "sql|*.sql";
            DialogResult res = of.ShowDialog();
            if (res == DialogResult.OK)
            {
                try
                {
                    conn.Close();
                    using (MySqlConnection con = new MySqlConnection(connStr))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = con;
                                con.Open();
                                mb.ImportFromFile(of.FileName);
                                con.Close();
                            }
                        }
                    }
                    /*cmd = new MySqlCommand("use master", conn);
                    cmd.ExecuteNonQuery();
                    cmd = new MySqlCommand("RESTORE DATABASE mobile1 FROM DISK = '" + of.FileName + "'", conn);
                    cmd.ExecuteNonQuery();
                    cmd = new MySqlCommand("use mobile1", conn);
                    cmd.ExecuteNonQuery();*/
                    conn.Open();
                    MessageBox.Show("تمت الاستعاده بنجاح");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
        }

        public Form1()
        {
            InitializeComponent();
        }
    }
}
