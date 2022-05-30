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
    
    public partial class frmRestore : frmMaster
    {
        SqlConnection conn = new SqlConnection(Properties.Settings.Default.UserConnectionString);
        public frmRestore()
        {
            InitializeComponent();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            string query = null;
            if (txtPath.Text == String.Empty)
            {
                MessageBox.Show("هذا الحقل مطلوب ومن فضلك تاكد من صحه المسار");
                return;
            }
            //
            using (var db = new dbDataContext())
            {
                if (db.DatabaseExists() == false)
                {
                    string x = Properties.Settings.Default.UserConnectionString;
                    x = x.Replace("Store2","master");
                    conn.ConnectionString = x;
                     query = "Restore Database Store2 From Disk='" + txtPath.Text + "' WITH REPLACE;alter database Store2 SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;";
                }
                else query = "ALTER Database Store2 SET OFFLINE WITH ROLLBACK IMMEDIATE; Restore Database Store2 From Disk='" + txtPath.Text + "' WITH REPLACE;alter database Store2 SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;";
            }
            
            //or this query ALTER DATABASE [DatabaseName] SET Single_User WITH Rollback Immediate GO;
            //  RESTORE DATABASE DatabaseName FROM DISK = 'C:\DBName-Full Database Backup' WITH REPLACE;
            //ALTER DATABASE[DatabaseName] SET Multi_User GO;
            var cmd = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("تم استعاده النسخه الاحتياطيه في المسار بنجاح");
            }
            catch 
            {
                MessageBox.Show("لم يتم تنفيذ الامر لحدوث خطأ ما ");
                conn.Close();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog =new OpenFileDialog();

            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = fileDialog.FileName;
            }
        }
    }
}
