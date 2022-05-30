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
    public partial class frmCustomerVendor : frmMaster
    {
       
        bool IsCustomer;
        CustomersAndVendor CusVen;
        public frmCustomerVendor(bool isCustomer)
        {
            InitializeComponent();
            this.IsCustomer = isCustomer;
            New();
        }
        public frmCustomerVendor(bool isCustomer, int id)
        {
            InitializeComponent();
            this.IsCustomer = isCustomer;
            LoadData(id);
        }
        void LoadData(int id)
        {
            using (var db = new dbDataContext())
            {
                CusVen = db.CustomersAndVendors.Single(s => s.ID == id);
                IsCustomer=CusVen.IsCustomer;
                Getdata();
            }
        }

        private void frmCustomerVendor_Load(object sender, EventArgs e)
        {
            this.Name = (IsCustomer)?Screens.AddCustomer.ScreenName:Screens.AddVendor.ScreenName;
            this.Text = (IsCustomer) ? "عمـــيل": "مــورد";
          
        }
         void New()
        {
            CusVen = new CustomersAndVendor();
            spinEdit1.EditValue = 5000;
            isnew = true;
            Getdata();
        }
         void Getdata()
        {
            txtName.Text =  CusVen.Name;
            txtPhone.Text = CusVen.Phone;
            txtMobile.Text = CusVen.Mobile;
            txtAddress.Text = CusVen.Address;   
            txtAccountNumber.Text=CusVen.AccountID.ToString();
            spinEdit1.EditValue = CusVen.MaxCredit;
            
        }
        void setdata()
        {
            CusVen.Name = txtName.Text;
            CusVen.Phone = txtPhone.Text;   
            CusVen.Mobile = txtMobile.Text; 
            CusVen.Address = txtAddress.Text;
            CusVen.IsCustomer = IsCustomer; 
            CusVen.MaxCredit=Convert.ToDouble(CusVen.MaxCredit);
        }
         void save()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit))
            {
                if (validData() == false)
                {

                    var db = new dbDataContext();
                    Account account;
                    if (CusVen.ID == 0)
                    {

                        account = new Account();
                        db.CustomersAndVendors.InsertOnSubmit(CusVen);
                        db.Accounts.InsertOnSubmit(account);

                    }

                    else
                    {
                        db.CustomersAndVendors.Attach(CusVen);
                        account = db.Accounts.Single(x => x.ID == CusVen.AccountID);
                    }
                    setdata();
                    account.Name = CusVen.Name;

                    db.SubmitChanges();
                    CusVen.AccountID = account.ID;
                    db.SubmitChanges();

                    MessageBox.Show("تم الحفظ بنجاااااح");
                    InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, CusVen.ID, CusVen.Name +( (IsCustomer) ? "عمـــيل" : "مــورد"),this.Name);
                }
            }
        }
        bool validData()
        {
            if(txtName.Text.Trim() == String.Empty)
            {
                MessageBox.Show("هذا الحــقل مطــلوب");
                return false;
            }
            //لازم تكون الداتا بيز بتقرا العربي لو اليوزر هيكتب عربي 
            //Database collation must be "Arabic CI AI" for arabic words !
            var db=new dbDataContext();
            if(db.CustomersAndVendors.Where(x=> x.Name.Trim()==txtName.Text.Trim() &&
            x.IsCustomer == IsCustomer &&
            x.ID!=CusVen.ID).Count() > 0  )
            {
                MessageBox.Show("هذا الاســم مــوجود مــسبقا");
                return false;
            }
            return true;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            New();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
                save();
        }
    }
}
