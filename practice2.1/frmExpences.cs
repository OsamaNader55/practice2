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
    public partial class frmExpences : frmMaster
    {
        Expence exp;
        Account account;
        ExpencesItem expences;
        Drawer insdrawer;
        Store insstore;
        public frmExpences()
        {
            InitializeComponent();
            checkEdit1.EditValueChanged += CheckEdit1_EditValueChanged;
            LoadData();
            RefreshData();
            New();
        }
       

        private void CheckEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (checkEdit1.Checked == false) { glkpDrawer.Enabled = false; }
            else glkpDrawer.Enabled = true;
        }

        void New()
        {
            exp = new Expence();
           
            isnew = true;
            GetData();
        }
        void SetData()
        {
           exp.ExpencesItemID=Convert.ToInt32(glkpExpenceItem.EditValue);
         if(checkEdit1.Checked)   exp.DrawerID = Convert.ToInt32(glkpDrawer.EditValue);
            exp.Notes=txtNotes.Text;
            exp.Value =Convert.ToDouble( spnValue.EditValue);
            exp.Date= DateTime.Now;
            exp.StoreID=Convert.ToInt32(glkpStore.EditValue);
            exp.UserID = Session.User.ID;
        }
        void GetData()
        {
            txtNotes.Text = exp.Notes;
            glkpExpenceItem.EditValue = exp.ExpencesItemID;
            glkpDrawer.EditValue = exp.DrawerID;
            glkpStore.EditValue = exp.StoreID;
            spnValue.EditValue = exp.Value;
            if (exp.DrawerID != null && exp.DrawerID != 0) { checkEdit1.Checked = true; }else { checkEdit1.Checked = false; }   
        }
        void LoadData()
        {using (var db = new dbDataContext())
            {
                glkpExpenceItem.InitializeData(db.ExpencesItems);
                
            }
            glkpDrawer.InitializeData(Session.Drawer);
            glkpStore.InitializeData(Session.Store);

        }

        private void frmExpences_Load(object sender, EventArgs e)
        {
            checkEdit1.Checked = true;
            glkpDrawer.Properties.PopulateViewColumns();
            glkpExpenceItem.Properties.PopulateViewColumns();
            glkpStore.Properties.PopulateViewColumns();
            gridView2.PopulateColumns();
            glkpExpenceItem.Properties.View.Columns[nameof(expences.ID)].Visible = false;
            glkpExpenceItem.Properties.View.Columns[nameof(expences.AccountID)].Visible = false;
            glkpExpenceItem.Properties.View.Columns[nameof(expences.Name)].Caption = "الاسم";
            glkpExpenceItem.Properties.View.Columns[nameof(expences.Notes)].Caption = "ملاحظات";
            glkpDrawer.Properties.View.Columns[nameof(insdrawer.ID)].Visible = false;
            glkpDrawer.Properties.View.Columns[nameof(insdrawer.AccountID)].Visible = false;
            glkpDrawer.Properties.View.Columns[nameof(insdrawer.Name)].Caption = "الاسم";
            glkpStore.Properties.View.Columns[nameof(insstore.ID)].Visible = false;
            glkpStore.Properties.View.Columns[nameof(insstore.SalesAccountID)].Visible = false;
            glkpStore.Properties.View.Columns[nameof(insstore.CostOfSoldGoodsAccountID)].Visible = false;
            glkpStore.Properties.View.Columns[nameof(insstore.DiscountAllowedAccountID)].Visible = false;
            glkpStore.Properties.View.Columns[nameof(insstore.DiscountReceivedAccountID)].Visible = false;
            glkpStore.Properties.View.Columns[nameof(insstore.ExpencesItemsAccountID)].Visible = false;
            glkpStore.Properties.View.Columns[nameof(insstore.InventoryAccountID)].Visible = false;
            glkpStore.Properties.View.Columns[nameof(insstore.SalesReturnAccountID)].Visible = false;
            glkpStore.Properties.View.Columns[nameof(insstore.Name)].Caption = "الاسم";
           
            gridView2.Columns["itemname"].Caption = "البند";
            gridView2.Columns["Storename"].Caption = "المخزن/الفرع";
            gridView2.Columns["drawername"].Caption = "الخزنه";
            gridView2.Columns["Value"].Caption = "القيمه";
            gridView2.Columns["Date"].Caption = "التاريخ";
            gridView2.Columns["Notes"].Caption = "ملاحظات";



        }
        void Save()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit) == false) return;
            if (glkpExpenceItem.IsEditValueValidAndNotZero() == false || spnValue.IsValuenotEqualORlessthanZero() == false||glkpStore.IsEditValueValidAndNotZero()==false)
            {
                if (checkEdit1.Checked == true)
                {
                    if (glkpDrawer.IsEditValueValidAndNotZero() == false) return;
                }
                return;
            }
            using (var db = new dbDataContext())
            {
                if(exp.ID == 0)
                {
                    db.Expences.InsertOnSubmit(exp);
                   
                }
                else
                {
                    db.Expences.Attach(exp);
                }
                SetData();
                db.Journals.DeleteAllOnSubmit(db.Journals.Where(x => x.Sourcetype == (byte)Master.SourceType.ExpencesItems && x.SourceID == exp.ID));
                db.SubmitChanges();
                var storeAccount = db.Stores.Single(x => x.ID == exp.StoreID);
              var name=db.ExpencesItems.Single(x=>x.ID==exp.ID).Name;
                var drawerAccount = db.Drawers.Single(x => x.ID == exp.DrawerID).AccountID;
                db.Journals.InsertOnSubmit(new Journal() //store Inventory
                {
                    AccountID = storeAccount.InventoryAccountID,
                    Code = 5555,
                    Credit = 0,
                    Debit = exp.Value,
                    OperationType = (byte)Master.JournalsOperationType.StoreInventory,
                    InsertDate = exp.Date,
                    SourceID = exp.ID,
                    Sourcetype =(byte) Master.SourceType.ExpencesItems,
                    Notes ="بند مصروفات"+name,
                });
                db.Journals.InsertOnSubmit(new Journal() //store expencesItem
                {
                    AccountID = storeAccount.ExpencesItemsAccountID,
                    Code = 5555,
                    Credit = exp.Value,
                    Debit = 0,
                    OperationType = (byte)Master.JournalsOperationType.ExpenecesItems,
                    InsertDate = exp.Date,
                    SourceID = exp.ID,
                    Sourcetype = (byte)Master.SourceType.ExpencesItems,
                    Notes = "بند مصروفات"+name,
                });
                if (exp.DrawerID != null)
                {
                    db.Journals.InsertOnSubmit(new Journal() //drawer 
                    {
                        AccountID = drawerAccount,
                        Code = 5555,
                        Credit = exp.Value,
                        Debit = 0,
                        OperationType = (byte)Master.JournalsOperationType.Drawer,
                        InsertDate = exp.Date,
                        SourceID = exp.ID,
                        Sourcetype = (byte)Master.SourceType.ExpencesItems,
                        Notes = "بند مصروفات" + name,
                    });
                }

                db.SubmitChanges();
                InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, exp.ID, "بند مصروفات "+name);
                isnew = false;
                
                btnDelete.Enabled = true;
                RefreshData();
                MessageBox.Show("تم الحفظ بنجاااااح");

            }
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            New();
        }
        void RefreshData()
        {
            using (var db = new dbDataContext())
            {
                var q = from exp in db.Expences.DefaultIfEmpty()
                        from str in db.Stores.Where(x => x.ID == exp.StoreID).DefaultIfEmpty()
                        from drw in db.Drawers.Where(x => x.ID == exp.DrawerID).DefaultIfEmpty()
                        from itm in db.ExpencesItems.Where(x=> x.ID == exp.ExpencesItemID).DefaultIfEmpty()
                        select new
                        {
                          itemname=  itm.Name,
                          Storename=  str.Name,
                           drawername= drw.Name,
                           exp.Value,
                           exp.Date,
                           exp.Notes
                        };
                gridControl1.DataSource = q.ToList();
                
             
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshData();
        }
    }
}
