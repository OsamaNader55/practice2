using DevExpress.XtraEditors.Controls;
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
    public partial class frmAccountsCheck : frmMaster
    {
       
        bool IsCustomer;
        CustomersAndVendor insCusVen;
        public frmAccountsCheck(bool iscustomer)
        {
            IsCustomer = iscustomer;
            InitializeComponent();
            glkpName.EditValueChanged += GlkpName_EditValueChanged;
            LoadData();
        }

        private void GlkpName_EditValueChanged(object sender, EventArgs e)
        {
            LoadInfo();
        }

        private void frmAccountsCheck_Load(object sender, EventArgs e)
        {
            this.Text = IsCustomer ? "كشف حساب عميل" : "كشف حساب مورد";
            this.Name = IsCustomer ? Screens.CusomerAccountCheck.ScreenName : Screens.VendorsAccountCheck.ScreenName;
            gridView1.OptionsView.ShowAutoFilterRow = true;
            glkpName.Properties.PopulateViewColumns();
            glkpName.Properties.View.Columns[nameof(insCusVen.ID)].Visible = false;
            glkpName.Properties.View.Columns[nameof(insCusVen.Name)].Caption = "الاسم";
            glkpName.Properties.View.Columns[nameof(insCusVen.Mobile)].Caption = "موبايل";
            glkpName.Properties.View.Columns[nameof(insCusVen.Address)].Caption = "عنوان";
            glkpName.Properties.View.Columns[nameof(insCusVen.Phone)].Caption = "تليفون";
            glkpName.Properties.View.Columns[nameof(insCusVen.MaxCredit)].Caption = "حد الائتمان";
            glkpName.Properties.View.Columns[nameof(insCusVen.IsCustomer)].Visible = false;
            glkpName.Properties.View.Columns[nameof(insCusVen.AccountID)].Visible = false;
            glkpName.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
           glkpName.Properties.View.OptionsView.ShowAutoFilterRow = true;
        }
        void LoadData()
        {
            glkpName.InitializeData(IsCustomer ? Session.Customer : Session.Vendor);
            glkpName.Properties.NullText = "";
        }
        void LoadInfo()
        {
            Finance.AccountBalance accountBalance;
            int id = Convert.ToInt32(glkpName.EditValue);

            if (id != 0)
            {
                CustomersAndVendor account;
                if (IsCustomer)
                {
                    account = Session.Customer.Single(x => x.ID == id);
                }
                else
                {
                    account = Session.Vendor.Single(x => x.ID == id);
                }
               
                accountBalance = Finance.GetAccountBalance(account.AccountID);
                txtCredit.Text = accountBalance.TotalCredit.ToString();
                txtDebit.Text = accountBalance.TotalDebit.ToString();
                txtAccountBalance.Text = accountBalance.Balance.ToString();

            }
            else
            {
                txtAccountBalance.Text =
                txtCredit.Text =
                txtDebit.Text = "";
               
            }
        }
        int y = 0;
        void RefreshData()
        {
            if (glkpName.IsEditValueValidAndNotZero() == false)
            {
                MessageBox.Show("يجب اختيار اسم للبحث عنه");
                return;
            }
                var id =Convert.ToInt32(glkpName.EditValue); 
            using (var db = new dbDataContext())
            {
                var q = from inv in db.InvoiceHeaders.Where(x => x.PartID == id).DefaultIfEmpty()
                        from cs in db.CustomersAndVendors.Where(x => x.ID == id).DefaultIfEmpty()
                        select new
                        {
                            cs.Name,
                            inv.Code,
                            inv.Date,
                            inv.Total,
                            inv.DiscountRation,
                            inv.Tax,
                            inv.Expences,
                            inv.Net,
                            inv.Paid,
                            
                        };
                if (dtFrom.EditValue != null)
                {
                    q = q.Where(x => x.Date.Date >= dtFrom.DateTime.Date);
                }
                if (dtTo.EditValue != null)
                {
                    q = q.Where(x => x.Date.Date <= dtTo.DateTime.Date);
                }
                gridControl1.DataSource = q;
               
                if (y== 0)
                {
                    gridView1.Columns["Name"].Caption = "الاسم";
                    gridView1.Columns["Code"].Caption = "كود الفاتوره";
                    gridView1.Columns["Date"].Caption = "التاريخ";
                    gridView1.Columns["Total"].Caption = "الكل";
                    gridView1.Columns["Tax"].Caption = "ن.ض";
                    gridView1.Columns["Net"].Caption = "الصافي";
                    gridView1.Columns["Paid"].Caption = "المدفوع";
                    gridView1.Columns["DiscountRation"].Caption = "ن.خ";
                    gridView1.Columns["Expences"].Caption = "اضافه";
                    y = 1;
                }
              
            }
          
        }

        private void btnSearchAll_Click(object sender, EventArgs e)
        {
           
            dtTo.EditValue = dtFrom.EditValue = null;
            RefreshData();
        }

        private void btnSearchByDate_Click(object sender, EventArgs e)
        {
            if(dtTo.EditValue == null&& dtFrom.EditValue == null)
            {
                MessageBox.Show("يجب تحديد تاريخ واحد علي الاقل");
                return;
            }
            RefreshData();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (frmMain.CheckActionPermission(this.Name, Master.Actions.Print))
            {
                string VenOrCus = IsCustomer ? "عميل" : "مورد";
                Report.rptGrid.Print(gridControl1,
                     " كشف حساب " + VenOrCus + " " + glkpName.Text,
                     $"من {dtFrom.Text} الي {dtTo.Text}",
                     this.Name,
                     " كسف حساب  " + VenOrCus + " " + glkpName.Text);
            }
           
        }
    }
}
