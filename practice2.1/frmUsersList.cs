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
    public partial class frmUsersList : frmMaster
    {
        User ins;
        public frmUsersList()
        {
            InitializeComponent();
            gridView1.DoubleClick += GridView1_DoubleClick1;
            gridView1.OptionsBehavior.Editable = false;
            Refreshdata();
            gridView1.PopulateColumns();
           
            gridView1.Columns[nameof(ins.UserName)].Caption = "اسم الدخول";
            gridView1.Columns[nameof(ins.Name)].Caption = "الاسم";
            gridView1.Columns["AccessName"].Caption = "نموذج الصلاحيات";
            gridView1.Columns["SettingName"].Caption = "نموذج الاعدادات";
            gridView1.Columns[nameof(ins.IsActive)].Caption = "نشط";
            gridView1.Columns["TypeName"].Caption = "نوع الدخول";

        }
        private void GridView1_DoubleClick1(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                int id = Convert.ToInt32(view.GetFocusedRowCellValue("ID"));
                frmUserAdd frm = new frmUserAdd(id);
                frmMain.OpenFormWithPermissions(frm, true);
                Refreshdata();
            }
        }
       void Refreshdata()
        {
            using(var db = new dbDataContext ())
            {
               
                var query =( from t in Master.UserTypeList join u in db.Users on t.ID equals u.UserType
                             //can't make a join seq between sql data dource and local data source to make this right u have to make local data come first line 54
                             
                             from setting in db.UserSettingsProfiles.Where(x => x.ID == u.SettingsProfileID).DefaultIfEmpty()
                            from access in db.UserAccessProfiles.Where(x => x.ID == u.ScreenProfileID).DefaultIfEmpty()
                            select new
                            {
                                u.Name,
                                u.UserName,
                              SettingName=  setting.Name,
                                TypeName =t.Name ,
                                AccessName = access.Name,
                                u.IsActive
                            }).ToList();
                gridControl1.DataSource = query;
            }
            
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmUserAdd frm = new frmUserAdd();
            frmMain.OpenFormWithPermissions(frm, true);
        }
    }
}
