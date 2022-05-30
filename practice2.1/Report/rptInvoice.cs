using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraReports;

namespace practice2._1.Reports
{
    public partial class rptInvoice : DevExpress.XtraReports.UI.XtraReport
    {
        public rptInvoice()
        {
            InitializeComponent();
            lblCompanyName.Text = Session.CompanyInfo.CompanyName;
            lblAddress.Text = Session.CompanyInfo.Address;   
            lblPhone.Text = Session.CompanyInfo.Phone;
            lblEmail.Text = Session.CompanyInfo.Email;
        }
        void BindingData()
        {
            lblInvoiceCode.DataBindings.Add("Text", this.DataSource, "Code");
            lblDate.DataBindings.Add("Text", this.DataSource, "Date");
            lblCust.DataBindings.Add("Text", this.DataSource, "Customer");
            lblDrawer.DataBindings.Add("Text", this.DataSource, "drawer");
            lblInvoicetype.DataBindings.Add("Text", this.DataSource, "InvoiceType");
            lblStore.DataBindings.Add("Text", this.DataSource, "store");
            lblTax.DataBindings.Add("Text", this.DataSource, "Tax");
            lblDiscount.DataBindings.Add("Text", this.DataSource, "DiscountRation");
            lblExpences.DataBindings.Add("Text", this.DataSource, "Expences");
            lblNet.DataBindings.Add("Text", this.DataSource, "Net");
            lblRemaining.DataBindings.Add("Text", this.DataSource, "Remaining");
            lblPaid.DataBindings.Add("Text", this.DataSource, "Paid");
            lblTotal.DataBindings.Add("Text", this.DataSource, "Total");
            lblQty.DataBindings.Add("Text", this.DataSource, "productCount");
            cell_product.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "productname"));
            cell_price.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "Price"));
            cell_Qty.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "ItemQty"));
            cell_totalPrice.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "TotalPrice"));
           
            /*  inv.ID,
                                   inv.Code,
                                   store = str.Name,
                                   drawer = drw.Name,
                                   inv.Tax,
                                   inv.DiscountRation,
                                   inv.Date,
                                   inv.Remaining,
                                   inv.Net,
                                   inv.Paid,
                                   inv.Total,
                                   Customer = prt.Name,
                                   InvoiceType = type.Name,
                                   inv.Expences,
                                   Products = (from pr in db.InvoiceDetails.Where(x => x.InvoiceID == inv.ID)
                                               from p in db.Products.Where(x => x.ID == pr.ItemID)
                                               select new
                                               {
                                                   productname = p.Name,
                                                   pr.ItemQty,
                                                   pr.Price,
                                                   pr.TotalPrice */
        }
        public static void print( object datasource )
        {
            rptInvoice rpt = new rptInvoice();  
            rpt.DataSource = datasource;
            rpt.DetailReport.DataSource = rpt.DataSource;
            rpt.DetailReport.DataMember = "Products";
            rpt.BindingData();
            switch (Session.UserSettings.Invoiceprintmode)
            {
                case Master.PrintMode.PrintDialog: rpt.PrintDialog(); break;
                case Master.PrintMode.Print: rpt.Print(); break;
                case Master.PrintMode.ShowPreview: rpt.ShowPreview(); break;
                    default: rpt.ShowPreview(); break;  
            }

        }
    }
}
