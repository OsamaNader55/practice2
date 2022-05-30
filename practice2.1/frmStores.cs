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
using Sales;
namespace practice2._1
{
    public partial class frmStores : frmMaster
    {
     
        Store store;
        public frmStores()
        {
            InitializeComponent();
            New();

        }
        public frmStores(int id)
        {
            InitializeComponent();
            dbDataContext db = new dbDataContext();
            store =db.Stores.Where(s=> s.ID==id).First();
            getdata();  

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
                Account SalesAccount = new Account();
                Account SalesReturnAccount = new Account();
                Account InventoryAccount = new Account();
                Account CostOfSoldGoodsAccount = new Account();
                Account ExpencesItemAccount = new Account();
                dbDataContext db = new dbDataContext();

                if (store.ID == 0)
                {
                    db.Stores.InsertOnSubmit(store);
                    db.Accounts.InsertOnSubmit(SalesAccount);
                    db.Accounts.InsertOnSubmit(SalesReturnAccount);
                    db.Accounts.InsertOnSubmit(InventoryAccount);
                    db.Accounts.InsertOnSubmit(CostOfSoldGoodsAccount);
                    db.Accounts.InsertOnSubmit(ExpencesItemAccount);
                }
                else
                {
                    db.Stores.Attach(store);
                    SalesAccount = db.Accounts.Single(x => x.ID == store.SalesAccountID);
                    SalesReturnAccount = db.Accounts.Single(x => x.ID == store.SalesReturnAccountID);
                    InventoryAccount = db.Accounts.Single(x => x.ID == store.InventoryAccountID);
                    CostOfSoldGoodsAccount = db.Accounts.Single(x => x.ID == store.CostOfSoldGoodsAccountID);
                    ExpencesItemAccount = db.Accounts.Single(x => x.ID == store.ExpencesItemsAccountID);
                }

                setdata();
                SalesAccount.Name = " مبيعات" + store.Name;
                SalesReturnAccount.Name = " مردود المبيعات" +store.Name;
                InventoryAccount.Name =  " مخزن"+store.Name;
                CostOfSoldGoodsAccount.Name = " تكلفه البضاعه المباعه" + store.Name;
                ExpencesItemAccount.Name =  " بنود مصروفات" + store.Name;
                db.SubmitChanges();
                store.SalesAccountID = SalesAccount.ID;
                store.SalesReturnAccountID = SalesReturnAccount.ID;
                store.InventoryAccountID = InventoryAccount.ID;
                store.CostOfSoldGoodsAccountID = CostOfSoldGoodsAccount.ID;
                store.ExpencesItemsAccountID= ExpencesItemAccount.ID;
                store.DiscountAllowedAccountID = Session.Defaults.DiscountAllowedAccountID;
                store.DiscountReceivedAccountID = Session.Defaults.DiscountReceivedAccountID;
                db.SubmitChanges();

                MessageBox.Show("تم الحفظ بنجاااااح");
                InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, store.ID, store.Name + "مخزن", this.Name);
                isnew = false;
                var form = (frmStoreList)Application.OpenForms[nameof(frmStoreList)];
                if (form != null) form.refresh();
            }
        }
        void getdata()
        {
            txtName.Text =store.Name;
        }
        void setdata()
        {
            store.Name = txtName.Text;
        }
        void New()
        {
            store = new Store();
            isnew = true;
            getdata();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
           
                save();
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            New();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
             Delete(); 
        }
        private void frmStores_Load(object sender, EventArgs e)
        {
            
        }
        void Delete()
        {
            if (frmMain.CheckActionPermission(this.Name, Master.Actions.Delete))
            {
                dbDataContext db = new dbDataContext();
                if (MessageBox.Show(text: "هل تريد الحذف؟", caption: "تأكيد الحذف", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var log = db.StoreLogs.Where(x => x.SourceID == store.ID).Count();
                    var journals = db.Journals.Where(x => x.AccountID == store.SalesAccountID ||
                    x.AccountID == store.SalesReturnAccountID ||
                    x.AccountID == store.CostOfSoldGoodsAccountID ||
                    x.AccountID == store.InventoryAccountID||
                    x.AccountID==store.ExpencesItemsAccountID).Count();
                    if (log + journals > 0)
                    {
                        XtraMessageBox.Show(text: "عفوا لايمكن حذف هذا الفرغ حيث تم استخدامه بالنظام",
                           buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error, caption: "");
                        return;
                    }

                    db.Stores.Attach(store);
                    db.Stores.DeleteOnSubmit(store);
                    db.Accounts.DeleteAllOnSubmit(db.Accounts.Where(x => x.ID == store.SalesAccountID ||
                    x.ID == store.SalesReturnAccountID ||
                    x.ID == store.CostOfSoldGoodsAccountID ||
                    x.ID == store.InventoryAccountID||
                    x.ID==store.ExpencesItemsAccountID));
                    db.SubmitChanges();
                    MessageBox.Show("تم الحذف بنجــاح");
                    InsertUserLog(Master.Actions.Delete,store.ID, store.Name + "مخزن", this.Name);
                    New();
                }
            }
        }
    }
    
   
}
