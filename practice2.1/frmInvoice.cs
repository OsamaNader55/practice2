using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraLayout;

namespace practice2._1
{
    public partial class frmInvoice : frmMaster
    {
        //Instances
        CustomersAndVendor insCusVen;
        InvoiceHeader Invoice;
        Master.InvoiceType Type;
        dbDataContext GeneralDB;
        InvoiceDetail insDetails = new InvoiceDetail();
        UnitName insUnitnames = new UnitName();
        Session.ProductViewClass.ProductUOMview insUnits = new Session.ProductViewClass.ProductUOMview();
        Session.ProductViewClass insProducts = new Session.ProductViewClass();
        RepositoryItemGridLookUpEdit repoItem;
        RepositoryItemGridLookUpEdit repoItemsAll;
        RepositoryItemGridLookUpEdit repoUOm;
        RepositoryItemLookUpEdit repoStore;
        public frmInvoice(Master.InvoiceType _type)
        {
            InitializeComponent();
            Type = _type;
            lkpPartType.EditValueChanged += LkpPartType_EditValueChanged;
            glkpPartID.EditValueChanged += GlkpPartID_EditValueChanged;
            Refreshdata();  
          
            New();
        }
        public frmInvoice(Master.InvoiceType _type, int id)
        {
            InitializeComponent();
            Type = _type;
            Refreshdata();
            lkpPartType.EditValueChanged += LkpPartType_EditValueChanged;
            glkpPartID.EditValueChanged += GlkpPartID_EditValueChanged;
            using ( var db = new dbDataContext())
            {
                Invoice=db.InvoiceHeaders.Single(x=> x.ID==id);
            }
            Getdata();
        }
        private void GlkpPartID_EditValueChanged(object sender, EventArgs e)
        {
            Finance.AccountBalance accountBalance;
            int id = Convert.ToInt32(glkpPartID.EditValue);
            if(id != 0)
            {
                CustomersAndVendor account;
                if (Convert.ToInt32( lkpPartType.EditValue)==(int)Master.Parttype.Vendor)
                {
                    account=Session.Vendor.Single(x=>x.ID == id);
                }
                else  
                {
                    account = Session.Customer.Single(x => x.ID == id);
                }
                if (Type == Master.InvoiceType.purchaseReturn || Type == Master.InvoiceType.salesReturn)
                {
                    using (var db = new dbDataContext())
                    {
                        var inv = db.InvoiceHeaders.Where(x => x.InvoiceType == ((Type == Master.InvoiceType.purchaseReturn) ? (byte)Master.InvoiceType.purchase : (byte)Master.InvoiceType.sales)
                          && x.PartType == Convert.ToByte(lkpPartType.EditValue) && x.PartID == id).Select(x => new { x.Code, x.ID }).ToList();
                        glkpSourceID.InitializeData(inv, "ID", "Code");
                        glkpSourceID.EditValue = null;
                    }
                }
                txtPartAddress.Text = account.Address;  
                txtPartPhone.Text = account.Phone;  
                spnMaxCredit.EditValue = account.MaxCredit;
                accountBalance = Finance.GetAccountBalance(account.AccountID);
                if(accountBalance != null)
                txtPartBalnce.Text = accountBalance.Balance;
            }
            else
            {
                txtPartAddress.Text =
                txtPartPhone.Text = 
                txtPartBalnce.Text = "";
                spnMaxCredit.EditValue = 0;
            }
        }

        string GetNewInvoiceCode()
        {
            string maxcode;
            using (var db = new dbDataContext())
            {
                maxcode = db.InvoiceHeaders.Where(x => x.InvoiceType == (int)Type).OrderByDescending(x=>x.Date).Select(x=>x.Code??"1").FirstOrDefault();
            }
            return Master.GetNextNumberInSTring(maxcode);
        }
        private void frmInvoice_Load(object sender, EventArgs e)
        {
            if (Debugger.IsAttached == false)
            {
                gridView1.RestoreGridLayout(this.Name);
                layoutControl1.RestoreLayoutControlLayout(this.Name);
            }
            switch (Type)
            {
                case Master.InvoiceType.purchase:
                    this.Text = "فاتوره مشتريات ";
                    this.Name = Screens.InvoicePurchase.ScreenName;
                    gridView1.Columns[nameof(insDetails.StoreID)].OptionsColumn.AllowFocus = false;
                    checkPostToStore.Checked = false;
                    lkpPartType.EditValue = Master.Parttype.Vendor;
                    gridView1.RowCountChanged += GridView1_RowCountChanged;
                    gridView1.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
                    break;
                case Master.InvoiceType.sales:
                    this.Text = "فاتوره مبيعات ";
                    this.Name = Screens.InvoiceSales.ScreenName;
                    this.checkPostToStore.Checked = true;
                    lkpPartType.EditValue = Master.Parttype.Customer;
                    gridView1.RowCountChanged += GridView1_RowCountChanged;
                    gridView1.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
                   
                    break;
                case Master.InvoiceType.purchaseReturn:
                    this.Text = "فاتوره مردود مشتريات ";
                    this.Name = Screens.InvoicePurchaseReturn.ScreenName;
                    gridView1.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                    gridView1.OptionsBehavior.AllowAddRows = DefaultBoolean.False;
                    AddReturnColumn();
                
                    break;
                case Master.InvoiceType.salesReturn:
                    this.Text = "فاتوره مردود مبيعات ";
                    this.Name = Screens.InvoicePurchaseReturn.ScreenName;
                    gridView1.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                    gridView1.OptionsBehavior.AllowAddRows = DefaultBoolean.False;
                    AddReturnColumn();
                    break;
                
            }
            void AddReturnColumn()
            {
                gridView1.Columns[nameof(insDetails.StoreID)].OptionsColumn.AllowFocus = false;
                checkPostToStore.Checked = false;
                lycSourceId.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                gridView1.Columns.Add(new GridColumn()
                {
                    Name = "clmSourceQty",
                   
                    Caption="الكميه الاساسيه",
                    FieldName = "SourceQty",
                    UnboundType = DevExpress.Data.UnboundColumnType.Decimal,
                    VisibleIndex = gridView1.Columns[nameof(insDetails.ItemQty)].VisibleIndex - 1,

                });
                gridView1.Columns.Add(new GridColumn()
                {
                    Name = "clmOtherQty",
                    FieldName = "OtherQty",
                    Caption = "الكميه الاخري",
                    UnboundType = DevExpress.Data.UnboundColumnType.Decimal,
                    VisibleIndex = gridView1.Columns[nameof(insDetails.ItemQty)].VisibleIndex - 1,

                });
                gridView1.Columns["OtherQty"].OptionsColumn.AllowEdit = gridView1.Columns["SourceQty"].OptionsColumn.AllowEdit = true;
            }
           
            spnNet.EditValue = NoTaxNoDiscount();
            lkpPartType.InitializeData(Master.PartTypeList);

            #region gridRepos
            //gridrepo items prop
            repoItem = new RepositoryItemGridLookUpEdit();
            repoItemsAll = new RepositoryItemGridLookUpEdit();
            //when the user clicks on the repogrid it shows the data with filering with active but in in the normal it takes all the dataSource with no filtering 
            repoItem.InitializeData(Session.ProductsView.Where(x => x.IsActive == true), gridView1.Columns[nameof(insDetails.ItemID)], gridControl1);
            repoItemsAll.InitializeData(Session.ProductsView, gridView1.Columns[nameof(insDetails.ItemID)], gridControl1);
            repoItem.ValidateOnEnterKey = true;
            repoItem.AllowNullInput =DefaultBoolean.False;
            repoItem.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            repoItem.ImmediatePopup = true;
            repoItem.PopulateViewColumns();
            var repoView = repoItem.View;
            repoView.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            repoView.OptionsView.ShowAutoFilterRow = true;
            
            
            repoView.Columns[nameof(insProducts.IsActive)].Visible = false;
            repoView.Columns[nameof(insProducts.ID)].Visible = false;
            repoView.Columns[nameof(insProducts.Name)].Caption = "الاســـم";
            repoView.Columns[nameof(insProducts.Code)].Caption = "الكود";
            repoView.Columns[nameof(insProducts.Type)].Caption = "النوع";
            repoView.Columns[nameof(insProducts.CategoryName)].Caption = "الفئه";
            repoView.Columns[nameof(insProducts.Description)].Caption = "الوصف";
            repoView.Columns[nameof(insProducts.Name)].MinWidth = 125;
            




            repoUOm =new RepositoryItemGridLookUpEdit();
            repoUOm.InitializeData(Session.UnitNames, gridView1.Columns[nameof(insDetails.ItemUnitID)], gridControl1);
            repoUOm.PopulateViewColumns();
           repoUOm.View.Columns["ID"].Visible = false;
            gridView1.Columns[nameof(insDetails.TotalPrice)].OptionsColumn.AllowFocus = false;



            repoStore = new RepositoryItemLookUpEdit();
            repoStore.InitializeData(Session.Store, gridView1.Columns[nameof(insDetails.StoreID)], gridControl1);


            RepositoryItemSpinEdit repoSpn = new RepositoryItemSpinEdit();
            gridView1.Columns[nameof(insDetails.Price)].ColumnEdit = repoSpn;
            gridView1.Columns[nameof(insDetails.ItemQty)].ColumnEdit = repoSpn;
            gridView1.Columns[nameof(insDetails.DiscountValue)].ColumnEdit = repoSpn;
            gridControl1.RepositoryItems.Add(repoSpn);


            RepositoryItemSpinEdit repoSpnRatio = new RepositoryItemSpinEdit();
            gridView1.Columns[nameof(insDetails.Discount)].ColumnEdit = repoSpnRatio;
            repoSpnRatio.Increment = 0.01m;
            repoSpnRatio.MaxValue = 1;
            repoSpnRatio.EditMask = "p";
            repoSpnRatio.UseMaskAsDisplayFormat = true; 
            gridControl1.RepositoryItems.Add(repoSpnRatio);


            //Adding product code to the grid view that doesn't exist in datasource so u have to edit it manually because data won't be saved 
            //Adding Unbounded column for Barcode 
            gridView1.Columns.Add(new GridColumn() {  Name = "clmCode", FieldName = "Code",Caption="الكود", UnboundType =DevExpress.Data.UnboundColumnType.String}) ;
            gridView1.CustomUnboundColumnData += GridView1_CustomUnboundColumnData;

            //Ading deleteColumn button 
            RepositoryItemButtonEdit repoButton = new RepositoryItemButtonEdit();
            gridView1.Columns.Add(new GridColumn { Name = "clmBtn", FieldName = "Delete", Caption = "حذف", VisibleIndex = 100,MaxWidth=40 });
            gridControl1.RepositoryItems.Add(repoButton)  ;
            repoButton.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            repoButton.Buttons.Clear();
            repoButton.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
            gridView1.Columns["Delete"].ColumnEdit= repoButton;
            repoButton.ButtonClick += RepoButton_ButtonClick;
            #endregion
            #region GridViewProp
            gridView1.Columns["Code"].VisibleIndex = 0;
            gridView1.Columns[nameof(insDetails.ItemID)].VisibleIndex = 1;
            gridView1.Columns[nameof(insDetails.ItemUnitID)].VisibleIndex = 2;
            gridView1.Columns[nameof(insDetails.ItemQty)].VisibleIndex = 3;
            gridView1.Columns[nameof(insDetails.Price)].VisibleIndex = 6;
            gridView1.Columns[nameof(insDetails.Discount)].VisibleIndex = 7;
            gridView1.Columns[nameof(insDetails.DiscountValue)].VisibleIndex = 8;
            gridView1.Columns[nameof(insDetails.TotalPrice)].VisibleIndex = 9;
            gridView1.Columns[nameof(insDetails.CostValue)].VisibleIndex = 10;
            gridView1.Columns[nameof(insDetails.TotalCostValue)].VisibleIndex = 11;
            gridView1.Columns[nameof(insDetails.StoreID)].VisibleIndex = 12;
            gridView1.Columns[nameof(insDetails.ItemID)].Caption = "الصنف";
            gridView1.Columns[nameof(insDetails.ItemUnitID)].Caption = "الوحده";
            gridView1.Columns[nameof(insDetails.ItemQty)].Caption = "الكميه";
            gridView1.Columns[nameof(insDetails.Price)].Caption = "سعر المنتج";
            gridView1.Columns[nameof(insDetails.Discount)].Caption = "نسبه الخصم";
            gridView1.Columns[nameof(insDetails.DiscountValue)].Caption = "قيمه الخصم";
            gridView1.Columns[nameof(insDetails.TotalPrice)].Caption = "السعر الكلي";
            gridView1.Columns[nameof(insDetails.CostValue)].Caption = "سعر التكلفه";
            gridView1.Columns[nameof(insDetails.TotalCostValue)].Caption = "اجمالي سعر التكلفه";
            gridView1.Columns[nameof(insDetails.StoreID)].Caption = "المخزن";
            gridView1.Columns[nameof(insDetails.CostValue)].OptionsColumn.AllowFocus = false;
            gridView1.Columns[nameof(insDetails.TotalCostValue)].OptionsColumn.AllowFocus = false;
          
            gridView1.Columns[nameof(insDetails.ID)].Visible = false;
            gridView1.Columns[nameof(insDetails.InvoiceID)].Visible = false;
            gridView1.Columns[nameof(insDetails.SourceRowId)].Visible = false;
            gridView1.Columns[nameof(insDetails.ItemID)].MinWidth = 125;
            //lkp partname
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
            
                
            //for back color even rows 
            gridView1.Appearance.EvenRow.BackColor = Color.FromArgb(206, 221, 245);
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            #endregion 
            ReaduserSettings();
            #region events
            glkpPartID.ButtonClick += LkpPartID_ButtonClick;
            spnDiscountValue.Enter += new System.EventHandler(this.spnDiscountValue_Enter);
            spnDiscountValue.Leave += SpnDiscountValue_Leave;
            spnDiscountValue.EditValueChanged += SpnDiscountValue_EditValueChanged;
            spnDiscountRation.EditValueChanged += SpnDiscountValue_EditValueChanged;
            spnTaxValue.EditValueChanged += SpnTaxValue_EditValueChanged;
            spnTaxRation.EditValueChanged += SpnTaxValue_EditValueChanged;
            spnTaxValue.Leave += SpnTaxValue_Leave;
            spnTaxValue.Enter += SpnTaxValue_Enter;
            spnDiscountValue.EditValueChanged += Spn_EditValueChanged;
            spnTotal.EditValueChanged += Spn_EditValueChanged;
            spnTaxValue.EditValueChanged += Spn_EditValueChanged;
            spnExpences.EditValueChanged += Spn_EditValueChanged;
            spnPaid.EditValueChanged += SpnPaid_EditValueChanged;
            spnNet.EditValueChanged += SpnPaid_EditValueChanged;
            spnNet.EditValueChanging += SpnNet_EditValueChanging;
          
            gridView1.CustomRowCellEditForEditing += GridView1_CustomRowCellEditForEditing;
            gridView1.CellValueChanged += GridView1_CellValueChanged;
            
            gridView1.RowUpdated += GridView1_RowUpdated;
            lkpBranch.EditValueChanging += LkpBranch_EditValueChanging;
            gridControl1.ProcessGridKey += GridControl1_ProcessGridKey;
            gridView1.ValidateRow += GridView1_ValidateRow;
            gridView1.InvalidRowException += GridView1_InvalidRowException;
            gridView1.CellValueChanging += GridView1_CellValueChanging;
            glkpSourceID.EditValueChanged += GlkpSourceID_EditValueChanged;
            this.Activated += FrmInvoice_Activated;
            this.KeyPreview = true;
            this.KeyDown += FrmInvoice_KeyDown;
            this.FormClosing += FrmInvoice_FormClosing;

            #endregion
            //Allow user to Cutomize layout design
            btnCustomization.ItemClick += (ss, ee) => { layoutControl1.ShowCustomizationForm(); };
            layoutControl1.OptionsCustomizationForm.ShowSaveButton = layoutControl1.OptionsCustomizationForm.ShowLoadButton = layoutControl1.OptionsCustomizationForm.ShowPropertyGrid = false;
            //u have to prevent user from hiding items 
            foreach (BaseLayoutItem item in layoutControl1.Items)
            {
                item.AllowHide = false;
            }
            MoveFocusToView();
        }
        private void GlkpSourceID_EditValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.RowCount;)
                gridView1.DeleteRow(i);

            var source = gridView1.DataSource as Collection<InvoiceDetail>;
            //Never use Clear() as it removes all data 
           
            if (source == null) return;
            if (glkpSourceID.EditValue is int SourceId && SourceId>0)
            {
                using (var db = new dbDataContext())
                {
                    ReturnSourceDtail = db.InvoiceDetails.Where(x => x.InvoiceID == SourceId).ToList() ;
                    
                    var inv =db.InvoiceHeaders.Single(x=>x.ID == SourceId);
                    spnDiscountRation.EditValue = inv.DiscountRation;
                    spnTaxRation.EditValue=inv.Tax;
                    foreach (var row in ReturnSourceDtail)
                    {
                        if (source.Where(x => x.SourceRowId == row.ID).Count() == 0)
                        {
                            source.Add(new InvoiceDetail()
                            {
                                SourceRowId = row.ID,
                                CostValue = row.CostValue,
                                ItemID = row.ItemID,
                                ItemUnitID = row.ItemUnitID,
                                ItemQty =0,
                                Discount = row.Discount,
                                DiscountValue = row.DiscountValue,
                                Price = row.Price,
                                StoreID = row.StoreID,
                                TotalCostValue = row.TotalCostValue,
                                TotalPrice = row.TotalPrice,
                                InvoiceID = row.InvoiceID,
                                ID=row.ID,
                            });
                        }
                       
                    }
                }
            }
        }

        private void FrmInvoice_FormClosing(object sender, FormClosingEventArgs e)
        {
            gridView1.SaveGridLayout(this.Name);
            layoutControl1.SaveLayoutControlLayout(this.Name);
        }
        private void GridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == nameof(insDetails.ItemID))
            {
                var row = gridView1.GetRow(e.RowHandle) as InvoiceDetail;
                if(row != null)
                {
                    if(row.ItemID!= 0 && e.Value.Equals(row.ItemID)==false) row.ItemID = 0;

                }

            }
        }
        //fast commands using keyboard
        private void FrmInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                MoveFocusToView(e.Modifiers==Keys.Shift);
            }
            else if (e.KeyCode == Keys.F7)
            {
                glkpPartID.Focus();
            }
            else if (e.KeyCode == Keys.F8)
            {
                lkpBranch.Focus();
            }
            else if (e.KeyCode == Keys.F9)
            {
                spnPaid.Focus();
            }
            else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
            {
                if (gridView1.FocusedRowHandle >= 0)
                {
                    gridView1.DeleteSelectedRows();
                    MoveFocusToView();
                }
            }
        }
        //Delet row
        private void RepoButton_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            GridView view = ((GridControl)((ButtonEdit)sender).Parent).MainView as GridView;
         if(view.FocusedRowHandle>=0)
            {
                view.DeleteSelectedRows();
                MoveFocusToView();
            }
        }
        private void GridView1_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
            var row = e.Row as InvoiceDetail;
            //double? sourceQty = gridView1.GetRowCellValue(e.RowHandle, "SourceQty") as double?;
            //double? otherQty = gridView1.GetRowCellValue(e.RowHandle, "OtherQty") as double?;
            if (row == null || row.ItemID == 0 || row.ItemUnitID == 0 /*|| row.ItemQty > (sourceQty ?? 0 - otherQty ?? 0)*/)
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.Ignore; 
            } 
        }
        //don't allow to add a null column 
        private void GridView1_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            var row = e.Row as InvoiceDetail;
            if (row == null || row.ItemID == 0 || row.ItemUnitID == 0)
            {
                e.Valid = false;
                return;
            }
                switch (Type)
                {
                    case Master.InvoiceType.purchase:
                        break;
                    case Master.InvoiceType.sales:
                    if (row.CostValue > 0)
                    {
                        if (row.CostValue > row.Price)
                        {
                            switch (Session.Settings.Sales.SellingItemWithPriceLowerThanCostPrice)
                            {
                                case Master.WarningLevels.DonnotEnterupt:
                                    break;
                                case Master.WarningLevels.ShowWarning:
                                    if (MessageBox.Show(caption: "تاكيد البيع", text: "لقد تخطي هذا العميل حد الائتمان هل تريد المتابعه ؟", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                                    {
                                        e.Valid = false;
                                        gridView1.SetColumnError(gridView1.Columns[nameof(row.Price)], "سعر البيع اقل من سعر التكلفه");
                                    }
                                    break;
                                case Master.WarningLevels.Prevent:
                                    e.Valid = false;
                                    gridView1.SetColumnError(gridView1.Columns[nameof(row.Price)], "سعر البيع اقل من سعر التكلفه");
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if ((decimal)row.Discount > Session.Settings.Sales.MaxDiscountPerItem)
                    {
                        e.Valid = false;
                        gridView1.SetColumnError(gridView1.Columns[nameof(row.Discount)], "نسبه الخصم غير مسوح بها");
                    }
                    break;
                    
                    case Master.InvoiceType.purchaseReturn:
                      
                    case Master.InvoiceType.salesReturn:
                    double? sourceQty = gridView1.GetRowCellValue(e.RowHandle, "SourceQty") as double?;
                    double? otherQty = gridView1.GetRowCellValue(e.RowHandle, "OtherQty") as double?;
                    double? itemQty = gridView1.GetRowCellValue(e.RowHandle, nameof(insDetails.ItemQty)) as double?;
                    if (itemQty > (sourceQty ?? 0 - otherQty ?? 0))
                          {
                        e.Valid = false;
                        gridView1.SetColumnError(gridView1. Columns[nameof(insDetails.ItemQty)], "لا يممكن ان تكون الكميه المرتجعه اكبر من الكميه المتاحه من المصدر");
                          }
                        break;
                    default:throw new NotFiniteNumberException();    
                }
        }

        //Set the enter key 
        private void GridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            GridControl control = sender as GridControl;
            if (control == null) return;
            GridView view = control.FocusedView as GridView;    
            if(view == null) return;
            if (e.KeyCode==Keys.Return)
            {
                if (view.FocusedColumn == null) return;
                string focusedColumn = view.FocusedColumn.FieldName;
                if(focusedColumn == "Code" || focusedColumn == nameof(insDetails.ItemID))
                {
                    GridControl1_ProcessGridKey(sender,new KeyEventArgs(Keys.Tab));
                }
                if(view.FocusedRowHandle < 0&&(Type==Master.InvoiceType.purchase||Type==Master.InvoiceType.purchase))
                {
                    view.AddNewRow();
                    
                    gridView1.Focus();
                    view.FocusedColumn=view.Columns[focusedColumn];
                   
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Tab)
            {
                view.FocusedColumn = view.VisibleColumns[view.FocusedColumn.VisibleIndex + 1];
                e.Handled=true; 
            }
        }
        //setting the code to change to empty after using it 
        string enterCode;
        private void GridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
           if (e.Column.FieldName == "Code")
            {
                if (e.IsSetData)
                {
                    enterCode = e.Value.ToString();  
                    
                }
                else if (e.IsGetData)
                {
                    e.Value = enterCode;
                }
            }
            else if (e.Column.FieldName == "SourceQty")
            {
                var row = e.Row as InvoiceDetail;
                if (row == null) return;
               
                if (e.IsGetData)
                {
                    var sourcerow =ReturnSourceDtail.SingleOrDefault(x=>x.ID==row.SourceRowId);
                    if(sourcerow != null)
                        e.Value=sourcerow.ItemQty;
                    else e.Value = 0;
                    
                }
            }
            else if (e.Column.FieldName == "OtherQty")
            {
                if (e.IsGetData)
                {
                    using (var db = new dbDataContext())
                    {
                        var row = e.Row as InvoiceDetail;
                        if (row == null) return;
                        var otherReturnRows =db.InvoiceDetails.Where(x=>x.SourceRowId==row.SourceRowId&&x.ID!=row.ID).Sum(x=>(double?)x.ItemQty)??0;
                        e.Value = otherReturnRows;
                    }
                }
            }
        }
       List< InvoiceDetail> ReturnSourceDtail =new List<InvoiceDetail>() ;

        // if user changed the default store the grid will change the store auto
        private void LkpBranch_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            var items = gridView1.DataSource as Collection<InvoiceDetail>;
            if (e.OldValue is int && e.NewValue is int)
            {
                foreach (var row in items)
                {
                    if (row.StoreID == Convert.ToInt32(e.OldValue))
                    {
                        row.StoreID= Convert.ToInt32(e.NewValue);   
                    }
                }
            }
        }
        //the paid changes auto when NetChanges
        private void SpnNet_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            //the compiler cannot compare between objects so u need to convert it's value 
            
             if(Type==Master.InvoiceType.sales&& Session.Settings.Sales.DefaultPayMethodinSales == Master.PayMethod.Cash)
            {
                if (Convert.ToDouble(e.OldValue) == Convert.ToDouble(spnPaid.EditValue))
                    spnPaid.EditValue = e.NewValue;
            }
            else if (Type == Master.InvoiceType.sales && Session.Settings.Sales.DefaultPayMethodinSales == Master.PayMethod.Credit)
            {
                spnPaid.EditValue = 0;
            }
        }
        //Counting the total price
        private void GridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            var items = gridView1.DataSource as Collection<InvoiceDetail>;
            if (items == null)
            {
                spnTotal.EditValue = 0;
            }
            else
            {
                spnTotal.EditValue = items.Sum(x => x.TotalPrice);
            }
            
        }
        //Edit item quantity if the user added the same item or changed it's quantity
        int currentRowsCount = 0;
        private void GridView1_RowCountChanged(object sender, EventArgs e)
        {
           
            if (currentRowsCount < gridView1.RowCount)
            {
                var rows = gridView1.DataSource as Collection<InvoiceDetail>;
                var lastRow = rows.Last();
                var row=rows.FirstOrDefault(x=> x.ItemID==lastRow.ItemID && x.ItemUnitID==lastRow.ItemUnitID && x.StoreID == lastRow.StoreID && x.Price == lastRow.Price&& x !=lastRow);
                if(row != null)
                {
                    row.ItemQty+=lastRow.ItemQty;
                    rows.Remove(lastRow);
                    //Doing this to update data changed in quantity and do it's calacs
                    GridView1_CellValueChanged(sender, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(gridView1.GetRowHandleByRowObject(row), gridView1.Columns[nameof(insDetails.ItemQty)], row.ItemQty));
                }
            }

            currentRowsCount = gridView1.RowCount;

            //Counting the new total
            GridView1_RowUpdated(sender, null);
           
        }
       //Getting the products info with many ways (barecode-serach in items)
        private void GridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
           
            var row = gridView1.GetRow(e.RowHandle) as InvoiceDetail;
            Session.ProductViewClass itemv = null;
            Session.ProductViewClass.ProductUOMview unitv = null;

            if (row == null )
                return;
            //filtering with barecode first
            if(e.Column.FieldName == "Code")
            {
                if ( e.Value == null || e.Value.ToString() == string.Empty )
                    return;
                itemv = Session.ProductsView.FirstOrDefault(x => x.Units.Select(s => s.Barcode).Contains(e.Value.ToString()));
                if (itemv != null)
                {
                    row.ItemID = itemv.ID;
                    unitv = itemv.Units.First(x=> x.Barcode==e.Value.ToString());
                    row.ItemUnitID = unitv.UnitID;
                    GridView1_CellValueChanged(sender, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(insDetails.ItemID)], row.ItemID));
                    GridView1_CellValueChanged(sender, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(insDetails.ItemUnitID)], row.ItemUnitID));
                    enterCode = String.Empty;
                   
                    return;
                }
                enterCode = String.Empty;
            }
            //or byitem
            if ( row.ItemID == 0)
                return;
             itemv=Session.ProductsView.Single(s=> s.ID == row.ItemID); 
            if(row.ItemUnitID == 0)
            {
                var defUnit= itemv.Units.Count();
                if (defUnit == 0) return;
                
                row.ItemUnitID = itemv.Units.First().UnitID;
                GridView1_CellValueChanged(sender, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(insDetails.ItemUnitID)], row.ItemUnitID));
              
            }         
             unitv=itemv.Units.First(x=> x.UnitID==row.ItemUnitID);
            switch (e.Column.FieldName)
            {
                case nameof(insDetails.ItemID):
                    if(row.ItemID == 0)
                    {
                        gridView1.DeleteRow(e.RowHandle);
                        return;
                    }
                    
                    if (row.StoreID==0&& lkpBranch.IsEditValueValidAndNotZero())
                        row.StoreID=Convert.ToInt32(lkpBranch.EditValue);
                    GridView1_CellValueChanged(sender, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(insDetails.ItemUnitID)], row.ItemUnitID));
                    break;
                case nameof(insDetails.ItemUnitID):
                    switch (Type)
                    {
                        case Master.InvoiceType.purchase:
                            row.Price = unitv.BuyPrice;
                            break;
                        case Master.InvoiceType.sales:
                            row.Price = unitv.SellPrice;
                            break;
                        case Master.InvoiceType.purchaseReturn:
                        case Master.InvoiceType.salesReturn:


                            var Returnsourcerow = ReturnSourceDtail.Where(x => x.ID == row.SourceRowId).SingleOrDefault();
                            if (Returnsourcerow != null)
                            {
                                row.Price = Returnsourcerow.Price;
                            }

                            break;
                        default:
                            throw new NotImplementedException();
                    }
                                   
                    if (row.ItemQty == 0)
                        row.ItemQty = 1;
                    GridView1_CellValueChanged(sender, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(insDetails.Price)], row.Price));
                    break;
                    case nameof(insDetails.Price):
                case nameof(insDetails.Discount):
                case nameof(insDetails.ItemQty):
                    row.DiscountValue = row.Discount * (row.ItemQty * row.Price);
                    row.TotalPrice = (row.ItemQty * row.Price) - row.DiscountValue;
                    GridView1_CellValueChanged(sender, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(insDetails.DiscountValue)], row.DiscountValue));
                    break;
                case nameof(insDetails.DiscountValue):
                    if(gridView1.FocusedColumn.FieldName==nameof(insDetails.DiscountValue))   
                        row.Discount=row.DiscountValue/(row.ItemQty*row.Price);
                    row.TotalPrice = (row.ItemQty * row.Price) - row.DiscountValue;
                    row.CostValue = row.TotalPrice / row.ItemQty;
                    row.TotalCostValue = row.TotalPrice;
                    break;
                default:
                    break;
            }
           
        }
        //Adding the repo items >> the extinsion method is in master class
        private void GridView1_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

            if (e.Column.FieldName == nameof(insDetails.ItemUnitID))
            {
                RepositoryItemLookUpEdit repo = new RepositoryItemLookUpEdit();
                repo.NullText = "";

                //e.RepositoryItem = repo;
                var row = gridView1.GetRow(e.RowHandle) as InvoiceDetail;
                if (row == null)
                {
                    return;
                };
                var item = Session.ProductsView.SingleOrDefault(x => x.ID == row.ItemID);
                if (item == null)
                {
                    return;
                };
                repo.InitializeData(item.Units, e.Column, gridControl1, nameof(insUnits.UnitID), nameof(insUnits.UnitName));
                repo.Columns.Clear();
                repo.Columns.Add(new LookUpColumnInfo( nameof(insUnits.UnitName)));
                repo.ShowHeader = false;

            }
            else if (e.Column.FieldName == nameof(insDetails.ItemID))
            { 
                e.RepositoryItem = repoItem;
            }
        }
        //Adding new Cus or Ven
        //u can do this also to Store or what ever what user wants
        private void LkpPartID_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
            {
                frmCustomerVendor frm = new frmCustomerVendor(Convert.ToInt32(lkpPartType.EditValue) == (int)Master.Parttype.Customer);
                
                    frmMain.OpenFormWithPermissions(frm,true);
                if (Type == (byte)Master.InvoiceType.purchase)
                {
                    glkpPartID.EditValue = Session.Vendor.Select(x=>x.ID).LastOrDefault();
                }
                else glkpPartID.EditValue = Session.Customer.Select(x => x.ID).LastOrDefault();
            }
            
        }
        bool IScodeExist()
        {
           using (var db = new dbDataContext())
            {
              var count = db.InvoiceHeaders.Where(x=> x.ID != Invoice.ID&&x.InvoiceType==(byte)Type&&x.Code==txtCode.Text).Count();
                if(count > 0)
                {
                    txtCode.ErrorText = "هذا الكود موجود ";
                    return false;
                }
                else return true;
            }
        }
   
        //Checkeing Data before Saving >> the extinsion methods is in master class
        bool IsdataValid()
        {
            int NumberOfErrors = 0;
            if (gridView1.RowCount == 0)
            {
                NumberOfErrors++;
                MessageBox.Show("برجاء ادخال صنف واحد علي الاقل");
            }
            if (Type == Master.InvoiceType.purchaseReturn || Type == Master.InvoiceType.salesReturn)
            {
                //if (gridView1.HasColumnErrors)
                //{
                //    NumberOfErrors++;
                //    MessageBox.Show("برجاء التحقق من اخطاء الجدول ");
                //}
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    gridView1.FocusedRowHandle = i;
                    GridView1_ValidateRow(gridView1, new DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs(1, gridView1.GetRow(i)));
                    if (gridView1.HasColumnErrors)
                    {
                        NumberOfErrors++;
                        MessageBox.Show("من فضلك تحقق من كميات المردود ,لا يممكن ان تكون الكميه المرتجعه اكبر من الكميه المتاحه من المصدر");
                        GridView1_ValidateRow(gridView1, new DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs(1, gridView1.GetRow(i)));
                    }
                }
            }
            NumberOfErrors += txtCode.validateText() ? 0 : 1;
            NumberOfErrors +=IScodeExist() ? 0 : 1;
            NumberOfErrors += lkpPartType.IsEditvalueNotNull() ? 0 : 1;
            NumberOfErrors += lkpBranch.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += lkpDrawer.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += glkpPartID.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += dtDate.IsdateValid() ? 0 : 1;
            NumberOfErrors += spnDiscountValue.IsValuenotlessthanZero() ? 0 : 1;
            NumberOfErrors += spnDiscountRation.IsValuenotlessthanZero() ? 0 : 1;
            NumberOfErrors += spnTaxValue.IsValuenotlessthanZero() ? 0 : 1;
            NumberOfErrors += spnTaxRation.IsValuenotlessthanZero() ? 0 : 1;
            NumberOfErrors += spnPaid.IsValuenotlessthanZero() ? 0 : 1;
            if (checkPostToStore.Checked)
            {
                NumberOfErrors += dtPostDate.IsdateValid() ? 0 : 1;
            }
            switch (Type)
            {
                case Master.InvoiceType.purchase:
                    break;
                case Master.InvoiceType.sales:
                    if (Invoice.DiscountRation != Convert.ToDouble(spnDiscountRation.EditValue) && Session.Settings.Sales.MaxDiscountInInvoice < (decimal)spnDiscountRation.EditValue)
                    {
                        NumberOfErrors++;
                        spnDiscountRation.ErrorText = "هذا الخصم غير مسموح به";
                    }
                    if (Invoice.ID == 0)
                    {
                        Finance.AccountBalance accountBalance;
                        int id = Convert.ToInt32(glkpPartID.EditValue);

                        if (id != 0)
                        {
                            CustomersAndVendor account;
                            if (Convert.ToInt32(lkpPartType.EditValue) == (int)Master.Parttype.Vendor)
                            {
                                account = Session.Vendor.Single(x => x.ID == id);
                            }
                            else
                            {
                                account = Session.Customer.Single(x => x.ID == id);
                            }
                            accountBalance = Finance.GetAccountBalance(account.ID);
                            if (account.MaxCredit < accountBalance.BalanceAmount && accountBalance.BalanceType == Finance.AccountBalance.BalanceTypes.Credit)
                            {
                                switch (Session.Settings.Sales.SellingToCustomerExceededMaxCredit)
                                {
                                    case Master.WarningLevels.DonnotEnterupt:
                                        break;
                                    case Master.WarningLevels.ShowWarning:
                                        if (MessageBox.Show(caption: "تاكيد البيع", text: "لقد تخطي هذا العميل حد الائتمان هل تريد المتابعه ؟", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                                        {
                                            NumberOfErrors++;
                                        }
                                        break;
                                    case Master.WarningLevels.Prevent:
                                        NumberOfErrors++;
                                        MessageBox.Show("لا يمكن البيع لهذا العميل لتخطيه حد الائتمان");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case Master.InvoiceType.purchaseReturn:

                case Master.InvoiceType.salesReturn:
                    break;
                default: throw new NotImplementedException();

            }
            return NumberOfErrors == 0; 
        }
       //Spin Header Calcualtions
        #region spnCalculations
        //calc auto Spn Discount and Tax (values and percent) when any of them changes
        Boolean IsValueFocused;
        private void spnDiscountValue_Enter(object sender, EventArgs e)
        {
            IsValueFocused = true;
        }
        private void SpnDiscountValue_Leave(object sender, EventArgs e)
        {
            IsValueFocused = false;
        }
        private void SpnDiscountValue_EditValueChanged(object sender, EventArgs e)
        {
            var total = Convert.ToDouble(spnTotal.EditValue);
            var Value = Convert.ToDouble(spnDiscountValue.EditValue);
            var Ration = Convert.ToDouble(spnDiscountRation.EditValue);
            if (IsValueFocused)
            {
                spnDiscountRation.EditValue = (Value / total);
            }
            else
            {
                spnDiscountValue.EditValue = (Ration * total);
            }
        }
        private void SpnTaxValue_EditValueChanged(object sender, EventArgs e)
        {
            var total = Convert.ToDouble(spnTotal.EditValue);
            var value=Convert.ToDouble(spnTaxValue.EditValue);  
            var Ration=Convert.ToDouble(spnTaxRation.EditValue);
            if (IsValueFocused)
            {
                spnTaxRation.EditValue= (value / total);
            }
            else
            {
                spnTaxValue.EditValue= (Ration*total); 
            }
        }

        private void SpnTaxValue_Leave(object sender, EventArgs e)
        {
            IsValueFocused = false;
        }
        private void SpnTaxValue_Enter(object sender, EventArgs e)
        {
            IsValueFocused = true;
        }
        //calc the net
        private void Spn_EditValueChanged(object sender, EventArgs e)
        {
            var total = Convert.ToDouble(spnTotal.EditValue);
            SpnTaxValue_EditValueChanged(sender, e);
            SpnDiscountValue_EditValueChanged(sender, e);
            var taxValue = Convert.ToDouble(spnTaxValue.EditValue);
            var discountValue = Convert.ToDouble(spnDiscountValue.EditValue);
            var expencesValue = Convert.ToDouble(spnExpences.EditValue);
            spnNet.EditValue = (total + taxValue + expencesValue - discountValue);
        }
        //calc the remaining
        private void SpnPaid_EditValueChanged(object sender, EventArgs e)
        {
            var Net = Convert.ToDouble(spnNet.EditValue);
            var paid =Convert.ToDouble(spnPaid.EditValue);
            spnRemaining.EditValue = paid-Net;
        }
        //if No Discount or tax added added(often in when u open the form)
        double NoTaxNoDiscount()
        {
          return Convert.ToDouble( spnNet.EditValue = spnTotal.EditValue);
        }
        #endregion
        //Getting invoice info from database  
       void Getdata()
        {
            lkpBranch.EditValue = Invoice.Branch;
            lkpDrawer.EditValue = Invoice.Drawer;
           lkpPartType.EditValue= Invoice.PartType;
           glkpPartID.EditValue = Invoice.PartID;
            glkpSourceID.EditValue=Invoice.SourceID;
            GlkpSourceID_EditValueChanged(null, null);
            txtCode.Text        = Invoice.Code;    
                dtDate.DateTime = Invoice.Date;
       dtDeliveryDate.EditValue = Invoice.DelivaryDate;
           dtPostDate.EditValue = Invoice.PostDate;
       memoShoppingAddress.Text = Invoice.ShippingAddress;
                 memoNotes.Text = Invoice.Notes;
       checkPostToStore.Checked = Invoice.PostedToStore;
     spnDiscountRation.EditValue=Invoice.DiscountRation;
     spnDiscountValue.EditValue = Invoice.DiscountValue;
         spnTaxRation.EditValue = Invoice.Tax;
          spnTaxValue.EditValue = Invoice.TaxValue;   
          spnExpences.EditValue = Invoice.Expences;   
               spnNet.EditValue = Invoice.Net; 
              spnPaid.EditValue = Invoice.Paid;   
             spnTotal.EditValue = Invoice.Total;
         spnRemaining.EditValue = Invoice.Remaining;
            GeneralDB = new dbDataContext();
            gridControl1.DataSource = GeneralDB.InvoiceDetails.Where(x => x.InvoiceID == Invoice.ID) ;
        }
        //Setting the data in save 
        void SetData()
        {
            Invoice.Branch = Convert.ToInt32(lkpBranch.EditValue);
            Invoice.Drawer = Convert.ToInt32(lkpDrawer.EditValue);
            Invoice.PartType = Convert.ToByte(lkpPartType.EditValue);
            Invoice.PartID = Convert.ToInt32(glkpPartID.EditValue);
            Invoice.Code = txtCode.Text;
            Invoice.Date = dtDate.DateTime;
            Invoice.DelivaryDate = dtDeliveryDate.EditValue as DateTime?;
            Invoice.PostDate = dtPostDate.EditValue as DateTime?;
            Invoice.ShippingAddress = memoShoppingAddress.Text;
            Invoice.Notes = memoNotes.Text;
            Invoice.PostedToStore = checkPostToStore.Checked;
            Invoice.DiscountRation = Convert.ToDouble(spnDiscountRation.EditValue);
            Invoice.DiscountValue = Convert.ToInt32(spnDiscountValue.EditValue);
            Invoice.Tax = Convert.ToDouble(spnTaxRation.EditValue);
            Invoice.TaxValue = Convert.ToDouble(spnTaxValue.EditValue);
            Invoice.Expences = Convert.ToDouble(spnExpences.EditValue);
            Invoice.Net = Convert.ToDouble(spnNet.EditValue);
            Invoice.Paid = Convert.ToDouble(spnPaid.EditValue);
            Invoice.Total = Convert.ToDouble(spnTotal.EditValue);
            Invoice.Remaining = Convert.ToDouble(spnRemaining.EditValue);
            Invoice.InvoiceType = (byte)Type;
            Invoice.UserID = Session.User.ID;
            Invoice.SourceID= Convert.ToInt32(glkpSourceID.EditValue);

        }
        //making new invoice
        //this is for Checking Actions permissions in frmMain(it's made to diffrentiate between Save-Edit Actions)it's Necessry to make it tre if u make new invoice
        void New()
        {
            Invoice = new InvoiceHeader
            //Setting some Defaults 
            {
                Drawer = Session.Defaults.Drawer,
                Date = Session.Defaults.Date,
                PostDate = Session.Defaults.Date,
                PostedToStore = true,
                Code=GetNewInvoiceCode(),
            };
            isnew = true;
            btnDelete.Enabled = false;
            switch (Type)
            {
                case Master.InvoiceType.purchase:
                
                    Invoice.PartType = (int)Master.Parttype.Vendor;
                    Invoice.PartID = Session.Defaults.Vendor;
                    Invoice.Branch = Session.Defaults.RawStore;
                    break;
                case Master.InvoiceType.sales:
               
                    Invoice.PartType = (int)Master.Parttype.Customer;
                    Invoice.PartID=Session.Defaults.Customer;
                    Invoice.Branch = Session.Defaults.Store;
                    break;
                case Master.InvoiceType.purchaseReturn:
                    Invoice.Branch = Session.Defaults.Store;
                    Invoice.PartID = Session.Defaults.Vendor;
                    break;
                case Master.InvoiceType.salesReturn:
                    Invoice.Branch = Session.Defaults.Store;
                    Invoice.PartID = Session.Defaults.Customer;
                    break;
                default:throw new NotImplementedException();
            }
            Getdata();
            MoveFocusToView();
        }
        private void FrmInvoice_Activated(object sender, EventArgs e)
        {
            MoveFocusToView();
        }
        void MoveFocusToView(bool focusetoitem=false)
        {
            if (Type == Master.InvoiceType.purchase || Type == Master.InvoiceType.sales)
            {
                if (gridView1.FocusedRowHandle < 0)
                {

                    gridView1.Focus();
                    gridView1.FocusedColumn = focusetoitem ? gridView1.Columns["ItemID"] : gridView1.Columns["Code"];

                }
                gridView1.AddNewRow();
                gridView1.Focus();
                gridView1.FocusedColumn = focusetoitem ? gridView1.Columns["ItemID"] : gridView1.Columns["Code"];
            }
        }
        //lkp DataSources  the extinsionMethod is in masterclass
        void Refreshdata()
        {
            lkpBranch.InitializeData(Session.Store);
            lkpDrawer.InitializeData(Session.Drawer);    
        }
      //setting data source depends on invoice type 
        private void LkpPartType_EditValueChanged(object sender, EventArgs e)
        {
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
            }
        }
        //Saving Data
        void save()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit))
            {
                if (IsdataValid())
                {
                    gridView1.UpdateCurrentRow();
                    if (gridView1.FocusedRowHandle < 0)
                    {
                        GridView1_ValidateRow(gridView1, new DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs(gridView1.FocusedRowHandle, gridView1.GetRow(gridView1.FocusedRowHandle)));
                    }
                    using (var db = new dbDataContext())
                    {


                        if (Invoice.ID == 0)
                        {
                            db.InvoiceHeaders.InsertOnSubmit(Invoice);
                        }
                        else
                        {
                            db.InvoiceHeaders.Attach(Invoice);
                        }
                        SetData();
                        var items = gridView1.DataSource as Collection<InvoiceDetail>;
                        //توزيع سعر التكلفه 
                        switch (Type)
                        {
                            case Master.InvoiceType.purchase:
                                if (Invoice.Expences > 0)
                                {

                                    var totlaPrice = items.Sum(x => x.TotalPrice);
                                    var totlaQty = items.Sum(x => x.ItemQty);
                                    var BypriceUnit = Invoice.Expences / totlaPrice; //السعر لكل جنيه واحد
                                    var ByQtyUnit = Invoice.Expences / totlaQty; //السعر لكل وحده 

                                    foreach (var item in items)
                                    {
                                        if (radioButton1.Checked) { item.CostValue = (item.TotalPrice / item.ItemQty) + (ByQtyUnit * item.ItemQty); } // توزيع بالكميه
                                        else if (radioButton2.Checked) { item.CostValue = (item.TotalPrice / item.ItemQty) + (BypriceUnit); } //توزيع بالسعر 


                                        item.TotalCostValue = item.CostValue * item.ItemQty;
                                    }

                                }
                                else
                                {

                                    foreach (var row in items)
                                    {
                                        row.CostValue = row.TotalPrice / row.ItemQty;
                                        row.TotalCostValue = row.TotalPrice;
                                    }
                                }

                                break;
                            case Master.InvoiceType.sales:

                                break;
                            case Master.InvoiceType.purchaseReturn:

                            case Master.InvoiceType.salesReturn:
                                //TODO
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        #region journals
                        //القيــــود
                        db.Journals.DeleteAllOnSubmit(db.Journals.Where(x => x.Sourcetype == (byte)Type && x.SourceID == Invoice.ID));
                        db.SubmitChanges();

                        var store = db.Stores.Single(x => x.ID == Invoice.Branch);
                        var drawer = db.Drawers.Single(x => x.ID == Invoice.Drawer);
                        string msg;
                        int StoreAccount;
                        int TaxAccount;
                        bool IsPurchaseTax;
                        bool IsDiscountPurchaseRecieved;
                        int PartAccountID;
                        bool IsVendor;
                        int DiscountAccount;
                        bool IsPartCredit;
                       
                        bool isIn;
                        PartAccountID = db.CustomersAndVendors.Single(x => x.ID == Invoice.PartID).AccountID;
                        switch (Type)
                        {
                            case Master.InvoiceType.purchase:
                                StoreAccount = store.InventoryAccountID;
                                TaxAccount = Session.Defaults.PurchaseTax;
                                IsPurchaseTax = true;
                                DiscountAccount = store.DiscountReceivedAccountID;
                                IsDiscountPurchaseRecieved = true;
                                IsPartCredit = true;
                                IsVendor = true;
                                isIn = true;
                               
                                msg = $" فاتوره شراء رقم {Invoice.ID}        لمورد {glkpPartID.Text}  ";
                                break;
                            case Master.InvoiceType.sales:
                                StoreAccount = store.SalesAccountID;
                                TaxAccount = Session.Defaults.SalesTax;
                                IsPurchaseTax = false;
                                DiscountAccount = store.DiscountAllowedAccountID;
                                IsDiscountPurchaseRecieved = false;
                                IsPartCredit = false;
                                IsVendor = false;
                                isIn = false;
                               
                                msg = $" فاتوره مبيعات رقم {Invoice.ID}        لعميل {glkpPartID.Text}  ";
                                break;
                            case Master.InvoiceType.purchaseReturn:
                                StoreAccount = store.InventoryAccountID;
                                TaxAccount = Session.Defaults.PurchaseTax;
                                IsPurchaseTax = true;
                                DiscountAccount = store.DiscountReceivedAccountID;
                                IsDiscountPurchaseRecieved = true;
                                IsPartCredit = false;
                                IsVendor = true;
                                isIn = false;
                               
                                msg = $" فاتوره مردود شراء رقم  {Invoice.ID}        لمورد {glkpPartID.Text}   ";
                                break;

                            case Master.InvoiceType.salesReturn:
                                StoreAccount = store.SalesReturnAccountID;
                                TaxAccount = Session.Defaults.SalesTax;
                                IsPurchaseTax = false;
                                DiscountAccount = store.DiscountAllowedAccountID;
                                IsDiscountPurchaseRecieved = false;
                                IsPartCredit = true;
                                IsVendor = false;
                                isIn = true;
                               
                                msg = $" فاتوره مردود مبيعات رقم {Invoice.ID}        لعميل {glkpPartID.Text}  ";
                                break;
                            default: throw new NotImplementedException();

                        }
                        db.Journals.InsertOnSubmit(new Journal() //part
                        {
                            AccountID = PartAccountID,
                            Code = 5555,
                            Credit = (IsPartCredit) ? Invoice.Total + Invoice.TaxValue + Invoice.Expences : 0,
                            Debit = (!IsPartCredit) ? Invoice.Total + Invoice.TaxValue + Invoice.Expences : 0,
                            OperationType = (IsVendor) ? (byte)Master.JournalsOperationType.VendorGeneral : (byte)Master.JournalsOperationType.CustomerGeneral,
                            InsertDate = Invoice.Date,
                            SourceID = Invoice.ID,
                            Sourcetype = Invoice.InvoiceType,
                            Notes = msg + "حساب شخص عام",
                        });
                        db.Journals.InsertOnSubmit(new Journal() //store Inventory
                        {
                            AccountID = StoreAccount,
                            Code = 5555,
                            Credit = (!IsPartCredit) ? Invoice.Total + Invoice.Expences : 0,
                            Debit = (IsPartCredit) ? Invoice.Total + Invoice.Expences : 0,
                            OperationType = (byte)Master.JournalsOperationType.StoreInventory,
                            InsertDate = Invoice.Date,
                            SourceID = Invoice.ID,
                            Sourcetype = Invoice.InvoiceType,
                            Notes = msg + " حساب مخزن عام",
                        });
                        if (Invoice.TaxValue > 0)
                        {
                            db.Journals.InsertOnSubmit(new Journal() //store tax
                            {
                                AccountID = TaxAccount,
                                Code = 5555,
                                Credit = (!IsPartCredit) ? Invoice.TaxValue : 0,
                                Debit = (IsPartCredit) ? Invoice.TaxValue : 0,
                                OperationType = (IsPurchaseTax) ? (byte)Master.JournalsOperationType.StoreTaxPurchase : (byte)Master.JournalsOperationType.StoreTaxSell,
                                InsertDate = Invoice.Date,
                                SourceID = Invoice.ID,
                                Sourcetype = Invoice.InvoiceType,
                                Notes = msg + " حساب ضريبه مخزن ",

                            });
                        }
                        if (Invoice.Expences > 0) //Expences
                        {
                            db.Journals.InsertOnSubmit(new Journal()
                            {
                                AccountID = Session.Defaults.PurchaseExpenses,
                                Code = 5555,
                                Credit = (!IsPartCredit) ? Invoice.Expences : 0,
                                Debit = (IsPartCredit) ? Invoice.Expences : 0,
                                OperationType = (byte)Master.JournalsOperationType.PurchaseExpences,
                                InsertDate = Invoice.Date,
                                SourceID = Invoice.ID,
                                Sourcetype = Invoice.InvoiceType,
                                Notes = msg + " حساب مصروفات مخزن ",
                            });
                        }
                        if (Invoice.DiscountValue > 0) //discount
                        {
                            db.Journals.InsertOnSubmit(new Journal()
                            {
                                AccountID = DiscountAccount,
                                Code = 5555,
                                Credit = (IsPartCredit) ? Invoice.DiscountValue : 0,
                                Debit = (!IsPartCredit) ? Invoice.DiscountValue : 0,
                                OperationType = (IsDiscountPurchaseRecieved) ? (byte)Master.JournalsOperationType.StoreDiscountPurchaseRecieved : (byte)Master.JournalsOperationType.StoreDiscountSellAllowed,
                                InsertDate = Invoice.Date,
                                SourceID = Invoice.ID,
                                Sourcetype = Invoice.InvoiceType,
                                Notes = msg + " حساب خصم مخزن ",
                            });
                            db.Journals.InsertOnSubmit(new Journal()
                            {
                                AccountID = PartAccountID,
                                Code = 5555,
                                Credit = (!IsPartCredit) ? Invoice.DiscountValue : 0,
                                Debit = (IsPartCredit) ? Invoice.DiscountValue : 0,
                                OperationType = (IsVendor) ? (byte)Master.JournalsOperationType.VendorDisocunt : (byte)Master.JournalsOperationType.CustomerDisocunt,
                                InsertDate = Invoice.Date,
                                SourceID = Invoice.ID,
                                Sourcetype = Invoice.InvoiceType,
                                Notes = msg + " حساب خصم شخص ",
                            });
                        }
                        if (Invoice.Paid > 0)
                        {
                            db.Journals.InsertOnSubmit(new Journal() //Drawer 
                            {
                                AccountID = drawer.AccountID,
                                Code = 5555,
                                Credit = (IsPartCredit) ? Invoice.Paid : 0,
                                Debit = (!IsPartCredit) ? Invoice.Paid : 0,
                                OperationType = (byte)Master.JournalsOperationType.Drawer,
                                InsertDate = Invoice.Date,
                                SourceID = Invoice.ID,
                                Sourcetype = Invoice.InvoiceType,
                                Notes = msg + "حساب سداد خزنه",
                            });
                            db.Journals.InsertOnSubmit(new Journal()
                            {
                                AccountID = PartAccountID,
                                Code = 5555,
                                Credit = (!IsPartCredit) ? Invoice.Paid : 0,
                                Debit = (IsPartCredit) ? Invoice.Paid : 0,
                                OperationType = (IsVendor) ? (byte)Master.JournalsOperationType.VendorPaid : (byte)Master.JournalsOperationType.CustomerPaid,
                                InsertDate = Invoice.Date,
                                SourceID = Invoice.ID,
                                Sourcetype = Invoice.InvoiceType,
                                Notes = msg + "حساب ســداد شخص",
                            });
                        }
                      
                        #endregion
                        foreach (var row in items)
                        {
                            row.InvoiceID = Invoice.ID;
                        }
                        db.StoreLogs.DeleteAllOnSubmit(db.StoreLogs.Where(x => x.SourceType == (byte)Type &&
                        db.InvoiceDetails.Where(s => s.InvoiceID == Invoice.ID).Select(q => q.ID).Contains(x.SourceID)));
                        GeneralDB.SubmitChanges();
                        db.SubmitChanges();
                        if (Invoice.PostedToStore)
                        {
                            foreach (var row in items)
                            {
                                var unitView = Session.ProductsView.Single(x => x.ID == row.ItemID).Units.Single(x => x.UnitID == row.ItemUnitID);
                                db.StoreLogs.InsertOnSubmit(new StoreLog()
                                {
                                    ProductID = row.ItemID,
                                    InsertTime = Invoice.PostDate.Value,
                                    SourceID = row.ID,
                                    SourceType = (byte)Type,
                                    Notes = msg,
                                    IsInTransaction = isIn,
                                    StoreID = Invoice.Branch,
                                    Qty = row.ItemQty * unitView.Factor,
                                    CostValue = row.CostValue / unitView.Factor,
                                });
                            }
                        }
                        db.SubmitChanges();
                        InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, Invoice.ID, Invoice.Code + "فاتوره");
                        isnew = false;
                        btnDelete.Enabled = true;

                        MessageBox.Show("تم الحفظ بنجاااااح");
                        //to refreshdata in invoice list while it's open or u can use refreshdata button or using class databasechanges to update data 
                        //Note<this mithod is for local system Only>
                        var forms = Application.OpenForms.Cast<Form>().Where(x => x.Name == nameof(frmInvoiceList));
                        foreach (var form in forms)
                        {
                            if (form != null && form is frmInvoiceList) ((frmInvoiceList)form).RefreshData();
                        }
                        // Or
                        /*
                         * var form = (frmInvoiceList)Application.OpenForms[nameof(frmInvoiceList)];
                        if (form != null) form.RefreshData();
                        */
                        New();
                    }
                }
            }
        }
        //if no expences added u cannot determine CostValue
        private void spnExpences_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(spnExpences.EditValue) == 0)
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton1.Checked = false;
                radioButton2.Checked = false;
            }
            else
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton1.Checked = true;
            }
        }
        //printing
         void Print()
        {
            
                Print(Invoice.ID); 
        }
        //printing 1 single invoice
        void Print(int id)
        {
                Print(new List<int> {id},this.Name);
        }
        //overload printing multible invoice
       public static void Print(List<int> ids, string screenname)
        {
            if (frmMain.CheckActionPermission(screenname, Master.Actions.Print))
            {
                using (var db = new dbDataContext())
                {
                    var invoice = (from inv in db.InvoiceHeaders
                                   join str in db.Stores on inv.Branch equals str.ID

                                   //join prt in db.CustomersAndVendors on inv.PartID equals prt.ID 
                                   from prt in db.CustomersAndVendors.Where(x => x.ID == inv.PartID).DefaultIfEmpty()
                                   from drw in db.Drawers.Where(x => x.ID == inv.Drawer)
                                   where ids.Contains(inv.ID)
                                   select new
                                   {
                                       inv.ID,
                                       inv.Code,
                                       store = str.Name,
                                       drawer = drw.Name,
                                       productCount = db.InvoiceDetails.Where(x => x.InvoiceID == inv.ID).Sum(x => (double?)x.ItemQty) ?? 0,
                                       inv.Tax,
                                       inv.DiscountRation,
                                       inv.Date,
                                       inv.Remaining,
                                       inv.Net,
                                       inv.Paid,
                                       inv.Total,
                                       Customer = prt.Name,
                                       InvoiceType = (inv.InvoiceType == (byte)Master.InvoiceType.sales) ? "فاتوره بيع" :
                                                     (inv.InvoiceType == (byte)Master.InvoiceType.salesReturn) ? "فاتوره مردود بيع" :
                                                     (inv.InvoiceType == (byte)Master.InvoiceType.purchase) ? "فاتوره شراء" :
                                                     (inv.InvoiceType == (byte)Master.InvoiceType.purchaseReturn) ? "فاتوره مردود شراء" : "undefined",
                                       inv.Expences,
                                       Products = (from pr in db.InvoiceDetails.Where(x => x.InvoiceID == inv.ID)
                                                   from p in db.Products.Where(x => x.ID == pr.ItemID)
                                                   select new
                                                   {
                                                       productname = p.Name,
                                                       ItemQty = (double?)pr.ItemQty ?? 0,
                                                       Price = (double?)pr.Price ?? 0,
                                                       TotalPrice = (double?)pr.TotalPrice ?? 0,
                                                   }).ToList()
                                   }).ToList();
                    Reports.rptInvoice.print(invoice);
                    invoice.ForEach(x =>
                    {
                        InsertUserLog(Master.Actions.Print, x.ID, x.Code + "فاتوره", screenname);
                    });
                }

            }
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //checking action permission for users
            //The value is new == true>> it's a new invoice (save),false>>old invoice ()
                save();
        }

        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            New();
        }

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
                Print();
        }
        void delete()
        {
            if (frmMain.CheckActionPermission(this.Name, Master.Actions.Delete))
            {
                if (Master.DeleteConfirmateionMsg())
                {
                    using (var db = new dbDataContext())
                    {
                        db.StoreLogs.DeleteAllOnSubmit(
                            db.StoreLogs.Where(x => x.SourceType == (byte)Type
                            && db.InvoiceDetails.Where(y => y.InvoiceID == Invoice.ID).Select(y => y.ID).Contains(x.SourceID)));
                        db.SubmitChanges();
                        db.InvoiceDetails.DeleteAllOnSubmit(db.InvoiceDetails.Where(x => x.InvoiceID == Invoice.ID));
                        db.SubmitChanges();
                        db.InvoiceHeaders.Attach(Invoice);
                        db.InvoiceHeaders.DeleteOnSubmit(Invoice);
                        db.SubmitChanges();
                        btnNew.PerformClick();
                        MessageBox.Show("تم الحذف");
                    }
                }
            }
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            delete();
        }
        void ReaduserSettings()
        {
            switch (Type)
            {
                case Master.InvoiceType.purchase:
                    break;
                case Master.InvoiceType.sales:
                    spnPaid.Enabled = Session.Settings.Sales.CanChangePaidinSales;
                    checkPostToStore.Enabled= Session.Settings.Sales.CanNotPostToStoreInSales;
                    gridView1.Columns[nameof(insDetails.Price)].OptionsColumn.AllowEdit = Session.Settings.Sales.CanChangeItemPriceInSales;
                    gridView1.Columns[nameof(insDetails.ItemQty)].OptionsColumn.AllowEdit = Session.Settings.Sales.CanChangeQuantityInSales;
                    gridView1.Columns[nameof(insDetails.CostValue)].Visible =( Session.Settings.Sales.HideCostSales)?false:true;
                    gridView1.Columns[nameof(insDetails.TotalCostValue)].Visible = (Session.Settings.Sales.HideCostSales) ? false : true;
                    gridView1.Columns[nameof(insDetails.CostValue)].OptionsColumn.ShowInCustomizationForm = (Session.Settings.Sales.HideCostSales) ? false : true;
                    lkpPartType.Enabled= Session.Settings.Sales.sellToVendors;
                    dtDate.Enabled= Session.Settings.Sales.CanChangeSalesInvoiceDate;
                    break;
                case Master.InvoiceType.purchaseReturn:
                    break;
                case Master.InvoiceType.salesReturn:
                    break;
                default: throw new NotFiniteNumberException();
            }

        }

        private void checkPostToStore_CheckedChanged(object sender, EventArgs e)
        {
            if (checkPostToStore.Checked == false)
            {
                dtPostDate.EditValue = null;
                dtPostDate.Enabled = false;
            }
            else
            {
                dtPostDate.EditValue = Session.Defaults.Date;
                dtPostDate.Enabled = true;
            }
        }
    }
    
}
