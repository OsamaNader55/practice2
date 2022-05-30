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
    public partial class frmDrawerActivity : frmMaster
    {
        public frmDrawerActivity()
        {
            InitializeComponent();
            LoadData();
        }
        void LoadData()
        {
            lkpDrawer.InitializeData(Session.Drawer);
        }

        private void frmDrawerActivity_Load(object sender, EventArgs e)
        {
            dtFrom.DateTime = dtTo.DateTime = DateTime.Now;
        }
        Int16 x=0 ;
        void Refreshdata()
        {
            if (lkpDrawer.EditValue == null) return;
            using (var db = new dbDataContext())
            {
                var drw = Session.Drawer.SingleOrDefault(x => x.ID == (int)lkpDrawer.EditValue);
                var q = from inv in Master.InvoiceTypeList join 
                         journal in db.Journals.Where(x => x.AccountID == drw.AccountID) on inv.ID equals journal.Sourcetype
                       
                        select new
                        {
                            journal.Code,
                            inv.Name,
                            journal.Credit,
                            journal.Debit,
                            journal.InsertDate,
                            journal.Notes,
                        };
                if (dtFrom.EditValue != null)
                {
                    q = q.Where(x => x.InsertDate.Date >= dtFrom.DateTime.Date);
                }
                if (dtTo.EditValue != null)
                {
                    q = q.Where(x => x.InsertDate.Date <= dtFrom.DateTime.Date);
                }
                gridControl1.DataSource = q.ToList();
                gridControl1.DataSource = q;
                if (x == 0)
                {
                    gridView1.PopulateColumns();
                    gridView1.Columns["Code"].Caption = "كود العمليه";
                    gridView1.Columns["Name"].Caption = "نوع الفااتوره";
                    gridView1.Columns["Credit"].Caption = "خرج";
                    gridView1.Columns["Debit"].Caption = "دخل";
                    gridView1.Columns["InsertDate"].Caption = "التاريخ";
                    gridView1.Columns["Notes"].Caption = "ملاحظات";
                    x++;

                }
            }
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Refreshdata();
        }
    }
}
