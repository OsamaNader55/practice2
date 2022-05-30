using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using static practice2._1.Master;
using Sales;
namespace practice2._1
{
    
    public partial class MasterForm : XtraForm
    {
       
        public MasterForm()
        {
            InitializeComponent();
        }
       
        public virtual void New()
        {
           

        }
        public virtual void Getdata()
        {

        }
        public virtual void save()
        {

           
            
        }
        public virtual void Print()
        {
            
        }
        public virtual void Delete()
        {

        }
        public virtual void Refreshdata()
        {

        }
        public virtual bool IsdataValid()
        {
            return true;    
        }



        private void MasterForm_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.F1)
            //{

            //    save();
            //   //btnSave.PerformClick();
            //    //Or
            //  //  btnSave_Click_1(null, null);
            //}
            //if (e.KeyCode == Keys.F2)
            //{
            //    New();
            //}
            //if (e.KeyCode == Keys.F3)
            //{
            //    Delete();
            //}
        }

        //private void btnSave_Click_1(object sender, EventArgs e)
        //{
           
        //    //if(IsdataValid())
        //    //save();
        //}

        //private void btnNew_Click_1(object sender, EventArgs e)
        //{
        //    New();
        //}

        //private void btnDelete_Click(object sender, EventArgs e)
        //{
        //    Delete();
        //}

        private void MasterForm_Load(object sender, EventArgs e)
        {
            
        }

        //private void btnPrint_Click(object sender, EventArgs e)
        //{
        //    Print();
        //}

       
    }
}
