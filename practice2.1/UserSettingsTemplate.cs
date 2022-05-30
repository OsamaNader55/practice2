using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales;
namespace practice2._1
{
    public class UserSettingsTemplate
    {
        int ProfileID { get; set; }
        public UserSettingsTemplate(int profileID)
        {
              ProfileID=profileID;
            General = new GeneralSettings(ProfileID);
            Sales = new SalesSettings(ProfileID);
            Purchase = new PurchaseSettings(ProfileID);
            Invoices = new InvoicesSettings(ProfileID);
        }

        public GeneralSettings General { get; set; }
        public SalesSettings Sales { get; set; }
        public PurchaseSettings Purchase { get; set; }
        public InvoicesSettings Invoices { get; set; }

        public static string GetpropCaption(string propname)
        {
            switch (propname)
            {
                case nameof(UserSettingsTemplate.General):return "اعدادات عامه"; 
                case nameof(UserSettingsTemplate.Sales): return "اعدادات البيع";
                case nameof(UserSettingsTemplate.Purchase): return "اعدادات الشراء";
                case nameof(UserSettingsTemplate.Invoices): return "اعدادات الفواتير";
                case nameof(GeneralSettings.DefaultStore): return "المخزن الافتراضي";
                case nameof(GeneralSettings.CanChangeVendor): return " السماح بتغيــر المورد";
                case nameof(GeneralSettings.CanChangeCustomer): return " السماح بتغيــر العميل";
                case nameof(GeneralSettings.DefaultVendor): return " المورد الافتراضي";
                case nameof(GeneralSettings.DefaultCustomer): return " العميل الافتراضي";
                case nameof(GeneralSettings.DefaultDrawer): return " الخزنه الافتراضيه";
                case nameof(GeneralSettings.DefaultRawStore): return " مخزن الخامات الافتراضي";
                case nameof(GeneralSettings.CanChangeDrawer): return " السماح بتغيــر الخزنه";
                case nameof(GeneralSettings.CanChangeStore): return " السماح بتغيــر المخزن";
                case nameof(GeneralSettings.CanViewDocumentHistory): return " السماح بعرض سجل التغييرات";
                case nameof(InvoicesSettings.CanChangeTax): return " السماح بتغير الضريبه";
                case nameof(InvoicesSettings.CanDeleteItemsInInvoices): return " السماح بحذف الاصناف من الفواتير";
                case nameof(SalesSettings.CanChangeItemPriceInSales): return " السماح بتغير سعر الصنف عند البيع";
                case nameof(SalesSettings.CanChangeQuantityInSales): return " السماح بتغير الكميه عند البيع";
                case nameof(SalesSettings.CanChangeSalesInvoiceDate): return " السماح بتغير تاريخ فواتير البيع";
                case nameof(SalesSettings.HideCostSales): return " عرض التكلفه عند البيع";
                case nameof(SalesSettings.CanChangePaidinSales): return " السماح بتغير المبلغ المدفوع ";
                case nameof(SalesSettings.CanNotPostToStoreInSales): return " انشاء فواتير بدون صرف";
                case nameof(SalesSettings.sellToVendors): return " السماح بالبيع للموردين";
                case nameof(PurchaseSettings.CanChangeItemPriceInPurchase): return " السماح بتغير سعر الصنف عند البيع";
                case nameof(PurchaseSettings.CanChangePurchaseInvoiceDate): return " السماح بتغير تاريخ فواتير الشراء";
                case nameof(PurchaseSettings.BuyFromCustomers): return " السماح بالشراء من العملاء";
                case nameof(SalesSettings.DefaultPayMethodinSales): return " طريقة الدفع الافتراضيه";
                case nameof(SalesSettings.SellingToCustomerExceededMaxCredit): return "  عند البيع لعيمل تجاوز حد الائتمان";
                case nameof(SalesSettings.SellingItemReaChedReorderLevel): return " عند بيع صنف تجاوز حد الطلب";
                case nameof(SalesSettings.SellingItemWithPriceLowerThanCostPrice): return " عند البيع بسعر اقل من سعر التكلفه";
                case nameof(SalesSettings.SellingItemwithQtyMoreThanAvilableQty): return " عند بيع كميه من صنف اكبر من المتاح";
                case nameof(SalesSettings.MaxDiscountInInvoice): return " اقصي خصم مسموح للفاتوره";
                case nameof(SalesSettings.MaxDiscountPerItem): return " اقصي خصم مسموح للصنف";
                default: return "Error";
                
                    
            }
        }
        public static BaseEdit GetPropControl(string propname , object propValue)
        {
            BaseEdit edit = null;
            switch (propname)
            {
                case nameof(GeneralSettings.CanChangeCustomer):
                case nameof(GeneralSettings.CanChangeVendor):
                case nameof(GeneralSettings.CanChangeStore):
                case nameof(GeneralSettings.CanChangeDrawer):
                case nameof(GeneralSettings.CanViewDocumentHistory):
                case nameof(InvoicesSettings.CanChangeTax):
                case nameof(InvoicesSettings.CanDeleteItemsInInvoices):
                case nameof(SalesSettings.CanChangeItemPriceInSales):
                case nameof(SalesSettings.CanChangeQuantityInSales):
                case nameof(SalesSettings.CanChangeSalesInvoiceDate):
                case nameof(SalesSettings.HideCostSales):
                case nameof(SalesSettings.CanChangePaidinSales):
                case nameof(SalesSettings.CanNotPostToStoreInSales):
                case nameof(SalesSettings.sellToVendors):
                case nameof(PurchaseSettings.CanChangeItemPriceInPurchase):
                case nameof(PurchaseSettings.CanChangePurchaseInvoiceDate):
                case nameof(PurchaseSettings.BuyFromCustomers):
                    edit = new ToggleSwitch();
                    ((ToggleSwitch)edit).Properties.OnText = "نعم";
                    ((ToggleSwitch)edit).Properties.OffText = "لا";
                    break;
                case nameof(GeneralSettings.DefaultStore):
                case nameof(GeneralSettings.DefaultRawStore):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(Session.Store);
                    break;
                case nameof(GeneralSettings.DefaultDrawer):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(Session.Drawer);
                    break;
                case nameof(GeneralSettings.DefaultCustomer):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(Session.Customer);
                    break;
                case nameof(GeneralSettings.DefaultVendor):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(Session.Vendor);
                    break;
                case nameof(SalesSettings.DefaultPayMethodinSales):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(Master.PayMthodList);
                    break;
                case nameof(SalesSettings.SellingToCustomerExceededMaxCredit):
                case nameof(SalesSettings.SellingItemReaChedReorderLevel):
                case nameof(SalesSettings.SellingItemWithPriceLowerThanCostPrice):
                case nameof(SalesSettings.SellingItemwithQtyMoreThanAvilableQty):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(Master.WarningLevelsList);
                    break;
                case nameof(SalesSettings.MaxDiscountInInvoice):
                case nameof(SalesSettings.MaxDiscountPerItem):
                    edit = new SpinEdit();
                    
                    ((SpinEdit)edit).Properties.Increment = 0.01m;
                    ((SpinEdit)edit).Properties.MaxValue = 1;
                    ((SpinEdit)edit).Properties.EditMask = "p";
                    ((SpinEdit)edit).Properties.UseMaskAsDisplayFormat = true;
                    break;

            }
            if(edit != null)
            {
                edit.Name = propname;
                edit.Properties.NullText = "";
                edit.EditValue = propValue;
                
                
            }
            return edit;
        }



    }
    public class GeneralSettings
    {int ProfileID { get; set; }
        public GeneralSettings(int profileID)
        {
             ProfileID=profileID;
        }
        
        public bool CanChangeStore { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } } 
        public int DefaultStore { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public int DefaultRawStore { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
      
        public bool CanChangeDrawer { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public int DefaultDrawer { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeCustomer { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public int DefaultCustomer { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeVendor { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public int DefaultVendor { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanViewDocumentHistory { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }


    }
    public class SalesSettings
    { int ProfileID { get; set; }
        public SalesSettings(int profileID)
        {
            ProfileID = profileID;
        }
       
        public bool CanChangePaidinSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public Master.PayMethod DefaultPayMethodinSales { get { return Master.FromByteArray<Master.PayMethod>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanNotPostToStoreInSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public Master.WarningLevels SellingToCustomerExceededMaxCredit { get { return Master.FromByteArray<Master.WarningLevels>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeItemPriceInSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public Master.WarningLevels SellingItemReaChedReorderLevel { get { return Master.FromByteArray<Master.WarningLevels>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public Master.WarningLevels SellingItemwithQtyMoreThanAvilableQty { get { return Master.FromByteArray<Master.WarningLevels>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public Master.WarningLevels SellingItemWithPriceLowerThanCostPrice { get { return Master.FromByteArray<Master.WarningLevels>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool HideCostSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public decimal MaxDiscountInInvoice { get { return Master.FromByteArray<decimal>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }//spinedit's value is Decimal Not double
        public decimal MaxDiscountPerItem { get { return Master.FromByteArray<decimal>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeQuantityInSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeSalesInvoiceDate { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool sellToVendors { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
    }
    public class InvoicesSettings
    {
        int ProfileID { get; set; }
        public InvoicesSettings(int profileID)
        {
            ProfileID = profileID;
        }
        public bool CanDeleteItemsInInvoices { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeTax { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
    }
    public class PurchaseSettings
    {
        int ProfileID { get; set; }
        public PurchaseSettings(int profileID)
        {
            ProfileID = profileID;
        }
        public bool CanChangeItemPriceInPurchase { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool BuyFromCustomers { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangePurchaseInvoiceDate { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
    }
}