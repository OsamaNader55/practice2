using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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
    public partial class frmProductList : frmMaster
    {
        Session.ProductViewClass ins = new Session.ProductViewClass();

        public frmProductList()
        {
            InitializeComponent();
        }
         void Refreshdata()
        { 
               
            gridControl1.DataSource = Session.ProductsView;  
        }

        private void frmProductList_Load(object sender, EventArgs e)
        {
           Refreshdata();   
            gridView1.OptionsBehavior.Editable = false;
            
            gridView1.DoubleClick += GridView1_DoubleClick;
            gridControl1.ViewRegistered += GridControl1_ViewRegistered;
            gridView1.OptionsDetail.ShowDetailTabs = false;
            gridView1.Columns[nameof(ins.Description)].Caption = "الوصــف";
            gridView1.Columns[nameof(ins.CategoryName)].Caption = "الفئـــه";
            gridView1.Columns[nameof(ins.Code)].Caption = "الكــود";
            gridView1.Columns[nameof(ins.Name)].Caption = "الاســـم";
            gridView1.Columns[nameof(ins.Type)].Caption = "النـــوع";
            gridView1.Columns[nameof(ins.IsActive)].Caption = "نــشـط";
            gridView1.Columns[nameof(ins.ID)].Visible = false;
        }
        private void GridControl1_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
            var insUOM = new Session.ProductViewClass.ProductUOMview();
            if (e.View.LevelName ==nameof(ins.Units) )
            {
                var view = e.View as GridView;
                view.OptionsView.ShowViewCaption = true;
                view.ViewCaption = "وحــدات المنتــج";
                view.Columns[nameof(insUOM.UnitName)].Caption = "اســم الوحــده";
                view.Columns[nameof(insUOM.Factor)].Caption = "مـعامل الوحـده";
                view.Columns[nameof(insUOM.SellPrice)].Caption = "ســعر البـيع";
                view.Columns[nameof(insUOM.SellDisscount)].Caption = "خــصـم البـيع";
                view.Columns[nameof(insUOM.BuyPrice)].Caption = "ســعر الشــراء";
                view.Columns[nameof(insUOM.Barcode)].Caption = "البـاركــود";
                view.Columns[nameof(insUOM.UnitID)].Visible = false;
            }
        }
        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            int id =Convert.ToInt32( gridView1.GetFocusedRowCellValue("ID"));
            if (id > 0)
            {
                frmProducts frm = new frmProducts(id);
                frmMain.OpenFormWithPermissions(frm, true);

            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            frmProducts frm = new frmProducts();
            frmMain.OpenFormWithPermissions(frm, true);
        }
    }
}
