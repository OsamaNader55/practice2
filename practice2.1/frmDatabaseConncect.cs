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
using Sales;

namespace practice2._1
{
    public partial class frmDatabaseConncect : frmMaster
    {
        public frmDatabaseConncect()
        {
            InitializeComponent();
            radioWindows.CheckedChanged += RadioWindows_CheckedChanged;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
           simpleButton1.PerformClick();
            if (error == 1) return;
           Sales. Properties.Settings.Default.UserConnectionString = txtConnectionString.Text;
            Sales. Properties.Settings.Default.Save();
            MessageBox.Show("تم ضبط الاتصال ");
            
        }

        private void frmDatabaseConncect_Load(object sender, EventArgs e)
        {
            radioWindows.Checked = true;
        }

        private void RadioWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (radioWindows.Checked)
            {
                layoutControlItem3.Enabled=layoutControlItem4.Enabled=false;
            }
            else
            {
                layoutControlItem3.Enabled = layoutControlItem4.Enabled = true;
            }
        }
        int error = 0;
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (txtServer.Text == string.Empty|| txtDatabase.Text == string.Empty)
            {
                MessageBox.Show("من فضلك ادخل البيانات اولا");
                error = 1;
                return;
            }
            error = 0;
            string server = txtServer.Text;
            string database = txtDatabase.Text;
            string username = txtUserName.Text;
            string password = txtPassword.Text;
            if (radioWindows.Checked)
            {
               txtConnectionString.Text = "Data Source=" + server + ";Initial Catalog=" + database + ";Integrated Security=True";
                
            }
            else
            {
                txtConnectionString.Text = "Data Source=" + server + ";Initial Catalog=" + database + ";User Id=" + username + ";Password=" + password + ";";
               
            }
        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            if (txtConnectionString.Text == String.Empty)
            {
                simpleButton1.PerformClick();
                if (error==1) return;

            }
            using (var db = new dbDataContext(txtConnectionString.Text))
            {
               
                try
                {
                    db.Connection.Open();
                    db.Connection.Close();
                    MessageBox.Show("نجح الاتصال");
                }
                catch (SqlException)
                {
                    MessageBox.Show("فشل الاتصال , برجاء التاكد من البيانات او نص الاتصال");
                    return;
                }

            }
          
        }
    }
}
