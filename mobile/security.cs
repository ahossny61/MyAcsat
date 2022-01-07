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

namespace mobile
{
    public partial class security : Form
    {
        SqlConnection conn;
        string connStr = @"server=.\Ahmed;database= mobile1; integrated security=SSPI;";
        Form1 f;
        public security()
        {
            InitializeComponent();
            conn = new SqlConnection(connStr);
            f = new Form1();
        }

        private void button1_Click(object sender, EventArgs e)
        {
      
            if ((Form1.tries==1 && textBox1.Text == "cSharp15@") || (Form1.tries == 2 && textBox1.Text == "cSharp180@@")||
                (Form1.tries == 3 && textBox1.Text == "cSharp365@@@")|| (Form1.tries == 4 && textBox1.Text == "cSharp500@@")|| 
                (Form1.tries == 5 && textBox1.Text == "cSharp1024@"))
            {
                try
                {
                    conn.Open();
                    SqlCommand ad = new SqlCommand("update setting set Days +=1 where id=1", conn);
                    ad.ExecuteNonQuery();
                    MessageBox.Show("تم الدخول بنجاح ...نعتذر لكم عن التأخير");
                   f.Show();
                    this.Hide();
                    conn.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(" 01100643170 الرجاء الاتصال بالمبرمج المسؤل حتى تتمكن من الدخول");
            }


        }

        private void security_Load(object sender, EventArgs e)
        {

        }
    }
}
