﻿using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
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
using Sales;
namespace practice2._1
{
    public partial class frmAccessProfileList : XtraForm
    {
        
        public frmAccessProfileList()
        {
            InitializeComponent();
            gridView1.OptionsBehavior.Editable = false;
            gridView1.DoubleClick += GridView1_DoubleClick;
            Refreshdata();
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["Name"].Caption = "الاســـم";
        }
        void Refreshdata()
        {
            using (var db = new dbDataContext())
            {
                gridControl1.DataSource = db.UserAccessProfiles.ToList();
            }
        }
        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                int id = Convert.ToInt32(view.GetFocusedRowCellValue("ID"));
                frmAccessProfile frm = new frmAccessProfile(id);
               frmMain.OpenFormWithPermissions(frm,true);
                Refreshdata();
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmMain.OpenFormWithPermissions(new frmAccessProfile());
            Refreshdata();

        }

        private void frmAccessProfileList_Load(object sender, EventArgs e)
        {
           
           
        }
    }
}

