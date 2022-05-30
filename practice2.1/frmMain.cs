using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

using DevExpress.LookAndFeel;
using Sales;
namespace practice2._1
{
    public partial class frmMain : frmMaster
    {
        public static frmMain _insMain;
        public static frmMain InsMain { get
            {
                if (_insMain == null)
                    _insMain = new frmMain();
                return _insMain;
            } }

        public frmMain()
        {
            InitializeComponent();
            UserLookAndFeel.Default.SkinName =Sales.Properties. Settings.Default["ApplicationSkinName"].ToString();
        }
        public static void OpenForm(string name)
        {
            Form frm = null;
            switch (name)
            {

                case "frmVendor":
                    frm = new frmCustomerVendor(false);
                    break;
                case "frmCustomer":
                    frm = new frmCustomerVendor(true);
                    break;
                case "frmCustomerList":
                    frm = new frmCustomerVendorList(true);
                    break;
                case "frmVendorList":
                    frm = new frmCustomerVendorList(false);
                    break;
                case "frmPurchaseInvoice":
                    frm = new frmInvoice(Master.InvoiceType.purchase);
                    break;
                case "frmSalesInvoice":
                    frm = new frmInvoice(Master.InvoiceType.sales);
                    break;
                case "frmPurchaseInvoiceList":
                    frm = new frmInvoiceList(Master.InvoiceType.purchase);
                    break;
                case "frmSalesInvoiceList":
                    frm = new frmInvoiceList(Master.InvoiceType.sales);
                    break;
                case "frmPurchaseReturnInvoice":
                    frm = new frmInvoice(Master.InvoiceType.purchaseReturn);
                    break;
                case "frmSalesReturnInvoice":
                    frm = new frmInvoice(Master.InvoiceType.salesReturn);
                    break;
                case "frmPurchaseReturnInvoiceList":
                    frm = new frmInvoiceList(Master.InvoiceType.purchaseReturn);
                    break;
                case "frmSalesReturnInvoiceList":
                    frm = new frmInvoiceList(Master.InvoiceType.salesReturn);
                    break;
                case "frmVendorAccountCheck":
                    frm = new frmAccountsCheck(false);
                    break;
                case "frmCustomerAccountCheck":
                    frm = new frmAccountsCheck(true);
                    break ;
                default:
                    var ins = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.Name == name);
                   
                    if (ins != null)
                    {
                        frm = Activator.CreateInstance(ins) as Form;
                        if (Application.OpenForms[frm.Name] != null)
                        {
                            frm = Application.OpenForms[frm.Name];
                            frm.BringToFront();
                        }
                        else { if (frm != null) { OpenFormWithPermissions(frm); } }
                        return;
                        
                    }
                    break ;
            }
            //Using this method because forms with multible structure are opened from the main type form  
            var openforms = Application.OpenForms;
            foreach (var item in openforms)
            {
                var type = item.GetType();
                if (type.Equals(frm.GetType()) )
                {
                  var  form = item as Form;
                    form.Close();
                    break;
                } 
            }
                if (frm != null) { OpenFormWithPermissions(frm); }
            
            //{

            //    //frm = Application.OpenForms[frm.Name];
            //    //frm.BringToFront();
            //    var openforms = Application.OpenForms;
            //    foreach (var item in openforms)
            //    {
            //        if (item.Equals(frm)) frm.BringToFront();
            //    }
            //}


            /* Or this but Switch case better,,

             Form frm = null;
             if (name == "frmVendor")
             {
                 frm = new frmCustomerVendor(false);
                 
             }
            else if (name == "frmCustomer")
             {
                 frm = new frmCustomerVendor(true);
                 
             }
             else if (name == "frmCustomerList")
             {
                 frm = new frmCustomerVendorList(true);
                
             }
             else if (name == "frmVendorList")
             {
                 frm = new frmCustomerVendorList(false);
                 
             }
             else
             {
                 var ins = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.Name == name);
                 if (ins != null)
                 {
                     frm = Activator.CreateInstance(ins) as Form;
                     if (Application.OpenForms[frm.Name] != null)
                     {
                         frm = Application.OpenForms[frm.Name];
                         frm.BringToFront();
                     }
                     else
                     {
                         frm.Show();
                     }
                 } 
            if(frm != null) { frm.Show(); }
            */
        }
        public static void OpenFormWithPermissions (Form frm , bool openinDialog = false)
        {
            if (Session.User.UserType == (byte)Master.UserType.Admin)
            {
                if (openinDialog == true)
                {
                    frm.ShowDialog();
                    InsertUserLog(Master.Actions.Open, 000, "فتح شاشه " +frm.Name, frm.Name) ;
                }
                else
                    frm.Show();
                InsertUserLog(Master.Actions.Open, 000, "فتح شاشه " + frm.Name, frm.Name);
                return;
            }
            var screen = Session.ScreenAccessProfile.FirstOrDefault(x => x.ScreenName == frm.Name);
            if(screen != null)
            {
                if (screen.CanOpen == true)
                {
                    if (openinDialog == true)
                    { 
                        frm.ShowDialog(); 
                        InsertUserLog(Master.Actions.Open, 000, "فتح شاشه " + frm.Name, frm.Name); 
                    }
                    else
                    {
                        frm.Show();
                        InsertUserLog(Master.Actions.Open, 000, "فتح شاشه " + frm.Name, frm.Name);
                    }
                  
                    return;
                }
                else MessageBox.Show("غير مصرح");
            }
        }
        public static bool CheckActionPermission( string formName ,Master.Actions actions,User user =null )
        {
            if(user==null) user = Session.User;
            if(user.UserType==(byte)Master.UserType.Admin) return true;
            else
            {
                var screen = Session.ScreenAccessProfile.FirstOrDefault(x => x.ScreenName == formName);
                bool flag = true;
                if (screen != null)
                {
                    switch (actions)
                    {
                       
                        case Master.Actions.Add:
                            flag = screen.CanAdd;
                            break;
                        case Master.Actions.Edit:
                            flag = screen.CanEdit;
                            break;
                        case Master.Actions.Delete:
                            flag = screen.CanDelete;
                            break;
                        case Master.Actions.Print:
                            flag = screen.CanPrint;
                            break;
                        default:
                            break;
                    }
                }
                if (flag == false) MessageBox.Show("غير مصرح");
                return flag;
                
            }
            
        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var tag = e.ClickedItem.Tag as string;
            if(tag==null||tag==String.Empty) return;
            if (tag != string.Empty)
            {
                OpenForm(tag);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            simpleLabelItem1.Text = "User: " + Session.User.Name;  
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
          Sales.Properties.  Settings.Default["ApplicationSkinName"] = UserLookAndFeel.Default.SkinName;
         Sales.Properties.  Settings.Default.Save();
            Application.Exit();
            
        }
        private void timer1_Tick(object sender, EventArgs e) => simpleLabelItem2.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt dddd,MMMM");
    }
}
