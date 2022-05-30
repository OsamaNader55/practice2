using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace practice2._1
{
    public partial class frmStoreList : frmMaster
    {
        public frmStoreList()
        {
            InitializeComponent();
           
        }
        private void frmStoreList_Load(object sender, EventArgs e)
        {
            refresh();
            dataGridView1.Columns["ID"].Visible = false;
            dataGridView1.Columns["Name"].HeaderText = "اســم المخـــزن";
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
         int   id =Convert.ToInt32( dataGridView1.Rows[e.RowIndex].Cells["ID"].Value);   
            frmStores frm = new frmStores(id);
            frm.ShowDialog();
            refresh();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            refresh();
        }
       public void refresh()
        {
            dbDataContext db = new dbDataContext();
            dataGridView1.DataSource =Session.Store.Select(x => new { x.Name, x.ID }).ToList();
            
           
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            frmStores frm = new frmStores();
            frmMain.OpenFormWithPermissions(frm, true);
            refresh();
        }
    }
    
}
