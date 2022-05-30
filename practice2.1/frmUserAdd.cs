using DevExpress.XtraEditors;
using Liphsoft.Crypto.Argon2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class frmUserAdd : frmMaster
    {
        User user;
      
        public frmUserAdd()
        {
            InitializeComponent();
           
            RefreshData();
            New();
            GetData();
            toggleSwitch1.EditValue = true;
            
        }

        

        public frmUserAdd(int id)
        {
            InitializeComponent();
           using (var db = new dbDataContext ())
            {
                user= db .Users.SingleOrDefault(x=> x.ID == id);
            }
            RefreshData();
            GetData();
        }
        void New()
        {
            user = new User();
            isnew = true;
        }
        void Setdata()
        {
            if (user.Password.Trim() != txtPassWord.Text.Trim())
            {
                var hasher = new Liphsoft.Crypto.Argon2. PasswordHasher();

                string myhash = hasher.Hash(txtPassWord.Text);
                txtPassWord.Text = myhash;
            }
            user.Name = txtName.Text;
            user.Password = txtPassWord.Text.Trim();
            user.UserName = txtUserName.Text.Trim();
            user.ScreenProfileID =( (byte)lkpUserType.EditValue==(byte)Master.UserType.Admin)?1 :Convert.ToInt32(lkpUserAccessProfile.EditValue);
            user.SettingsProfileID = Convert.ToInt32(lkpUserSettingProfile.EditValue);
            user.UserType = Convert.ToByte(lkpUserType.EditValue);
            user.IsActive = Convert.ToBoolean(toggleSwitch1.EditValue);
        }
        void GetData()
        {
            txtName.Text = user.Name;
            txtPassWord.Text = user.Password;
            txtUserName.Text = user.UserName;
            lkpUserAccessProfile.EditValue = user.ScreenProfileID;
            lkpUserSettingProfile.EditValue = user.SettingsProfileID;
            lkpUserType.EditValue = user.UserType;
            toggleSwitch1.EditValue = user.IsActive;
        }
        void Save()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit))
            {
                if (IsdataValid())
                {
                    using (var db = new dbDataContext())
                    {
                        if (user.ID == 0)
                        {
                            db.Users.InsertOnSubmit(user);

                        }
                        else
                        {
                            db.Users.Attach(user);
                        }
                        Setdata();
                        db.SubmitChanges();
                    }
                    MessageBox.Show("تم الحفظ بنجاح");
                    InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, user.ID, user.Name + "مستخدم", this.Name);
                    isnew = false;
                }
            }
        }
        void RefreshData()
        {
            using (var db = new dbDataContext ())
            {
                lkpUserAccessProfile.InitializeData(db.UserAccessProfiles.Select(x=> new {x.ID,x.Name}).ToList());
                lkpUserSettingProfile.InitializeData(db.UserSettingsProfiles.Select(x => new { x.ID, x.Name }).ToList());
                lkpUserType.InitializeData(Master.UserTypeList);
            }
        }
        bool IsdataValid()
        {
            int NumberOfErrors = 0;
            using (var db = new dbDataContext())
            {
                if (db.Users.Where(x => x.UserName.Trim() == txtUserName.Text.Trim() && x.ID != user.ID).Count() > 0)
                {
                    NumberOfErrors += 1;
                    txtUserName.ErrorText = "هذا الاسم موجود مسبقا";
                }
                if (db.Users.Where(x => x.Name.Trim() == txtName.Text.Trim() && x.ID != user.ID).Count() > 0)
                {
                    NumberOfErrors += 1;
                    txtName.ErrorText = "هذا الاسم موجود مسبقا";
                }
            } 
            NumberOfErrors += txtName.validateText() ? 0 : 1;
            NumberOfErrors += txtUserName.validateText() ? 0 : 1;
            NumberOfErrors += txtPassWord.validateText() ? 0 : 1;
            NumberOfErrors += lkpUserAccessProfile.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += lkpUserSettingProfile.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += lkpUserType.IsEditValueValidAndNotZero() ? 0 : 1;
            return NumberOfErrors == 0;
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           Save();
        }

        private void lkpUserType_EditValueChanged(object sender, EventArgs e)
        {
           
        }
    }
}
