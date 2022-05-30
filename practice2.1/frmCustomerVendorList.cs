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
    public partial class frmCustomerVendorList : frmMaster
    {
        bool IsCustomer;
        public frmCustomerVendorList(bool iscustomer)
        {
            InitializeComponent();
            this.IsCustomer = iscustomer;
            this.Name = (IsCustomer) ? Screens.CustomerList.ScreenName : Screens.VendorList.ScreenName;
            this.Text = this.IsCustomer ? "قـائــمه العمــلاء" : "قـائــمه المــوردين";
        }
        void Refreshdata()
        {
            
                gridControl1.DataSource = (IsCustomer) ? Session.Customer.ToList() : Session.Vendor.ToList();    
        }

        private void frmCustomerVendorList_Load(object sender, EventArgs e)
        {
            Refreshdata();
            gridView1.OptionsBehavior.Editable = false; 
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["IsCustomer"].Visible = false;
            gridView1.Columns["Name"].Caption = "الاســـم";
            gridView1.Columns["Address"].Caption = "العنوان";
            gridView1.Columns["Mobile"].Caption = "المـوبايل";
            gridView1.Columns["Phone"].Caption = "التـلـيفون";
            gridView1.Columns["AccountID"].Visible = false;
            gridView1.DoubleClick += GridView1_DoubleClick;
            Session.Vendor.ListChanged += Vendor_ListChanged;
            Session.Customer.ListChanged += Customer_ListChanged;
        }

        private void Customer_ListChanged(object sender, ListChangedEventArgs e)
        {
            Refreshdata();
        }
        private void Vendor_ListChanged(object sender, ListChangedEventArgs e)
        {
            Refreshdata();
        }
        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                int id = Convert.ToInt32(view.GetFocusedRowCellValue("ID"));
                frmCustomerVendor frm = new frmCustomerVendor(IsCustomer, id);
                frmMain.OpenFormWithPermissions(frm, true);
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            frmCustomerVendor frm = new frmCustomerVendor(IsCustomer);
            frmMain.OpenFormWithPermissions(frm, true);
        }
    }
}
