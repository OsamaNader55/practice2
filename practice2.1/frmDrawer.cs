using DevExpress.XtraEditors;
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
    public partial class frmDrawer : frmMaster

    {
       
        Drawer drawer;
        Account account;    
        public frmDrawer()
        {
            InitializeComponent();
            New();
        }
        public frmDrawer(int id)
        {
            InitializeComponent();
            LoadDrawer(id);
        }
        void LoadDrawer(int id)
        {
            using (var db = new dbDataContext())
            {
                drawer=db.Drawers.Single(s=> s.ID==id);
                Getdata();
            }
        }
       void New()
        {

            drawer = new Drawer();  
           Getdata(); 
            isnew = true;
        }
         void Getdata()
        {
            txtName.Text = drawer.Name; 
           
        }
        void setdata()
        {
            drawer.Name = txtName.Text;
        }
        void save()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit))
            {
                if (txtName.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("eneter a name");
                    return;
                }
                dbDataContext db = new dbDataContext();
                if (drawer.ID == 0)
                {
                    account = new Account();
                    setdata();
                    db.Drawers.InsertOnSubmit(drawer);
                    db.Accounts.InsertOnSubmit(account);
                }
                else
                {
                    account = db.Accounts.Single(s => s.ID == drawer.AccountID);
                }
                setdata();
                account.Name = drawer.Name;
                db.SubmitChanges();
                drawer.AccountID = account.ID;
                db.SubmitChanges();
                MessageBox.Show("تم الحفظ بنجاااااح");
                InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, drawer.ID, drawer.Name + "خزنه", this.Name);
                isnew = false;
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            New();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
          
            save();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
        }
    }
}
