using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
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
    public partial class frmAccessProfile : frmMaster
    {
        UserAccessProfile profile;
        public frmAccessProfile()
        {
            InitializeComponent();
            treeList1.CustomNodeCellEdit += TreeList1_CustomNodeCellEdit;
         
            New();
            Getdata();
        }
        public frmAccessProfile(int id)
        {
            InitializeComponent();
            treeList1.CustomNodeCellEdit += TreeList1_CustomNodeCellEdit;
            using (var db = new dbDataContext())
            {
                profile = db.UserAccessProfiles.Single(x => x.ID == id);

            }
            textEdit1.Text = profile.Name;  
            Getdata();
        }
        private void TreeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            if(e.Node.Id >= 0)
            {
                var row = treeList1.GetRow(e.Node.Id) as ScreenAccessProfile;
                if (row != null)
                {
                    if (e.Column.FieldName == nameof(ScreenAccessProfile.CanAdd) && row.Actions.Contains(Master.Actions.Add) == false)
                    {
                        e.RepositoryItem= new DevExpress.XtraEditors.Repository.RepositoryItem();
                    }
                    else if (e.Column.FieldName == nameof(ScreenAccessProfile.CanDelete) && row.Actions.Contains(Master.Actions.Delete) == false)
                    {
                        e.RepositoryItem = new DevExpress.XtraEditors.Repository.RepositoryItem();
                    }
                    if (e.Column.FieldName == nameof(ScreenAccessProfile.CanEdit) && row.Actions.Contains(Master.Actions.Edit) == false)
                    {
                        e.RepositoryItem = new DevExpress.XtraEditors.Repository.RepositoryItem();
                    }
                    if (e.Column.FieldName == nameof(ScreenAccessProfile.CanOpen) && row.Actions.Contains(Master.Actions.Open) == false)
                    {
                        e.RepositoryItem = new DevExpress.XtraEditors.Repository.RepositoryItem();
                    }
                    if (e.Column.FieldName == nameof(ScreenAccessProfile.CanPrint) && row.Actions.Contains(Master.Actions.Print) == false)
                    {
                        e.RepositoryItem = new DevExpress.XtraEditors.Repository.RepositoryItem();
                    }
                    if (e.Column.FieldName == nameof(ScreenAccessProfile.CanShow) && row.Actions.Contains(Master.Actions.Show) == false)
                    {
                        e.RepositoryItem = new DevExpress.XtraEditors.Repository.RepositoryItem();
                    }
                }
            }
        }
        void Getdata()
        {
            List<ScreenAccessProfile> data;
            using (var db = new dbDataContext ())
            {
                 data = (from s in Screens.GetScreensProp
                         from d in db.UserAccessProfileDetails
                         .Where(x => x.ProfileID == profile.ID&& x.ScreenID == s.ScreenID).DefaultIfEmpty()
                         select new ScreenAccessProfile(s.ScreenName)
                         {
                             CanAdd = (d== null)?true:d.CanAdd,
                             CanOpen = (d == null) ? true : d.CanOpen,
                             CanShow = (d == null) ? true : d.CanShow,
                             CanEdit = (d == null) ? true : d.CanEdit,
                             CanDelete = (d == null) ? true : d.CanDelete,
                             CanPrint = (d == null) ? true :d.CanPrint,
                            Actions=s.Actions,
                            Screencaption=s.Screencaption,
                            ScreenName=s.ScreenName,
                            ScreenID=s.ScreenID,    
                            ParentScreenID=s.ParentScreenID,    


                         }).ToList();   
            }
            treeList1.DataSource =data ;
            treeList1.KeyFieldName = nameof(ScreenAccessProfile.ScreenID);
            treeList1.ParentFieldName = nameof(ScreenAccessProfile.ParentScreenID);   
            treeList1.Columns[nameof(ScreenAccessProfile.ScreenName)].Visible = false;
            treeList1.Columns[nameof(ScreenAccessProfile.ScreenName)].OptionsColumn.AllowEdit = false;
            treeList1.BestFitColumns();
        }
        void New()
        {
            profile = new UserAccessProfile();
            textEdit1.Text = profile.Name; 
            isnew = true;
        }
        public bool IsDataValid()
        {
            int number = 0;
            number += textEdit1.validateText() ? 0 : 1;
            return (number == 0);
        }
        void Save()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit))
            {
                if (IsDataValid())
                {
                    var db = new dbDataContext();
                    if (profile.ID == 0)
                    {
                        db.UserAccessProfiles.InsertOnSubmit(profile);
                    }
                    else
                    {
                        db.UserAccessProfiles.Attach(profile);
                    }
                    profile.Name = textEdit1.Text;
                    db.SubmitChanges();
                    db.UserAccessProfileDetails.DeleteAllOnSubmit(db.UserAccessProfileDetails.Where(x => x.ProfileID == profile.ID));
                    db.SubmitChanges();
                    var data = treeList1.DataSource as List<ScreenAccessProfile>;
                    var baseData = data.Select(s => new UserAccessProfileDetail
                    {
                        CanAdd = s.CanAdd,
                        CanEdit = s.CanEdit,
                        CanDelete = s.CanDelete,
                        CanOpen = s.CanOpen,
                        CanPrint = s.CanPrint,
                        CanShow = s.CanShow,
                        ProfileID = profile.ID,
                        ScreenID = s.ScreenID

                    }).ToList();
                    db.UserAccessProfileDetails.InsertAllOnSubmit(baseData);
                    db.SubmitChanges();
                    MessageBox.Show("تم الحفظ بنجاااح");
                    InsertUserLog(isnew? Master.Actions.Add:Master.Actions.Edit,profile.ID,profile.Name,this.Name);
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             Save();
        }

        private void frmAccessProfile_Load(object sender, EventArgs e)
        {
            RepositoryItemCheckEdit repocheck = new RepositoryItemCheckEdit();
            repocheck.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgToggle1;
             treeList1.Columns[nameof(ScreenAccessProfile.CanAdd)].ColumnEdit = repocheck;
            treeList1.Columns[nameof(ScreenAccessProfile.CanDelete)].ColumnEdit = repocheck;
            treeList1.Columns[nameof(ScreenAccessProfile.CanEdit)].ColumnEdit = repocheck;
            treeList1.Columns[nameof(ScreenAccessProfile.CanOpen)].ColumnEdit = repocheck;
            treeList1.Columns[nameof(ScreenAccessProfile.CanPrint)].ColumnEdit = repocheck;
            treeList1.Columns[nameof(ScreenAccessProfile.CanShow)].ColumnEdit = repocheck;
            treeList1.Columns[nameof(ScreenAccessProfile.CanAdd)].Caption = "اضافه";
            treeList1.Columns[nameof(ScreenAccessProfile.CanOpen)].Caption = "فتح";
            treeList1.Columns[nameof(ScreenAccessProfile.CanPrint)].Caption = "طباعه";
            treeList1.Columns[nameof(ScreenAccessProfile.CanShow)].Caption = "اظهار";
            treeList1.Columns[nameof(ScreenAccessProfile.CanDelete)].Caption = "حذف";
            treeList1.Columns[nameof(ScreenAccessProfile.Screencaption)].Caption = "اسم الشاشه";
            treeList1.Columns[nameof(ScreenAccessProfile.Screencaption)].MinWidth = 250;
        }
    }
}
