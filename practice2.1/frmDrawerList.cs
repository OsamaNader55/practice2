using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace practice2._1
{
    public partial class frmDrawerList : frmMaster
    {
        public frmDrawerList()
        {
            InitializeComponent();
        }

        private void frmDrawerList_Load(object sender, EventArgs e)
        {
            //btnSave.Enabled = false;    
            //btnDelete.Enabled = false; 
            Refreshdata();
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["AccountID"].Visible = false;
            gridView1.Columns["Name"].Caption = "اســم الخزنـــه";
            gridView1.OptionsBehavior.Editable = false;
            gridView1.DoubleClick += GridView1_DoubleClick;
           
        }
        //How to make a double click event on DevGridView
        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                int id = Convert.ToInt32(view.GetFocusedRowCellValue("ID"));
                frmDrawer frm = new frmDrawer(id);
                frmMain.OpenFormWithPermissions(frm, true);
                Refreshdata();
            }
        }
        void Refreshdata()
        {
            using (var db = new dbDataContext())
            {
                gridControl1.DataSource = db.Drawers.ToList();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
           frmDrawer frm = new frmDrawer();
            frmMain.OpenFormWithPermissions(frm, true);
        }
    }
}
