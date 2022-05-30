using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace practice2._1.Report
{
    public partial class rptGrid : DevExpress.XtraReports.UI.XtraReport
    {
        public rptGrid()
        {
            InitializeComponent();
            
            lblAddress.Text = Session.CompanyInfo.Address;
            lblPhone.Text = Session.CompanyInfo.Phone +" - "+ Session.CompanyInfo.Mobile;
            lblName.Text = Session.CompanyInfo.CompanyName;
            xrPictureBox1.Image = Master.GetimageFromByteArray(Session.CompanyInfo.Logo.ToArray());
        }
       public static void Print( GridControl control,string Reportname , string filter , string screenname,string logPrintNote)
        {
            rptGrid rpt = new rptGrid();
            rpt.AfterPrint += (sender, e) => { frmMaster.InsertUserLog(Master.Actions.Print, 0,logPrintNote , screenname); };

            rpt.lblReportName.Text = Reportname;
            rpt.xrLabel1.Text = filter;
            PrintableComponentLink link = new PrintableComponentLink();
            link.Component = control;
            rpt.printableComponentContainer1.PrintableComponent = link;
            rpt.PrintingSystem.Document.AutoFitToPagesWidth = 1;
            rpt.ShowRibbonPreview();
            //Allow user to design his report 
           // rpt.ShowDesigner();
            
           
        }

    }
}
