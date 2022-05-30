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
    
    public partial class frmCompanyInfo : frmMaster
    {
        
        dbDataContext db = new dbDataContext(); 
        public frmCompanyInfo()
        {
            InitializeComponent();
            
        }

        private void frmCompanyInfo_Load(object sender, EventArgs e)
        {
            getdata();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            save();
            

        }

        private void frmCompanyInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.F1)
            {
                save();
            }
        }
        void save()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                MessageBox.Show("eneter a name");
                return;
            }
            if (txtPhone.Text.Trim() == string.Empty)
            {
                MessageBox.Show("eneter a phone number");
                return;
            }
            CompanyInfo info = db.CompanyInfos.FirstOrDefault();
            if(info == null)
            {
                info = new CompanyInfo();
                db.CompanyInfos.InsertOnSubmit(info);

            }
            info.CompanyName = txtName.Text;
            info.Phone = txtPhone.Text;
            info.Mobile = txtMobile.Text;
            info.Address = txtAddress.Text;
            info.Email = txtEmail.Text;
            info.Logo = Master.ToByteArray(pictureEdit1.Image);
            db.SubmitChanges();
            Session.ResetCompanyInfo();
            MessageBox.Show("تم الحفظ بنجاااااح");
        }
        void getdata()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit))
            {
                CompanyInfo info = db.CompanyInfos.FirstOrDefault();
                if (info == null)
                { return; }
                txtName.Text = info.CompanyName;
                txtPhone.Text = info.Phone;
                txtMobile.Text = info.Mobile;
                txtAddress.Text = info.Address;
                txtEmail.Text = info.Email;
                if (info.Logo == null) pictureEdit1.Image = Properties.Resources.noimage;
                else pictureEdit1.Image = Master.GetimageFromByteArray(info.Logo.ToArray());
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
           
                save();
        }
    }
}
