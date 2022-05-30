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
    
    public partial class frmProductCategory : frmMaster
    {
        ProductCategory category;
        public frmProductCategory()
        {
            InitializeComponent();
            New();
        }

        private void frmProductCategory_Load(object sender, EventArgs e)
        {
            Refreshdata();
            comboBox1.DisplayMember = nameof(category.Name);
            comboBox1.ValueMember = nameof(category.ID);
            treeList1.ParentFieldName = nameof(category.ParentID);
            treeList1.KeyFieldName = nameof(category.ID);
            treeList1.OptionsBehavior.Editable = false;
            treeList1.Columns[nameof(category.Number)].Visible = false;
            treeList1.Columns[nameof(category.Name)].Caption = "الاسم";
            treeList1.FocusedNodeChanged += TreeList1_FocusedNodeChanged;

        }

        private void TreeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            int id = 0;
            if (int.TryParse(e.Node.GetValue(nameof(category.ID)).ToString(), out id))
            {
                using(var db = new dbDataContext())
                {
                    category=db.ProductCategories.SingleOrDefault(x=> x.ID == id);
                    Getdata();
                }
              

            }
        }

        void New()
        {
            category = new ProductCategory();
            isnew = true;
            Getdata();
        }
      
         void Getdata()
        {
            txtName.Text = category.Name;
            comboBox1.SelectedValue = category.ParentID;
        }
        void Setdata()
        {
            category.Name = txtName.Text;
            category.ParentID = (comboBox1.SelectedValue as int?)??0;
            category.Number = "0";
        }
        void Refreshdata()
        {
           using(var db = new dbDataContext())
            {
                var groups = db.ProductCategories;
                comboBox1.DataSource = groups;
                treeList1.DataSource = db.ProductCategories;
            }

        }
        bool validData()
        {
            if (txtName.Text.Trim() == String.Empty)
            {
                MessageBox.Show("هذا الحــقل مطــلوب");
                return false;
            }
            //لازم تكون الداتا بيز بتقرا العربي لو اليوزر هيكتب عربي 
            //Database collation must be "Arabic CI AI" for arabic words !
            var db = new dbDataContext();
            if (db.ProductCategories.Where(x => x.Name.Trim() == txtName.Text.Trim() 
              &&
             x.ID != category.ID).Count() > 0)
            {
                MessageBox.Show("هذا الاســم مــوجود مــسبقا");
                return false;
            }
            return true;
        }
         void save()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit))
            {
                if (validData() == false)
                    return;
                var db = new dbDataContext();

                if (category.ID == 0)
                {
                    db.ProductCategories.InsertOnSubmit(category);
                }
                else
                {
                    db.ProductCategories.Attach(category);
                }
                Setdata();
                db.SubmitChanges();
                MessageBox.Show("تم الحفظ بنجاااااح");
                InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, category.ID, category.Name + "فئه", this.Name);
                isnew = false;
                Refreshdata();
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
    }
}
