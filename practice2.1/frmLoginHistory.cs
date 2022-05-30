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
    public partial class frmLoginHistory : frmMaster
    {
        public frmLoginHistory()
        {
            InitializeComponent();
            dtFrom.DateTime = dtTo.DateTime = DateTime.Now;
        }

        private void frmLoginHistory_Load(object sender, EventArgs e)
        {
           

            RefreshData();
            gridView1.PopulateColumns();
            gridView1.Columns["Name"].Caption = "الاسم";
            gridView1.Columns["ActionDate"].Caption = "التاريخ";
            gridView1.Columns["ActionDate"].DisplayFormat.FormatString = "dd/MM/yyyy   hh:mm:ss tt";
            
        }
        void RefreshData()
        {
            using(var db = new dbDataContext())
            {
                var q = from log in db.UserLogs.Where(x => x.ActionType == (byte)Master.Actions.LogIn).DefaultIfEmpty()
                        from user in db.Users.Where(x => x.ID == log.UserID).DefaultIfEmpty()
                        select new
                        {
                            user.Name,
                            log.ActionDate,
                           
                        };
                if (dtFrom.EditValue != null)
                {
                    q=q.Where(x=>x.ActionDate.Date>=dtFrom.DateTime.Date);
                }
                if (dtTo.EditValue != null)
                {
                    q = q.Where(x => x.ActionDate.Date <= dtFrom.DateTime.Date);
                }
                gridControl1.DataSource = q.ToList();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            dtFrom.EditValue = dtTo.EditValue = null;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            dtFrom.EditValue = dtTo.EditValue = null;
            RefreshData();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
