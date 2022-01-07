using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.IO;

namespace mobile
{
    public partial class mainForm : Form
    {
        float last_month = 0;
        string connStr = @"server=127.0.0.1;database= mobile_shop;uid=root;password=rootroot;";
        DataTable dt;
        DataTable dt_detail;
        DataTable dt_late;
        DataTable dt_paid;
        MySqlDataAdapter adapter;
        MySqlConnection conn;
        BindingSource bs = new BindingSource();
        MySqlCommand cmd;
        List<string> detail_names;
        List<int> detail_id;
        List<int> late_id;
        List<int> paid_id;
        int paid_cid;
        DataTable monthsTable;
        int monthsNumber;
        DataRow clientRow;
        int cashNumber;
        int cashRemain;
        bool savePrint;
        string bill_clineName;
        float total_money;
        public mainForm()
        {
            InitializeComponent();
            conn = new MySqlConnection(connStr);
            conn.Open();
            //MessageBox.Show("تم التشغيل بنجاح");
            detail_names = new List<string>();
            detail_id = new List<int>();
            late_id = new List<int>();
            dt = new DataTable();
            paid_id = new List<int>();
            savePrint = false;
            bs.AllowNew = true;
            setInVisible();
            setInVisible_paid();
            checkDay(2);
            adapter = new MySqlDataAdapter("select * from mynote where id=1", conn);
            DataTable tb = new DataTable();
            adapter.Fill(tb);
            txt_mynotes.Text = tb.Rows[0]["body"].ToString();
            //set();
            total_money = 0; 
        }

        void set()
        {
            adapter = new MySqlDataAdapter("select * from  client ", conn);
            MySqlCommandBuilder cb2 = new MySqlCommandBuilder();
            cb2.DataAdapter = adapter;
            dt = new DataTable();
            adapter.Fill(dt);

            int length = dt.Rows.Count;

            for (int i = 0; i < length; i++)
            {
                DataRow row = dt.Rows[i];
                int id = int.Parse(row["id"].ToString());

                cmd = new MySqlCommand("select count(*) from months where months.cid = " + id + " and months.paid = 'true'", conn);
                int months_paid = int.Parse(cmd.ExecuteScalar().ToString());

                cmd = new MySqlCommand("select count(*) from months where months.cid = " + id + " and months.paid = 'false'", conn);
                int months_remain = int.Parse(cmd.ExecuteScalar().ToString());

                cmd = new MySqlCommand("update client set cashNum = " + months_paid + " , cashRemain= " + months_remain + " where id =" + id + "", conn);
                cmd.ExecuteNonQuery();

               // MessageBox.Show(id + " " + months_paid + " " + months_remain);

            }
        }
        void checkDay(int x)
        {
            try
            {
                DateTime current = DateTime.Now;
                int currentDay = int.Parse(current.Day.ToString());
                int currentMonth = int.Parse(current.Month.ToString());
                int currentyear = int.Parse(current.Year.ToString());

                
                adapter = new MySqlDataAdapter("select * from setting", conn);
                MySqlCommandBuilder cb = new MySqlCommandBuilder();
                cb.DataAdapter = adapter;
                dt = new DataTable();
                adapter.Fill(dt);

                int last_year = int.Parse(dt.Rows[0]["lastYear"].ToString());
                int lasy_month = int.Parse(dt.Rows[0]["lastMonth"].ToString());
                int last_day = int.Parse(dt.Rows[0]["lastDay"].ToString());


                if (currentyear == last_year && currentMonth == lasy_month && currentDay == last_day && x == 2)
                {

                }
                else
                {
                    if (x != 1)
                    {
                        string[] messages = { "صباح الفل ياعمنا","صباح الفل يا باشا ايوا كدا روق واخرب الدنيا",
                "الله ينور "
                ,"صلى على الحبيب المصطفي "
                ,"بسم الله توكلنا على الله ...ربنا يوسع عليك يا صاحبى"
                ,"بسم الله ما شاء الله اللهم تبارك"
                ,"صباح الورد عليك سمى الله واتوكل عليه هتتستر "
                ,"ربنا يصبرك على الزباين اللى دماغها مقفله "
                ,"احلى مسا عليك والله "
                ,"الله يقويك"
                ,"صباح السعاده يا فنان"
                ,"الاهلى فوق الجميع "
                ,"متكسلش وسجل الاقساط اول باول "
                ,"صباح الفل والروقان "
                ,"بسم الله الرحمن الرحيم "
                ,"بالتوفيق يا غالى "
                ,"دعواتك ههههههههه"
            };
                        Random rand = new Random();
                        int num = rand.Next(0, 16);
                        string message = messages[num];
                        MessageBox.Show(message);
                        
                        // MessageBox.Show(" جارى الأن حفظ النسخه الإحتياطية ");
                        string folderPath = "F:\\MOBILE\\القسط";
                        //MessageBox.Show(folderPath + "\\oil_backup" + DateTime.Now.ToString("yyyy,mm,dd-HH,mm,ss") + ".sql");
                        conn.Close();
                        using (MySqlConnection conn = new MySqlConnection("server=localhost;database=mobile_shop;uid=root;pwd=rootroot"))
                        {
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                using (MySqlBackup mb = new MySqlBackup(cmd))
                                {
                                    cmd.Connection = conn;
                                    conn.Open();
                                    mb.ExportToFile(folderPath + "\\mobile_backup" + DateTime.Now.ToString("yyyy,MM,dd-HH,mm,ss") + ".sql");
                                    conn.Close();
                                    MessageBox.Show("صباح الفل ربنا يوسع رزقك ..... تم انشاء نسخة احتياطية بنجاح الرجاء التأكد من الاتصال بالانترنت شكرا لك ");
                                }
                            }
                        }
                        conn.Open();

                        
                    }
                    MessageBox.Show("جارى الان التحقق من العملاء لايجاد المتأخرون عن القسط");
                    //MessageBox.Show("قد تستغرق العمليه عده دقائق الرجاء الانتظار حتى الانتهاء");

                    if (x == 2)
                    {


                        cmd = new MySqlCommand("update setting set lastYear=@s1 ,lastMonth=@s2 ,lastDay=@s3  where id=1", conn);
                        cmd.Parameters.AddWithValue("@s1", currentyear);
                        cmd.Parameters.AddWithValue("@s2", currentMonth);
                        cmd.Parameters.AddWithValue("@s3", currentDay);
                        cmd.ExecuteNonQuery();
                    }


                    adapter = new MySqlDataAdapter("select * from  client where  late=0 and cashRemain!=0", conn);
                    MySqlCommandBuilder cb2 = new MySqlCommandBuilder();
                    cb2.DataAdapter = adapter;
                    dt = new DataTable();
                    adapter.Fill(dt);

                    int length = dt.Rows.Count;
                    MySqlDataAdapter ad_year;
                    DataTable tt;
                    MySqlDataAdapter ad;
                    DataTable dt2;
                    for (int i = 0; i < length; i++)
                    {
                        DataRow row = dt.Rows[i];
                        int id = int.Parse(row["id"].ToString());
                        ad_year = new MySqlDataAdapter("select year  from months where  months.cid=" + id + " and months.paid='true' order by months.year DESC", conn);
                        tt = new DataTable();
                        ad_year.Fill(tt);

                        if (tt.Rows.Count > 0)
                        {
                            ad = new MySqlDataAdapter("select DateTime from months where months.cid=" + id + " and months.paid='true' and months.year= '" + tt.Rows[0]["year"].ToString() + "' order by months.monthNumber DESC", conn);
                            dt2 = new DataTable();
                            ad.Fill(dt2);
                            //MessageBox.Show("hi");
                            if (dt2.Rows.Count > 0)
                            {

                                DateTime date = DateTime.Parse(dt2.Rows[0]["DateTime"].ToString());
                                last_year = int.Parse(date.Year.ToString());
                                lasy_month = int.Parse(date.Month.ToString());
                                last_day = int.Parse(date.Day.ToString());
                                //MessageBox.Show(id+" "+last_year + " " + lasy_month + " " + last_day);
                                if (currentyear - last_year > 1 || (currentyear == last_year && currentMonth - lasy_month == 2 && currentDay > last_day) || (currentyear == last_year + 1 && currentMonth == 2 && lasy_month == 12 && currentDay > last_day) || (currentyear == last_year + 1 && currentMonth != 2 && lasy_month != 12) || (currentyear == last_year + 1 && currentMonth == 2 && lasy_month != 12) || (currentyear == last_year + 1 && currentMonth > 2 && lasy_month == 12) || (currentyear == last_year && currentMonth - lasy_month > 2))
                                {
                                    cmd = new MySqlCommand("Update client set late=1 where id=" + id + "", conn);
                                    cmd.ExecuteNonQuery();

                                    if (id == 5342)
                                    {
                                        MessageBox.Show("hi");
                                    }
                                }
                            }
                            else if (dt2.Rows.Count == 0)
                            {
                                /* MySqlDataAdapter ad_year = new MySqlDataAdapter("select year  from months where  months.cid=" + id + " and months.paid='false' order by months.year ASC", conn);
                                 DataTable tt = new DataTable();
                                 ad_year.Fill(tt);
                                 */

                            }
                            else if (tt.Rows.Count == 0)
                            {
                                cmd = new MySqlCommand("select year  from months where  months.cid=" + id + " and months.paid='false' order by months.year ASC ", conn);
                                int year = int.Parse(cmd.ExecuteScalar().ToString());

                                cmd = new MySqlCommand("select DateTime from months where months.cid = " + id + " and months.paid = 'false' and months.year = '" + year.ToString() + "' order by months.monthNumber ASC", conn);
                                DateTime start = DateTime.Parse(cmd.ExecuteScalar().ToString());
                                last_year = int.Parse(start.Year.ToString());
                                lasy_month = int.Parse(start.Month.ToString());
                                last_day = int.Parse(start.Day.ToString());
                                //MessageBox.Show(id + " " + last_year + " " + lasy_month + " " + last_day);
                                if (currentyear - last_year > 1 || currentMonth - lasy_month > 1 || (currentyear == last_year && currentMonth - lasy_month == 1 && currentDay > last_day) || (currentyear == last_year + 1 && currentMonth == 1 && lasy_month == 12 && currentDay > last_day) || (currentyear == last_year + 1 && currentMonth != 1 && lasy_month != 12) || (currentyear == last_year + 1 && currentMonth == 1 && lasy_month != 12) || (currentyear == last_year + 1 && currentMonth != 1 && lasy_month == 12))
                                {
                                    cmd = new MySqlCommand("Update client set late=1 where id=" + id + "", conn);
                                    cmd.ExecuteNonQuery();
                                    if (id == 5342)
                                    {
                                        MessageBox.Show("hi2");
                                    }
                                }
                                

                            }


                        }
                    }

                        if (x == 2)
                            findLate();

                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        void findLate()
        {
            try
            {
                list_late.Items.Clear();
                late_id.Clear();            
                adapter = new MySqlDataAdapter("select * from client where late=1 and cashRemain!=0", conn);
                MySqlCommandBuilder cb2 = new MySqlCommandBuilder();
                cb2.DataAdapter = adapter;
                dt_late = new DataTable();
                adapter.Fill(dt_late);
                //MessageBox.Show("hi");
                dt_late.Constraints.Add("id", dt_late.Columns[17], true);
                if (dt_late.Rows.Count == 0)
                {
                    MessageBox.Show("لا يوجد عملاء متأخرين حتى الان");
                }
                else
                {
                    cmd = new MySqlCommand("select count(*) from client", conn);
                    float total_num = int.Parse(cmd.ExecuteScalar().ToString());
                    float rate = (dt_late.Rows.Count / (float)total_num) * 100.0f;
                    label_lateRate.Text = ((int)rate).ToString() + " %";
                    //MessageBox.Show(total_num + " " + dt_late.Rows.Count);
                    for (int i = 0; i < dt_late.Rows.Count; i++)
                    {
                        list_late.Items.Add(dt_late.Rows[i]["name"].ToString());
                        late_id.Add(int.Parse(dt_late.Rows[i]["id"].ToString()));
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void findhasA()
        {
            try
            {
                list_late.Items.Clear();
                late_id.Clear();
                adapter = new MySqlDataAdapter("select * from client where hasA=1", conn);
                MySqlCommandBuilder cb2 = new MySqlCommandBuilder();
                cb2.DataAdapter = adapter;
                dt_late = new DataTable();
                adapter.Fill(dt_late);
                dt_late.Constraints.Add("id", dt_late.Columns[17], true);
                if (dt_late.Rows.Count == 0)
                {
                    MessageBox.Show("لا يوجد عملاء عليهم قضايا");
                }
                else
                {
                    for (int i = 0; i < dt_late.Rows.Count; i++)
                    {
                        list_late.Items.Add(dt_late.Rows[i]["name"].ToString());
                        late_id.Add(int.Parse(dt_late.Rows[i]["id"].ToString()));
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void setInVisible()
        {
            adapter = new MySqlDataAdapter("select * from mynote where id=1", conn);
            DataTable tb = new DataTable();
            adapter.Fill(tb);
            txt_mynotes.Text = tb.Rows[0]["body"].ToString();
            txt_detailName.Enabled = false;
             txt_detailMonthNum.Enabled = false;
             txt_detailmonthsPaid.Enabled = false;
             txt_detailDaphter.Enabled = false;
             txt_detailAddress.Enabled = false;
             txt_detailID.Enabled = false;
             txt_detailMonthValue.Enabled = false;
             txt_detailmonthsReamain.Enabled = false;
             txt_detailPhone.Enabled = false;
             txt_detailWatcher.Enabled = false;
             txt_detailWatcherPhone.Enabled = false;
             txt_detailProductPrice.Enabled = false;
             txt_detailProductName.Enabled = false;
             txt_detailNotes.Enabled = false;
             txt_detailDateStart.Enabled = false;
             txt_detailFirstCash.Enabled = false;
            txt_detailhasA.Enabled = false;

             //disable

             late_y1.Visible = false;
             late_y2.Visible = false;
             late_y3.Visible = false;
             late_y4.Visible = false;
             late_y5.Visible = false;
             late_y6.Visible = false;
             late_y7.Visible = false;
             late_y8.Visible = false;
             late_y9.Visible = false;
             late_y10.Visible = false;
             late_y11.Visible = false;
             late_y12.Visible = false;
             late_y13.Visible = false;
             late_y14.Visible = false;
             late_y15.Visible = false;
             late_y16.Visible = false;
             late_y17.Visible = false;
             late_y18.Visible = false;
             late_y19.Visible = false;
             late_y20.Visible = false;
             late_y21.Visible = false;
             late_y22.Visible = false;
             late_y23.Visible = false;
             late_y24.Visible = false;

             late_m1.Visible = false;
             late_m2.Visible = false;
             late_m3.Visible = false;
             late_m4.Visible = false;
             late_m5.Visible = false;
             late_m6.Visible = false;
             late_m7.Visible = false;
             late_m8.Visible = false;
             late_m9.Visible = false;
             late_m10.Visible = false;
             late_m11.Visible = false;
             late_m12.Visible = false;
             late_m13.Visible = false;
             late_m14.Visible = false;
             late_m15.Visible = false;
             late_m16.Visible = false;
             late_m17.Visible = false;
             late_m18.Visible = false;
             late_m19.Visible = false;
             late_m20.Visible = false;
             late_m21.Visible = false;
             late_m22.Visible = false;
             late_m23.Visible = false;
             late_m24.Visible = false;

             late_value1.Visible = false;
             late_value2.Visible = false;
             late_value3.Visible = false;
             late_value4.Visible = false;
             late_value5.Visible = false;
             late_value6.Visible = false;
             late_value7.Visible = false;
             late_value8.Visible = false;
             late_value9.Visible = false;
             late_value10.Visible = false;
             late_value11.Visible = false;
             late_value12.Visible = false;
             late_value13.Visible = false;
             late_value14.Visible = false;
             late_value15.Visible = false;
             late_value16.Visible = false;
             late_value17.Visible = false;
             late_value18.Visible = false;
             late_value19.Visible = false;
             late_value20.Visible = false;
             late_value21.Visible = false;
             late_value22.Visible = false;
             late_value23.Visible = false;
             late_value24.Visible = false;

             late_mName1.Visible = false;
             late_mName2.Visible = false;
             late_mName3.Visible = false;
             late_mName4.Visible = false;
             late_mName5.Visible = false;
             late_mName6.Visible = false;
             late_mName7.Visible = false;
             late_mName8.Visible = false;
             late_mName9.Visible = false;
             late_mName10.Visible = false;
             late_mName11.Visible = false;
             late_mName12.Visible = false;
             late_mName13.Visible = false;
             late_mName14.Visible = false;
             late_mName15.Visible = false;
             late_mName16.Visible = false;
             late_mName17.Visible = false;
             late_mName18.Visible = false;
             late_mName19.Visible = false;
             late_mName20.Visible = false;
             late_mName21.Visible = false;
             late_mName22.Visible = false;
             late_mName23.Visible = false;
             late_mName24.Visible = false;


             late_m1.ForeColor = Color.Black;
             late_y1.ForeColor = Color.Black;
             late_mName1.ForeColor = Color.Black;
             late_value1.ForeColor = Color.Black;

             late_m2.ForeColor = Color.Black;
             late_y2.ForeColor = Color.Black;
             late_mName2.ForeColor = Color.Black;
             late_value2.ForeColor = Color.Black;

             late_m3.ForeColor = Color.Black;
             late_y3.ForeColor = Color.Black;
             late_mName3.ForeColor = Color.Black;
             late_value3.ForeColor = Color.Black;

             late_m4.ForeColor = Color.Black;
             late_y4.ForeColor = Color.Black;
             late_mName4.ForeColor = Color.Black;
             late_value4.ForeColor = Color.Black;

             late_m5.ForeColor = Color.Black;
             late_y5.ForeColor = Color.Black;
             late_mName5.ForeColor = Color.Black;
             late_value5.ForeColor = Color.Black;

             late_m6.ForeColor = Color.Black;
             late_y6.ForeColor = Color.Black;
             late_mName6.ForeColor = Color.Black;
             late_value6.ForeColor = Color.Black;

             late_m7.ForeColor = Color.Black;
             late_y7.ForeColor = Color.Black;
             late_mName7.ForeColor = Color.Black;
             late_value7.ForeColor = Color.Black;

             late_m8.ForeColor = Color.Black;
             late_y8.ForeColor = Color.Black;
             late_mName8.ForeColor = Color.Black;
             late_value8.ForeColor = Color.Black;

             late_m9.ForeColor = Color.Black;
             late_y9.ForeColor = Color.Black;
             late_mName9.ForeColor = Color.Black;
             late_value9.ForeColor = Color.Black;


             late_m10.ForeColor = Color.Black;
             late_y10.ForeColor = Color.Black;
             late_mName10.ForeColor = Color.Black;
             late_value10.ForeColor = Color.Black;

             late_m11.ForeColor = Color.Black;
             late_y11.ForeColor = Color.Black;
             late_mName11.ForeColor = Color.Black;
             late_value11.ForeColor = Color.Black;

             late_m12.ForeColor = Color.Black;
             late_y12.ForeColor = Color.Black;
             late_mName12.ForeColor = Color.Black;
             late_value12.ForeColor = Color.Black;

             late_m13.ForeColor = Color.Black;
             late_y13.ForeColor = Color.Black;
             late_mName13.ForeColor = Color.Black;
             late_value13.ForeColor = Color.Black;

             late_m14.ForeColor = Color.Black;
             late_y14.ForeColor = Color.Black;
             late_mName14.ForeColor = Color.Black;
             late_value14.ForeColor = Color.Black;

             late_m15.ForeColor = Color.Black;
             late_y15.ForeColor = Color.Black;
             late_mName15.ForeColor = Color.Black;
             late_value15.ForeColor = Color.Black;


             late_m16.ForeColor = Color.Black;
             late_y16.ForeColor = Color.Black;
             late_mName16.ForeColor = Color.Black;
             late_value16.ForeColor = Color.Black;

             late_m17.ForeColor = Color.Black;
             late_y17.ForeColor = Color.Black;
             late_mName17.ForeColor = Color.Black;
             late_value17.ForeColor = Color.Black;

             late_m18.ForeColor = Color.Black;
             late_y18.ForeColor = Color.Black;
             late_mName18.ForeColor = Color.Black;
             late_value18.ForeColor = Color.Black;
             late_m19.ForeColor = Color.Black;
             late_y19.ForeColor = Color.Black;
             late_mName19.ForeColor = Color.Black;
             late_value19.ForeColor = Color.Black;

             late_m20.ForeColor = Color.Black;
             late_y20.ForeColor = Color.Black;
             late_mName20.ForeColor = Color.Black;
             late_value20.ForeColor = Color.Black;

             late_m21.ForeColor = Color.Black;
             late_y21.ForeColor = Color.Black;
             late_mName21.ForeColor = Color.Black;
             late_value21.ForeColor = Color.Black;

             late_m22.ForeColor = Color.Black;
             late_y22.ForeColor = Color.Black;
             late_mName22.ForeColor = Color.Black;
             late_value22.ForeColor = Color.Black;


             late_m23.ForeColor = Color.Black;
             late_y23.ForeColor = Color.Black;
             late_mName23.ForeColor = Color.Black;
             late_value23.ForeColor = Color.Black;

             late_m24.ForeColor = Color.Black;
             late_y24.ForeColor = Color.Black;
             late_mName24.ForeColor = Color.Black;
             late_value24.ForeColor = Color.Black;

           
        }

        void setInVisible_paid()
        {
            

             //disable

             lat_y1.Visible = false;
             lat_y2.Visible = false;
             lat_y3.Visible = false;
             lat_y4.Visible = false;
             lat_y5.Visible = false;
             lat_y6.Visible = false;
             lat_y7.Visible = false;
             lat_y8.Visible = false;
             lat_y9.Visible = false;
             lat_y10.Visible = false;
             lat_y11.Visible = false;
             lat_y12.Visible = false;
             lat_y13.Visible = false;
             lat_y14.Visible = false;
             lat_y15.Visible = false;
             lat_y16.Visible = false;
             lat_y17.Visible = false;
             lat_y18.Visible = false;
             lat_y19.Visible = false;
             lat_y20.Visible = false;
             lat_y21.Visible = false;
             lat_y22.Visible = false;
             lat_y23.Visible = false;
             lat_y24.Visible = false;

             lat_m1.Visible = false;
             lat_m2.Visible = false;
             lat_m3.Visible = false;
             lat_m4.Visible = false;
             lat_m5.Visible = false;
             lat_m6.Visible = false;
             lat_m7.Visible = false;
             lat_m8.Visible = false;
             lat_m9.Visible = false;
             lat_m10.Visible = false;
             lat_m11.Visible = false;
             lat_m12.Visible = false;
             lat_m13.Visible = false;
             lat_m14.Visible = false;
             lat_m15.Visible = false;
             lat_m16.Visible = false;
             lat_m17.Visible = false;
             lat_m18.Visible = false;
             lat_m19.Visible = false;
             lat_m20.Visible = false;
             lat_m21.Visible = false;
             lat_m22.Visible = false;
             lat_m23.Visible = false;
             lat_m24.Visible = false;
             lat_value1.Visible = false;
             lat_value2.Visible = false;
             lat_value3.Visible = false;
             lat_value4.Visible = false;
             lat_value5.Visible = false;
             lat_value6.Visible = false;
             lat_value7.Visible = false;
             lat_value8.Visible = false;
             lat_value9.Visible = false;
             lat_value10.Visible = false;
             lat_value11.Visible = false;
             lat_value12.Visible = false;
             lat_value13.Visible = false;
             lat_value14.Visible = false;
             lat_value15.Visible = false;
             lat_value16.Visible = false;
             lat_value17.Visible = false;
             lat_value18.Visible = false;
             lat_value19.Visible = false;
             lat_value20.Visible = false;
             lat_value21.Visible = false;
             lat_value22.Visible = false;
             lat_value23.Visible = false;
             lat_value24.Visible = false;
             lat_mName1.Visible = false;
             lat_mName2.Visible = false;
             lat_mName3.Visible = false;
             lat_mName4.Visible = false;
             lat_mName5.Visible = false;
             lat_mName6.Visible = false;
             lat_mName7.Visible = false;
             lat_mName8.Visible = false;
             lat_mName9.Visible = false;
             lat_mName10.Visible = false;
             lat_mName11.Visible = false;
             lat_mName12.Visible = false;
             lat_mName13.Visible = false;
             lat_mName14.Visible = false;
             lat_mName15.Visible = false;
             lat_mName16.Visible = false;
             lat_mName17.Visible = false;
             lat_mName18.Visible = false;
             lat_mName19.Visible = false;
             lat_mName20.Visible = false;
             lat_mName21.Visible = false;
             lat_mName22.Visible = false;
             lat_mName23.Visible = false;
             lat_mName24.Visible = false;

            lat_note1.Visible = false;
            lat_note2.Visible = false;
            lat_note3.Visible = false;
            lat_note4.Visible = false;
            lat_note5.Visible = false;
            lat_note6.Visible = false;
            lat_note7.Visible = false;
            lat_note8.Visible = false;
            lat_note9.Visible = false;
            lat_note10.Visible = false;
            lat_note11.Visible = false;
            lat_note12.Visible = false;
            lat_note13.Visible = false;
            lat_note14.Visible = false;
            lat_note15.Visible = false;
            lat_note16.Visible = false;
            lat_note17.Visible = false;
            lat_note18.Visible = false;
            lat_note19.Visible = false;
            lat_note20.Visible = false;
            lat_note21.Visible = false;
            lat_note22.Visible = false;
            lat_note23.Visible = false;
            lat_note24.Visible = false;
            

            lat_m1.ForeColor = Color.Black;
             lat_y1.ForeColor = Color.Black;
             lat_mName1.ForeColor = Color.Black;
             lat_value1.ForeColor = Color.Black;
             lat_m2.ForeColor = Color.Black;
             lat_y2.ForeColor = Color.Black;
             lat_mName2.ForeColor = Color.Black;
             lat_value2.ForeColor = Color.Black;
             lat_m3.ForeColor = Color.Black;
            lat_y3.ForeColor = Color.Black;
             lat_mName3.ForeColor = Color.Black;
             lat_value3.ForeColor = Color.Black;
             lat_m4.ForeColor = Color.Black;
             lat_y4.ForeColor = Color.Black;
             lat_mName4.ForeColor = Color.Black;
             lat_value4.ForeColor = Color.Black;
             lat_m5.ForeColor = Color.Black;
             lat_y5.ForeColor = Color.Black;
             lat_mName5.ForeColor = Color.Black;
             lat_value5.ForeColor = Color.Black;
             lat_m6.ForeColor = Color.Black;
             lat_y6.ForeColor = Color.Black;
             lat_mName6.ForeColor = Color.Black;
             lat_value6.ForeColor = Color.Black;
             lat_m7.ForeColor = Color.Black;
             lat_y7.ForeColor = Color.Black;
             lat_mName7.ForeColor = Color.Black;
             lat_value7.ForeColor = Color.Black;
             lat_m8.ForeColor = Color.Black;
             lat_y8.ForeColor = Color.Black;
             lat_mName8.ForeColor = Color.Black;
             lat_value8.ForeColor = Color.Black;
             lat_m9.ForeColor = Color.Black;
             lat_y9.ForeColor = Color.Black;
             lat_mName9.ForeColor = Color.Black;
             lat_value9.ForeColor = Color.Black;


             lat_m10.ForeColor = Color.Black;
             lat_y10.ForeColor = Color.Black;
             lat_mName10.ForeColor = Color.Black;
             lat_value10.ForeColor = Color.Black;
             lat_m11.ForeColor = Color.Black;
             lat_y11.ForeColor = Color.Black;
             lat_mName11.ForeColor = Color.Black;
             lat_value11.ForeColor = Color.Black;
             lat_m12.ForeColor = Color.Black;
             lat_y12.ForeColor = Color.Black;
             lat_mName12.ForeColor = Color.Black;
             lat_value12.ForeColor = Color.Black;

             lat_m13.ForeColor = Color.Black;
             lat_y13.ForeColor = Color.Black;
             lat_mName13.ForeColor = Color.Black;
             lat_value13.ForeColor = Color.Black;

             lat_m14.ForeColor = Color.Black;
             lat_y14.ForeColor = Color.Black;
             lat_mName14.ForeColor = Color.Black;
             lat_value14.ForeColor = Color.Black;
             lat_m15.ForeColor = Color.Black;
             lat_y15.ForeColor = Color.Black;
             lat_mName15.ForeColor = Color.Black;
             lat_value15.ForeColor = Color.Black;


             lat_m16.ForeColor = Color.Black;
             lat_y16.ForeColor = Color.Black;
             lat_mName16.ForeColor = Color.Black;
             lat_value16.ForeColor = Color.Black;
             lat_m17.ForeColor = Color.Black;
             lat_y17.ForeColor = Color.Black;
             lat_mName17.ForeColor = Color.Black;
             lat_value17.ForeColor = Color.Black;
             lat_m18.ForeColor = Color.Black;
             lat_y18.ForeColor = Color.Black;
             lat_mName18.ForeColor = Color.Black;
             lat_value18.ForeColor = Color.Black;
             lat_m19.ForeColor = Color.Black;
             lat_y19.ForeColor = Color.Black;
             lat_mName19.ForeColor = Color.Black;
             lat_value19.ForeColor = Color.Black;
             lat_m20.ForeColor = Color.Black;
             lat_y20.ForeColor = Color.Black;
             lat_mName20.ForeColor = Color.Black;
             lat_value20.ForeColor = Color.Black;
             lat_m21.ForeColor = Color.Black;
             lat_y21.ForeColor = Color.Black;
             lat_mName21.ForeColor = Color.Black;
             lat_value21.ForeColor = Color.Black;
             lat_m22.ForeColor = Color.Black;
             lat_y22.ForeColor = Color.Black;
             lat_mName22.ForeColor = Color.Black;
             lat_value22.ForeColor = Color.Black;

             lat_m23.ForeColor = Color.Black;
             lat_y23.ForeColor = Color.Black;
             lat_mName23.ForeColor = Color.Black;
             lat_value23.ForeColor = Color.Black;

             lat_m24.ForeColor = Color.Black;
             lat_y24.ForeColor = Color.Black;
             lat_mName24.ForeColor = Color.Black;
             lat_value24.ForeColor = Color.Black;

            lat_check1.ForeColor = Color.Black;
            lat_check2.ForeColor = Color.Black;
            lat_check3.ForeColor = Color.Black;
            lat_check4.ForeColor = Color.Black;
            lat_check5.ForeColor = Color.Black;
            lat_check6.ForeColor = Color.Black;
            lat_check7.ForeColor = Color.Black;
            lat_check8.ForeColor = Color.Black;
            lat_check9.ForeColor = Color.Black;
            lat_check10.ForeColor = Color.Black;
            lat_check11.ForeColor = Color.Black;
            lat_check12.ForeColor = Color.Black;
            lat_check13.ForeColor = Color.Black;
            lat_check14.ForeColor = Color.Black;
            lat_check15.ForeColor = Color.Black;
            lat_check16.ForeColor = Color.Black;
            lat_check17.ForeColor = Color.Black;
            lat_check18.ForeColor = Color.Black;
            lat_check19.ForeColor = Color.Black;
            lat_check20.ForeColor = Color.Black;
            lat_check21.ForeColor = Color.Black;
            lat_check22.ForeColor = Color.Black;
            lat_check23.ForeColor = Color.Black;
            lat_check24.ForeColor = Color.Black;

            lat_check1.Visible = false;
             lat_check2.Visible = false;
             lat_check3.Visible = false;
             lat_check4.Visible = false;
             lat_check5.Visible = false;
             lat_check6.Visible = false;
             lat_check7.Visible = false;
             lat_check8.Visible = false;
             lat_check9.Visible = false;
             lat_check10.Visible = false;
             lat_check11.Visible = false;
             lat_check12.Visible = false;
             lat_check13.Visible = false;
             lat_check14.Visible = false;
             lat_check15.Visible = false;
             lat_check16.Visible = false;
             lat_check17.Visible = false;
             lat_check18.Visible = false;
             lat_check19.Visible = false;
             lat_check20.Visible = false;
             lat_check21.Visible = false;
             lat_check22.Visible = false;
             lat_check23.Visible = false;
             lat_check24.Visible = false;

            lat_check1.Enabled = true;
            lat_check2.Enabled = true;
            lat_check3.Enabled = true;
            lat_check4.Enabled = true;
            lat_check5.Enabled = true;
            lat_check6.Enabled = true;
            lat_check7.Enabled = true;
            lat_check8.Enabled = true;
            lat_check9.Enabled = true;
            lat_check10.Enabled = true;
            lat_check11.Enabled = true;
            lat_check12.Enabled = true;
            lat_check13.Enabled = true;
            lat_check14.Enabled = true;
            lat_check15.Enabled = true;
            lat_check16.Enabled = true;
            lat_check17.Enabled = true;
            lat_check18.Enabled = true;
            lat_check19.Enabled = true;
            lat_check20.Enabled = true;
            lat_check21.Enabled = true;
            lat_check22.Enabled = true;
            lat_check23.Enabled = true;
            lat_check24.Enabled = true;

            lat_value1.Enabled = true;
            lat_value2.Enabled = true;
            lat_value3.Enabled = true;
            lat_value4.Enabled = true;
            lat_value5.Enabled = true;
            lat_value6.Enabled = true;
            lat_value7.Enabled = true;
            lat_value8.Enabled = true;
            lat_value9.Enabled = true;
            lat_value10.Enabled = true;
            lat_value11.Enabled = true;
            lat_value12.Enabled = true;
            lat_value13.Enabled = true;
            lat_value14.Enabled = true;
            lat_value15.Enabled = true;
            lat_value16.Enabled = true;
            lat_value17.Enabled = true;
            lat_value18.Enabled = true;
            lat_value19.Enabled = true;
            lat_value20.Enabled = true;
            lat_value21.Enabled = true;
            lat_value22.Enabled = true;
            lat_value23.Enabled = true;
            lat_value24.Enabled = true;

            lat_check1.Checked = false;
            lat_check2.Checked = false;
            lat_check3.Checked = false;
            lat_check4.Checked = false;
            lat_check5.Checked = false;
            lat_check6.Checked = false;
            lat_check7.Checked = false;
            lat_check8.Checked = false;
            lat_check9.Checked = false;
            lat_check10.Checked = false;
            lat_check11.Checked = false;
            lat_check12.Checked = false;
            lat_check13.Checked = false;
            lat_check14.Checked = false;
            lat_check15.Checked = false;
            lat_check16.Checked = false;
            lat_check17.Checked = false;
            lat_check18.Checked = false;
            lat_check19.Checked = false;
            lat_check20.Checked = false;
            lat_check21.Checked = false;
            lat_check22.Checked = false;
            lat_check23.Checked = false;
            lat_check24.Checked = false;



        }
        void init()
        {
            string name = txt_detailSearch.Text;
            if (name != "" && name != null && name != " ")
            {
                try
                {
                    list_deatail_result.Items.Clear();
                    detail_id.Clear();
                    adapter = new MySqlDataAdapter("select * from client where name like '%" + name + "%'", conn);
                    MySqlCommandBuilder cb = new MySqlCommandBuilder();
                    cb.DataAdapter = adapter;
                    dt_detail = new DataTable();
                    adapter.Fill(dt_detail);
                    dt_detail.Constraints.Add("id", dt_detail.Columns[17], true);
                    bs.DataSource = dt_detail;
                    for (int i = 0; i < dt_detail.Rows.Count; i++)
                    {
                        list_deatail_result.Items.Add(dt_detail.Rows[i]["client_num"].ToString() + "-" + dt_detail.Rows[i]["daphterNum"].ToString()+" "+dt_detail.Rows[i]["name"].ToString());
                        detail_id.Add(int.Parse(dt_detail.Rows[i]["id"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void paidRefresh()
        {

        }
        void paidSaerch()
        {
            string name = paid_search.Text.ToString();
            if (name != "" && name != null && name != " ")
            {
                try
                {
                    paid_list.Items.Clear();
                    paid_id.Clear();
                    string name2 = "";
                    for(int i = 0; i < name.Length-1; i++)
                    {
                       
                        if (name[i] == 'ي' && name[i + 1] == ' ')
                        {
                            name2 += 'ى';

                        }

                        else
                            name2 += name[i];
                    }

                    if(name[name.Length-1] == 'ي')
                    {
                        name2 += 'ى';
                    }
                    else
                    {
                        name2 += name[name.Length - 1];
                    }
                    //MessageBox.Show(name + "\n" + name2);
                    adapter = new MySqlDataAdapter("select * from client where name like '%" + name + "%' or name like '%" + name2 + "%'", conn);
                    MySqlCommandBuilder cb = new MySqlCommandBuilder();
                    cb.DataAdapter = adapter;
                    dt_paid = new DataTable();
                    adapter.Fill(dt_paid);
                    //MessageBox.Show(dt_paid.Columns[17].ColumnName.ToString());
                    dt_paid.Constraints.Add("id", dt_paid.Columns[17], true);
                    for (int i = 0; i < dt_paid.Rows.Count; i++)
                    {
                        paid_list.Items.Add(dt_paid.Rows[i]["client_num"].ToString()+"-"+ dt_paid.Rows[i]["daphterNum"].ToString()+" "+dt_paid.Rows[i]["name"].ToString());
                        paid_id.Add(int.Parse(dt_paid.Rows[i]["id"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btn_clintSave_Click(object sender, EventArgs e)
        {
            
            if (txt_cachFirst.Text == "" || txt_monthsNumber.Text == "" || txt_cId.Text == "" || txt_cDaphter.Text == "" || txt_cName.Text == ""
                 || txt_monthValue.Text == "" || txt_productPrice.Text == "" || txt_productName.Text == ""||dateByDay.Text=="")
            {
                MessageBox.Show(" كمل باقى البيانات الضروريه ");
            }
            else
            {
                try
                {
                    if (int.Parse(txt_monthsNumber.Text) > 24)
                    {
                        MessageBox.Show("تذكر ان الحد الاقصى لعدد الشهور هو 24 شهر ");
                    }
                    else
                    {
                        adapter = new MySqlDataAdapter("select * from client where daphterNum=" + int.Parse(txt_cDaphter.Text) + "", conn);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        bool found = false;
                        
                        for (int i = 0; i < tb.Rows.Count; i++)
                        {
                            if (int.Parse(tb.Rows[i]["client_num"].ToString()) == int.Parse(txt_cId.Text))
                                found = true;
                            //MessageBox.Show(tb.Rows[i]["client_num"].ToString());
                        }

                        if (found)
                        {
                            MessageBox.Show("رقم العميل مكرر خلى بالك يا احمد باشا");
                        }
                        else
                        {
                            cmd = new MySqlCommand();
                            //MessageBox.Show("hiiiiii");
                            int mN = int.Parse(txt_monthsNumber.Text);
                            //MessageBox.Show("hiiiiii");
                        
                            int mV = int.Parse(txt_monthValue.Text);
                            //MessageBox.Show("hiiiiii");
                            int theRest = mN* mV;
                            MessageBox.Show(theRest.ToString());
                            cmd.CommandText = "insert into client(client_num,phone,daphterNum,address,watcherName,watcherNum,productName,productPrice,dateStart,monthNum,firstCash,monthMoney,notes,name,late,cashNum,cashRemain,dateByDay,theRest) values (@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11,@s12,@s13, '" + txt_cName.Text + "' , 0 , 0, @s10,@s14,@s15)";
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@s1", int.Parse(txt_cId.Text));
                            if (txt_cPhone.Text == "" || txt_cPhone.Text == " ")
                                cmd.Parameters.AddWithValue("@s2", "");
                            else
                            {
                                cmd.Parameters.AddWithValue("@s2", int.Parse(txt_cPhone.Text));
                            }
                            cmd.Parameters.AddWithValue("@s3", int.Parse(txt_cDaphter.Text));
                            if (txt_cAddress.Text == "" || txt_cAddress.Text == " ")
                                cmd.Parameters.AddWithValue("@s4", "");
                            else
                                cmd.Parameters.AddWithValue("@s4", txt_cAddress.Text);
                            if (txt_watherName.Text == "" || txt_watherName.Text == " ")
                                cmd.Parameters.AddWithValue("@s5", "");
                            else
                                cmd.Parameters.AddWithValue("@s5", txt_watherName.Text);

                            if (txt_watcherPhone.Text == "" || txt_watcherPhone.Text == " ")
                                cmd.Parameters.AddWithValue("@s6", "");
                            else
                                cmd.Parameters.AddWithValue("@s6", int.Parse(txt_watcherPhone.Text));
                            cmd.Parameters.AddWithValue("@s7", txt_productName.Text);
                            cmd.Parameters.AddWithValue("@s8", float.Parse(txt_productPrice.Text));
                            cmd.Parameters.AddWithValue("@s9", dateTimePicker1.Value.ToString());
                            cmd.Parameters.AddWithValue("@s10", int.Parse(txt_monthsNumber.Text));
                            cmd.Parameters.AddWithValue("@s11", float.Parse(txt_cachFirst.Text));
                            cmd.Parameters.AddWithValue("@s12", float.Parse(txt_monthValue.Text));
                            cmd.Parameters.AddWithValue("@s13", txt_notes.Text);
                            cmd.Parameters.AddWithValue("@s14", int.Parse(dateByDay.Text));
                            cmd.Parameters.AddWithValue("@s15", theRest);
                            //cmd.Parameters.AddWithValue("@s14",);
                            cmd.ExecuteNonQuery();

                            cmd = new MySqlCommand("select id from client order by id desc", conn);
                            int c_id = int.Parse(cmd.ExecuteScalar().ToString());
                            //  MessageBox.Show(c_id+"");
                            DateTime time = dateTimePicker1.Value;
                            //int monthsNumber = int.Parse(txt_monthsNumber.Text);
                            int year = int.Parse(time.Year.ToString());
                            int month = int.Parse(time.Month.ToString());

                            if (month == 12)
                            {
                                month = 1;
                                year++;
                            }
                            else
                            {
                                month++;
                            }
                            int count = int.Parse(txt_monthsNumber.Text);
                            int day = int.Parse(time.Day.ToString());
                            if (day > 28)
                                day = 28;

                            while (count > 0)
                            {
                                try
                                {
                                    //MessageBox.Show(time.Day.ToString());
                                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                                    string date_str = year + "-" + month + "-" + day; 
                                    DateTime date = DateTime.Parse(date_str);
                                    //MessageBox.Show(date.ToString());
                                    cmd = new MySqlCommand("insert into months(cid,year,monthNumber,monthName,monthValue,paid,note,DateTime) values(@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8)", conn);
                                    cmd.Parameters.AddWithValue("@s1", c_id);
                                    cmd.Parameters.AddWithValue("@s2", year.ToString());
                                    cmd.Parameters.AddWithValue("@s3", month);
                                    cmd.Parameters.AddWithValue("@s4", monthName);
                                    cmd.Parameters.AddWithValue("@s5", float.Parse(txt_monthValue.Text));
                                    cmd.Parameters.AddWithValue("@s6", "false");
                                    cmd.Parameters.AddWithValue("@s7", string.Empty);
                                    cmd.Parameters.AddWithValue("@s8", date);
                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                                if (month == 12)
                                {
                                    year++;
                                    month = 0;
                                }
                                count--;
                                month++;
                            }

                            MessageBox.Show("تم الاضافه بنجاح");

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تأكد من ان رقم العميل ليس مكرر");
                    MessageBox.Show(ex.Message);
                }
            }


        }

        private void txt_cId_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9)
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void txt_cPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9)
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void txt_cDaphter_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9)
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void txt_watcherPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9)
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void txt_productPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }

        }

        private void txt_monthsNumber_TextChanged(object sender, EventArgs e)
        {

        }


        private void txt_cachFirst_TextChanged(object sender, EventArgs e)
        {
            if (txt_productPrice.Text != "" && txt_cachFirst.Text != "")
            {
                double total = int.Parse(txt_productPrice.Text);
                double paid = int.Parse(txt_cachFirst.Text);
                double remain = total - paid;
                double after = remain * 25 / 100 + remain;
                txt_remain_client.Text = remain.ToString();
                txt_afterpaid_client.Text = after.ToString();
            }
           // MessageBox.Show(txt_productPrice.Text + " " + txt_cachFirst.Text);
        }

        private void btn_clintClear_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("هل انت متاكد من اجراء هذه العمليه ؟", "رساله تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                txt_cId.Clear();
                txt_cachFirst.Clear();
                txt_cAddress.Clear();
                txt_cDaphter.Clear();
                txt_cId.Clear();
                txt_cName.Clear();
                txt_cPhone.Clear();
                txt_monthsNumber.Clear();
                txt_monthValue.Clear();
                txt_productName.Clear();
                txt_productPrice.Clear();
                txt_watcherPhone.Clear();
                txt_watherName.Clear();
            }



        }

        float calculateMonthsNumber()
        {
            float remain = float.Parse(txt_productPrice.Text) - float.Parse(txt_cachFirst.Text);
            float total = remain/float.Parse(txt_monthValue.Text);
            int temp = (int)total;
            if (temp!= total)
            {
                
                last_month = (remain - ((temp-1) * float.Parse(txt_monthValue.Text)));
               
                //MessageBox.Show(last_month + "");

            }


            return temp;
        }

        private void txt_productPrice_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txt_cachFirst_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void txt_detailSearch_TextChanged(object sender, EventArgs e)
        {
            init();
        }

        private void txt_detailSearch_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void list_deatail_result_SelectedIndexChanged(object sender, EventArgs e)
        {
            list_detailRefresh();
        }

        void list_detailRefresh()
        {
            DataRow drow = null;
            int index = list_deatail_result.SelectedIndex;
            if (index >= 0)
            {
                drow = dt_detail.Rows.Find(detail_id[index]);
            }
            if (drow != null)
            {
                int rno = dt_detail.Rows.IndexOf(drow);
                txt_detailName.Text = drow["name"].ToString();
                txt_detailPhone.Text = drow["phone"].ToString();
                txt_detailID.Text = drow["id"].ToString();
                txt_detailDaphter.Text = drow["daphterNum"].ToString();
                txt_detailAddress.Text = drow["address"].ToString();
                txt_detailWatcher.Text = drow["watcherName"].ToString();
                txt_detailWatcherPhone.Text = drow["watcherNum"].ToString();
                txt_detailProductName.Text = drow["productName"].ToString();
                txt_detailProductPrice.Text = drow["productPrice"].ToString();
                txt_detailDateStart.Text = drow["dateStart"].ToString();
                txt_detailMonthNum.Text = drow["monthNum"].ToString();
                txt_detailFirstCash.Text = drow["firstCash"].ToString();
                txt_detailMonthValue.Text = float.Parse(drow["monthMoney"].ToString()).ToString("0.00");
                txt_detailNotes.Text = drow["notes"].ToString();
                txt_detailmonthsPaid.Text = drow["cashNum"].ToString();
                txt_detailmonthsReamain.Text = drow["cashRemain"].ToString();
                if (txt_detailmonthsPaid.Text == null || txt_detailmonthsPaid.Text == "")
                {
                    txt_detailmonthsPaid.Text = "0";
                }
                if (txt_detailmonthsReamain.Text == null || txt_detailmonthsReamain.Text == "")
                {
                    txt_detailmonthsReamain.Text = txt_detailMonthNum.Text;
                }

                int hasA = int.Parse(drow["hasA"].ToString());
                if (hasA == 0)
                {
                    txt_detailhasA.Text = "لا";

                }
                else
                {
                    txt_detailhasA.Text = "نعم";
                }

            }
            }

        private void button2_Click(object sender, EventArgs e)
        {
            int index = list_deatail_result.SelectedIndex;
            if (index >= 0)
            {
                int i = detail_id[index];
                DialogResult dialogResult = MessageBox.Show("هل انت متأكد من حذف هذا العميل", "رساله تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    cmd = new MySqlCommand("delete from client where id =@id", conn);
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.ExecuteNonQuery();

                    cmd = new MySqlCommand("delete from months where cid =@id", conn);
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.ExecuteNonQuery(); 
                    MessageBox.Show("تم الحذف بنجاح");
                    detail_id.RemoveAt(index);
                    list_deatail_result.Items.RemoveAt(index);

                }
                else if (dialogResult == DialogResult.No)
                {

                }

            }
            else
            {
                MessageBox.Show("اختر العميل اللى عايز تحذفه");
            }

        }

        private void label39_Click(object sender, EventArgs e)
        {

        }

        private void label42_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void list_late_SelectedIndexChanged(object sender, EventArgs e)
        {
            setInVisible();
            DataRow drow = null;
            int index = list_late.SelectedIndex;
            if (index >= 0)
            {
                
                drow = dt_late.Rows.Find(late_id[index]);
            }
            if (drow != null)
            {
                int id = int.Parse(drow["id"].ToString());
                int monthsNumber = int.Parse(drow["monthNum"].ToString());
                late_totalMonths.Text = monthsNumber.ToString();
                late_remainMonths.Text = drow["cashRemain"].ToString();
                late_paidMonths.Text = drow["cashNum"].ToString();
                int hasA = int.Parse(drow["hasA"].ToString());
                label_latePhone.Text = drow["phone"].ToString();
                label_clientid.Text = drow["id"].ToString();
                if (hasA == 0)
                {
                    checkBox_hasA.Checked = false;
                }
                else
                {
                    checkBox_hasA.Checked = true;
                }

                adapter = new MySqlDataAdapter("select * from months where cid="+id+ " order by year , monthNumber ", conn);
                MySqlCommandBuilder cb2 = new MySqlCommandBuilder();
                cb2.DataAdapter = adapter;
                DataTable dt2 = new DataTable();
                adapter.Fill(dt2);
                if (monthsNumber == 1)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }

                }
                else if (monthsNumber == 2)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

    
                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }


                }
                else if (monthsNumber == 3)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;


                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

         

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }


                }
                else if (monthsNumber == 4)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();


                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                }
                else if (monthsNumber == 5)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 6)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();


                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 7)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

 

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                }
                 else if (monthsNumber == 8)
                {

                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;
                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();


                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 9)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;
                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();



                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 10)
                {

                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;

                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();
                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 11)
                {

                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();


                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 12)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true; 

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

      

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 13)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;

                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 14)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;
                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;
                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

 
                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 15)
                {

                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;

                    late_value10.Visible = true;
                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;
                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;
                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;

                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();



                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 16)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;

                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;


                    late_m16.Visible = true;
                    late_y16.Visible = true;
                    late_mName16.Visible = true;
                    late_value16.Visible = true;

                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();

                    late_m16.Text = dt2.Rows[15]["monthNumber"].ToString();
                    late_mName16.Text = dt2.Rows[15]["monthName"].ToString();
                    late_value16.Text = float.Parse(dt2.Rows[15]["monthValue"].ToString()).ToString("0.00");
                    late_y16.Text = dt2.Rows[15]["year"].ToString();


                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[15]["paid"].ToString() == "true")
                    {
                        late_m16.ForeColor = Color.Red;
                        late_y16.ForeColor = Color.Red;
                        late_mName16.ForeColor = Color.Red;
                        late_value16.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 17)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;
                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;
                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;


                    late_m16.Visible = true;
                    late_y16.Visible = true;
                    late_mName16.Visible = true;
                    late_value16.Visible = true;

                    late_m17.Visible = true;
                    late_y17.Visible = true;
                    late_mName17.Visible = true;
                    late_value17.Visible = true;

                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();

                    late_m16.Text = dt2.Rows[15]["monthNumber"].ToString();
                    late_mName16.Text = dt2.Rows[15]["monthName"].ToString();
                    late_value16.Text = float.Parse(dt2.Rows[15]["monthValue"].ToString()).ToString("0.00");
                    late_y16.Text = dt2.Rows[15]["year"].ToString();

                    late_m17.Text = dt2.Rows[16]["monthNumber"].ToString();
                    late_mName17.Text = dt2.Rows[16]["monthName"].ToString();
                    late_value17.Text = float.Parse(dt2.Rows[16]["monthValue"].ToString()).ToString("0.00");
                    late_y17.Text = dt2.Rows[16]["year"].ToString();



                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[15]["paid"].ToString() == "true")
                    {
                        late_m16.ForeColor = Color.Red;
                        late_y16.ForeColor = Color.Red;
                        late_mName16.ForeColor = Color.Red;
                        late_value16.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[16]["paid"].ToString() == "true")
                    {
                        late_m17.ForeColor = Color.Red;
                        late_y17.ForeColor = Color.Red;
                        late_mName17.ForeColor = Color.Red;
                        late_value17.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 18)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;

                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;

                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;


                    late_m16.Visible = true;
                    late_y16.Visible = true;
                    late_mName16.Visible = true;
                    late_value16.Visible = true;

                    late_m17.Visible = true;
                    late_y17.Visible = true;
                    late_mName17.Visible = true;
                    late_value17.Visible = true;

                    late_m18.Visible = true;
                    late_y18.Visible = true;
                    late_mName18.Visible = true;
                    late_value18.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();

                    late_m16.Text = dt2.Rows[15]["monthNumber"].ToString();
                    late_mName16.Text = dt2.Rows[15]["monthName"].ToString();
                    late_value16.Text = float.Parse(dt2.Rows[15]["monthValue"].ToString()).ToString("0.00");
                    late_y16.Text = dt2.Rows[15]["year"].ToString();

                    late_m17.Text = dt2.Rows[16]["monthNumber"].ToString();
                    late_mName17.Text = dt2.Rows[16]["monthName"].ToString();
                    late_value17.Text = float.Parse(dt2.Rows[16]["monthValue"].ToString()).ToString("0.00");
                    late_y17.Text = dt2.Rows[16]["year"].ToString();

                    late_m18.Text = dt2.Rows[17]["monthNumber"].ToString();
                    late_mName18.Text = dt2.Rows[17]["monthName"].ToString();
                    late_value18.Text = float.Parse(dt2.Rows[17]["monthValue"].ToString()).ToString("0.00");
                    late_y18.Text = dt2.Rows[17]["year"].ToString();

 

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[15]["paid"].ToString() == "true")
                    {
                        late_m16.ForeColor = Color.Red;
                        late_y16.ForeColor = Color.Red;
                        late_mName16.ForeColor = Color.Red;
                        late_value16.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[16]["paid"].ToString() == "true")
                    {
                        late_m17.ForeColor = Color.Red;
                        late_y17.ForeColor = Color.Red;
                        late_mName17.ForeColor = Color.Red;
                        late_value17.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[17]["paid"].ToString() == "true")
                    {
                        late_m18.ForeColor = Color.Red;
                        late_y18.ForeColor = Color.Red;
                        late_mName18.ForeColor = Color.Red;
                        late_value18.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 19)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;

                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;

                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;


                    late_m16.Visible = true;
                    late_y16.Visible = true;
                    late_mName16.Visible = true;
                    late_value16.Visible = true;

                    late_m17.Visible = true;
                    late_y17.Visible = true;
                    late_mName17.Visible = true;
                    late_value17.Visible = true;

                    late_m18.Visible = true;
                    late_y18.Visible = true;
                    late_mName18.Visible = true;
                    late_value18.Visible = true;
                    late_m19.Visible = true;
                    late_y19.Visible = true;
                    late_mName19.Visible = true;
                    late_value19.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();

                    late_m16.Text = dt2.Rows[15]["monthNumber"].ToString();
                    late_mName16.Text = dt2.Rows[15]["monthName"].ToString();
                    late_value16.Text = float.Parse(dt2.Rows[15]["monthValue"].ToString()).ToString("0.00");
                    late_y16.Text = dt2.Rows[15]["year"].ToString();

                    late_m17.Text = dt2.Rows[16]["monthNumber"].ToString();
                    late_mName17.Text = dt2.Rows[16]["monthName"].ToString();
                    late_value17.Text = float.Parse(dt2.Rows[16]["monthValue"].ToString()).ToString("0.00");
                    late_y17.Text = dt2.Rows[16]["year"].ToString();

                    late_m18.Text = dt2.Rows[17]["monthNumber"].ToString();
                    late_mName18.Text = dt2.Rows[17]["monthName"].ToString();
                    late_value18.Text = float.Parse(dt2.Rows[17]["monthValue"].ToString()).ToString("0.00");
                    late_y18.Text = dt2.Rows[17]["year"].ToString();

                    late_m19.Text = dt2.Rows[18]["monthNumber"].ToString();
                    late_mName19.Text = dt2.Rows[18]["monthName"].ToString();
                    late_value19.Text = float.Parse(dt2.Rows[18]["monthValue"].ToString()).ToString("0.00");
                    late_y19.Text = dt2.Rows[18]["year"].ToString();

         

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[15]["paid"].ToString() == "true")
                    {
                        late_m16.ForeColor = Color.Red;
                        late_y16.ForeColor = Color.Red;
                        late_mName16.ForeColor = Color.Red;
                        late_value16.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[16]["paid"].ToString() == "true")
                    {
                        late_m17.ForeColor = Color.Red;
                        late_y17.ForeColor = Color.Red;
                        late_mName17.ForeColor = Color.Red;
                        late_value17.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[17]["paid"].ToString() == "true")
                    {
                        late_m18.ForeColor = Color.Red;
                        late_y18.ForeColor = Color.Red;
                        late_mName18.ForeColor = Color.Red;
                        late_value18.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[18]["paid"].ToString() == "true")
                    {
                        late_m19.ForeColor = Color.Red;
                        late_y19.ForeColor = Color.Red;
                        late_mName19.ForeColor = Color.Red;
                        late_value19.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 20)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;

                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;

                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;


                    late_m16.Visible = true;
                    late_y16.Visible = true;
                    late_mName16.Visible = true;
                    late_value16.Visible = true;

                    late_m17.Visible = true;
                    late_y17.Visible = true;
                    late_mName17.Visible = true;
                    late_value17.Visible = true;

                    late_m18.Visible = true;
                    late_y18.Visible = true;
                    late_mName18.Visible = true;
                    late_value18.Visible = true;
                    late_m19.Visible = true;
                    late_y19.Visible = true;
                    late_mName19.Visible = true;
                    late_value19.Visible = true;

                    late_m20.Visible = true;
                    late_y20.Visible = true;
                    late_mName20.Visible = true;
                    late_value20.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();

                    late_m16.Text = dt2.Rows[15]["monthNumber"].ToString();
                    late_mName16.Text = dt2.Rows[15]["monthName"].ToString();
                    late_value16.Text = float.Parse(dt2.Rows[15]["monthValue"].ToString()).ToString("0.00");
                    late_y16.Text = dt2.Rows[15]["year"].ToString();

                    late_m17.Text = dt2.Rows[16]["monthNumber"].ToString();
                    late_mName17.Text = dt2.Rows[16]["monthName"].ToString();
                    late_value17.Text = float.Parse(dt2.Rows[16]["monthValue"].ToString()).ToString("0.00");
                    late_y17.Text = dt2.Rows[16]["year"].ToString();

                    late_m18.Text = dt2.Rows[17]["monthNumber"].ToString();
                    late_mName18.Text = dt2.Rows[17]["monthName"].ToString();
                    late_value18.Text = float.Parse(dt2.Rows[17]["monthValue"].ToString()).ToString("0.00");
                    late_y18.Text = dt2.Rows[17]["year"].ToString();

                    late_m19.Text = dt2.Rows[18]["monthNumber"].ToString();
                    late_mName19.Text = dt2.Rows[18]["monthName"].ToString();
                    late_value19.Text = float.Parse(dt2.Rows[18]["monthValue"].ToString()).ToString("0.00");
                    late_y19.Text = dt2.Rows[18]["year"].ToString();

                    late_m20.Text = dt2.Rows[19]["monthNumber"].ToString();
                    late_mName20.Text = dt2.Rows[19]["monthName"].ToString();
                    late_value20.Text = float.Parse(dt2.Rows[19]["monthValue"].ToString()).ToString("0.00");
                    late_y20.Text = dt2.Rows[19]["year"].ToString();

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[15]["paid"].ToString() == "true")
                    {
                        late_m16.ForeColor = Color.Red;
                        late_y16.ForeColor = Color.Red;
                        late_mName16.ForeColor = Color.Red;
                        late_value16.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[16]["paid"].ToString() == "true")
                    {
                        late_m17.ForeColor = Color.Red;
                        late_y17.ForeColor = Color.Red;
                        late_mName17.ForeColor = Color.Red;
                        late_value17.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[17]["paid"].ToString() == "true")
                    {
                        late_m18.ForeColor = Color.Red;
                        late_y18.ForeColor = Color.Red;
                        late_mName18.ForeColor = Color.Red;
                        late_value18.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[18]["paid"].ToString() == "true")
                    {
                        late_m19.ForeColor = Color.Red;
                        late_y19.ForeColor = Color.Red;
                        late_mName19.ForeColor = Color.Red;
                        late_value19.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[19]["paid"].ToString() == "true")
                    {
                        late_m20.ForeColor = Color.Red;
                        late_y20.ForeColor = Color.Red;
                        late_mName20.ForeColor = Color.Red;
                        late_value20.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 21)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;
                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;

                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;


                    late_m16.Visible = true;
                    late_y16.Visible = true;
                    late_mName16.Visible = true;
                    late_value16.Visible = true;

                    late_m17.Visible = true;
                    late_y17.Visible = true;
                    late_mName17.Visible = true;
                    late_value17.Visible = true;

                    late_m18.Visible = true;
                    late_y18.Visible = true;
                    late_mName18.Visible = true;
                    late_value18.Visible = true;
                    late_m19.Visible = true;
                    late_y19.Visible = true;
                    late_mName19.Visible = true;
                    late_value19.Visible = true;

                    late_m20.Visible = true;
                    late_y20.Visible = true;
                    late_mName20.Visible = true;
                    late_value20.Visible = true;

                    late_m21.Visible = true;
                    late_y21.Visible = true;
                    late_mName21.Visible = true;
                    late_value21.Visible = true;

                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();

                    late_m16.Text = dt2.Rows[15]["monthNumber"].ToString();
                    late_mName16.Text = dt2.Rows[15]["monthName"].ToString();
                    late_value16.Text = float.Parse(dt2.Rows[15]["monthValue"].ToString()).ToString("0.00");
                    late_y16.Text = dt2.Rows[15]["year"].ToString();

                    late_m17.Text = dt2.Rows[16]["monthNumber"].ToString();
                    late_mName17.Text = dt2.Rows[16]["monthName"].ToString();
                    late_value17.Text = float.Parse(dt2.Rows[16]["monthValue"].ToString()).ToString("0.00");
                    late_y17.Text = dt2.Rows[16]["year"].ToString();

                    late_m18.Text = dt2.Rows[17]["monthNumber"].ToString();
                    late_mName18.Text = dt2.Rows[17]["monthName"].ToString();
                    late_value18.Text = float.Parse(dt2.Rows[17]["monthValue"].ToString()).ToString("0.00");
                    late_y18.Text = dt2.Rows[17]["year"].ToString();

                    late_m19.Text = dt2.Rows[18]["monthNumber"].ToString();
                    late_mName19.Text = dt2.Rows[18]["monthName"].ToString();
                    late_value19.Text = float.Parse(dt2.Rows[18]["monthValue"].ToString()).ToString("0.00");
                    late_y19.Text = dt2.Rows[18]["year"].ToString();

                    late_m20.Text = dt2.Rows[19]["monthNumber"].ToString();
                    late_mName20.Text = dt2.Rows[19]["monthName"].ToString();
                    late_value20.Text = float.Parse(dt2.Rows[19]["monthValue"].ToString()).ToString("0.00");
                    late_y20.Text = dt2.Rows[19]["year"].ToString();

                    late_m21.Text = dt2.Rows[20]["monthNumber"].ToString();
                    late_mName21.Text = dt2.Rows[20]["monthName"].ToString();
                    late_value21.Text = float.Parse(dt2.Rows[20]["monthValue"].ToString()).ToString("0.00");
                    late_y21.Text = dt2.Rows[20]["year"].ToString();




                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[15]["paid"].ToString() == "true")
                    {
                        late_m16.ForeColor = Color.Red;
                        late_y16.ForeColor = Color.Red;
                        late_mName16.ForeColor = Color.Red;
                        late_value16.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[16]["paid"].ToString() == "true")
                    {
                        late_m17.ForeColor = Color.Red;
                        late_y17.ForeColor = Color.Red;
                        late_mName17.ForeColor = Color.Red;
                        late_value17.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[17]["paid"].ToString() == "true")
                    {
                        late_m18.ForeColor = Color.Red;
                        late_y18.ForeColor = Color.Red;
                        late_mName18.ForeColor = Color.Red;
                        late_value18.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[18]["paid"].ToString() == "true")
                    {
                        late_m19.ForeColor = Color.Red;
                        late_y19.ForeColor = Color.Red;
                        late_mName19.ForeColor = Color.Red;
                        late_value19.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[19]["paid"].ToString() == "true")
                    {
                        late_m20.ForeColor = Color.Red;
                        late_y20.ForeColor = Color.Red;
                        late_mName20.ForeColor = Color.Red;
                        late_value20.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[20]["paid"].ToString() == "true")
                    {
                        late_m21.ForeColor = Color.Red;
                        late_y21.ForeColor = Color.Red;
                        late_mName21.ForeColor = Color.Red;
                        late_value21.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 22)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;

                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;

                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;


                    late_m16.Visible = true;
                    late_y16.Visible = true;
                    late_mName16.Visible = true;
                    late_value16.Visible = true;

                    late_m17.Visible = true;
                    late_y17.Visible = true;
                    late_mName17.Visible = true;
                    late_value17.Visible = true;

                    late_m18.Visible = true;
                    late_y18.Visible = true;
                    late_mName18.Visible = true;
                    late_value18.Visible = true;
                    late_m19.Visible = true;
                    late_y19.Visible = true;
                    late_mName19.Visible = true;
                    late_value19.Visible = true;

                    late_m20.Visible = true;
                    late_y20.Visible = true;
                    late_mName20.Visible = true;
                    late_value20.Visible = true;

                    late_m21.Visible = true;
                    late_y21.Visible = true;
                    late_mName21.Visible = true;
                    late_value21.Visible = true;

                    late_m22.Visible = true;
                    late_y22.Visible = true;
                    late_mName22.Visible = true;
                    late_value22.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();

                    late_m16.Text = dt2.Rows[15]["monthNumber"].ToString();
                    late_mName16.Text = dt2.Rows[15]["monthName"].ToString();
                    late_value16.Text = float.Parse(dt2.Rows[15]["monthValue"].ToString()).ToString("0.00");
                    late_y16.Text = dt2.Rows[15]["year"].ToString();

                    late_m17.Text = dt2.Rows[16]["monthNumber"].ToString();
                    late_mName17.Text = dt2.Rows[16]["monthName"].ToString();
                    late_value17.Text = float.Parse(dt2.Rows[16]["monthValue"].ToString()).ToString("0.00");
                    late_y17.Text = dt2.Rows[16]["year"].ToString();

                    late_m18.Text = dt2.Rows[17]["monthNumber"].ToString();
                    late_mName18.Text = dt2.Rows[17]["monthName"].ToString();
                    late_value18.Text = float.Parse(dt2.Rows[17]["monthValue"].ToString()).ToString("0.00");
                    late_y18.Text = dt2.Rows[17]["year"].ToString();

                    late_m19.Text = dt2.Rows[18]["monthNumber"].ToString();
                    late_mName19.Text = dt2.Rows[18]["monthName"].ToString();
                    late_value19.Text = float.Parse(dt2.Rows[18]["monthValue"].ToString()).ToString("0.00");
                    late_y19.Text = dt2.Rows[18]["year"].ToString();

                    late_m20.Text = dt2.Rows[19]["monthNumber"].ToString();
                    late_mName20.Text = dt2.Rows[19]["monthName"].ToString();
                    late_value20.Text = float.Parse(dt2.Rows[19]["monthValue"].ToString()).ToString("0.00");
                    late_y20.Text = dt2.Rows[19]["year"].ToString();

                    late_m21.Text = dt2.Rows[20]["monthNumber"].ToString();
                    late_mName21.Text = dt2.Rows[20]["monthName"].ToString();
                    late_value21.Text = float.Parse(dt2.Rows[20]["monthValue"].ToString()).ToString("0.00");
                    late_y21.Text = dt2.Rows[20]["year"].ToString();

                    late_m22.Text = dt2.Rows[21]["monthNumber"].ToString();
                    late_mName22.Text = dt2.Rows[21]["monthName"].ToString();
                    late_value22.Text = float.Parse(dt2.Rows[21]["monthValue"].ToString()).ToString("0.00");
                    late_y22.Text = dt2.Rows[21]["year"].ToString();


                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[15]["paid"].ToString() == "true")
                    {
                        late_m16.ForeColor = Color.Red;
                        late_y16.ForeColor = Color.Red;
                        late_mName16.ForeColor = Color.Red;
                        late_value16.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[16]["paid"].ToString() == "true")
                    {
                        late_m17.ForeColor = Color.Red;
                        late_y17.ForeColor = Color.Red;
                        late_mName17.ForeColor = Color.Red;
                        late_value17.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[17]["paid"].ToString() == "true")
                    {
                        late_m18.ForeColor = Color.Red;
                        late_y18.ForeColor = Color.Red;
                        late_mName18.ForeColor = Color.Red;
                        late_value18.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[18]["paid"].ToString() == "true")
                    {
                        late_m19.ForeColor = Color.Red;
                        late_y19.ForeColor = Color.Red;
                        late_mName19.ForeColor = Color.Red;
                        late_value19.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[19]["paid"].ToString() == "true")
                    {
                        late_m20.ForeColor = Color.Red;
                        late_y20.ForeColor = Color.Red;
                        late_mName20.ForeColor = Color.Red;
                        late_value20.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[20]["paid"].ToString() == "true")
                    {
                        late_m21.ForeColor = Color.Red;
                        late_y21.ForeColor = Color.Red;
                        late_mName21.ForeColor = Color.Red;
                        late_value21.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[21]["paid"].ToString() == "true")
                    {
                        late_m22.ForeColor = Color.Red;
                        late_y22.ForeColor = Color.Red;
                        late_mName22.ForeColor = Color.Red;
                        late_value22.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 23)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;

                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;

                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;


                    late_m16.Visible = true;
                    late_y16.Visible = true;
                    late_mName16.Visible = true;
                    late_value16.Visible = true;

                    late_m17.Visible = true;
                    late_y17.Visible = true;
                    late_mName17.Visible = true;
                    late_value17.Visible = true;

                    late_m18.Visible = true;
                    late_y18.Visible = true;
                    late_mName18.Visible = true;
                    late_value18.Visible = true;
                    late_m19.Visible = true;
                    late_y19.Visible = true;
                    late_mName19.Visible = true;
                    late_value19.Visible = true;

                    late_m20.Visible = true;
                    late_y20.Visible = true;
                    late_mName20.Visible = true;
                    late_value20.Visible = true;

                    late_m21.Visible = true;
                    late_y21.Visible = true;
                    late_mName21.Visible = true;
                    late_value21.Visible = true;

                    late_m22.Visible = true;
                    late_y22.Visible = true;
                    late_mName22.Visible = true;
                    late_value22.Visible = true;


                    late_m23.Visible = true;
                    late_y23.Visible = true;
                    late_mName23.Visible = true;
                    late_value23.Visible = true;
                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();

                    late_m16.Text = dt2.Rows[15]["monthNumber"].ToString();
                    late_mName16.Text = dt2.Rows[15]["monthName"].ToString();
                    late_value16.Text = float.Parse(dt2.Rows[15]["monthValue"].ToString()).ToString("0.00");
                    late_y16.Text = dt2.Rows[15]["year"].ToString();

                    late_m17.Text = dt2.Rows[16]["monthNumber"].ToString();
                    late_mName17.Text = dt2.Rows[16]["monthName"].ToString();
                    late_value17.Text = float.Parse(dt2.Rows[16]["monthValue"].ToString()).ToString("0.00");
                    late_y17.Text = dt2.Rows[16]["year"].ToString();

                    late_m18.Text = dt2.Rows[17]["monthNumber"].ToString();
                    late_mName18.Text = dt2.Rows[17]["monthName"].ToString();
                    late_value18.Text = float.Parse(dt2.Rows[17]["monthValue"].ToString()).ToString("0.00");
                    late_y18.Text = dt2.Rows[17]["year"].ToString();

                    late_m19.Text = dt2.Rows[18]["monthNumber"].ToString();
                    late_mName19.Text = dt2.Rows[18]["monthName"].ToString();
                    late_value19.Text = float.Parse(dt2.Rows[18]["monthValue"].ToString()).ToString("0.00");
                    late_y19.Text = dt2.Rows[18]["year"].ToString();

                    late_m20.Text = dt2.Rows[19]["monthNumber"].ToString();
                    late_mName20.Text = dt2.Rows[19]["monthName"].ToString();
                    late_value20.Text = float.Parse(dt2.Rows[19]["monthValue"].ToString()).ToString("0.00");
                    late_y20.Text = dt2.Rows[19]["year"].ToString();

                    late_m21.Text = dt2.Rows[20]["monthNumber"].ToString();
                    late_mName21.Text = dt2.Rows[20]["monthName"].ToString();
                    late_value21.Text = float.Parse(dt2.Rows[20]["monthValue"].ToString()).ToString("0.00");
                    late_y21.Text = dt2.Rows[20]["year"].ToString();

                    late_m22.Text = dt2.Rows[21]["monthNumber"].ToString();
                    late_mName22.Text = dt2.Rows[21]["monthName"].ToString();
                    late_value22.Text = float.Parse(dt2.Rows[21]["monthValue"].ToString()).ToString("0.00");
                    late_y22.Text = dt2.Rows[21]["year"].ToString();

                    late_m23.Text = dt2.Rows[22]["monthNumber"].ToString();
                    late_mName23.Text = dt2.Rows[22]["monthName"].ToString();
                    late_value23.Text = float.Parse(dt2.Rows[22]["monthValue"].ToString()).ToString("0.00");
                    late_y23.Text = dt2.Rows[22]["year"].ToString();

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[15]["paid"].ToString() == "true")
                    {
                        late_m16.ForeColor = Color.Red;
                        late_y16.ForeColor = Color.Red;
                        late_mName16.ForeColor = Color.Red;
                        late_value16.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[16]["paid"].ToString() == "true")
                    {
                        late_m17.ForeColor = Color.Red;
                        late_y17.ForeColor = Color.Red;
                        late_mName17.ForeColor = Color.Red;
                        late_value17.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[17]["paid"].ToString() == "true")
                    {
                        late_m18.ForeColor = Color.Red;
                        late_y18.ForeColor = Color.Red;
                        late_mName18.ForeColor = Color.Red;
                        late_value18.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[18]["paid"].ToString() == "true")
                    {
                        late_m19.ForeColor = Color.Red;
                        late_y19.ForeColor = Color.Red;
                        late_mName19.ForeColor = Color.Red;
                        late_value19.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[19]["paid"].ToString() == "true")
                    {
                        late_m20.ForeColor = Color.Red;
                        late_y20.ForeColor = Color.Red;
                        late_mName20.ForeColor = Color.Red;
                        late_value20.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[20]["paid"].ToString() == "true")
                    {
                        late_m21.ForeColor = Color.Red;
                        late_y21.ForeColor = Color.Red;
                        late_mName21.ForeColor = Color.Red;
                        late_value21.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[21]["paid"].ToString() == "true")
                    {
                        late_m22.ForeColor = Color.Red;
                        late_y22.ForeColor = Color.Red;
                        late_mName22.ForeColor = Color.Red;
                        late_value22.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[22]["paid"].ToString() == "true")
                    {
                        late_m23.ForeColor = Color.Red;
                        late_y23.ForeColor = Color.Red;
                        late_mName23.ForeColor = Color.Red;
                        late_value23.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 24)
                {
                    late_m1.Visible = true;
                    late_y1.Visible = true;
                    late_mName1.Visible = true;
                    late_value1.Visible = true;

                    late_m2.Visible = true;
                    late_y2.Visible = true;
                    late_mName2.Visible = true;
                    late_value2.Visible = true;

                    late_m3.Visible = true;
                    late_y3.Visible = true;
                    late_mName3.Visible = true;
                    late_value3.Visible = true;

                    late_m4.Visible = true;
                    late_y4.Visible = true;
                    late_mName4.Visible = true;
                    late_value4.Visible = true;

                    late_m5.Visible = true;
                    late_y5.Visible = true;
                    late_mName5.Visible = true;
                    late_value5.Visible = true;

                    late_m6.Visible = true;
                    late_y6.Visible = true;
                    late_mName6.Visible = true;
                    late_value6.Visible = true;

                    late_m7.Visible = true;
                    late_y7.Visible = true;
                    late_mName7.Visible = true;
                    late_value7.Visible = true;

                    late_m8.Visible = true;
                    late_y8.Visible = true;
                    late_mName8.Visible = true;
                    late_value8.Visible = true;

                    late_m9.Visible = true;
                    late_y9.Visible = true;
                    late_mName9.Visible = true;
                    late_value9.Visible = true;


                    late_m10.Visible = true;
                    late_y10.Visible = true;
                    late_mName10.Visible = true;
                    late_value10.Visible = true;

                    late_m11.Visible = true;
                    late_y11.Visible = true;
                    late_mName11.Visible = true;
                    late_value11.Visible = true;

                    late_m12.Visible = true;
                    late_y12.Visible = true;
                    late_mName12.Visible = true;
                    late_value12.Visible = true;

                    late_m13.Visible = true;
                    late_y13.Visible = true;
                    late_mName13.Visible = true;
                    late_value13.Visible = true;

                    late_m14.Visible = true;
                    late_y14.Visible = true;
                    late_mName14.Visible = true;
                    late_value14.Visible = true;

                    late_m15.Visible = true;
                    late_y15.Visible = true;
                    late_mName15.Visible = true;
                    late_value15.Visible = true;


                    late_m16.Visible = true;
                    late_y16.Visible = true;
                    late_mName16.Visible = true;
                    late_value16.Visible = true;

                    late_m17.Visible = true;
                    late_y17.Visible = true;
                    late_mName17.Visible = true;
                    late_value17.Visible = true;

                    late_m18.Visible = true;
                    late_y18.Visible = true;
                    late_mName18.Visible = true;
                    late_value18.Visible = true;
                    late_m19.Visible = true;
                    late_y19.Visible = true;
                    late_mName19.Visible = true;
                    late_value19.Visible = true;

                    late_m20.Visible = true;
                    late_y20.Visible = true;
                    late_mName20.Visible = true;
                    late_value20.Visible = true;

                    late_m21.Visible = true;
                    late_y21.Visible = true;
                    late_mName21.Visible = true;
                    late_value21.Visible = true;

                    late_m22.Visible = true;
                    late_y22.Visible = true;
                    late_mName22.Visible = true;
                    late_value22.Visible = true;


                    late_m23.Visible = true;
                    late_y23.Visible = true;
                    late_mName23.Visible = true;
                    late_value23.Visible = true;

                    late_m24.Visible = true;
                    late_y24.Visible = true;
                    late_mName24.Visible = true;
                    late_value24.Visible = true;

                    late_m1.Text = dt2.Rows[0]["monthNumber"].ToString();
                    late_mName1.Text = dt2.Rows[0]["monthName"].ToString();
                    late_value1.Text = float.Parse(dt2.Rows[0]["monthValue"].ToString()).ToString("0.00");
                    late_y1.Text = dt2.Rows[0]["year"].ToString();

                    late_m2.Text = dt2.Rows[1]["monthNumber"].ToString();
                    late_mName2.Text = dt2.Rows[1]["monthName"].ToString();
                    late_value2.Text = float.Parse(dt2.Rows[1]["monthValue"].ToString()).ToString("0.00");
                    late_y2.Text = dt2.Rows[1]["year"].ToString();

                    late_m3.Text = dt2.Rows[2]["monthNumber"].ToString();
                    late_mName3.Text = dt2.Rows[2]["monthName"].ToString();
                    late_value3.Text = float.Parse(dt2.Rows[2]["monthValue"].ToString()).ToString("0.00");
                    late_y3.Text = dt2.Rows[2]["year"].ToString();

                    late_m4.Text = dt2.Rows[3]["monthNumber"].ToString();
                    late_mName4.Text = dt2.Rows[3]["monthName"].ToString();
                    late_value4.Text = float.Parse(dt2.Rows[3]["monthValue"].ToString()).ToString("0.00");
                    late_y4.Text = dt2.Rows[3]["year"].ToString();

                    late_m5.Text = dt2.Rows[4]["monthNumber"].ToString();
                    late_mName5.Text = dt2.Rows[4]["monthName"].ToString();
                    late_value5.Text = float.Parse(dt2.Rows[4]["monthValue"].ToString()).ToString("0.00");
                    late_y5.Text = dt2.Rows[4]["year"].ToString();

                    late_m6.Text = dt2.Rows[5]["monthNumber"].ToString();
                    late_mName6.Text = dt2.Rows[5]["monthName"].ToString();
                    late_value6.Text = float.Parse(dt2.Rows[5]["monthValue"].ToString()).ToString("0.00");
                    late_y6.Text = dt2.Rows[5]["year"].ToString();

                    late_m7.Text = dt2.Rows[6]["monthNumber"].ToString();
                    late_mName7.Text = dt2.Rows[6]["monthName"].ToString();
                    late_value7.Text = float.Parse(dt2.Rows[6]["monthValue"].ToString()).ToString("0.00");
                    late_y7.Text = dt2.Rows[6]["year"].ToString();

                    late_m8.Text = dt2.Rows[7]["monthNumber"].ToString();
                    late_mName8.Text = dt2.Rows[7]["monthName"].ToString();
                    late_value8.Text = float.Parse(dt2.Rows[7]["monthValue"].ToString()).ToString("0.00");
                    late_y8.Text = dt2.Rows[7]["year"].ToString();

                    late_m9.Text = dt2.Rows[8]["monthNumber"].ToString();
                    late_mName9.Text = dt2.Rows[8]["monthName"].ToString();
                    late_value9.Text = float.Parse(dt2.Rows[8]["monthValue"].ToString()).ToString("0.00");
                    late_y9.Text = dt2.Rows[8]["year"].ToString();

                    late_m10.Text = dt2.Rows[9]["monthNumber"].ToString();
                    late_mName10.Text = dt2.Rows[9]["monthName"].ToString();
                    late_value10.Text = float.Parse(dt2.Rows[9]["monthValue"].ToString()).ToString("0.00");
                    late_y10.Text = dt2.Rows[9]["year"].ToString();

                    late_m11.Text = dt2.Rows[10]["monthNumber"].ToString();
                    late_mName11.Text = dt2.Rows[10]["monthName"].ToString();
                    late_value11.Text = float.Parse(dt2.Rows[10]["monthValue"].ToString()).ToString("0.00");
                    late_y11.Text = dt2.Rows[10]["year"].ToString();

                    late_m12.Text = dt2.Rows[11]["monthNumber"].ToString();
                    late_mName12.Text = dt2.Rows[11]["monthName"].ToString();
                    late_value12.Text = float.Parse(dt2.Rows[11]["monthValue"].ToString()).ToString("0.00");
                    late_y12.Text = dt2.Rows[11]["year"].ToString();

                    late_m13.Text = dt2.Rows[12]["monthNumber"].ToString();
                    late_mName13.Text = dt2.Rows[12]["monthName"].ToString();
                    late_value13.Text = float.Parse(dt2.Rows[12]["monthValue"].ToString()).ToString("0.00");
                    late_y13.Text = dt2.Rows[12]["year"].ToString();

                    late_m14.Text = dt2.Rows[13]["monthNumber"].ToString();
                    late_mName14.Text = dt2.Rows[13]["monthName"].ToString();
                    late_value14.Text = float.Parse(dt2.Rows[13]["monthValue"].ToString()).ToString("0.00");
                    late_y14.Text = dt2.Rows[13]["year"].ToString();

                    late_m15.Text = dt2.Rows[14]["monthNumber"].ToString();
                    late_mName15.Text = dt2.Rows[14]["monthName"].ToString();
                    late_value15.Text = float.Parse(dt2.Rows[14]["monthValue"].ToString()).ToString("0.00");
                    late_y15.Text = dt2.Rows[14]["year"].ToString();

                    late_m16.Text = dt2.Rows[15]["monthNumber"].ToString();
                    late_mName16.Text = dt2.Rows[15]["monthName"].ToString();
                    late_value16.Text = float.Parse(dt2.Rows[15]["monthValue"].ToString()).ToString("0.00");
                    late_y16.Text = dt2.Rows[15]["year"].ToString();

                    late_m17.Text = dt2.Rows[16]["monthNumber"].ToString();
                    late_mName17.Text = dt2.Rows[16]["monthName"].ToString();
                    late_value17.Text = float.Parse(dt2.Rows[16]["monthValue"].ToString()).ToString("0.00");
                    late_y17.Text = dt2.Rows[16]["year"].ToString();

                    late_m18.Text = dt2.Rows[17]["monthNumber"].ToString();
                    late_mName18.Text = dt2.Rows[17]["monthName"].ToString();
                    late_value18.Text = float.Parse(dt2.Rows[17]["monthValue"].ToString()).ToString("0.00");
                    late_y18.Text = dt2.Rows[17]["year"].ToString();

                    late_m19.Text = dt2.Rows[18]["monthNumber"].ToString();
                    late_mName19.Text = dt2.Rows[18]["monthName"].ToString();
                    late_value19.Text = float.Parse(dt2.Rows[18]["monthValue"].ToString()).ToString("0.00");
                    late_y19.Text = dt2.Rows[18]["year"].ToString();

                    late_m20.Text = dt2.Rows[19]["monthNumber"].ToString();
                    late_mName20.Text = dt2.Rows[19]["monthName"].ToString();
                    late_value20.Text = float.Parse(dt2.Rows[19]["monthValue"].ToString()).ToString("0.00");
                    late_y20.Text = dt2.Rows[19]["year"].ToString();

                    late_m21.Text = dt2.Rows[20]["monthNumber"].ToString();
                    late_mName21.Text = dt2.Rows[20]["monthName"].ToString();
                    late_value21.Text = float.Parse(dt2.Rows[20]["monthValue"].ToString()).ToString("0.00");
                    late_y21.Text = dt2.Rows[20]["year"].ToString();

                    late_m22.Text = dt2.Rows[21]["monthNumber"].ToString();
                    late_mName22.Text = dt2.Rows[21]["monthName"].ToString();
                    late_value22.Text = float.Parse(dt2.Rows[21]["monthValue"].ToString()).ToString("0.00");
                    late_y22.Text = dt2.Rows[21]["year"].ToString();

                    late_m23.Text = dt2.Rows[22]["monthNumber"].ToString();
                    late_mName23.Text = dt2.Rows[22]["monthName"].ToString();
                    late_value23.Text = float.Parse(dt2.Rows[22]["monthValue"].ToString()).ToString("0.00");
                    late_y23.Text = dt2.Rows[22]["year"].ToString();

                    late_m24.Text = dt2.Rows[23]["monthNumber"].ToString();
                    late_mName24.Text = dt2.Rows[23]["monthName"].ToString();
                    late_value24.Text = float.Parse(dt2.Rows[23]["monthValue"].ToString()).ToString("0.00");
                    late_y24.Text = dt2.Rows[23]["year"].ToString();

                    if (dt2.Rows[0]["paid"].ToString() == "true")
                    {
                        late_m1.ForeColor = Color.Red;
                        late_y1.ForeColor = Color.Red;
                        late_mName1.ForeColor = Color.Red;
                        late_value1.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[1]["paid"].ToString() == "true")
                    {
                        late_m2.ForeColor = Color.Red;
                        late_y2.ForeColor = Color.Red;
                        late_mName2.ForeColor = Color.Red;
                        late_value2.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[2]["paid"].ToString() == "true")
                    {
                        late_m3.ForeColor = Color.Red;
                        late_y3.ForeColor = Color.Red;
                        late_mName3.ForeColor = Color.Red;
                        late_value3.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[3]["paid"].ToString() == "true")
                    {
                        late_m4.ForeColor = Color.Red;
                        late_y4.ForeColor = Color.Red;
                        late_mName4.ForeColor = Color.Red;
                        late_value4.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[4]["paid"].ToString() == "true")
                    {
                        late_m5.ForeColor = Color.Red;
                        late_y5.ForeColor = Color.Red;
                        late_mName5.ForeColor = Color.Red;
                        late_value5.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[5]["paid"].ToString() == "true")
                    {
                        late_m6.ForeColor = Color.Red;
                        late_y6.ForeColor = Color.Red;
                        late_mName6.ForeColor = Color.Red;
                        late_value6.ForeColor = Color.Red;
                    }


                    if (dt2.Rows[6]["paid"].ToString() == "true")
                    {
                        late_m7.ForeColor = Color.Red;
                        late_y7.ForeColor = Color.Red;
                        late_mName7.ForeColor = Color.Red;
                        late_value7.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[7]["paid"].ToString() == "true")
                    {
                        late_m8.ForeColor = Color.Red;
                        late_y8.ForeColor = Color.Red;
                        late_mName8.ForeColor = Color.Red;
                        late_value8.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[8]["paid"].ToString() == "true")
                    {
                        late_m9.ForeColor = Color.Red;
                        late_y9.ForeColor = Color.Red;
                        late_mName9.ForeColor = Color.Red;
                        late_value9.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[9]["paid"].ToString() == "true")
                    {
                        late_m10.ForeColor = Color.Red;
                        late_y10.ForeColor = Color.Red;
                        late_mName10.ForeColor = Color.Red;
                        late_value10.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[10]["paid"].ToString() == "true")
                    {
                        late_m11.ForeColor = Color.Red;
                        late_y11.ForeColor = Color.Red;
                        late_mName11.ForeColor = Color.Red;
                        late_value11.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[11]["paid"].ToString() == "true")
                    {
                        late_m12.ForeColor = Color.Red;
                        late_y12.ForeColor = Color.Red;
                        late_mName12.ForeColor = Color.Red;
                        late_value12.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[12]["paid"].ToString() == "true")
                    {
                        late_m13.ForeColor = Color.Red;
                        late_y13.ForeColor = Color.Red;
                        late_mName13.ForeColor = Color.Red;
                        late_value13.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[13]["paid"].ToString() == "true")
                    {
                        late_m14.ForeColor = Color.Red;
                        late_y14.ForeColor = Color.Red;
                        late_mName14.ForeColor = Color.Red;
                        late_value14.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[14]["paid"].ToString() == "true")
                    {
                        late_m15.ForeColor = Color.Red;
                        late_y15.ForeColor = Color.Red;
                        late_mName15.ForeColor = Color.Red;
                        late_value15.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[15]["paid"].ToString() == "true")
                    {
                        late_m16.ForeColor = Color.Red;
                        late_y16.ForeColor = Color.Red;
                        late_mName16.ForeColor = Color.Red;
                        late_value16.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[16]["paid"].ToString() == "true")
                    {
                        late_m17.ForeColor = Color.Red;
                        late_y17.ForeColor = Color.Red;
                        late_mName17.ForeColor = Color.Red;
                        late_value17.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[17]["paid"].ToString() == "true")
                    {
                        late_m18.ForeColor = Color.Red;
                        late_y18.ForeColor = Color.Red;
                        late_mName18.ForeColor = Color.Red;
                        late_value18.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[18]["paid"].ToString() == "true")
                    {
                        late_m19.ForeColor = Color.Red;
                        late_y19.ForeColor = Color.Red;
                        late_mName19.ForeColor = Color.Red;
                        late_value19.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[19]["paid"].ToString() == "true")
                    {
                        late_m20.ForeColor = Color.Red;
                        late_y20.ForeColor = Color.Red;
                        late_mName20.ForeColor = Color.Red;
                        late_value20.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[20]["paid"].ToString() == "true")
                    {
                        late_m21.ForeColor = Color.Red;
                        late_y21.ForeColor = Color.Red;
                        late_mName21.ForeColor = Color.Red;
                        late_value21.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[21]["paid"].ToString() == "true")
                    {
                        late_m22.ForeColor = Color.Red;
                        late_y22.ForeColor = Color.Red;
                        late_mName22.ForeColor = Color.Red;
                        late_value22.ForeColor = Color.Red;
                    }
                    if (dt2.Rows[22]["paid"].ToString() == "true")
                    {
                        late_m23.ForeColor = Color.Red;
                        late_y23.ForeColor = Color.Red;
                        late_mName23.ForeColor = Color.Red;
                        late_value23.ForeColor = Color.Red;
                    }

                    if (dt2.Rows[23]["paid"].ToString() == "true")
                    {
                        late_m24.ForeColor = Color.Red;
                        late_y24.ForeColor = Color.Red;
                        late_mName24.ForeColor = Color.Red;
                        late_value24.ForeColor = Color.Red;
                    }
                }

            }
        }

        private void textBox49_TextChanged(object sender, EventArgs e)
        {
            paidSaerch();
        }

        private void label117_Click(object sender, EventArgs e)
        {

        }

        void getmonthsTable()
        {
            adapter = new MySqlDataAdapter("select * from months where cid=" + paid_cid + " order by year , monthNumber ", conn);
            MySqlCommandBuilder cb2 = new MySqlCommandBuilder();
            cb2.DataAdapter = adapter;
            monthsTable = new DataTable();
            adapter.Fill(monthsTable);
            int total_remain = 0;
            for (int i = 0; i < monthsTable.Rows.Count; i++)
            {
                if (monthsTable.Rows[i]["paid"].ToString() == "false")
                {
                    total_remain += int.Parse(monthsTable.Rows[i]["monthValue"].ToString());
                }
            }

            client_total.Text = total_remain.ToString();
        }

        private void paid_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            setInVisible_paid();
            clientRow = null;
            int index = paid_list.SelectedIndex;
            if (index >= 0)
            {
                clientRow = dt_paid.Rows.Find(paid_id[index]);
            }
            if (clientRow != null)
            {
                cashNumber = int.Parse(clientRow["cashNum"].ToString());
                cashRemain = int.Parse(clientRow["cashRemain"].ToString());
                client_monthsPaid.Text = cashNumber.ToString();
                client_monthsRemain.Text = cashRemain.ToString();
                paid_cid = int.Parse(clientRow["id"].ToString());
                monthsNumber = int.Parse(clientRow["monthNum"].ToString());
                label_cName.Text = clientRow["name"].ToString();
                //MessageBox.Show(paid_cid + " id");
                dateByDayPaid.Text = clientRow["dateByDay"].ToString();

                getmonthsTable();
                if (monthsNumber == 1)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString();
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }        

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 2)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }



                }
                else if (monthsNumber == 3)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 4)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }
                }
                else if (monthsNumber == 5)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                
                }
                else if (monthsNumber == 6)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();


                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                   
                }
                else if (monthsNumber == 7)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

           
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                  
                }
                else if (monthsNumber == 8)
                {

                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }


                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                
                }
                else if (monthsNumber == 9)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;

                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }


                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                  
                }
                else if (monthsNumber == 10)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if(monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
               
                }
                else if (monthsNumber == 11)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();


                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                  
                }
                else if (monthsNumber == 12)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;
                    lat_note1.Visible = true;
                    try
                    {
                        lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
             
                }
                else if (monthsNumber == 13)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();


                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if(monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                
                }
                else if (monthsNumber == 14)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = float.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
             
                }
                else if (monthsNumber == 15)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;

                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }


                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                
                }
                else if (monthsNumber == 16)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;


                    lat_m16.Visible = true;
                    lat_y16.Visible = true;
                    lat_mName16.Visible = true;
                    lat_value16.Visible = true;
                    lat_check16.Visible = true;

                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();
                    lat_note16.Visible = true;
                    lat_note16.Text = monthsTable.Rows[15]["note"].ToString();


                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }

                    lat_m16.Text = monthsTable.Rows[15]["monthNumber"].ToString();
                    lat_mName16.Text = monthsTable.Rows[15]["monthName"].ToString();
                    lat_value16.Text = int.Parse(monthsTable.Rows[15]["monthValue"].ToString()).ToString("");
                    lat_y16.Text = monthsTable.Rows[15]["year"].ToString();
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_check16.Checked = true;
                        lat_check16.Enabled = false;
                        lat_value16.Enabled = false;
                    }

      

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_m16.ForeColor = Color.Red;
                        lat_y16.ForeColor = Color.Red;
                        lat_mName16.ForeColor = Color.Red;
                        lat_value16.ForeColor = Color.Red;
                    }

              
                }
                else if (monthsNumber == 17)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;


                    lat_m16.Visible = true;
                    lat_y16.Visible = true;
                    lat_mName16.Visible = true;
                    lat_value16.Visible = true;
                    lat_check16.Visible = true;

                    late_m17.Visible = true;
                    lat_y17.Visible = true;
                    lat_mName17.Visible = true;
                    lat_value17.Visible = true;
                    lat_check17.Visible = true;

                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();
                    lat_note16.Visible = true;
                    lat_note16.Text = monthsTable.Rows[15]["note"].ToString();
                    lat_note17.Visible = true;
                    lat_note17.Text = monthsTable.Rows[16]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }

                    lat_m16.Text = monthsTable.Rows[15]["monthNumber"].ToString();
                    lat_mName16.Text = monthsTable.Rows[15]["monthName"].ToString();
                    lat_value16.Text = int.Parse(monthsTable.Rows[15]["monthValue"].ToString()).ToString("");
                    lat_y16.Text = monthsTable.Rows[15]["year"].ToString();
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_check16.Checked = true;
                        lat_check16.Enabled = false;
                        lat_value16.Enabled = false;
                    }

                    lat_m17.Text = monthsTable.Rows[16]["monthNumber"].ToString();
                    lat_mName17.Text = monthsTable.Rows[16]["monthName"].ToString();
                    lat_value17.Text = int.Parse(monthsTable.Rows[16]["monthValue"].ToString()).ToString("");
                    lat_y17.Text = monthsTable.Rows[16]["year"].ToString();
                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_check17.Checked = true;
                        lat_check17.Enabled = false;
                        lat_value17.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_m16.ForeColor = Color.Red;
                        lat_y16.ForeColor = Color.Red;
                        lat_mName16.ForeColor = Color.Red;
                        lat_value16.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_m17.ForeColor = Color.Red;
                        lat_y17.ForeColor = Color.Red;
                        lat_mName17.ForeColor = Color.Red;
                        lat_value17.ForeColor = Color.Red;
                    }
            
                }
                else if (monthsNumber == 18)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;


                    lat_m16.Visible = true;
                    lat_y16.Visible = true;
                    lat_mName16.Visible = true;
                    lat_value16.Visible = true;
                    lat_check16.Visible = true;

                    late_m17.Visible = true;
                    lat_y17.Visible = true;
                    lat_mName17.Visible = true;
                    lat_value17.Visible = true;
                    lat_check17.Visible = true;

                    lat_m18.Visible = true;
                    lat_y18.Visible = true;
                    lat_mName18.Visible = true;
                    lat_value18.Visible = true;
                    lat_check18.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();
                    lat_note16.Visible = true;
                    lat_note16.Text = monthsTable.Rows[15]["note"].ToString();
                    lat_note17.Visible = true;
                    lat_note17.Text = monthsTable.Rows[16]["note"].ToString();
                    lat_note18.Visible = true;
                    lat_note18.Text = monthsTable.Rows[17]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = float.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }

                    lat_m16.Text = monthsTable.Rows[15]["monthNumber"].ToString();
                    lat_mName16.Text = monthsTable.Rows[15]["monthName"].ToString();
                    lat_value16.Text = int.Parse(monthsTable.Rows[15]["monthValue"].ToString()).ToString("");
                    lat_y16.Text = monthsTable.Rows[15]["year"].ToString();
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_check16.Checked = true;
                        lat_check16.Enabled = false;
                        lat_value16.Enabled = false;
                    }

                    lat_m17.Text = monthsTable.Rows[16]["monthNumber"].ToString();
                    lat_mName17.Text = monthsTable.Rows[16]["monthName"].ToString();
                    lat_value17.Text = int.Parse(monthsTable.Rows[16]["monthValue"].ToString()).ToString("");
                    lat_y17.Text = monthsTable.Rows[16]["year"].ToString();
                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_check17.Checked = true;
                        lat_check17.Enabled = false;
                        lat_value17.Enabled = false;
                    }

                    lat_m18.Text = monthsTable.Rows[17]["monthNumber"].ToString();
                    lat_mName18.Text = monthsTable.Rows[17]["monthName"].ToString();
                    lat_value18.Text = int.Parse(monthsTable.Rows[17]["monthValue"].ToString()).ToString("");
                    lat_y18.Text = monthsTable.Rows[17]["year"].ToString();
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_check18.Checked = true;
                        lat_check18.Enabled = false;
                        lat_value18.Enabled = false;
                    }


                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_m16.ForeColor = Color.Red;
                        lat_y16.ForeColor = Color.Red;
                        lat_mName16.ForeColor = Color.Red;
                        lat_value16.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_m17.ForeColor = Color.Red;
                        lat_y17.ForeColor = Color.Red;
                        lat_mName17.ForeColor = Color.Red;
                        lat_value17.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_m18.ForeColor = Color.Red;
                        lat_y18.ForeColor = Color.Red;
                        lat_mName18.ForeColor = Color.Red;
                        lat_value18.ForeColor = Color.Red;
                    }
     
                }
                else if (monthsNumber == 19)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;


                    lat_m16.Visible = true;
                    lat_y16.Visible = true;
                    lat_mName16.Visible = true;
                    lat_value16.Visible = true;
                    lat_check16.Visible = true;

                    late_m17.Visible = true;
                    lat_y17.Visible = true;
                    lat_mName17.Visible = true;
                    lat_value17.Visible = true;
                    lat_check17.Visible = true;

                    lat_m18.Visible = true;
                    lat_y18.Visible = true;
                    lat_mName18.Visible = true;
                    lat_value18.Visible = true;
                    lat_check18.Visible = true;

                    lat_m19.Visible = true;
                    lat_y19.Visible = true;
                    lat_mName19.Visible = true;
                    lat_value19.Visible = true;
                    lat_check19.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();
                    lat_note16.Visible = true;
                    lat_note16.Text = monthsTable.Rows[15]["note"].ToString();
                    lat_note17.Visible = true;
                    lat_note17.Text = monthsTable.Rows[16]["note"].ToString();
                    lat_note18.Visible = true;
                    lat_note18.Text = monthsTable.Rows[17]["note"].ToString();
                    lat_note19.Visible = true;
                    lat_note19.Text = monthsTable.Rows[18]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }

                    lat_m16.Text = monthsTable.Rows[15]["monthNumber"].ToString();
                    lat_mName16.Text = monthsTable.Rows[15]["monthName"].ToString();
                    lat_value16.Text = int.Parse(monthsTable.Rows[15]["monthValue"].ToString()).ToString("");
                    lat_y16.Text = monthsTable.Rows[15]["year"].ToString();
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_check16.Checked = true;
                        lat_check16.Enabled = false;
                        lat_value16.Enabled = false;
                    }

                    lat_m17.Text = monthsTable.Rows[16]["monthNumber"].ToString();
                    lat_mName17.Text = monthsTable.Rows[16]["monthName"].ToString();
                    lat_value17.Text = int.Parse(monthsTable.Rows[16]["monthValue"].ToString()).ToString("");
                    lat_y17.Text = monthsTable.Rows[16]["year"].ToString();
                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_check17.Checked = true;
                        lat_check17.Enabled = false;
                        lat_value17.Enabled = false;
                    }

                    lat_m18.Text = monthsTable.Rows[17]["monthNumber"].ToString();
                    lat_mName18.Text = monthsTable.Rows[17]["monthName"].ToString();
                    lat_value18.Text = int.Parse(monthsTable.Rows[17]["monthValue"].ToString()).ToString("");
                    lat_y18.Text = monthsTable.Rows[17]["year"].ToString();
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_check18.Checked = true;
                        lat_check18.Enabled = false;
                        lat_value18.Enabled = false;
                    }

                    lat_m19.Text = monthsTable.Rows[18]["monthNumber"].ToString();
                    lat_mName19.Text = monthsTable.Rows[18]["monthName"].ToString();
                    lat_value19.Text = int.Parse(monthsTable.Rows[18]["monthValue"].ToString()).ToString("");
                    lat_y19.Text = monthsTable.Rows[18]["year"].ToString();
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_check19.Checked = true;
                        lat_check19.Enabled = false;
                        lat_value19.Enabled = false;
                    }

                  


                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_m16.ForeColor = Color.Red;
                        lat_y16.ForeColor = Color.Red;
                        lat_mName16.ForeColor = Color.Red;
                        lat_value16.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_m17.ForeColor = Color.Red;
                        lat_y17.ForeColor = Color.Red;
                        lat_mName17.ForeColor = Color.Red;
                        lat_value17.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_m18.ForeColor = Color.Red;
                        lat_y18.ForeColor = Color.Red;
                        lat_mName18.ForeColor = Color.Red;
                        lat_value18.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_m19.ForeColor = Color.Red;
                        lat_y19.ForeColor = Color.Red;
                        lat_mName19.ForeColor = Color.Red;
                        lat_value19.ForeColor = Color.Red;
                    }
          
                }
                else if (monthsNumber == 20)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;


                    lat_m16.Visible = true;
                    lat_y16.Visible = true;
                    lat_mName16.Visible = true;
                    lat_value16.Visible = true;
                    lat_check16.Visible = true;

                    late_m17.Visible = true;
                    lat_y17.Visible = true;
                    lat_mName17.Visible = true;
                    lat_value17.Visible = true;
                    lat_check17.Visible = true;

                    lat_m18.Visible = true;
                    lat_y18.Visible = true;
                    lat_mName18.Visible = true;
                    lat_value18.Visible = true;
                    lat_check18.Visible = true;

                    lat_m19.Visible = true;
                    lat_y19.Visible = true;
                    lat_mName19.Visible = true;
                    lat_value19.Visible = true;
                    lat_check19.Visible = true;

                    lat_m20.Visible = true;
                    lat_y20.Visible = true;
                    lat_mName20.Visible = true;
                    lat_value20.Visible = true;
                    lat_check20.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();
                    lat_note16.Visible = true;
                    lat_note16.Text = monthsTable.Rows[15]["note"].ToString();
                    lat_note17.Visible = true;
                    lat_note17.Text = monthsTable.Rows[16]["note"].ToString();
                    lat_note18.Visible = true;
                    lat_note18.Text = monthsTable.Rows[17]["note"].ToString();
                    lat_note19.Visible = true;
                    lat_note19.Text = monthsTable.Rows[18]["note"].ToString();
                    lat_note20.Visible = true;
                    lat_note20.Text = monthsTable.Rows[19]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }

                    lat_m16.Text = monthsTable.Rows[15]["monthNumber"].ToString();
                    lat_mName16.Text = monthsTable.Rows[15]["monthName"].ToString();
                    lat_value16.Text = int.Parse(monthsTable.Rows[15]["monthValue"].ToString()).ToString("");
                    lat_y16.Text = monthsTable.Rows[15]["year"].ToString();
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_check16.Checked = true;
                        lat_check16.Enabled = false;
                        lat_value16.Enabled = false;
                    }

                    lat_m17.Text = monthsTable.Rows[16]["monthNumber"].ToString();
                    lat_mName17.Text = monthsTable.Rows[16]["monthName"].ToString();
                    lat_value17.Text = int.Parse(monthsTable.Rows[16]["monthValue"].ToString()).ToString("");
                    lat_y17.Text = monthsTable.Rows[16]["year"].ToString();
                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_check17.Checked = true;
                        lat_check17.Enabled = false;
                        lat_value17.Enabled = false;
                    }

                    lat_m18.Text = monthsTable.Rows[17]["monthNumber"].ToString();
                    lat_mName18.Text = monthsTable.Rows[17]["monthName"].ToString();
                    lat_value18.Text = int.Parse(monthsTable.Rows[17]["monthValue"].ToString()).ToString("");
                    lat_y18.Text = monthsTable.Rows[17]["year"].ToString();
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_check18.Checked = true;
                        lat_check18.Enabled = false;
                        lat_value18.Enabled = false;
                    }

                    lat_m19.Text = monthsTable.Rows[18]["monthNumber"].ToString();
                    lat_mName19.Text = monthsTable.Rows[18]["monthName"].ToString();
                    lat_value19.Text = int.Parse(monthsTable.Rows[18]["monthValue"].ToString()).ToString("");
                    lat_y19.Text = monthsTable.Rows[18]["year"].ToString();
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_check19.Checked = true;
                        lat_check19.Enabled = false;
                        lat_value19.Enabled = false;
                    }

                    lat_m20.Text = monthsTable.Rows[19]["monthNumber"].ToString();
                    lat_mName20.Text = monthsTable.Rows[19]["monthName"].ToString();
                    lat_value20.Text = int.Parse(monthsTable.Rows[19]["monthValue"].ToString()).ToString("");
                    lat_y20.Text = monthsTable.Rows[19]["year"].ToString();
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_check20.Checked = true;
                        lat_check20.Enabled = false;
                        lat_value20.Enabled = false;
                    }

                   

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_m16.ForeColor = Color.Red;
                        lat_y16.ForeColor = Color.Red;
                        lat_mName16.ForeColor = Color.Red;
                        lat_value16.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_m17.ForeColor = Color.Red;
                        lat_y17.ForeColor = Color.Red;
                        lat_mName17.ForeColor = Color.Red;
                        lat_value17.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_m18.ForeColor = Color.Red;
                        lat_y18.ForeColor = Color.Red;
                        lat_mName18.ForeColor = Color.Red;
                        lat_value18.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_m19.ForeColor = Color.Red;
                        lat_y19.ForeColor = Color.Red;
                        lat_mName19.ForeColor = Color.Red;
                        lat_value19.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_m20.ForeColor = Color.Red;
                        lat_y20.ForeColor = Color.Red;
                        lat_mName20.ForeColor = Color.Red;
                        lat_value20.ForeColor = Color.Red;
                    }
             
            
                }
                else if (monthsNumber == 21)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;


                    lat_m16.Visible = true;
                    lat_y16.Visible = true;
                    lat_mName16.Visible = true;
                    lat_value16.Visible = true;
                    lat_check16.Visible = true;

                    late_m17.Visible = true;
                    lat_y17.Visible = true;
                    lat_mName17.Visible = true;
                    lat_value17.Visible = true;
                    lat_check17.Visible = true;

                    lat_m18.Visible = true;
                    lat_y18.Visible = true;
                    lat_mName18.Visible = true;
                    lat_value18.Visible = true;
                    lat_check18.Visible = true;

                    lat_m19.Visible = true;
                    lat_y19.Visible = true;
                    lat_mName19.Visible = true;
                    lat_value19.Visible = true;
                    lat_check19.Visible = true;

                    lat_m20.Visible = true;
                    lat_y20.Visible = true;
                    lat_mName20.Visible = true;
                    lat_value20.Visible = true;
                    lat_check20.Visible = true;

                    lat_m21.Visible = true;
                    lat_y21.Visible = true;
                    lat_mName21.Visible = true;
                    lat_value21.Visible = true;
                    lat_check21.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();
                    lat_note16.Visible = true;
                    lat_note16.Text = monthsTable.Rows[15]["note"].ToString();
                    lat_note17.Visible = true;
                    lat_note17.Text = monthsTable.Rows[16]["note"].ToString();
                    lat_note18.Visible = true;
                    lat_note18.Text = monthsTable.Rows[17]["note"].ToString();
                    lat_note19.Visible = true;
                    lat_note19.Text = monthsTable.Rows[18]["note"].ToString();
                    lat_note20.Visible = true;
                    lat_note20.Text = monthsTable.Rows[19]["note"].ToString();
                    lat_note21.Visible = true;
                    lat_note21.Text = monthsTable.Rows[20]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }

                    lat_m16.Text = monthsTable.Rows[15]["monthNumber"].ToString();
                    lat_mName16.Text = monthsTable.Rows[15]["monthName"].ToString();
                    lat_value16.Text = int.Parse(monthsTable.Rows[15]["monthValue"].ToString()).ToString("");
                    lat_y16.Text = monthsTable.Rows[15]["year"].ToString();
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_check16.Checked = true;
                        lat_check16.Enabled = false;
                        lat_value16.Enabled = false;
                    }

                    lat_m17.Text = monthsTable.Rows[16]["monthNumber"].ToString();
                    lat_mName17.Text = monthsTable.Rows[16]["monthName"].ToString();
                    lat_value17.Text = int.Parse(monthsTable.Rows[16]["monthValue"].ToString()).ToString("");
                    lat_y17.Text = monthsTable.Rows[16]["year"].ToString();
                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_check17.Checked = true;
                        lat_check17.Enabled = false;
                        lat_value17.Enabled = false;
                    }

                    lat_m18.Text = monthsTable.Rows[17]["monthNumber"].ToString();
                    lat_mName18.Text = monthsTable.Rows[17]["monthName"].ToString();
                    lat_value18.Text = int.Parse(monthsTable.Rows[17]["monthValue"].ToString()).ToString("");
                    lat_y18.Text = monthsTable.Rows[17]["year"].ToString();
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_check18.Checked = true;
                        lat_check18.Enabled = false;
                        lat_value18.Enabled = false;
                    }

                    lat_m19.Text = monthsTable.Rows[18]["monthNumber"].ToString();
                    lat_mName19.Text = monthsTable.Rows[18]["monthName"].ToString();
                    lat_value19.Text = int.Parse(monthsTable.Rows[18]["monthValue"].ToString()).ToString("");
                    lat_y19.Text = monthsTable.Rows[18]["year"].ToString();
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_check19.Checked = true;
                        lat_check19.Enabled = false;
                        lat_value19.Enabled = false;
                    }

                    lat_m20.Text = monthsTable.Rows[19]["monthNumber"].ToString();
                    lat_mName20.Text = monthsTable.Rows[19]["monthName"].ToString();
                    lat_value20.Text = int.Parse(monthsTable.Rows[19]["monthValue"].ToString()).ToString("");
                    lat_y20.Text = monthsTable.Rows[19]["year"].ToString();
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_check20.Checked = true;
                        lat_check20.Enabled = false;
                        lat_value20.Enabled = false;
                    }

                    lat_m21.Text = monthsTable.Rows[20]["monthNumber"].ToString();
                    lat_mName21.Text = monthsTable.Rows[20]["monthName"].ToString();
                    lat_value21.Text = int.Parse(monthsTable.Rows[20]["monthValue"].ToString()).ToString("");
                    lat_y21.Text = monthsTable.Rows[20]["year"].ToString();
                    if (monthsTable.Rows[20]["paid"].ToString() == "true")
                    {
                        lat_check21.Checked = true;
                        lat_check21.Enabled = false;
                        lat_value21.Enabled = false;
                    }

 

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_m16.ForeColor = Color.Red;
                        lat_y16.ForeColor = Color.Red;
                        lat_mName16.ForeColor = Color.Red;
                        lat_value16.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_m17.ForeColor = Color.Red;
                        lat_y17.ForeColor = Color.Red;
                        lat_mName17.ForeColor = Color.Red;
                        lat_value17.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_m18.ForeColor = Color.Red;
                        lat_y18.ForeColor = Color.Red;
                        lat_mName18.ForeColor = Color.Red;
                        lat_value18.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_m19.ForeColor = Color.Red;
                        lat_y19.ForeColor = Color.Red;
                        lat_mName19.ForeColor = Color.Red;
                        lat_value19.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_m20.ForeColor = Color.Red;
                        lat_y20.ForeColor = Color.Red;
                        lat_mName20.ForeColor = Color.Red;
                        lat_value20.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[20]["paid"].ToString() == "true")
                    {
                        lat_m21.ForeColor = Color.Red;
                        lat_y21.ForeColor = Color.Red;
                        lat_mName21.ForeColor = Color.Red;
                        lat_value21.ForeColor = Color.Red;
                    }

                }
                else if (monthsNumber == 22)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;


                    lat_m16.Visible = true;
                    lat_y16.Visible = true;
                    lat_mName16.Visible = true;
                    lat_value16.Visible = true;
                    lat_check16.Visible = true;

                    late_m17.Visible = true;
                    lat_y17.Visible = true;
                    lat_mName17.Visible = true;
                    lat_value17.Visible = true;
                    lat_check17.Visible = true;

                    lat_m18.Visible = true;
                    lat_y18.Visible = true;
                    lat_mName18.Visible = true;
                    lat_value18.Visible = true;
                    lat_check18.Visible = true;

                    lat_m19.Visible = true;
                    lat_y19.Visible = true;
                    lat_mName19.Visible = true;
                    lat_value19.Visible = true;
                    lat_check19.Visible = true;

                    lat_m20.Visible = true;
                    lat_y20.Visible = true;
                    lat_mName20.Visible = true;
                    lat_value20.Visible = true;
                    lat_check20.Visible = true;

                    lat_m21.Visible = true;
                    lat_y21.Visible = true;
                    lat_mName21.Visible = true;
                    lat_value21.Visible = true;
                    lat_check21.Visible = true;

                    lat_m22.Visible = true;
                    lat_y22.Visible = true;
                    lat_mName22.Visible = true;
                    lat_value22.Visible = true;
                    lat_check22.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();
                    lat_note16.Visible = true;
                    lat_note16.Text = monthsTable.Rows[15]["note"].ToString();
                    lat_note17.Visible = true;
                    lat_note17.Text = monthsTable.Rows[16]["note"].ToString();
                    lat_note18.Visible = true;
                    lat_note18.Text = monthsTable.Rows[17]["note"].ToString();
                    lat_note19.Visible = true;
                    lat_note19.Text = monthsTable.Rows[18]["note"].ToString();
                    lat_note20.Visible = true;
                    lat_note20.Text = monthsTable.Rows[19]["note"].ToString();
                    lat_note21.Visible = true;
                    lat_note21.Text = monthsTable.Rows[20]["note"].ToString();
                    lat_note22.Visible = true;
                    lat_note22.Text = monthsTable.Rows[21]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }

                    lat_m16.Text = monthsTable.Rows[15]["monthNumber"].ToString();
                    lat_mName16.Text = monthsTable.Rows[15]["monthName"].ToString();
                    lat_value16.Text = int.Parse(monthsTable.Rows[15]["monthValue"].ToString()).ToString("");
                    lat_y16.Text = monthsTable.Rows[15]["year"].ToString();
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_check16.Checked = true;
                        lat_check16.Enabled = false;
                        lat_value16.Enabled = false;
                    }

                    lat_m17.Text = monthsTable.Rows[16]["monthNumber"].ToString();
                    lat_mName17.Text = monthsTable.Rows[16]["monthName"].ToString();
                    lat_value17.Text = int.Parse(monthsTable.Rows[16]["monthValue"].ToString()).ToString("");
                    lat_y17.Text = monthsTable.Rows[16]["year"].ToString();
                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_check17.Checked = true;
                        lat_check17.Enabled = false;
                        lat_value17.Enabled = false;
                    }

                    lat_m18.Text = monthsTable.Rows[17]["monthNumber"].ToString();
                    lat_mName18.Text = monthsTable.Rows[17]["monthName"].ToString();
                    lat_value18.Text = int.Parse(monthsTable.Rows[17]["monthValue"].ToString()).ToString("");
                    lat_y18.Text = monthsTable.Rows[17]["year"].ToString();
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_check18.Checked = true;
                        lat_check18.Enabled = false;
                        lat_value18.Enabled = false;
                    }

                    lat_m19.Text = monthsTable.Rows[18]["monthNumber"].ToString();
                    lat_mName19.Text = monthsTable.Rows[18]["monthName"].ToString();
                    lat_value19.Text = int.Parse(monthsTable.Rows[18]["monthValue"].ToString()).ToString("");
                    lat_y19.Text = monthsTable.Rows[18]["year"].ToString();
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_check19.Checked = true;
                        lat_check19.Enabled = false;
                        lat_value19.Enabled = false;
                    }

                    lat_m20.Text = monthsTable.Rows[19]["monthNumber"].ToString();
                    lat_mName20.Text = monthsTable.Rows[19]["monthName"].ToString();
                    lat_value20.Text = int.Parse(monthsTable.Rows[19]["monthValue"].ToString()).ToString("");
                    lat_y20.Text = monthsTable.Rows[19]["year"].ToString();
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_check20.Checked = true;
                        lat_check20.Enabled = false;
                        lat_value20.Enabled = false;
                    }

                    lat_m21.Text = monthsTable.Rows[20]["monthNumber"].ToString();
                    lat_mName21.Text = monthsTable.Rows[20]["monthName"].ToString();
                    lat_value21.Text = int.Parse(monthsTable.Rows[20]["monthValue"].ToString()).ToString("");
                    lat_y21.Text = monthsTable.Rows[20]["year"].ToString();
                    if (monthsTable.Rows[20]["paid"].ToString() == "true")
                    {
                        lat_check21.Checked = true;
                        lat_check21.Enabled = false;
                        lat_value21.Enabled = false;
                    }

                    lat_m22.Text = monthsTable.Rows[21]["monthNumber"].ToString();
                    lat_mName22.Text = monthsTable.Rows[21]["monthName"].ToString();
                    lat_value22.Text = int.Parse(monthsTable.Rows[21]["monthValue"].ToString()).ToString("");
                    lat_y22.Text = monthsTable.Rows[21]["year"].ToString();
                    if (monthsTable.Rows[21]["paid"].ToString() == "true")
                    {
                        lat_check22.Checked = true;
                        lat_check22.Enabled = false;
                        lat_value22.Enabled = false;
                    }

                

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_m16.ForeColor = Color.Red;
                        lat_y16.ForeColor = Color.Red;
                        lat_mName16.ForeColor = Color.Red;
                        lat_value16.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_m17.ForeColor = Color.Red;
                        lat_y17.ForeColor = Color.Red;
                        lat_mName17.ForeColor = Color.Red;
                        lat_value17.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_m18.ForeColor = Color.Red;
                        lat_y18.ForeColor = Color.Red;
                        lat_mName18.ForeColor = Color.Red;
                        lat_value18.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_m19.ForeColor = Color.Red;
                        lat_y19.ForeColor = Color.Red;
                        lat_mName19.ForeColor = Color.Red;
                        lat_value19.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_m20.ForeColor = Color.Red;
                        lat_y20.ForeColor = Color.Red;
                        lat_mName20.ForeColor = Color.Red;
                        lat_value20.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[20]["paid"].ToString() == "true")
                    {
                        lat_m21.ForeColor = Color.Red;
                        lat_y21.ForeColor = Color.Red;
                        lat_mName21.ForeColor = Color.Red;
                        lat_value21.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[21]["paid"].ToString() == "true")
                    {
                        lat_m22.ForeColor = Color.Red;
                        lat_y22.ForeColor = Color.Red;
                        lat_mName22.ForeColor = Color.Red;
                        lat_value22.ForeColor = Color.Red;
                    }

                }
                else if (monthsNumber == 23)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;


                    lat_m16.Visible = true;
                    lat_y16.Visible = true;
                    lat_mName16.Visible = true;
                    lat_value16.Visible = true;
                    lat_check16.Visible = true;

                    late_m17.Visible = true;
                    lat_y17.Visible = true;
                    lat_mName17.Visible = true;
                    lat_value17.Visible = true;
                    lat_check17.Visible = true;

                    lat_m18.Visible = true;
                    lat_y18.Visible = true;
                    lat_mName18.Visible = true;
                    lat_value18.Visible = true;
                    lat_check18.Visible = true;

                    lat_m19.Visible = true;
                    lat_y19.Visible = true;
                    lat_mName19.Visible = true;
                    lat_value19.Visible = true;
                    lat_check19.Visible = true;

                    lat_m20.Visible = true;
                    lat_y20.Visible = true;
                    lat_mName20.Visible = true;
                    lat_value20.Visible = true;
                    lat_check20.Visible = true;

                    lat_m21.Visible = true;
                    lat_y21.Visible = true;
                    lat_mName21.Visible = true;
                    lat_value21.Visible = true;
                    lat_check21.Visible = true;

                    lat_m22.Visible = true;
                    lat_y22.Visible = true;
                    lat_mName22.Visible = true;
                    lat_value22.Visible = true;
                    lat_check22.Visible = true;


                    lat_m23.Visible = true;
                    lat_y23.Visible = true;
                    lat_mName23.Visible = true;
                    lat_value23.Visible = true;
                    lat_check23.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();
                    lat_note16.Visible = true;
                    lat_note16.Text = monthsTable.Rows[15]["note"].ToString();
                    lat_note17.Visible = true;
                    lat_note17.Text = monthsTable.Rows[16]["note"].ToString();
                    lat_note18.Visible = true;
                    lat_note18.Text = monthsTable.Rows[17]["note"].ToString();
                    lat_note19.Visible = true;
                    lat_note19.Text = monthsTable.Rows[18]["note"].ToString();
                    lat_note20.Visible = true;
                    lat_note20.Text = monthsTable.Rows[19]["note"].ToString();
                    lat_note21.Visible = true;
                    lat_note21.Text = monthsTable.Rows[20]["note"].ToString();
                    lat_note22.Visible = true;
                    lat_note22.Text = monthsTable.Rows[21]["note"].ToString();
                    lat_note23.Visible = true;
                    lat_note23.Text = monthsTable.Rows[22]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = int.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }

                    lat_m16.Text = monthsTable.Rows[15]["monthNumber"].ToString();
                    lat_mName16.Text = monthsTable.Rows[15]["monthName"].ToString();
                    lat_value16.Text = int.Parse(monthsTable.Rows[15]["monthValue"].ToString()).ToString("");
                    lat_y16.Text = monthsTable.Rows[15]["year"].ToString();
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_check16.Checked = true;
                        lat_check16.Enabled = false;
                        lat_value16.Enabled = false;
                    }

                    lat_m17.Text = monthsTable.Rows[16]["monthNumber"].ToString();
                    lat_mName17.Text = monthsTable.Rows[16]["monthName"].ToString();
                    lat_value17.Text = int.Parse(monthsTable.Rows[16]["monthValue"].ToString()).ToString("");
                    lat_y17.Text = monthsTable.Rows[16]["year"].ToString();
                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_check17.Checked = true;
                        lat_check17.Enabled = false;
                        lat_value17.Enabled = false;
                    }

                    lat_m18.Text = monthsTable.Rows[17]["monthNumber"].ToString();
                    lat_mName18.Text = monthsTable.Rows[17]["monthName"].ToString();
                    lat_value18.Text = int.Parse(monthsTable.Rows[17]["monthValue"].ToString()).ToString("");
                    lat_y18.Text = monthsTable.Rows[17]["year"].ToString();
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_check18.Checked = true;
                        lat_check18.Enabled = false;
                        lat_value18.Enabled = false;
                    }

                    lat_m19.Text = monthsTable.Rows[18]["monthNumber"].ToString();
                    lat_mName19.Text = monthsTable.Rows[18]["monthName"].ToString();
                    lat_value19.Text = int.Parse(monthsTable.Rows[18]["monthValue"].ToString()).ToString("");
                    lat_y19.Text = monthsTable.Rows[18]["year"].ToString();
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_check19.Checked = true;
                        lat_check19.Enabled = false;
                        lat_value19.Enabled = false;
                    }

                    lat_m20.Text = monthsTable.Rows[19]["monthNumber"].ToString();
                    lat_mName20.Text = monthsTable.Rows[19]["monthName"].ToString();
                    lat_value20.Text = int.Parse(monthsTable.Rows[19]["monthValue"].ToString()).ToString("");
                    lat_y20.Text = monthsTable.Rows[19]["year"].ToString();
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_check20.Checked = true;
                        lat_check20.Enabled = false;
                        lat_value20.Enabled = false;
                    }

                    lat_m21.Text = monthsTable.Rows[20]["monthNumber"].ToString();
                    lat_mName21.Text = monthsTable.Rows[20]["monthName"].ToString();
                    lat_value21.Text = int.Parse(monthsTable.Rows[20]["monthValue"].ToString()).ToString("");
                    lat_y21.Text = monthsTable.Rows[20]["year"].ToString();
                    if (monthsTable.Rows[20]["paid"].ToString() == "true")
                    {
                        lat_check21.Checked = true;
                        lat_check21.Enabled = false;
                        lat_value21.Enabled = false;
                    }

                    lat_m22.Text = monthsTable.Rows[21]["monthNumber"].ToString();
                    lat_mName22.Text = monthsTable.Rows[21]["monthName"].ToString();
                    lat_value22.Text = int.Parse(monthsTable.Rows[21]["monthValue"].ToString()).ToString("");
                    lat_y22.Text = monthsTable.Rows[21]["year"].ToString();
                    if (monthsTable.Rows[21]["paid"].ToString() == "true")
                    {
                        lat_check22.Checked = true;
                        lat_check22.Enabled = false;
                        lat_value22.Enabled = false;
                    }

                    lat_m23.Text = monthsTable.Rows[22]["monthNumber"].ToString();
                    lat_mName23.Text = monthsTable.Rows[22]["monthName"].ToString();
                    lat_value23.Text = int.Parse(monthsTable.Rows[22]["monthValue"].ToString()).ToString("");
                    lat_y23.Text = monthsTable.Rows[22]["year"].ToString();
                    if (monthsTable.Rows[22]["paid"].ToString() == "true")
                    {
                        lat_check23.Checked = true;
                        lat_check23.Enabled = false;
                        lat_value23.Enabled = false;
                    }



                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_m16.ForeColor = Color.Red;
                        lat_y16.ForeColor = Color.Red;
                        lat_mName16.ForeColor = Color.Red;
                        lat_value16.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_m17.ForeColor = Color.Red;
                        lat_y17.ForeColor = Color.Red;
                        lat_mName17.ForeColor = Color.Red;
                        lat_value17.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_m18.ForeColor = Color.Red;
                        lat_y18.ForeColor = Color.Red;
                        lat_mName18.ForeColor = Color.Red;
                        lat_value18.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_m19.ForeColor = Color.Red;
                        lat_y19.ForeColor = Color.Red;
                        lat_mName19.ForeColor = Color.Red;
                        lat_value19.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_m20.ForeColor = Color.Red;
                        lat_y20.ForeColor = Color.Red;
                        lat_mName20.ForeColor = Color.Red;
                        lat_value20.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[20]["paid"].ToString() == "true")
                    {
                        lat_m21.ForeColor = Color.Red;
                        lat_y21.ForeColor = Color.Red;
                        lat_mName21.ForeColor = Color.Red;
                        lat_value21.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[21]["paid"].ToString() == "true")
                    {
                        lat_m22.ForeColor = Color.Red;
                        lat_y22.ForeColor = Color.Red;
                        lat_mName22.ForeColor = Color.Red;
                        lat_value22.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[22]["paid"].ToString() == "true")
                    {
                        lat_m23.ForeColor = Color.Red;
                        lat_y23.ForeColor = Color.Red;
                        lat_mName23.ForeColor = Color.Red;
                        lat_value23.ForeColor = Color.Red;
                    }


                }
                else if (monthsNumber == 24)
                {
                    lat_m1.Visible = true;
                    lat_y1.Visible = true;
                    lat_mName1.Visible = true;
                    lat_value1.Visible = true;
                    lat_check1.Visible = true;

                    lat_m2.Visible = true;
                    lat_y2.Visible = true;
                    lat_mName2.Visible = true;
                    lat_value2.Visible = true;
                    lat_check2.Visible = true;

                    lat_m3.Visible = true;
                    lat_y3.Visible = true;
                    lat_mName3.Visible = true;
                    lat_value3.Visible = true;
                    lat_check3.Visible = true;

                    lat_m4.Visible = true;
                    lat_y4.Visible = true;
                    lat_mName4.Visible = true;
                    lat_value4.Visible = true;
                    lat_check4.Visible = true;

                    lat_m5.Visible = true;
                    lat_y5.Visible = true;
                    lat_mName5.Visible = true;
                    lat_value5.Visible = true;
                    lat_check5.Visible = true;

                    lat_m6.Visible = true;
                    lat_y6.Visible = true;
                    lat_mName6.Visible = true;
                    lat_value6.Visible = true;
                    lat_check6.Visible = true;

                    lat_m7.Visible = true;
                    lat_y7.Visible = true;
                    lat_mName7.Visible = true;
                    lat_value7.Visible = true;
                    lat_check7.Visible = true;

                    lat_m8.Visible = true;
                    lat_y8.Visible = true;
                    lat_mName8.Visible = true;
                    lat_value8.Visible = true;
                    lat_check8.Visible = true;

                    lat_m9.Visible = true;
                    lat_y9.Visible = true;
                    lat_mName9.Visible = true;
                    lat_value9.Visible = true;
                    lat_check9.Visible = true;


                    lat_m10.Visible = true;
                    lat_y10.Visible = true;
                    lat_mName10.Visible = true;
                    lat_value10.Visible = true;
                    lat_check10.Visible = true;

                    lat_m11.Visible = true;
                    lat_y11.Visible = true;
                    lat_mName11.Visible = true;
                    lat_value11.Visible = true;
                    lat_check11.Visible = true;

                    lat_m12.Visible = true;
                    lat_y12.Visible = true;
                    lat_mName12.Visible = true;
                    lat_value12.Visible = true;
                    lat_check12.Visible = true;

                    lat_m13.Visible = true;
                    lat_y13.Visible = true;
                    lat_mName13.Visible = true;
                    lat_value13.Visible = true;
                    lat_check13.Visible = true;

                    lat_m14.Visible = true;
                    lat_y14.Visible = true;
                    lat_mName14.Visible = true;
                    lat_value14.Visible = true;
                    lat_check14.Visible = true;

                    lat_m15.Visible = true;
                    lat_y15.Visible = true;
                    lat_mName15.Visible = true;
                    lat_value15.Visible = true;
                    lat_check15.Visible = true;


                    lat_m16.Visible = true;
                    lat_y16.Visible = true;
                    lat_mName16.Visible = true;
                    lat_value16.Visible = true;
                    lat_check16.Visible = true;

                    lat_m17.Visible = true;
                    lat_y17.Visible = true;
                    lat_mName17.Visible = true;
                    lat_value17.Visible = true;
                    lat_check17.Visible = true;

                    lat_m18.Visible = true;
                    lat_y18.Visible = true;
                    lat_mName18.Visible = true;
                    lat_value18.Visible = true;
                    lat_check18.Visible = true;

                    lat_m19.Visible = true;
                    lat_y19.Visible = true;
                    lat_mName19.Visible = true;
                    lat_value19.Visible = true;
                    lat_check19.Visible = true;

                    lat_m20.Visible = true;
                    lat_y20.Visible = true;
                    lat_mName20.Visible = true;
                    lat_value20.Visible = true;
                    lat_check20.Visible = true;

                    lat_m21.Visible = true;
                    lat_y21.Visible = true;
                    lat_mName21.Visible = true;
                    lat_value21.Visible = true;
                    lat_check21.Visible = true;

                    lat_m22.Visible = true;
                    lat_y22.Visible = true;
                    lat_mName22.Visible = true;
                    lat_value22.Visible = true;
                    lat_check22.Visible = true;


                    lat_m23.Visible = true;
                    lat_y23.Visible = true;
                    lat_mName23.Visible = true;
                    lat_value23.Visible = true;
                    lat_check23.Visible = true;

                    lat_m24.Visible = true;
                    lat_y24.Visible = true;
                    lat_mName24.Visible = true;
                    lat_value24.Visible = true;
                    lat_check24.Visible = true;
                    lat_note1.Visible = true;
                    lat_note1.Text = monthsTable.Rows[0]["note"].ToString();
                    lat_note2.Visible = true;
                    lat_note2.Text = monthsTable.Rows[1]["note"].ToString();
                    lat_note3.Visible = true;
                    lat_note3.Text = monthsTable.Rows[2]["note"].ToString();
                    lat_note4.Visible = true;
                    lat_note4.Text = monthsTable.Rows[3]["note"].ToString();
                    lat_note5.Visible = true;
                    lat_note5.Text = monthsTable.Rows[4]["note"].ToString();
                    lat_note6.Visible = true;
                    lat_note6.Text = monthsTable.Rows[5]["note"].ToString();
                    lat_note7.Visible = true;
                    lat_note7.Text = monthsTable.Rows[6]["note"].ToString();
                    lat_note8.Visible = true;
                    lat_note8.Text = monthsTable.Rows[7]["note"].ToString();
                    lat_note9.Visible = true;
                    lat_note9.Text = monthsTable.Rows[8]["note"].ToString();
                    lat_note10.Visible = true;
                    lat_note10.Text = monthsTable.Rows[9]["note"].ToString();
                    lat_note12.Visible = true;
                    lat_note12.Text = monthsTable.Rows[11]["note"].ToString();
                    lat_note11.Visible = true;
                    lat_note11.Text = monthsTable.Rows[10]["note"].ToString();
                    lat_note13.Visible = true;
                    lat_note13.Text = monthsTable.Rows[12]["note"].ToString();
                    lat_note14.Visible = true;
                    lat_note14.Text = monthsTable.Rows[13]["note"].ToString();
                    lat_note15.Visible = true;
                    lat_note15.Text = monthsTable.Rows[14]["note"].ToString();
                    lat_note16.Visible = true;
                    lat_note16.Text = monthsTable.Rows[15]["note"].ToString();
                    lat_note17.Visible = true;
                    lat_note17.Text = monthsTable.Rows[16]["note"].ToString();
                    lat_note18.Visible = true;
                    lat_note18.Text = monthsTable.Rows[17]["note"].ToString();
                    lat_note19.Visible = true;
                    lat_note19.Text = monthsTable.Rows[18]["note"].ToString();
                    lat_note20.Visible = true;
                    lat_note20.Text = monthsTable.Rows[19]["note"].ToString();
                    lat_note21.Visible = true;
                    lat_note21.Text = monthsTable.Rows[20]["note"].ToString();
                    lat_note22.Visible = true;
                    lat_note22.Text = monthsTable.Rows[21]["note"].ToString();
                    lat_note23.Visible = true;
                    lat_note23.Text = monthsTable.Rows[22]["note"].ToString();
                    lat_note24.Visible = true;
                    lat_note24.Text = monthsTable.Rows[23]["note"].ToString();

                    lat_m1.Text = monthsTable.Rows[0]["monthNumber"].ToString();
                    lat_mName1.Text = monthsTable.Rows[0]["monthName"].ToString();
                    lat_value1.Text = float.Parse(monthsTable.Rows[0]["monthValue"].ToString()).ToString("");
                    lat_y1.Text = monthsTable.Rows[0]["year"].ToString();
                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_check1.Checked = true;
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                    }

                    lat_m2.Text = monthsTable.Rows[1]["monthNumber"].ToString();
                    lat_mName2.Text = monthsTable.Rows[1]["monthName"].ToString();
                    lat_value2.Text = int.Parse(monthsTable.Rows[1]["monthValue"].ToString()).ToString("");
                    lat_y2.Text = monthsTable.Rows[1]["year"].ToString();
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_check2.Checked = true;
                        lat_check2.Enabled = false;
                        lat_value2.Enabled = false;
                    }

                    lat_m3.Text = monthsTable.Rows[2]["monthNumber"].ToString();
                    lat_mName3.Text = monthsTable.Rows[2]["monthName"].ToString();
                    lat_value3.Text = int.Parse(monthsTable.Rows[2]["monthValue"].ToString()).ToString("");
                    lat_y3.Text = monthsTable.Rows[2]["year"].ToString();
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_check3.Checked = true;
                        lat_check3.Enabled = false;
                        lat_value3.Enabled = false;
                    }

                    lat_m4.Text = monthsTable.Rows[3]["monthNumber"].ToString();
                    lat_mName4.Text = monthsTable.Rows[3]["monthName"].ToString();
                    lat_value4.Text = int.Parse(monthsTable.Rows[3]["monthValue"].ToString()).ToString("");
                    lat_y4.Text = monthsTable.Rows[3]["year"].ToString();
                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_check4.Checked = true;
                        lat_check4.Enabled = false;
                        lat_value4.Enabled = false;
                    }

                    lat_m5.Text = monthsTable.Rows[4]["monthNumber"].ToString();
                    lat_mName5.Text = monthsTable.Rows[4]["monthName"].ToString();
                    lat_value5.Text = int.Parse(monthsTable.Rows[4]["monthValue"].ToString()).ToString("");
                    lat_y5.Text = monthsTable.Rows[4]["year"].ToString();
                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_check5.Checked = true;
                        lat_check5.Enabled = false;
                        lat_value5.Enabled = false;
                    }

                    lat_m6.Text = monthsTable.Rows[5]["monthNumber"].ToString();
                    lat_mName6.Text = monthsTable.Rows[5]["monthName"].ToString();
                    lat_value6.Text = int.Parse(monthsTable.Rows[5]["monthValue"].ToString()).ToString("");
                    lat_y6.Text = monthsTable.Rows[5]["year"].ToString();
                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_check6.Checked = true;
                        lat_check6.Enabled = false;
                        lat_value6.Enabled = false;
                    }

                    lat_m7.Text = monthsTable.Rows[6]["monthNumber"].ToString();
                    lat_mName7.Text = monthsTable.Rows[6]["monthName"].ToString();
                    lat_value7.Text = int.Parse(monthsTable.Rows[6]["monthValue"].ToString()).ToString("");
                    lat_y7.Text = monthsTable.Rows[6]["year"].ToString();
                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_check7.Checked = true;
                        lat_check7.Enabled = false;
                        lat_value7.Enabled = false;
                    }

                    lat_m8.Text = monthsTable.Rows[7]["monthNumber"].ToString();
                    lat_mName8.Text = monthsTable.Rows[7]["monthName"].ToString();
                    lat_value8.Text = int.Parse(monthsTable.Rows[7]["monthValue"].ToString()).ToString("");
                    lat_y8.Text = monthsTable.Rows[7]["year"].ToString();
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_check8.Checked = true;
                        lat_check8.Enabled = false;
                        lat_value8.Enabled = false;
                    }

                    lat_m9.Text = monthsTable.Rows[8]["monthNumber"].ToString();
                    lat_mName9.Text = monthsTable.Rows[8]["monthName"].ToString();
                    lat_value9.Text = int.Parse(monthsTable.Rows[8]["monthValue"].ToString()).ToString("");
                    lat_y9.Text = monthsTable.Rows[8]["year"].ToString();
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_check9.Checked = true;
                        lat_check9.Enabled = false;
                        lat_value9.Enabled = false;
                    }

                    lat_m10.Text = monthsTable.Rows[9]["monthNumber"].ToString();
                    lat_mName10.Text = monthsTable.Rows[9]["monthName"].ToString();
                    lat_value10.Text = int.Parse(monthsTable.Rows[9]["monthValue"].ToString()).ToString("");
                    lat_y10.Text = monthsTable.Rows[9]["year"].ToString();
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_check10.Checked = true;
                        lat_check10.Enabled = false;
                        lat_value10.Enabled = false;
                    }

                    lat_m11.Text = monthsTable.Rows[10]["monthNumber"].ToString();
                    lat_mName11.Text = monthsTable.Rows[10]["monthName"].ToString();
                    lat_value11.Text = int.Parse(monthsTable.Rows[10]["monthValue"].ToString()).ToString("");
                    lat_y11.Text = monthsTable.Rows[10]["year"].ToString();
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_check11.Checked = true;
                        lat_check11.Enabled = false;
                        lat_value11.Enabled = false;
                    }

                    lat_m12.Text = monthsTable.Rows[11]["monthNumber"].ToString();
                    lat_mName12.Text = monthsTable.Rows[11]["monthName"].ToString();
                    lat_value12.Text = int.Parse(monthsTable.Rows[11]["monthValue"].ToString()).ToString("");
                    lat_y12.Text = monthsTable.Rows[11]["year"].ToString();
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_check12.Checked = true;
                        lat_check12.Enabled = false;
                        lat_value12.Enabled = false;
                    }

                    lat_m13.Text = monthsTable.Rows[12]["monthNumber"].ToString();
                    lat_mName13.Text = monthsTable.Rows[12]["monthName"].ToString();
                    lat_value13.Text = int.Parse(monthsTable.Rows[12]["monthValue"].ToString()).ToString("");
                    lat_y13.Text = monthsTable.Rows[12]["year"].ToString();
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_check13.Checked = true;
                        lat_check13.Enabled = false;
                        lat_value13.Enabled = false;
                    }

                    lat_m14.Text = monthsTable.Rows[13]["monthNumber"].ToString();
                    lat_mName14.Text = monthsTable.Rows[13]["monthName"].ToString();
                    lat_value14.Text = int.Parse(monthsTable.Rows[13]["monthValue"].ToString()).ToString("");
                    lat_y14.Text = monthsTable.Rows[13]["year"].ToString();
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_check14.Checked = true;
                        lat_check14.Enabled = false;
                        lat_value14.Enabled = false;
                    }

                    lat_m15.Text = monthsTable.Rows[14]["monthNumber"].ToString();
                    lat_mName15.Text = monthsTable.Rows[14]["monthName"].ToString();
                    lat_value15.Text = int.Parse(monthsTable.Rows[14]["monthValue"].ToString()).ToString("");
                    lat_y15.Text = monthsTable.Rows[14]["year"].ToString();
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_check15.Checked = true;
                        lat_check15.Enabled = false;
                        lat_value15.Enabled = false;
                    }

                    lat_m16.Text = monthsTable.Rows[15]["monthNumber"].ToString();
                    lat_mName16.Text = monthsTable.Rows[15]["monthName"].ToString();
                    lat_value16.Text = int.Parse(monthsTable.Rows[15]["monthValue"].ToString()).ToString("");
                    lat_y16.Text = monthsTable.Rows[15]["year"].ToString();
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_check16.Checked = true;
                        lat_check16.Enabled = false;
                        lat_value16.Enabled = false;
                    }

                    lat_m17.Text = monthsTable.Rows[16]["monthNumber"].ToString();
                    lat_mName17.Text = monthsTable.Rows[16]["monthName"].ToString();
                    lat_value17.Text = int.Parse(monthsTable.Rows[16]["monthValue"].ToString()).ToString("");
                    lat_y17.Text = monthsTable.Rows[16]["year"].ToString();
                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_check17.Checked = true;
                        lat_check17.Enabled = false;
                        lat_value17.Enabled = false;
                    }

                    lat_m18.Text = monthsTable.Rows[17]["monthNumber"].ToString();
                    lat_mName18.Text = monthsTable.Rows[17]["monthName"].ToString();
                    lat_value18.Text = int.Parse(monthsTable.Rows[17]["monthValue"].ToString()).ToString("");
                    lat_y18.Text = monthsTable.Rows[17]["year"].ToString();
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_check18.Checked = true;
                        lat_check18.Enabled = false;
                        lat_value18.Enabled = false;
                    }

                    lat_m19.Text = monthsTable.Rows[18]["monthNumber"].ToString();
                    lat_mName19.Text = monthsTable.Rows[18]["monthName"].ToString();
                    lat_value19.Text = int.Parse(monthsTable.Rows[18]["monthValue"].ToString()).ToString("");
                    lat_y19.Text = monthsTable.Rows[18]["year"].ToString();
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_check19.Checked = true;
                        lat_check19.Enabled = false;
                        lat_value19.Enabled = false;
                    }

                    lat_m20.Text = monthsTable.Rows[19]["monthNumber"].ToString();
                    lat_mName20.Text = monthsTable.Rows[19]["monthName"].ToString();
                    lat_value20.Text = int.Parse(monthsTable.Rows[19]["monthValue"].ToString()).ToString("");
                    lat_y20.Text = monthsTable.Rows[19]["year"].ToString();
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_check20.Checked = true;
                        lat_check20.Enabled = false;
                        lat_value20.Enabled = false;
                    }

                    lat_m21.Text = monthsTable.Rows[20]["monthNumber"].ToString();
                    lat_mName21.Text = monthsTable.Rows[20]["monthName"].ToString();
                    lat_value21.Text = int.Parse(monthsTable.Rows[20]["monthValue"].ToString()).ToString("");
                    lat_y21.Text = monthsTable.Rows[20]["year"].ToString();
                    if (monthsTable.Rows[20]["paid"].ToString() == "true")
                    {
                        lat_check21.Checked = true;
                        lat_check21.Enabled = false;
                        lat_value21.Enabled = false;
                    }

                    lat_m22.Text = monthsTable.Rows[21]["monthNumber"].ToString();
                    lat_mName22.Text = monthsTable.Rows[21]["monthName"].ToString();
                    lat_value22.Text = int.Parse(monthsTable.Rows[21]["monthValue"].ToString()).ToString("");
                    lat_y22.Text = monthsTable.Rows[21]["year"].ToString();
                    if (monthsTable.Rows[21]["paid"].ToString() == "true")
                    {
                        lat_check22.Checked = true;
                        lat_check22.Enabled = false;
                        lat_value22.Enabled = false;
                    }

                    lat_m23.Text = monthsTable.Rows[22]["monthNumber"].ToString();
                    lat_mName23.Text = monthsTable.Rows[22]["monthName"].ToString();
                    lat_value23.Text = int.Parse(monthsTable.Rows[22]["monthValue"].ToString()).ToString("");
                    lat_y23.Text = monthsTable.Rows[22]["year"].ToString();
                    if (monthsTable.Rows[22]["paid"].ToString() == "true")
                    {
                        lat_check23.Checked = true;
                        lat_check23.Enabled = false;
                        lat_value23.Enabled = false;
                    }

                    lat_m24.Text = monthsTable.Rows[23]["monthNumber"].ToString();
                    lat_mName24.Text = monthsTable.Rows[23]["monthName"].ToString();
                    lat_value24.Text = int.Parse(monthsTable.Rows[23]["monthValue"].ToString()).ToString("");
                    lat_y24.Text = monthsTable.Rows[23]["year"].ToString();
                    if (monthsTable.Rows[23]["paid"].ToString() == "true")
                    {
                        lat_check24.Checked = true;
                        lat_check24.Enabled = false;
                        lat_value24.Enabled = false;
                    }

                    if (monthsTable.Rows[0]["paid"].ToString() == "true")
                    {
                        lat_m1.ForeColor = Color.Red;
                        lat_y1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[1]["paid"].ToString() == "true")
                    {
                        lat_m2.ForeColor = Color.Red;
                        lat_y2.ForeColor = Color.Red;
                        lat_mName2.ForeColor = Color.Red;
                        lat_value2.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[2]["paid"].ToString() == "true")
                    {
                        lat_m3.ForeColor = Color.Red;
                        lat_y3.ForeColor = Color.Red;
                        lat_mName3.ForeColor = Color.Red;
                        lat_value3.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[3]["paid"].ToString() == "true")
                    {
                        lat_m4.ForeColor = Color.Red;
                        lat_y4.ForeColor = Color.Red;
                        lat_mName4.ForeColor = Color.Red;
                        lat_value4.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[4]["paid"].ToString() == "true")
                    {
                        lat_m5.ForeColor = Color.Red;
                        lat_y5.ForeColor = Color.Red;
                        lat_mName5.ForeColor = Color.Red;
                        lat_value5.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[5]["paid"].ToString() == "true")
                    {
                        lat_m6.ForeColor = Color.Red;
                        lat_y6.ForeColor = Color.Red;
                        lat_mName6.ForeColor = Color.Red;
                        lat_value6.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[6]["paid"].ToString() == "true")
                    {
                        lat_m7.ForeColor = Color.Red;
                        lat_y7.ForeColor = Color.Red;
                        lat_mName7.ForeColor = Color.Red;
                        lat_value7.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[7]["paid"].ToString() == "true")
                    {
                        lat_m8.ForeColor = Color.Red;
                        lat_y8.ForeColor = Color.Red;
                        lat_mName8.ForeColor = Color.Red;
                        lat_value8.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[8]["paid"].ToString() == "true")
                    {
                        lat_m9.ForeColor = Color.Red;
                        lat_y9.ForeColor = Color.Red;
                        lat_mName9.ForeColor = Color.Red;
                        lat_value9.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[9]["paid"].ToString() == "true")
                    {
                        lat_m10.ForeColor = Color.Red;
                        lat_y10.ForeColor = Color.Red;
                        lat_mName10.ForeColor = Color.Red;
                        lat_value10.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[10]["paid"].ToString() == "true")
                    {
                        lat_m11.ForeColor = Color.Red;
                        lat_y11.ForeColor = Color.Red;
                        lat_mName11.ForeColor = Color.Red;
                        lat_value11.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[11]["paid"].ToString() == "true")
                    {
                        lat_m12.ForeColor = Color.Red;
                        lat_y12.ForeColor = Color.Red;
                        lat_mName12.ForeColor = Color.Red;
                        lat_value12.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[12]["paid"].ToString() == "true")
                    {
                        lat_m13.ForeColor = Color.Red;
                        lat_y13.ForeColor = Color.Red;
                        lat_mName13.ForeColor = Color.Red;
                        lat_value13.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[13]["paid"].ToString() == "true")
                    {
                        lat_m14.ForeColor = Color.Red;
                        lat_y14.ForeColor = Color.Red;
                        lat_mName14.ForeColor = Color.Red;
                        lat_value14.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[14]["paid"].ToString() == "true")
                    {
                        lat_m15.ForeColor = Color.Red;
                        lat_y15.ForeColor = Color.Red;
                        lat_mName15.ForeColor = Color.Red;
                        lat_value15.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[15]["paid"].ToString() == "true")
                    {
                        lat_m16.ForeColor = Color.Red;
                        lat_y16.ForeColor = Color.Red;
                        lat_mName16.ForeColor = Color.Red;
                        lat_value16.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[16]["paid"].ToString() == "true")
                    {
                        lat_m17.ForeColor = Color.Red;
                        lat_y17.ForeColor = Color.Red;
                        lat_mName17.ForeColor = Color.Red;
                        lat_value17.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[17]["paid"].ToString() == "true")
                    {
                        lat_m18.ForeColor = Color.Red;
                        lat_y18.ForeColor = Color.Red;
                        lat_mName18.ForeColor = Color.Red;
                        lat_value18.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[18]["paid"].ToString() == "true")
                    {
                        lat_m19.ForeColor = Color.Red;
                        lat_y19.ForeColor = Color.Red;
                        lat_mName19.ForeColor = Color.Red;
                        lat_value19.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[19]["paid"].ToString() == "true")
                    {
                        lat_m20.ForeColor = Color.Red;
                        lat_y20.ForeColor = Color.Red;
                        lat_mName20.ForeColor = Color.Red;
                        lat_value20.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[20]["paid"].ToString() == "true")
                    {
                        lat_m21.ForeColor = Color.Red;
                        lat_y21.ForeColor = Color.Red;
                        lat_mName21.ForeColor = Color.Red;
                        lat_value21.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[21]["paid"].ToString() == "true")
                    {
                        lat_m22.ForeColor = Color.Red;
                        lat_y22.ForeColor = Color.Red;
                        lat_mName22.ForeColor = Color.Red;
                        lat_value22.ForeColor = Color.Red;
                    }
                    if (monthsTable.Rows[22]["paid"].ToString() == "true")
                    {
                        lat_m23.ForeColor = Color.Red;
                        lat_y23.ForeColor = Color.Red;
                        lat_mName23.ForeColor = Color.Red;
                        lat_value23.ForeColor = Color.Red;
                    }

                    if (monthsTable.Rows[23]["paid"].ToString() == "true")
                    {
                        lat_m24.ForeColor = Color.Red;
                        lat_y24.ForeColor = Color.Red;
                        lat_mName24.ForeColor = Color.Red;
                        lat_value24.ForeColor = Color.Red;
                    }
                }

            }
        }

        private void pay_save_Click(object sender, EventArgs e)
        {
            savePrint = false;
            total_money = 0;
            save_func(); 

        }
        void save_func()
        {

            DialogResult dialogResult = MessageBox.Show("هل انت متاكد من اجراء هذه العمليه ؟", "رساله تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m1.Text) + " and year ='" + lat_y1.Text + "'", conn);
                    cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value1.Text));
                    cmd.Parameters.AddWithValue("@s2", lat_note1.Text);
                    cmd.ExecuteNonQuery();
                    if (lat_check1.Checked == true && monthsTable.Rows[0]["paid"].ToString() == "false")
                    {

                        cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m1.Text) + " and year ='" + lat_y1.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value1.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note1.Text);
                        cmd.ExecuteNonQuery();
                        //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                        //cmd2.ExecuteNonQuery();
                        lat_check1.Enabled = false;
                        lat_value1.Enabled = false;
                        lat_y1.ForeColor = Color.Red;
                        lat_m1.ForeColor = Color.Red;
                        lat_mName1.ForeColor = Color.Red;
                        lat_value1.ForeColor = Color.Red;
                        lat_check1.ForeColor = Color.Red;
                        cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                        cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                        cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value1.Text));
                        cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                        cmd.ExecuteNonQuery();
                        total_money = total_money + float.Parse(lat_value1.Text);

                    }

                    if (monthsNumber != 1)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m2.Text) + " and year ='" + lat_y2.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value2.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note2.Text);
                        cmd.ExecuteNonQuery();

                        if (lat_check2.Checked == true && monthsTable.Rows[1]["paid"].ToString() == "false")
                        {

                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m2.Text) + " and year ='" + lat_y2.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value2.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note2.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check2.Enabled = false;
                            lat_value2.Enabled = false;
                            lat_y2.ForeColor = Color.Red;
                            lat_m2.ForeColor = Color.Red;
                            lat_mName2.ForeColor = Color.Red;
                            lat_value2.ForeColor = Color.Red;
                            lat_check2.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value2.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value2.Text);

                        }
                    }

                    if (monthsNumber > 2)
                    {

                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m3.Text) + " and year ='" + lat_y3.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value3.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note3.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check3.Checked == true && monthsTable.Rows[2]["paid"].ToString() == "false")
                        {
                            //MessageBox.Show("1");
                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m3.Text) + " and year ='" + lat_y3.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value3.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note3.Text);
                            cmd.ExecuteNonQuery();
                            //MessageBox.Show("2");
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check3.Enabled = false;
                            lat_value3.Enabled = false;
                            lat_y3.ForeColor = Color.Red;
                            lat_m3.ForeColor = Color.Red;
                            lat_mName3.ForeColor = Color.Red;
                            lat_value3.ForeColor = Color.Red;
                            lat_check3.ForeColor = Color.Red;
                            //MessageBox.Show("3");
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value3.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            //MessageBox.Show("4");
                            total_money = total_money + float.Parse(lat_value3.Text);

                        }
                    }
                    if (monthsNumber > 3)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m4.Text) + " and year ='" + lat_y4.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value4.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note4.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check4.Checked == true && monthsTable.Rows[3]["paid"].ToString() == "false")
                        {


                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m4.Text) + " and year ='" + lat_y4.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value4.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note4.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check4.Enabled = false;
                            lat_value4.Enabled = false;
                            lat_y4.ForeColor = Color.Red;
                            lat_m4.ForeColor = Color.Red;
                            lat_mName4.ForeColor = Color.Red;
                            lat_value4.ForeColor = Color.Red;
                            lat_check4.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value4.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value4.Text);
                        }
                    }
                    if (monthsNumber > 4)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m5.Text) + " and year ='" + lat_y5.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value5.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note5.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check5.Checked == true && monthsTable.Rows[4]["paid"].ToString() == "false")
                        {

                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m5.Text) + " and year ='" + lat_y5.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value5.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note5.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check5.Enabled = false;
                            lat_value5.Enabled = false;
                            lat_y5.ForeColor = Color.Red;
                            lat_m5.ForeColor = Color.Red;
                            lat_mName5.ForeColor = Color.Red;
                            lat_value5.ForeColor = Color.Red;
                            lat_check5.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value5.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value5.Text);

                        }
                    }
                    if (monthsNumber > 5)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m6.Text) + " and year ='" + lat_y6.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value6.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note6.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check6.Checked == true && monthsTable.Rows[5]["paid"].ToString() == "false")
                        {

                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m6.Text) + " and year ='" + lat_y6.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value6.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note6.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check6.Enabled = false;
                            lat_value6.Enabled = false;
                            lat_y6.ForeColor = Color.Red;
                            lat_m6.ForeColor = Color.Red;
                            lat_mName6.ForeColor = Color.Red;
                            lat_value6.ForeColor = Color.Red;
                            lat_check6.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value6.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value6.Text);

                        }
                    }
                    if (monthsNumber > 6)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m7.Text) + " and year ='" + lat_y7.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value7.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note7.Text);
                        cmd.ExecuteNonQuery();

                        if (lat_check7.Checked == true && monthsTable.Rows[6]["paid"].ToString() == "false")
                        {

                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m7.Text) + " and year ='" + lat_y7.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value7.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note7.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check7.Enabled = false;
                            lat_value7.Enabled = false;
                            lat_y7.ForeColor = Color.Red;
                            lat_m7.ForeColor = Color.Red;
                            lat_mName7.ForeColor = Color.Red;
                            lat_value7.ForeColor = Color.Red;
                            lat_check7.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value7.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value7.Text);

                        }
                    }
                    if (monthsNumber > 7)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m8.Text) + " and year ='" + lat_y8.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value8.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note8.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check8.Checked == true && monthsTable.Rows[7]["paid"].ToString() == "false")
                        {

                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m8.Text) + " and year ='" + lat_y8.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value8.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note8.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check8.Enabled = false;
                            lat_value8.Enabled = false;
                            lat_y8.ForeColor = Color.Red;
                            lat_m8.ForeColor = Color.Red;
                            lat_mName8.ForeColor = Color.Red;
                            lat_value8.ForeColor = Color.Red;
                            lat_check8.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value8.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value8.Text);

                        }
                    }
                    if (monthsNumber > 8)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m9.Text) + " and year ='" + lat_y9.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value9.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note9.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check9.Checked == true && monthsTable.Rows[8]["paid"].ToString() == "false")
                        {

                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m9.Text) + " and year ='" + lat_y9.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value9.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note9.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd.ExecuteNonQuery();
                            lat_check9.Enabled = false;
                            lat_value9.Enabled = false;
                            lat_y9.ForeColor = Color.Red;
                            lat_m9.ForeColor = Color.Red;
                            lat_mName9.ForeColor = Color.Red;
                            lat_value9.ForeColor = Color.Red;
                            lat_check9.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value9.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value9.Text);

                        }
                    }

                    if (monthsNumber > 9)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m10.Text) + " and year ='" + lat_y10.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value10.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note10.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check10.Checked == true && monthsTable.Rows[9]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m10.Text) + " and year ='" + lat_y10.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value10.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note10.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            // cmd2.ExecuteNonQuery();
                            lat_check10.Enabled = false;
                            lat_value10.Enabled = false;
                            lat_y10.ForeColor = Color.Red;
                            lat_m10.ForeColor = Color.Red;
                            lat_mName10.ForeColor = Color.Red;
                            lat_value10.ForeColor = Color.Red;
                            lat_check10.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value10.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value10.Text);

                        }
                    }
                    if (monthsNumber > 10)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m11.Text) + " and year ='" + lat_y11.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value11.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note11.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check11.Checked == true && monthsTable.Rows[10]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m11.Text) + " and year ='" + lat_y11.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value11.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note11.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check11.Enabled = false;
                            lat_value11.Enabled = false;
                            lat_y11.ForeColor = Color.Red;
                            lat_m11.ForeColor = Color.Red;
                            lat_mName11.ForeColor = Color.Red;
                            lat_value11.ForeColor = Color.Red;
                            lat_check11.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value11.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value11.Text);

                        }
                    }
                    if (monthsNumber > 11)
                    {
                        cmd = new MySqlCommand("update months set  monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m12.Text) + " and year ='" + lat_y12.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value12.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note12.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check12.Checked == true && monthsTable.Rows[11]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m12.Text) + " and year ='" + lat_y12.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value12.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note12.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check12.Enabled = false;
                            lat_value12.Enabled = false;
                            lat_y12.ForeColor = Color.Red;
                            lat_m12.ForeColor = Color.Red;
                            lat_mName12.ForeColor = Color.Red;
                            lat_value12.ForeColor = Color.Red;
                            lat_check12.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value12.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value12.Text);

                        }
                    }
                    if (monthsNumber > 12)
                    {
                        cmd = new MySqlCommand("update months set  monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m13.Text) + " and year ='" + lat_y13.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value13.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note13.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check13.Checked == true && monthsTable.Rows[12]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m13.Text) + " and year ='" + lat_y13.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value13.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note13.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check13.Enabled = false;
                            lat_value13.Enabled = false;
                            lat_y13.ForeColor = Color.Red;
                            lat_m13.ForeColor = Color.Red;
                            lat_mName13.ForeColor = Color.Red;
                            lat_value13.ForeColor = Color.Red;
                            lat_check13.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value13.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value13.Text);

                        }
                    }
                    if (monthsNumber > 13)
                    {
                        cmd = new MySqlCommand("update months set  monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m14.Text) + " and year ='" + lat_y14.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value14.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note14.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check14.Checked == true && monthsTable.Rows[13]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m14.Text) + " and year ='" + lat_y14.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value14.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note14.Text);
                            cmd.ExecuteNonQuery();
                            // MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check14.Enabled = false;
                            lat_value14.Enabled = false;
                            lat_y14.ForeColor = Color.Red;
                            lat_m14.ForeColor = Color.Red;
                            lat_mName14.ForeColor = Color.Red;
                            lat_value14.ForeColor = Color.Red;
                            lat_check14.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value14.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value14.Text);

                        }
                    }
                    if (monthsNumber > 14)
                    {

                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m15.Text) + " and year ='" + lat_y15.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value15.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note15.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check15.Checked == true && monthsTable.Rows[14]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m15.Text) + " and year ='" + lat_y15.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value15.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note15.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check15.Enabled = false;
                            lat_value15.Enabled = false;
                            lat_y15.ForeColor = Color.Red;
                            lat_m15.ForeColor = Color.Red;
                            lat_mName15.ForeColor = Color.Red;
                            lat_value15.ForeColor = Color.Red;
                            lat_check15.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value15.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value15.Text);

                        }
                    }
                    if (monthsNumber > 15)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m16.Text) + " and year ='" + lat_y16.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value16.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note16.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check16.Checked == true && monthsTable.Rows[15]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m16.Text) + " and year ='" + lat_y16.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value16.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note16.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check16.Enabled = false;
                            lat_value16.Enabled = false;
                            lat_y16.ForeColor = Color.Red;
                            lat_m16.ForeColor = Color.Red;
                            lat_mName16.ForeColor = Color.Red;
                            lat_value16.ForeColor = Color.Red;
                            lat_check16.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value16.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value16.Text);

                        }
                    }
                    if (monthsNumber > 16)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m17.Text) + " and year ='" + lat_y17.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value17.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note17.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check17.Checked == true && monthsTable.Rows[16]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m17.Text) + " and year ='" + lat_y17.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value17.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note17.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check17.Enabled = false;
                            lat_value17.Enabled = false;
                            lat_y17.ForeColor = Color.Red;
                            lat_m17.ForeColor = Color.Red;
                            lat_mName17.ForeColor = Color.Red;
                            lat_value17.ForeColor = Color.Red;
                            lat_check17.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value17.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value17.Text);

                        }
                    }
                    if (monthsNumber > 17)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m18.Text) + " and year ='" + lat_y18.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value18.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note18.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check18.Checked == true && monthsTable.Rows[17]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m18.Text) + " and year ='" + lat_y18.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value18.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note18.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check18.Enabled = false;
                            lat_value18.Enabled = false;
                            lat_y18.ForeColor = Color.Red;
                            lat_m18.ForeColor = Color.Red;
                            lat_mName18.ForeColor = Color.Red;
                            lat_value18.ForeColor = Color.Red;
                            lat_check18.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value18.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value18.Text);
                        }
                    }
                    if (monthsNumber > 18)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m19.Text) + " and year ='" + lat_y19.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value19.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note19.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check19.Checked == true && monthsTable.Rows[18]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m19.Text) + " and year ='" + lat_y19.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value19.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note19.Text);
                            cmd.ExecuteNonQuery();
                            // MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check19.Enabled = false;
                            lat_value19.Enabled = false;
                            lat_y19.ForeColor = Color.Red;
                            lat_m19.ForeColor = Color.Red;
                            lat_mName19.ForeColor = Color.Red;
                            lat_value19.ForeColor = Color.Red;
                            lat_check19.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value19.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value19.Text);
                        }
                    }
                    if (monthsNumber > 19)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m20.Text) + " and year ='" + lat_y20.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value20.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note20.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check20.Checked == true && monthsTable.Rows[19]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m20.Text) + " and year ='" + lat_y20.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value20.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note20.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd.ExecuteNonQuery();
                            lat_check20.Enabled = false;
                            lat_value20.Enabled = false;
                            lat_y20.ForeColor = Color.Red;
                            lat_m20.ForeColor = Color.Red;
                            lat_mName20.ForeColor = Color.Red;
                            lat_value20.ForeColor = Color.Red;
                            lat_check20.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value20.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value20.Text);
                        }
                    }
                    if (monthsNumber > 20)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m21.Text) + " and year ='" + lat_y21.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value21.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note21.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check21.Checked == true && monthsTable.Rows[20]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true', monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m21.Text) + " and year ='" + lat_y21.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value21.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note21.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd.ExecuteNonQuery();
                            lat_check21.Enabled = false;
                            lat_value21.Enabled = false;
                            lat_y21.ForeColor = Color.Red;
                            lat_m21.ForeColor = Color.Red;
                            lat_mName21.ForeColor = Color.Red;
                            lat_value21.ForeColor = Color.Red;
                            lat_check21.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value21.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value21.Text);
                        }
                    }
                    if (monthsNumber > 21)
                    {
                        cmd = new MySqlCommand("update months set  monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m22.Text) + " and year ='" + lat_y22.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value22.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note22.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check22.Checked == true && monthsTable.Rows[21]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m22.Text) + " and year ='" + lat_y22.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value22.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note22.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd.ExecuteNonQuery();
                            lat_check22.Enabled = false;
                            lat_value22.Enabled = false;
                            lat_y22.ForeColor = Color.Red;
                            lat_m22.ForeColor = Color.Red;
                            lat_mName22.ForeColor = Color.Red;
                            lat_value22.ForeColor = Color.Red;
                            lat_check22.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value22.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value22.Text);
                        }
                    }
                    if (monthsNumber > 22)
                    {
                        cmd = new MySqlCommand("update months set  monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m23.Text) + " and year ='" + lat_y23.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value23.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note23.Text);
                        cmd.ExecuteNonQuery();
                        if (lat_check23.Checked == true && monthsTable.Rows[22]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m23.Text) + " and year ='" + lat_y23.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value23.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note23.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            //cmd2.ExecuteNonQuery();
                            lat_check23.Enabled = false;
                            lat_value23.Enabled = false;
                            lat_y23.ForeColor = Color.Red;
                            lat_m23.ForeColor = Color.Red;
                            lat_mName23.ForeColor = Color.Red;
                            lat_value23.ForeColor = Color.Red;
                            lat_check23.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value23.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value23.Text);
                        }
                    }
                    if (monthsNumber > 23)
                    {
                        cmd = new MySqlCommand("update months set monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m24.Text) + " and year ='" + lat_y24.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value24.Text));
                        cmd.Parameters.AddWithValue("@s2", lat_note24.Text);
                        cmd.ExecuteNonQuery();

                        if (lat_check24.Checked == true && monthsTable.Rows[23]["paid"].ToString() == "false")
                        {
                            cmd = new MySqlCommand("update months set paid='true' , monthValue = @s1 , note = @s2 where cid= " + paid_cid + " and monthNumber =" + int.Parse(lat_m24.Text) + " and year ='" + lat_y24.Text + "'", conn);
                            cmd.Parameters.AddWithValue("@s1", float.Parse(lat_value24.Text));
                            cmd.Parameters.AddWithValue("@s2", lat_note24.Text);
                            cmd.ExecuteNonQuery();
                            //MySqlCommand cmd2 = new MySqlCommand("update client set cashNum = cashNum+1 , cashRemain = cashRemain-1 where id =" + paid_cid + "", conn);
                            // cmd2.ExecuteNonQuery();
                            lat_check24.Enabled = false;
                            lat_value24.Enabled = false;
                            lat_y24.ForeColor = Color.Red;
                            lat_m24.ForeColor = Color.Red;
                            lat_mName24.ForeColor = Color.Red;
                            lat_value24.ForeColor = Color.Red;
                            lat_check24.ForeColor = Color.Red;
                            cmd = new MySqlCommand("insert into income(date,money,name) values (@s1,@s2,@s3)", conn);
                            cmd.Parameters.AddWithValue("@s1", DateTime.Now);
                            cmd.Parameters.AddWithValue("@s2", float.Parse(lat_value24.Text));
                            cmd.Parameters.AddWithValue("@s3", paid_list.SelectedItem.ToString());
                        
                            cmd.ExecuteNonQuery();
                            total_money = total_money + float.Parse(lat_value24.Text);

                        }
                    }


                    //set();
                    bill_clineName = label_cName.Text;
                    // init();
                    adapter = new MySqlDataAdapter("select * from  client where id=" + paid_cid + " ", conn);
                    MySqlCommandBuilder cb2 = new MySqlCommandBuilder();
                    cb2.DataAdapter = adapter;
                    dt = new DataTable();
                    adapter.Fill(dt);
                    DataRow row = dt.Rows[0];
                    cmd = new MySqlCommand("select count(*) from months where months.cid = " + paid_cid + " and months.paid = 'true'", conn);
                    int months_paid = int.Parse(cmd.ExecuteScalar().ToString());
                    cmd = new MySqlCommand("select count(*) from months where months.cid = " + paid_cid + " and months.paid = 'false'", conn);
                    int months_remain = int.Parse(cmd.ExecuteScalar().ToString());

                    cmd = new MySqlCommand("update client set cashNum = " + months_paid + " , cashRemain= " + months_remain + " where id =" + paid_cid + "", conn);
                    cmd.ExecuteNonQuery();
                    //paidSaerch();
                    getmonthsTable();
                    cmd = new MySqlCommand("update client set theRest= " + float.Parse(client_total.Text) + " where id =" + paid_cid + "", conn);
                    cmd.ExecuteNonQuery();

                    cmd = new MySqlCommand("insert into bill (client_id,months_paid,months_remain,theReset,date,paid)values(@v1,@v2,@v3,@v4,@v5,@v6)", conn);
                    cmd.Parameters.AddWithValue("@v1", paid_cid);
                    cmd.Parameters.AddWithValue("@v2", months_paid);
                    cmd.Parameters.AddWithValue("@v3", months_remain);
                    cmd.Parameters.AddWithValue("@v4", float.Parse(client_total.Text));
                    cmd.Parameters.AddWithValue("@v5", DateTime.Now);
                    cmd.Parameters.AddWithValue("@v6", total_money);

                    cmd.ExecuteNonQuery();

                    if (savePrint)
                    {
                        printFunc();
                    }
                    
                    //list_detailRefresh();
                    //MessageBox.Show("يتم الان الحفظ الرجاء الانتظار");
                    // refreshLate();
                    MessageBox.Show("تم الحفظ بنجاح");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
            else if (dialogResult == DialogResult.No)
            {

            }
        }

      
        
        void refreshLate()
        {
            try
            {
                DateTime current = DateTime.Now;
                int currentDay = int.Parse(current.Day.ToString());
                int currentMonth = int.Parse(current.Month.ToString());
                int currentyear = int.Parse(current.Year.ToString());

                adapter = new MySqlDataAdapter("select * from client where late=1 and cashRemain!=0", conn);
             
                MySqlCommandBuilder cb2 = new MySqlCommandBuilder();
                cb2.DataAdapter = adapter;
                dt = new DataTable();
                adapter.Fill(dt);

                int length = dt.Rows.Count;

                for (int i = 0; i < length; i++)
                {
                   
                    DataRow row = dt.Rows[i];
                    int id = int.Parse(row["id"].ToString());
                    MySqlDataAdapter ad_year = new MySqlDataAdapter("select year  from months where  months.cid=" + id + " and months.paid='true' order by months.year DESC", conn);
                   
                    DataTable tt = new DataTable();
                    ad_year.Fill(tt);
                    if (tt.Rows.Count == 0)
                        continue;
                    MySqlDataAdapter ad = new MySqlDataAdapter("select DateTime from months where months.cid=" + id + " and months.paid='true' and months.year= '" + tt.Rows[0]["year"].ToString() + "' order by months.monthNumber DESC", conn);
                    DataTable dt2 = new DataTable();
                    ad.Fill(dt2);
                    //MessageBox.Show("hi");
                    if (dt2.Rows.Count > 0)
                    {

                        DateTime date = DateTime.Parse(dt2.Rows[0]["DateTime"].ToString());
                        int last_year = int.Parse(date.Year.ToString());
                        int lasy_month = int.Parse(date.Month.ToString());
                        int last_day = int.Parse(date.Day.ToString());
                        //MessageBox.Show(dt2.Rows[0]["name"].ToString()+currentyear + " " + currentMonth  + " " + last_year + " " + lasy_month );
                        if (currentyear<last_year || (currentyear == last_year && currentMonth <= lasy_month+1 )||(currentyear == last_year && currentMonth == lasy_month+2 && currentDay<=last_day)||(currentyear == last_year + 1 && currentMonth == 2 && lasy_month == 12 && currentDay<=last_day)||(currentyear == last_year + 1&& currentMonth == 1 && lasy_month == 11 && currentDay <= last_day))
                        {
                           // MessageBox.Show(id + "");
                            cmd = new MySqlCommand("Update client set late=0 where id=" + id + "", conn);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            
                        }
                    }
                    else if (dt2.Rows.Count == 0)
                    {
                        //MessageBox.Show("hi");

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            setInVisible();
            checkDay(1);
            refreshLate();
            findLate();
            MessageBox.Show("تم الانتهاء نشكرك على الانتظار ");
        }

        private void lat_note1_TextChanged(object sender, EventArgs e)
        {

           
        }

        private void lat_value1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txt_monthValue_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txt_monthValue_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txt_cachFirst_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txt_monthsNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void txt_cDaphter_TextChanged(object sender, EventArgs e)
        {

        }

        private void lat_value1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value4_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value5_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value6_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value7_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value8_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value9_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value10_TextChanged(object sender, EventArgs e)
        {

        }

        private void lat_value10_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value11_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value12_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value13_TextChanged(object sender, EventArgs e)
        {

        }

        private void lat_value13_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value14_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value15_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value16_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value17_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value18_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value19_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value20_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value21_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value22_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value23_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void lat_value24_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void btn_showResult_Click(object sender, EventArgs e)
        {
            try
            {
                float total = 0;
                adapter = new MySqlDataAdapter("select date as التاريخ ,money as المبلغ , name as 'اسم العميل'   from income where date BETWEEN '" + dateFrom.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTo.Value.ToString("yyyy-MM-dd") + "'", conn);
                DataTable tt = new DataTable();
                adapter.Fill(tt);
                dataGridView1.DataSource = tt;

                //MessageBox.Show(tt.Rows.Count+" "+ dateFrom.Value.Date+" "+ dateTo.Value.ToString());
                for (int i = 0; i < tt.Rows.Count; i++)
                {
                    total += float.Parse(tt.Rows[i]["المبلغ"].ToString());
                }

                totalIncome.Text = total.ToString("0.00");

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            setInVisible_paid();
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void تسجيلالخروجToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 ff = new Form1();
            ff.Show();
        }

        private void تعديلالحسابToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new changecs().Show();
            this.Hide();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {

        }

        private void txt_profit_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!char.IsDigit(c) && c != 8 && c != 27 && c != 25 && c != 26 && c != 28 && c != 9 && c != '.')
            {
                e.Handled = true;
                MessageBox.Show("خلى بالك من القيمه اللى حضرتك مدخلها");
            }
        }

        private void txt_profit_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_productPrice.Text != "" && txt_monthsNumber.Text != "" && txt_cachFirst.Text != "" && txt_productPrice.Text != " " && txt_monthsNumber.Text != " " && txt_cachFirst.Text != " ")
            {
                txt_monthValue.Text = calculateMonthsNumber() + "";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int index = list_late.SelectedIndex;
            if (index >= 0) {
                int id = late_id[index];
                if (checkBox_hasA.Checked)
                {
                    cmd = new MySqlCommand("update client set hasA=1 where id =" + id + "", conn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم حفظ التعديلات");
                }
                else
                {
                    cmd = new MySqlCommand("update client set hasA=0 where id =" + id + "", conn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم حفظ التعديلات");
                }

              }
            else
            {
                MessageBox.Show("معلش اختار عميل لحفظ التعديلات");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            setInVisible();
            findhasA();
        }

        private void انشاءنسخهاحتياطيهToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog folderBrowser = new OpenFileDialog();
                // Set validate names and check file exists to false otherwise windows will
                // not let you select "Folder Selection."
                folderBrowser.ValidateNames = false;
                folderBrowser.CheckFileExists = false;
                folderBrowser.CheckPathExists = true;
                // Always default to Folder Selection.
                folderBrowser.FileName = "Folder Selection.";
              
                
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                    MessageBox.Show(folderPath+ "\\mobile" + DateTime.Now.ToString("yyyy,mm,dd-HH,mm,ss") + ".sql");
                    conn.Close();
                    using (MySqlConnection con = new MySqlConnection(connStr))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = con;
                                con.Open();
                                mb.ExportToFile(folderPath + "\\mobile" + DateTime.Now.ToString("yyyy,MM,dd-HH,mm,ss") + ".sql");
                                con.Close();
                                
                            }
                        }
                    }
                   
                    // cmd = new MySqlCommand(@"BACKUP DATABASE mobile1 TO DISK = '" + ,conn);
                    // cmd.ExecuteNonQuery();
                    MessageBox.Show("تم انشاء نسخه احتياطيه بنجاح");
                    conn.Open();
                    // ...
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void استعادهنسخهمحفوظهToolStripMenuItem_Click(object sender, EventArgs e)
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
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

              
            }
        }

        private void txt_monthValue_KeyUp_1(object sender, KeyEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                float total = 0;
                adapter = new MySqlDataAdapter("select name as 'الاسم',theRest as 'القيمة المتبقيه' from client where theRest!=0", conn);
                DataTable tt = new DataTable();
                adapter.Fill(tt);
                gride_out.DataSource = tt;
                for (int i = 0; i < tt.Rows.Count; i++)
                {
                    total += (float.Parse(tt.Rows[i]["القيمة المتبقيه"].ToString()));
                }

                txt_out_all.Text = total.ToString("0.00");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_cName_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_cId_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_cPhone_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_cDaphter_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_cAddress_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_watherName_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_watcherPhone_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_productName_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_productPrice_KeyDown(object sender, KeyEventArgs e)
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

        private void dateTimePicker1_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_monthsNumber_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_cachFirst_KeyDown(object sender, KeyEventArgs e)
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

        private void txt_monthValue_KeyDown(object sender, KeyEventArgs e)
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            string text = txt_mynotes.Text;

            cmd = new MySqlCommand("update mynote set body='" + text + "' where id=1",conn);
            cmd.ExecuteNonQuery();

            MessageBox.Show("done");
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void txt_productPrice_TextChanged(object sender, EventArgs e)
        {
            if(txt_productPrice.Text!=""&&txt_cachFirst.Text!="")
            {
                double total = int.Parse(txt_productPrice.Text);
                double paid = int.Parse(txt_cachFirst.Text);
                double remain = total - paid;
                double after = remain * 25 / 100 + remain;
                txt_remain_client.Text = remain.ToString();
                txt_afterpaid_client.Text = after.ToString();
            }
                
                    
        }

        private void txt_afterpaid_client_TextChanged(object sender, EventArgs e)
        {
         
        }

        private void dateByDay_TextChanged(object sender, EventArgs e)
        {
            if (int.Parse(dateByDay.Text) > 28)
            {
                MessageBox.Show("عدا 29و30و31");

            }
        }

    void printFunc()
    {
        try
        {

            ((Form)printPreviewDialog1).WindowState = FormWindowState.Maximized;
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {

                printDocument1.Print();


            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
        private void button6_Click(object sender, EventArgs e)
        {
            total_money = 0;
             savePrint = true;
             save_func();
            
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            
            adapter = new MySqlDataAdapter("select * from bill order by id desc limit 1 ", conn);
            DataTable billTable = new DataTable();
            adapter.Fill(billTable);


            Font f1 = new Font("Arial", 24, FontStyle.Bold);
            Font f2 = new Font("Arial", 20, FontStyle.Bold);
            Font f3 = new Font("Arial", 16, FontStyle.Bold);
            Font f4 = new Font("Arial", 13, FontStyle.Bold);
            Font f5 = new Font("Arial", 10, FontStyle.Bold);

            float margin = 30;

            string StoreName = "مؤسسة اليسر";
            string StoreInfo = "(للأجهزة الكهربائية , والادوات المنزلية , والتلفون المحمول )";
            string address = "إيصال سداد";        
            string line = "___________________";
            string clientname1 = "اسم العميل";
            string clientname2 = bill_clineName;
            string bill_no = "رقم الفاتورة :" + billTable.Rows[0]["id"].ToString();
            string str_paid = "المبلغ المدفوع : "+ billTable.Rows[0]["paid"].ToString();
            string str_remain = "المبلغ المتبقى : "+ billTable.Rows[0]["theReset"].ToString();
            string str_monthPaid = "عدد الأشهر المدفوعة : "+ billTable.Rows[0]["months_paid"].ToString();
            string str_monthRemain = "عدد الأشهر المتبقية : "+ billTable.Rows[0]["months_remain"].ToString() ;
            string str_dateOnly = ((DateTime)billTable.Rows[0]["date"]).ToString("yyyy-MM-dd");
            string str_date = "تحريرا فى "+ str_dateOnly;
            string str_m1 = "الرجاء الاحتفاظ بالإيصال لحين الانتهاء من سداد جميع الاقساط";
            string str_m2 = "إدارة";
            string str_m3 = "علي كمال الخطيب /01143463955 ---أحمد السويفي /01102711500";
            string str_m4 = "شكرا لزيارتكم لمعرض مؤسسة اليسر ";



            SizeF fzStoreName = e.Graphics.MeasureString(StoreName,f1);
            SizeF fzStoreInfo = e.Graphics.MeasureString(StoreInfo, f5);
            SizeF fzAddree = e.Graphics.MeasureString(address, f2);
            SizeF fzName1 = e.Graphics.MeasureString(clientname1, f3);
            SizeF fzName2 = e.Graphics.MeasureString(clientname2, f4);
            SizeF fzBillNo = e.Graphics.MeasureString(bill_no, f3);
            SizeF fzLine = e.Graphics.MeasureString(line, f1);
            SizeF fzPaid = e.Graphics.MeasureString(str_paid, f3);
            SizeF fzRemain = e.Graphics.MeasureString(str_remain, f3);
            SizeF fzMonthPaid = e.Graphics.MeasureString(str_monthPaid, f3);
            SizeF fzMonthRemain = e.Graphics.MeasureString(str_monthRemain, f3);
            SizeF fzDate = e.Graphics.MeasureString(str_date, f3);
            SizeF fzM1 = e.Graphics.MeasureString(str_m1, f5);
            SizeF fzM2 = e.Graphics.MeasureString(str_m2, f3);
            SizeF fzM3 = e.Graphics.MeasureString(str_m3, f5);
            SizeF fzM4 = e.Graphics.MeasureString(str_m4, f5);


            //Draw Store name

            e.Graphics.DrawString(StoreName, f1,Brushes.Black, (e.PageBounds.Width - fzStoreName.Width) / 2, margin);
            e.Graphics.DrawString(StoreInfo, f5, Brushes.Black, (e.PageBounds.Width - fzStoreInfo.Width) / 2, margin+ fzStoreName.Height);
            e.Graphics.DrawString(address, f2, Brushes.Black, (e.PageBounds.Width - fzAddree.Width)/2, margin + fzStoreName.Height+ fzStoreInfo.Height);
            e.Graphics.DrawString(line, f1, Brushes.Black, (e.PageBounds.Width - fzLine.Width ) / 2,   fzStoreName.Height+fzAddree.Height+ fzStoreInfo.Height);

            e.Graphics.DrawString(bill_no, f3, Brushes.Black, (e.PageBounds.Width - fzBillNo.Width-margin) ,fzLine.Height+ fzAddree.Height+ margin+fzStoreName.Height+ fzStoreInfo.Height);
            e.Graphics.DrawString(clientname1, f3, Brushes.Black, (e.PageBounds.Width - fzName1.Width-margin) , margin + fzLine.Height+ fzAddree.Height+ fzStoreName.Height+fzBillNo.Height+ fzStoreInfo.Height);
            e.Graphics.DrawString(clientname2, f4, Brushes.Black, (e.PageBounds.Width - fzName2.Width - margin), margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height+ fzName1.Height);
            e.Graphics.DrawString(str_paid, f3, Brushes.Black, (e.PageBounds.Width - fzPaid.Width - margin), margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height+fzName1.Height+ fzName2.Height);
            e.Graphics.DrawString(str_remain, f3, Brushes.Black, (e.PageBounds.Width - fzRemain.Width - margin), margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height+ fzName1.Height+fzRemain.Height+ fzName2.Height);
            e.Graphics.DrawString(str_monthPaid, f3, Brushes.Black, (e.PageBounds.Width - fzMonthPaid.Width - margin), margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height + fzName1.Height + fzPaid.Height+fzRemain.Height+ fzName2.Height);
            e.Graphics.DrawString(str_monthRemain, f3, Brushes.Black, (e.PageBounds.Width - fzMonthRemain.Width - margin), margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height + fzName1.Height + fzPaid.Height + fzRemain.Height+fzMonthPaid.Height+ fzName2.Height);
            
            e.Graphics.DrawString(str_date, f3, Brushes.Black, (e.PageBounds.Width - fzDate.Width )/2, margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height + fzName1.Height + fzPaid.Height + fzRemain.Height + fzMonthPaid.Height+fzMonthRemain.Height + fzName2.Height);
            e.Graphics.DrawString(line, f1, Brushes.Black, (e.PageBounds.Width - fzLine.Width) / 2, fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height + fzName1.Height + fzPaid.Height + fzRemain.Height + fzMonthPaid.Height + fzLine.Height+fzDate.Height+ fzName2.Height);
            e.Graphics.DrawString(str_m1, f5, Brushes.Black, (e.PageBounds.Width - fzM1.Width )/2, margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height + fzName1.Height + fzPaid.Height + fzRemain.Height + fzMonthPaid.Height + fzMonthRemain.Height+fzDate.Height + fzLine.Height+ fzName2.Height);
            e.Graphics.DrawString(str_m2, f3, Brushes.Black, (e.PageBounds.Width - fzM2.Width )/2, margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height + fzName1.Height + fzPaid.Height + fzRemain.Height + fzMonthPaid.Height + fzMonthRemain.Height + fzDate.Height+ fzM1.Height + fzLine.Height+ fzName2.Height);
            e.Graphics.DrawString(str_m3, f5, Brushes.Black, (e.PageBounds.Width - fzM3.Width )/2, margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height + fzName1.Height + fzPaid.Height + fzRemain.Height + fzMonthPaid.Height + fzMonthRemain.Height + fzDate.Height + fzM1.Height+fzM2.Height + fzLine.Height+ fzName2.Height);
            e.Graphics.DrawString(str_m4, f5, Brushes.Black, (e.PageBounds.Width - fzM4.Width) / 2, margin + fzLine.Height + fzAddree.Height + fzStoreName.Height + fzBillNo.Height + fzStoreInfo.Height + fzName1.Height + fzPaid.Height + fzRemain.Height + fzMonthPaid.Height + fzMonthRemain.Height + fzDate.Height + fzM1.Height + fzM2.Height + fzLine.Height + fzName2.Height+ fzM3.Height);
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            try {
                
                adapter = new MySqlDataAdapter("select bill.id as 'رقم الفاتورة',client.name as 'اسم العميل',bill.paid as 'المدفوع',bill.theReset as 'المتبقي',bill.months_paid as 'الاشهر المدفوعة',bill.months_remain as 'الاشهر المتبقية',bill.date as 'تاريخ الدفع' from bill,client where bill.client_id = client.id;", conn);
                DataTable tt = new DataTable();
                adapter.Fill(tt);
                bill_dgv.DataSource = tt;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                
                adapter = new MySqlDataAdapter("select bill.id as 'رقم الفاتورة',client.name as 'اسم العميل',bill.paid as 'المدفوع',bill.theReset as 'المتبقي',bill.months_paid as 'الاشهر المدفوعة',bill.months_remain as 'الاشهر المتبقية',bill.date as 'تاريخ الدفع' from bill,client where bill.client_id = client.id and bill.client_id=" + int.Parse(txt_clientId.Text)+"", conn);
                DataTable tt = new DataTable();
                adapter.Fill(tt);
                bill_dgv.DataSource = tt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {

                adapter = new MySqlDataAdapter("select bill.id as 'رقم الفاتورة',client.name as 'اسم العميل',bill.paid as 'المدفوع',bill.theReset as 'المتبقي',bill.months_paid as 'الاشهر المدفوعة',bill.months_remain as 'الاشهر المتبقية',bill.date as 'تاريخ الدفع' from bill,client where bill.client_id = client.id and bill.id=" + int.Parse(txt_billNo.Text) + "", conn);
                DataTable tt = new DataTable();
                adapter.Fill(tt);
                bill_dgv.DataSource = tt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
