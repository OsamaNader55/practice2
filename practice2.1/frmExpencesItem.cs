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
    public partial class frmExpencesItem : frmMaster
    {
        ExpencesItem item;
        Account account;
        public frmExpencesItem()
        {
            InitializeComponent();
            Refreshdata();
            New();
        }
       
        void New()
        {
            item = new ExpencesItem();
            account = new Account();
            isnew = true;
            GetData();
        }
        void SetData()
        {
            item.Name=txtName.Text;
            item.Notes=txtNotes.Text;
        }
        void GetData()
        {
            txtName.Text = item.Name;
           txtNotes.Text = item.Notes;
        }
        void Save()
        {
            if (Validate() == false) return;
            if(frmMain.CheckActionPermission(this.Name,isnew? Master.Actions.Add: Master.Actions.Edit))
            {
                using(var db = new dbDataContext())
                {
                    if (item.ID == 0)
                    {
                        db.ExpencesItems.InsertOnSubmit(item); 
                        db.Accounts.InsertOnSubmit(account);
                    }
                    else
                    {
                        db.ExpencesItems.Attach(item);
                        account = db.Accounts.Single(x => x.ID == item.AccountID);
                    }
                    SetData();
                    account.Name=item.Name;
                    db.SubmitChanges();
                    item.AccountID = account.ID;
                    db.SubmitChanges();
                    Refreshdata();
                    MessageBox.Show("تم الحفظ بنجاح");
                    InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, item.ID, item.Name);
                    isnew=false;
                }
            }
        }
        bool ValidateData()
        {
            if (txtName.Text == String.Empty)
            {
                txtName.ErrorText = "هذا الحقل مطلوب";
                return false;
            }
            using(var db = new dbDataContext())
            {
                if (db.ExpencesItems.Where(x => x.ID != item.ID && x.Name.Trim() == txtName.Text.Trim()).Count() > 0)
                {
                    MessageBox.Show("هذا الاسم موجود مسبقا ");
                    return false;
                }
            }
            return true;
        }

        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            New();
        }

       
        void Refreshdata()
        {
            using(var db = new dbDataContext())
            {
                var q = db.ExpencesItems;
                gridControl1.DataSource = q;
            }
        }

        private void frmExpencesItem_Load(object sender, EventArgs e)
        {
            gridView1.Columns["Name"].Caption = "الاسم";
            gridView1.Columns["Notes"].Caption = "ملاحظات";
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["AccountID"].Visible =false;
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Refreshdata();
        }
    }
}
