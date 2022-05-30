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
using Sales;
namespace practice2._1
{
    public partial class frmCashNote : frmMaster
    {
        bool IsCashIn;
        CashNote cash;
        Account Account=null;
        CustomersAndVendor insCusVen;
        public frmCashNote(bool iscashin)
        {
            IsCashIn = iscashin;
            InitializeComponent();
            lkpPartType.EditValueChanged += LkpParttype_EditValueChanged;
            glkpPartID.EditValueChanged += GlkpPartID_EditValueChanged;
            spnPaid.EditValueChanged += SpnPaid_EditValueChanged;
            New();
        }

        private void SpnPaid_EditValueChanged(object sender, EventArgs e)
        {
            if (spnPaid.Value == 0)
            {
                txtAccountStateAfterpay.Text=String.Empty;
                return;
            }
            double x = Math.Abs( (double)spnPaid.Value - balance);
            if (balance > (double)spnPaid.Value&& spnPaid.Value!=0)
            {
                txtAccountStateAfterpay.Text = x.ToString() + "عليه/مدين";
            }
            else if (balance < (double)spnPaid.Value && spnPaid.Value != 0)
            {
                txtAccountStateAfterpay.Text = x.ToString() + "له/دائن";
            }
            else if (x == 0) txtAccountStateAfterpay.Text = "تم تصفيه الحساب";
            
        }
        double balance = 0;
        private void GlkpPartID_EditValueChanged(object sender, EventArgs e)
        {
            txtAccountStateBeforepay.Text = String.Empty;
            SpnPaid_EditValueChanged(null,null);
            Finance.AccountBalance accountBalance;
            int id = Convert.ToInt32(glkpPartID.EditValue);

            if (id != 0)
            {
                CustomersAndVendor cusven;
                Account account;
                if (Convert.ToInt32(lkpPartType.EditValue) == (int)Master.Parttype.Vendor)
                {
                    cusven = Session.Vendor.SingleOrDefault(x => x.ID == id);
                    account=Session.Accounts.SingleOrDefault(x=>x.ID==cusven.AccountID);
                }
                else if (Convert.ToInt32(lkpPartType.EditValue) == (int)Master.Parttype.Customer)
                {
                    cusven = Session.Customer.SingleOrDefault(x => x.ID == id);
                     account = Session.Accounts.SingleOrDefault(x => x.ID == cusven.AccountID);
                }
                else
                {
                    account = Session.Accounts.SingleOrDefault(x => x.ID == id);
                }
                Account = account;
                if (account == null) return;
                accountBalance = Finance.GetAccountBalance(account.ID);
                if (accountBalance == null) return; 
                txtAccountStateBeforepay.Text = accountBalance.Balance;
                balance = accountBalance.BalanceAmount;
            }
            else
            {
                txtAccountStateBeforepay.Text = txtAccountStateAfterpay.Text= "";
            }
        }

        private void frmCashNote_Load(object sender, EventArgs e)
        {

            Refreshdata();
            
            glkpPartID.Properties.PopulateViewColumns();
            glkpPartID.Properties.View.Columns[nameof(insCusVen.ID)].Visible = false;
            glkpPartID.Properties.View.Columns[nameof(insCusVen.Name)].Caption = "الاسم";
            glkpPartID.Properties.View.Columns[nameof(insCusVen.Mobile)].Caption = "موبايل";
            glkpPartID.Properties.View.Columns[nameof(insCusVen.Address)].Caption = "عنوان";
            glkpPartID.Properties.View.Columns[nameof(insCusVen.Phone)].Caption = "تليفون";
            glkpPartID.Properties.View.Columns[nameof(insCusVen.MaxCredit)].Caption = "حد الائتمان";
            glkpPartID.Properties.View.Columns[nameof(insCusVen.IsCustomer)].Visible = false;
            glkpPartID.Properties.View.Columns[nameof(insCusVen.AccountID)].Visible = false;
            glkpPartID.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        }

        private void LkpParttype_EditValueChanged(object sender, EventArgs e)
        {
            txtAccountStateAfterpay.Text= txtAccountStateBeforepay.Text=String.Empty;
            if (lkpPartType.IsEditvalueNotNull())
            {
                int parttype = Convert.ToInt32(lkpPartType.EditValue);
                if (parttype == (int)Master.Parttype.Customer)
                {
                    glkpPartID.InitializeData(Session.Customer);
                    glkpPartID.EditValue = Session.Defaults.Customer;
                }
                else if (parttype == (int)Master.Parttype.Vendor)
                {
                    glkpPartID.InitializeData(Session.Vendor);
                    glkpPartID.EditValue = Session.Defaults.Vendor;
                }
                else if (parttype == (int)Master.Parttype.Account)
                {
                    glkpPartID.InitializeData(Session.Accounts);
                    glkpPartID.EditValue = null;
                }
            }
        }

        void Refreshdata()
        {
            lkpDrawer.InitializeData(Session.Drawer);
            lkpStore.InitializeData(Session.Store);
            lkpPartType.InitializeData(Master.PartTypeList);
        }
        string GetNewCasheCode()
        {
            string maxcode;
            using (var db = new dbDataContext())
            {
                maxcode = db.CashNotes.Where(x => x.IsCashIn == IsCashIn).OrderByDescending(x => x.Date).Select(x => x.Code ?? "0").FirstOrDefault();
            }
            return Master.GetNextNumberInSTring(maxcode);
        }
        void New()
        {
            cash = new CashNote() { Date = DateTime.Now, Drawer = Session.Defaults.Drawer, Code = GetNewCasheCode(),StoreID=Session.Defaults.Store };
            isnew = true;
            Getdata();
        }
        void SetData()
        {
            cash.Amount = Convert.ToDouble(spnPaid.EditValue);
            cash.Code = txtCode.Text;
            cash.Drawer = Convert.ToInt32(lkpDrawer.EditValue);
            cash.StoreID = Convert.ToInt32(lkpStore.EditValue);
            cash.PartType =Convert.ToByte( lkpPartType.EditValue);
            cash.PartID = Convert.ToInt32(glkpPartID.EditValue);
            cash.Notes = memoNotes.Text;
            cash.Date =DateTime.Now;
            cash.IsCashIn = IsCashIn;
        }
        void Getdata()
        {
            spnPaid.EditValue = cash.Amount;
            txtCode.Text = cash.Code;
            lkpDrawer.EditValue = cash.Drawer;
            lkpStore.EditValue = cash.StoreID;
            lkpPartType.EditValue = cash.PartType;
            glkpPartID.EditValue = cash.PartID;
            memoNotes.Text = cash.Notes;
            dtdate.EditValue = cash.Date;
        }
        bool IScodeExist()
        {
            using (var db = new dbDataContext())
            {
                var count = db.CashNotes.Where(x => x.ID != cash.ID && x.IsCashIn == IsCashIn && x.Code == txtCode.Text).Count();
                if (count > 0)
                {
                    txtCode.ErrorText = "هذا الكود موجود ";
                    return false;
                }
                else return true;
            }
        }
       
        bool IsdataValid()
        {
            int NumberOfErrors = 0;
            NumberOfErrors += txtCode.validateText() ? 0 : 1;
            NumberOfErrors += IScodeExist() ? 0 : 1;
            NumberOfErrors += lkpPartType.IsEditvalueNotNull() ? 0 : 1;
            NumberOfErrors += lkpStore.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += lkpDrawer.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += glkpPartID.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += dtdate.IsdateValid() ? 0 : 1;
            NumberOfErrors += spnPaid.IsValuenotEqualORlessthanZero() ? 0 : 1;
            return NumberOfErrors==0;
        }
        void Save()
        {
            if (frmMain.CheckActionPermission(this.Name, Master.Actions.Add))
            {
                if (IsdataValid())
                {
                    using (var db = new dbDataContext())
                    {
                        db.CashNotes.InsertOnSubmit(cash);
                        SetData();
                        db.SubmitChanges();
                       
                        var drawer = db.Drawers.Single(x => x.ID == cash.Drawer);
                        var msg = this.Text +" "+ lkpPartType.Text + " " + glkpPartID.Text;
                            db.Journals.InsertOnSubmit(new Journal() //Drawer 
                            {
                                AccountID = drawer.AccountID,
                                Code = 5555,
                                Credit = (IsCashIn) ?  cash.Amount : 0,
                                Debit = (!IsCashIn) ? cash.Amount : 0,
                                OperationType = IsCashIn ? (byte)Master.JournalsOperationType.CashnoteIn : (byte)Master.JournalsOperationType.CashNoteOut,
                                InsertDate = cash.Date,
                                SourceID = cash.ID,
                                Sourcetype = IsCashIn ? (byte)Master.SourceType.CashNoteIN : (byte)Master.SourceType.CashNoteOut,
                                Notes = msg,
                            });
                            db.Journals.InsertOnSubmit(new Journal()
                            {
                                AccountID = Account.ID,
                                Code = 5555,
                                Credit = (!IsCashIn) ? cash.Amount : 0,
                                Debit = (IsCashIn) ? cash.Amount : 0,
                                OperationType = IsCashIn ? (byte)Master.JournalsOperationType.CashnoteIn : (byte)Master.JournalsOperationType.CashNoteOut,
                                InsertDate = cash.Date,
                                SourceID = cash.ID,
                                Sourcetype = IsCashIn ? (byte)Master.SourceType.CashNoteIN : (byte)Master.SourceType.CashNoteOut,
                                Notes = msg,
                            });
                        db.SubmitChanges();
                        InsertUserLog(Master.Actions.Add, cash.ID, msg);
                        MessageBox.Show("تم حفظ العمليه");
                    }
                }
            }
           
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            New();
        }

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}
