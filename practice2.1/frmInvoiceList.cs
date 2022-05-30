using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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
    public partial class frmInvoiceList : frmMaster

    {
        Master.InvoiceType Type;
        public frmInvoiceList(Master.InvoiceType _type)
        {
            InitializeComponent();
            DateFrom.EditValue =
                DateTo.EditValue =DateTime.Today;
            Type = _type;
            gridView1.DoubleClick += GridView1_DoubleClick;
        }
        private void GridView1_DoubleClick(object sender, EventArgs e)
        {

            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                int id = Convert.ToInt32(view.GetFocusedRowCellValue("ID"));
                frmInvoice frm = new frmInvoice(Type,id);
                frmMain.OpenFormWithPermissions(frm, true);
                RefreshData();
            }
        }
        private void frmInvoiceList_Load(object sender, EventArgs e)
        {
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsView.ShowAutoFilterRow = true;
            gridView1.OptionsDetail.ShowDetailTabs = false;
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsPrint.AllowMultilineHeaders = true;
            gridView1.OptionsPrint.AutoWidth = true;
            gridView1.AppearancePrint.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
           
            gridView1.OptionsPrint.UsePrintStyles = true;
            gridView1.AppearancePrint.HeaderPanel.Font = new Font(gridView1.AppearancePrint.HeaderPanel.Font, FontStyle.Bold);
            gridControl1.ViewRegistered += GridControl1_ViewRegistered;
             gridView1.PopupMenuShowing += GridView1_PopupMenuShowing;
           
           

            switch (Type)
            {
                case Master.InvoiceType.purchase:
                    this.Text = "فواتير مشتريات ";
                    this.Name = Screens.InvoicePurchaseList.ScreenName;
                    break;
                case Master.InvoiceType.sales:
                    this.Text = "فواتير مبيعات ";
                    this.Name = Screens.InvoiceSalesList.ScreenName;
                    break;
                case Master.InvoiceType.purchaseReturn:
                    this.Text = "فواتير مردود مشتريات ";
                    break;
                case Master.InvoiceType.salesReturn:
                    this.Text = "فواتير مردود مشتريات ";
                    break;
            }
            
            RefreshData();
            gridView1.PopulateColumns();
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["Date"].DisplayFormat.FormatType = FormatType.Custom;
                gridView1.Columns["Date"].DisplayFormat.FormatString = "dd-MM-yyyy hh:mm tt";
            gridView1.Columns["Date"].Caption = "التاريخ";
            gridView1.Columns["Code"].Caption = "الكود";
            gridView1.Columns["PostedToStore"].Caption = "مرحل الي مخزن";
            gridView1.Columns["PartName"].Caption = "طرف التعامل";
            gridView1.Columns["StoreName"].Caption = "المخزن";
            gridView1.Columns["DrawerName"].Caption = "الخزينه";
            gridView1.Columns["productCount"].Caption = "الكميه";
            gridView1.Columns["Total"].Caption = "الكل";
            gridView1.Columns["TaxValue"].Caption = "قيمه الضريبه";
            gridView1.Columns["Tax"].Caption = "الضريبه";
            gridView1.Columns["DiscountRation"].Caption = "قيمه الخصم";
            gridView1.Columns["DiscountValue"].Caption = "الخصم";
            gridView1.Columns["Paid"].Caption = "المدفوع";
            gridView1.Columns["Net"].Caption = "الصافي";
            gridView1.Columns["Expences"].Caption = "مصاريف اخري";
            gridView1.Columns["Remaining"].Caption = "المتبقي";
            gridView1.Columns["ShippingAddress"].Caption = "عنوان الشحن";
            gridView1.Columns["PayStatus"].Caption = "حاله الدفع";
            gridView1.Columns["Date"].MinWidth = 130;
            gridView1.Columns["PayStatus"].MinWidth = 60;
            gridView1.Columns["Code"].MinWidth = 30;
            gridView1.Columns["PartName"].MinWidth = 80;
            gridView1.Columns["StoreName"].MinWidth = 80;
            gridView1.Columns["DrawerName"].MinWidth = 80;

           
        }

        private void GridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow || e.HitInfo.InRowCell)
            {
                var buttonPrint = new DevExpress.Utils.Menu.DXMenuItem() { Caption = "طباعه" };
                buttonPrint.ImageOptions.Image = Sales.Properties.Resources.printer_32x32;
                buttonPrint.Click += ButtonPrint_Click;
                e.Menu.Items.Add(buttonPrint);
            }
        }

        private void ButtonPrint_Click(object sender, EventArgs e)
        {
          var rows=  gridView1.GetSelectedRows();
            List<int> ids = new List<int>();
            foreach (var handle in rows)
            {
                ids.Add(Convert.ToInt32(gridView1.GetRowCellValue(handle, "ID")));
            }
            if (ids.Count == 0)
            {
                MessageBox.Show("يجب اختيار عنصر واحد علي الاقل");
                return;
            }
            if (frmMain.CheckActionPermission(this.Name, Master.Actions.Print)) 
            frmInvoice.Print(ids,this.Name);
        }

        private void GridControl1_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
            var view = e.View as GridView;
            if (view != null&&view.LevelName== "Products")
            {
                view.OptionsView.ShowViewCaption= true;
                view.ViewCaption = "الاصناف";
                view.Columns["ProductName"].Caption = "الاســم";
                view.Columns["unitName"].Caption = "الوحده";
                view.Columns["itemprice"].Caption = "السعر";
                view.Columns["itemqty"].Caption = "الكميه";
                view.Columns["itemdiscount"].Caption = "الخصم";
                view.Columns["itemtotalprice"].Caption = "السعر الكلي";
            }
        }
      public  void RefreshData()
        {
           
            using (var db = new dbDataContext())
            {
                var qu = db.InvoiceHeaders.Where(x => x.InvoiceType == (byte)Type);
                if (qu.Count() <= 0)
                {
                    MessageBox.Show("NoData"); return;
                }
                var squ = from inv in qu.DefaultIfEmpty()
                                 from prt in db.CustomersAndVendors.Where(x => x.ID == (int?)inv.PartID).DefaultIfEmpty()
                                     from str in db.Stores.Where(x => x.ID == (int?)inv.Branch).DefaultIfEmpty()
                                     from drw in db.Drawers.Where(x => x.ID == (int?)inv.Drawer).DefaultIfEmpty()
                          select new
                          {
                             ID=(int?) inv.ID,
                              inv.Code,
                           Date=   inv.Date,
                              inv.PostedToStore,
                             PartName = prt.Name,
                              StoreName = str.Name,
                             DrawerName = drw.Name,
                              productCount = db.InvoiceDetails.Where(x => x.InvoiceID == (int?)inv.ID).Sum(x => (double?)x.ItemQty)??0,
                              inv.Total,
                              inv.TaxValue,
                              inv.Tax,
                              inv.DiscountRation,
                              inv.DiscountValue,
                              inv.Paid,
                              inv.Net,
                              inv.Expences,
                              inv.Remaining,
                              inv.ShippingAddress,
                              PayStatus =(inv.Remaining==0)? "تم السداد": (inv.Remaining == inv.Net) ? "غير مسدد" :"سداد جزئي",
                              Products = (from itm in db.InvoiceDetails.Where(x => x.InvoiceID == (int?)inv.ID).DefaultIfEmpty()
                                              from pr in db.Products.Where(x => x.ID == (int?)itm.ItemID).DefaultIfEmpty()
                                                 from unt in db.UnitNames.Where(x => x.ID == (int?)itm.ItemUnitID).DefaultIfEmpty()
                                          select new
                                          {
                                               ProductName = pr.Name,
                                                   unitName = unt.Name,
                                              itemprice = (double?)itm.Price,
                                              itemqty = (double?)itm.ItemQty,
                                              itemdiscount = (double?)itm.DiscountValue,
                                              itemtotalprice = (double?)itm.TotalPrice,
                                          }).ToList()
                          };
                if (DateFrom.EditValue != null)
                {
                    //x.Date.Date must be of date only to compare dates not time if time the result is always nothing 
                    squ = squ.Where(x => x.Date.Date >= DateFrom.DateTime);
                }
                if (DateTo.EditValue != null)
                {

                    squ = squ.Where(x => x.Date.Date <= DateTo.DateTime);
                }
                
                gridControl1.DataSource = squ;
               
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DateFrom.EditValue =
                DateTo.EditValue = null;
           
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmInvoice frm = new frmInvoice(Type);
            frmMain.OpenFormWithPermissions(frm);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

           
          
            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
            gridView1.Columns["ShippingAddress"].Visible = false;
            Report.rptGrid.Print(gridControl1,
                $"كشف فواتير {((Type == Master.InvoiceType.purchase) ? "مشتريات" : "مبيعات")}",
                $"من {DateFrom.Text} الي {DateTo.Text}",
                this.Name,
                ((Type == Master.InvoiceType.purchase) ? "تقرير مشتريات" : "تقرير مبيعات"));
            

        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshData();
        }

        private void frmInvoiceList_Activated(object sender, EventArgs e)
        {
            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
