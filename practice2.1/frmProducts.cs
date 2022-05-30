using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using Sales;
namespace practice2._1
{
    public partial class frmProducts : frmMaster
    {
       
        Product product;
        RepositoryItemLookUpEdit lkp = new RepositoryItemLookUpEdit();
        dbDataContext sdb = new dbDataContext();
        public frmProducts()
        {
            InitializeComponent();
            Refreshdata();
          
            New();
           
        }
        public frmProducts(int id)
        {
            InitializeComponent();
            Refreshdata();
            LoadProduct( id);
        }
        void LoadProduct(int id)
        {
            using (var db = new dbDataContext())
            {
                product=db.Products.Single(x=> x.ID==id);   
            } 
            Getdata();
        }
         void New()
        {
            product = new Product() { IsActive = true };
            isnew = true;
            comboCate.SelectedIndex = -1;
            Getdata();
            var data = gridView1.DataSource as BindingList<ProductUnit>;
            var db = new dbDataContext();
            if (db.UnitNames.Count() == 0)
            {
                db.UnitNames.InsertOnSubmit(new UnitName() { Name = "قطعه" });
                db.SubmitChanges();
                Refreshdata();
            }
            data.Add(new ProductUnit() { Factor = 1, UnitID = db.UnitNames.First().ID });
            gridView1.AddNewRow();
        }
         void Getdata()
        {
            txtCode.Text = product.Code;
            txtName.Text = product.Name;
            txtDesc.Text = product.Description;
            comboCate.SelectedValue = product.CategoryID;
            comboType.SelectedValue = product.ProductTypeID;
            checkBox1.Checked = product.IsActive;
            if (product.Image != null)
             pictureBox1.Image = Master.GetimageFromByteArray(product.Image.ToArray());

            gridControl1.DataSource = sdb.ProductUnits.Where(x => x.ProductID == product.ID);   
        }
         void Refreshdata()
        {
            var db = new dbDataContext();
            comboCate.DataSource = db.ProductCategories.Where(x => db.ProductCategories.Where(w => w.ParentID == x.ID).Count() == 0);
            comboCate.DisplayMember = "Name";
            comboCate.ValueMember = "ID";
    
            comboType.DataSource = db.ProductTypes;
            comboType.DisplayMember = "Name";
            comboType.ValueMember = "ID";
            lkp.DataSource = db.UnitNames.ToList();
           
           
        }
        ProductUnit  ins = new ProductUnit();
        private void frmProducts_Load(object sender, EventArgs e)
        {
            txtCode.Text = "AutoMatic";
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            
            gridView1.Columns[nameof(ins.ID)].Visible= false;
            gridView1.Columns[nameof(ins.ProductID)].Visible = false;
            RepositoryItemCalcEdit calc =new RepositoryItemCalcEdit();
            gridControl1.RepositoryItems.Add(calc);
            gridControl1.RepositoryItems.Add(lkp);
            gridView1.Columns[nameof(ins.SellPrice)].ColumnEdit = calc;
            gridView1.Columns[nameof(ins.SellDisscount)].ColumnEdit = calc;
            gridView1.Columns[nameof(ins.BuyPrice)].ColumnEdit = calc;
            gridView1.Columns[nameof(ins.Factor)].ColumnEdit = calc;
            gridView1.Columns[nameof(ins.UnitID)].ColumnEdit = lkp;
            gridView1.Columns[nameof(ins.UnitID)].Caption = "اســم الوحــده";
            gridView1.Columns[nameof(ins.SellDisscount)].Caption = "خصم البيع";
            gridView1.Columns[nameof(ins.Barcode)].Caption = "الباركود";
            gridView1.Columns[nameof(ins.Factor)].Caption = "معامل التحويل";
            gridView1.Columns[nameof(ins.BuyPrice)].Caption = "سعر الشراء";
            gridView1.Columns[nameof(ins.SellPrice)].Caption = "سعر البيع";
            lkp.ValueMember = "ID";
            lkp.DisplayMember = "Name";
            
            lkp.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            lkp.ProcessNewValue += Lkp_ProcessNewValue;
            gridView1.ValidateRow += GridView1_ValidateRow;
            gridView1.InvalidRowException += GridView1_InvalidRowException;
            gridView1.FocusedRowChanged += GridView1_FocusedRowChanged;
            gridView1.CustomRowCellEditForEditing += GridView1_CustomRowCellEditForEditing;    
              

        }

        private void GridView1_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == nameof(ins.UnitID))
            {
                var ids = ((Collection<ProductUnit>)gridView1.DataSource).Select(s=> s.UnitID).ToList();
                RepositoryItemLookUpEdit repo = new RepositoryItemLookUpEdit();
                using(var db = new dbDataContext())
                {
                    var currentID = (Int32?)e.CellValue;
                    ids.Remove(currentID??0);
                    repo.DataSource=db.UnitNames.Where(s=> ids.Contains(s.ID)==false).ToList();  
                    repo.ValueMember = "ID";
                    repo.DisplayMember = "Name";
                    repo.PopulateColumns();
                    repo.Columns["ID"].Visible = false;
                    
                    repo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                    repo.ProcessNewValue += Lkp_ProcessNewValue;
                    e.RepositoryItem = repo;
                }
            }
        }

        private void GridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            
            if (e.FocusedRowHandle == 0)
            {
                gridView1.Columns[nameof(ins.Factor)].OptionsColumn.AllowEdit = false;  
            }
            else
            {
                gridView1.Columns[nameof(ins.Factor)].OptionsColumn.AllowEdit = true;
            }
        }

        private void GridView1_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.Ignore;
        }

        private void GridView1_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            var row = e.Row as ProductUnit;
            if (row == null)
            {
                return;
            }
            if (row.Factor <= 0 && e.RowHandle != 0)
            {
                e.Valid = false;
                gridView1.SetColumnError(gridView1.Columns[nameof(row.Factor)], "يجب ان تكون القيمه اكبر من 0");
            }
            if (row.UnitID <= 0)
            {
                e.Valid = false;
                gridView1.SetColumnError(gridView1.Columns[nameof(row.UnitID)], "هذا الحقل مطلووب");
            }
        }

        private void Lkp_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
        {
            if(e.DisplayValue is string value && value.Trim() != string.Empty)
            {
                var newObj=new UnitName() { Name=value.Trim()};
                using(dbDataContext db =new dbDataContext())
                {
                    db.UnitNames.InsertOnSubmit(newObj);
                    db.SubmitChanges();
                   
                }
                ((List<UnitName>) lkp.DataSource).Add(newObj);
                ((List<UnitName>)(((LookUpEdit)sender).Properties.DataSource)).Add(newObj);
                e.Handled = true;
            }
        }

        void setdata()
        {
           
            product.Name = txtName.Text;
            product.CategoryID = (int)comboCate.SelectedValue;
            product.ProductTypeID = Convert.ToInt32(comboType.SelectedValue);
            product.Description = txtDesc.Text;
            product.IsActive = checkBox1.Checked;
            product.Image =Master. GetbyteFromImage(pictureBox1.Image);
        }
        bool validateData()
        {
            if (comboCate.SelectedValue == null)
            {
                MessageBox.Show("عفوا , تأكــد مــن حقل الفئــات");
               
               using(var db1 =new dbDataContext())
                {
                    if (db1.ProductCategories.Count() == 0)
                    {
                        var dlg = MessageBox.Show(" يرجــي تســجيل فئات للمنـتجات,هــل تريد تســـجيل فئـات جديده؟", "فئـــات المنتجات فارغه", MessageBoxButtons.YesNo); 
                        if (dlg == DialogResult.Yes)
                        {
                            frmProductCategory frm = new frmProductCategory();
                            frmMain.OpenFormWithPermissions(frm, true);
                            Refreshdata();
                        }
                    }
                }
                 return true;
            }
            if (comboType.SelectedValue == null)
            {
                MessageBox.Show("عفوا , تأكــد مــن حقل النــوع");
                
                return true;
            }
            if (txtName.Text.Trim() == String.Empty)
            {
                MessageBox.Show("عفوا , تأكــد مــن حقل الاســم");
                return true;
            }
            var db = new dbDataContext();   
           
            if (db.Products.Where(x => x.ID != product.ID && x.Name.Trim() == txtName.Text.Trim()).Count() > 0)
            {
                MessageBox.Show("عفوا , هذا الاســـم موجــود مسـبقا");
                return true;
            }
            if (db.Products.Where(x => x.ID != product.ID && x.Code.Trim() == txtCode.Text.Trim()).Count() > 0)
            {
                MessageBox.Show("عفوا , هذا الكـــود موجــود مسـبقا");
                return true;
            }
            return false;
        }
       void save()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit))
            {
                if (validateData())
                {
                    return;
                }
                var db = new dbDataContext();

                if (product.ID == 0)
                {
                    db.Products.InsertOnSubmit(product);

                }
                else
                {
                    db.Products.Attach(product);
                }
                setdata();
                db.SubmitChanges();
                int code = 10000 + product.ID;
                product.Code = code.ToString();
                db.SubmitChanges();

                var data = gridView1.DataSource as BindingList<ProductUnit>;
                foreach (var item in data)
                {
                    item.ProductID = product.ID;
                    if (string.IsNullOrEmpty(item.Barcode))
                    {
                        item.Barcode = "test";
                    }
                }

                sdb.SubmitChanges();
                MessageBox.Show("تم الحفظ بنجاااااح");
                InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, product.ID, product.Name, this.Name);
                isnew = false;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(dlg.FileName);
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            New();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
                save();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            
        }
    }
}
