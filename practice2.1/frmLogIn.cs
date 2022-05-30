using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Liphsoft.Crypto.Argon2;
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
    public partial class frmLogIn : frmMaster

    {
      
        public frmLogIn()
        {
            InitializeComponent();
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            if (Master.IsConnectionAvailable() == false) { MessageBox.Show("نص الاتصال غير صحيح الرجاء التاكد من نص الاتصال بقاعده البيانات "); return; }
            var hasher = new Liphsoft.Crypto.Argon2.PasswordHasher();
            using (var db = new  dbDataContext())
            {
                var user = db.Users.SingleOrDefault(x => x.UserName == txtUsername.Text);
                if (txtPassword.Text == String.Empty || txtUsername.Text == String.Empty)
                {
                    MessageBox.Show("يرجي ادخال اسم المستخدم وكلمه المرور");
                    return;
                }
                
               else if (user == null)
                {
                    MessageBox.Show("اسم المستخدم او كلمه المرور غير صحيحه");
                    return;
                }
                
                else
                {
                    if (hasher.Verify(user.Password, txtPassword.Text)==false)
                    {
                        MessageBox.Show("اسم المستخدم او كلمه المرور غير صحيحه");
                        return;
                    }
                    else if(user.IsActive==false)
                    {
                        MessageBox.Show("هذا المستخدم غير نشط");
                        return;
                    }
                    else
                    {
                        //Saving user Login
                        if (checkEdit2.Checked == true)
                        {
                         Sales.   Properties.Settings.Default.LastUserName = txtUsername.Text;
                         Sales.   Properties.Settings.Default.LastPassWord = txtPassword.Text;
                         Sales.   Properties.Settings.Default.Save();
                        }
                        else
                        {
                         Sales.   Properties.Settings.Default.LastUserName = null;
                         Sales.   Properties.Settings.Default.LastPassWord = null;
                            Sales.Properties.Settings.Default.Save();
                        }
                       Sales. Properties.Settings.Default.chkboxlastvalue =(bool)checkEdit2.EditValue;
                       
                        //add splash screen to load data    wrong using system.threading.sleep(5000);

                        SplashScreenManager.ShowForm(frmMain.InsMain, typeof(SplashScreen1));
                        this.Hide();
                        //loaded data
                        Type type = typeof(Session);
                        //then setting user id into session
                        Session.SetUser(user);

                        var properties = type.GetProperties(System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Static);
                        foreach (var item in properties)
                        {
                            var obj = item.GetValue(null);
                        }
                        InsertUserLog(Master.Actions.LogIn, 00, "تسجيل دخول", this.Name);
                        //then start
                        frmMain.InsMain.Show();
                        this.Close();
                        SplashScreenManager.CloseForm();
                        
                    }
                    
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkEdit1.Checked == true) txtPassword.Properties.UseSystemPasswordChar = false;
            else txtPassword.Properties.UseSystemPasswordChar = true;
        }

        private void frmLogIn_Load(object sender, EventArgs e)
        {
            txtUsername.Text =Sales. Properties.Settings.Default.LastUserName;
            txtPassword.Text = Sales.Properties.Settings.Default.LastPassWord;
            checkEdit2.Checked = Sales.Properties.Settings.Default.chkboxlastvalue;
        }

        private void frmLogIn_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
                { case  Keys.F4 :
                    if(e.Modifiers==Keys.Alt)

                    Application.Exit();
                    break;
                case Keys.F5:
                    if (e.Modifiers == Keys.Alt)
                    {
                        frmDatabaseConncect frm = new frmDatabaseConncect();
                        frm.ShowDialog();
                    }
                    break;
                case Keys.F6:
                    if (e.Modifiers == Keys.Alt)
                    {
                        frmRestore frm = new frmRestore();
                        frm.ShowDialog();
                    }
                    break;
                default: break;
            }
        }
    }
}
