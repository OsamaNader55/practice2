using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace practice2._1
{
    public partial class frmBackUp : frmMaster
    {
        SqlConnection conn=new SqlConnection(Properties.Settings.Default.UserConnectionString);
        public frmBackUp()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (txtPath.Text == String.Empty)
            {
                MessageBox.Show("هذا الحقل مطلوب ومن فضلك تاكد من صحه المسار");
                return;
            }
            string filename= txtPath.Text+ "\\Store2" + DateTime.Now.ToShortDateString().Replace('/', '-') + "-" + DateTime.Now.ToLongTimeString().Replace(':', '-');
            string defaultpath = @"C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\Backup\Store2" + DateTime.Now.ToShortDateString().Replace('/', '-') + "-" + DateTime.Now.ToLongTimeString().Replace(':', '-'); 
            string query = "Backup Database Store2 to Disk='"+defaultpath+".bak';Backup Database Store2 to Disk='" + filename+".bak';";
            var cmd = new SqlCommand(query,conn);
           
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("تم انشاء النسخه الاحتياطيه في المسار بنجاح");
            }
            catch { MessageBox.Show("لم يتم تنفيذ الامر لحدوث خطأ ما ");
                conn.Close();
            }

        }
    }
}
