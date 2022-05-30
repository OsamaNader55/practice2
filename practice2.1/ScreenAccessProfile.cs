using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sales;
namespace practice2._1
{
    public class ScreenAccessProfile
    {
        public static int maxID = 1;
        public ScreenAccessProfile(string name , ScreenAccessProfile parent = null)
        {
            ScreenName = name;
            ScreenID = maxID++;
            if (parent != null) ParentScreenID = parent.ScreenID;
            else ParentScreenID = 0;

            Actions = new List<Master.Actions> { Master.Actions.Open, Master.Actions.Show, Master.Actions.Add, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Print };
        }
        public int ScreenID { get; set; }
        public int ParentScreenID { get; set; }
        public string ScreenName { get; set; }
        public string Screencaption { get; set; }
        public bool CanShow { get; set; }
        public bool CanOpen { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
        public List<Master.Actions> Actions { get; set; }
    }
    public static class Screens
    {
        public static ScreenAccessProfile MainSettings = new ScreenAccessProfile("elmMainSettins")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            Screencaption = "الاعدادات",

        };
        public static ScreenAccessProfile CompanyInfo = new ScreenAccessProfile(nameof(frmCompanyInfo), MainSettings)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show , Master.Actions.Open, Master.Actions.Edit },
            Screencaption ="بيانات الشركه",
            
        };
        public static ScreenAccessProfile Customers = new ScreenAccessProfile("elmCustomers")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            Screencaption = "العملاء",
        };
        public static ScreenAccessProfile AddCustomer= new ScreenAccessProfile("frmCustomers", Customers)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "اضافه عميل",
        };
        public static ScreenAccessProfile CustomerList = new ScreenAccessProfile("frmCustomersList", Customers)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Print },
            Screencaption = "قائمه العملاء",
        };
        public static ScreenAccessProfile Vendors = new ScreenAccessProfile("elmVendors")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            Screencaption = "الموردين",
        };
        public static ScreenAccessProfile AddVendor = new ScreenAccessProfile("frmVendors", Vendors)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "اضافه مورد",
        };
        public static ScreenAccessProfile VendorList = new ScreenAccessProfile("frmVendorsList", Vendors)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Print },
            Screencaption = "قائمه الموردين",
        };
        public static ScreenAccessProfile Invoices = new ScreenAccessProfile("elmInvoices")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            Screencaption = "الفواتير",
        };
        public static ScreenAccessProfile InvoicePurchase = new ScreenAccessProfile("frmInvoicePurchase", Invoices)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add,Master.Actions.Print },
            Screencaption = "فاتوره شراء جديده",
        };
        public static ScreenAccessProfile InvoiceSales = new ScreenAccessProfile("frmInvoiceSales", Invoices)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add, Master.Actions.Print },
            Screencaption = "فاتوره بيع جديده",
        };
        public static ScreenAccessProfile InvoicePurchaseList = new ScreenAccessProfile("frmInvoicePurchaseList", Invoices)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Print },
            Screencaption = "قائمه فواتير الشراء",
        };
        public static ScreenAccessProfile InvoiceSalesList = new ScreenAccessProfile("frmInvoiceSalesList", Invoices)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Print },
            Screencaption = "قائمه فواتير البيع",
        };
        public static ScreenAccessProfile UserSetting = new ScreenAccessProfile(nameof(frm_UserSettingsProfile), MainSettings)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete,Master.Actions.Add },
            Screencaption = "نماذج اعدادات البرنامج للمستخدمين",
        };
        public static ScreenAccessProfile UserSettingList = new ScreenAccessProfile(nameof(frm_UserSettingsProfileList), MainSettings)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open},
            Screencaption = "قائمه نماذج اعدادات البرنامج للمستخدمين",
        };
        public static ScreenAccessProfile ScreenAccess = new ScreenAccessProfile(nameof(frmAccessProfile), MainSettings)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "نماذج صلاحيات الوصول",
        };
        public static ScreenAccessProfile ScreenAccessList = new ScreenAccessProfile(nameof(frmAccessProfileList), MainSettings)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open},
            Screencaption = "قائمه نماذج صلاحيات الوصول",
        };
        public static ScreenAccessProfile Finance = new ScreenAccessProfile("elmDrawers")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            Screencaption = "الماليه",

        };
        public static ScreenAccessProfile AddDrawer = new ScreenAccessProfile(nameof(frmDrawer), Finance)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "اضافه خزنه",
        };
        public static ScreenAccessProfile DrawerList = new ScreenAccessProfile(nameof(frmDrawerList), Finance)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open },
            Screencaption = "قائمه الخزنات",
        };
        public static ScreenAccessProfile Products = new ScreenAccessProfile("elmProducts")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show }, 
            Screencaption = "الاصناف",

        };
        public static ScreenAccessProfile AddProduct = new ScreenAccessProfile(nameof(frmProducts), Products)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "اضافه صنف ",
        };
        public static ScreenAccessProfile ProductsList = new ScreenAccessProfile(nameof(frmProductList), Products)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open },
            Screencaption = "قائمه الاصناف",
        };
        public static ScreenAccessProfile ProductCategories = new ScreenAccessProfile(nameof(frmProductCategory), Products)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "فئات الاصناف",
        };
        public static ScreenAccessProfile Stores = new ScreenAccessProfile("elmStores")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            Screencaption = "المخازن",

        };
        public static ScreenAccessProfile AddStore = new ScreenAccessProfile(nameof(frmStores), Stores)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "اضافه مخزن ",
        };
        public static ScreenAccessProfile StoreList = new ScreenAccessProfile(nameof(frmStoreList), Stores)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open },
            Screencaption = "قائمه المخازن",
        };
        public static ScreenAccessProfile Users = new ScreenAccessProfile("elmUsers")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            Screencaption = "المستخدمين",

        };
        public static ScreenAccessProfile AddUser = new ScreenAccessProfile(nameof(frmUserAdd), Users)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "اضافه مستخدم",
        };
        public static ScreenAccessProfile UsersList = new ScreenAccessProfile(nameof(frmUsersList), Users)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open },
            Screencaption = "قائمه المستخدمين",
        };
        public static ScreenAccessProfile DataBaseConnect = new ScreenAccessProfile(nameof(frmDatabaseConncect), MainSettings)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open,Master.Actions.Edit },
            Screencaption = "الاتصال بقاعده البيانات",
        };
        public static ScreenAccessProfile Login = new ScreenAccessProfile(nameof(frmLogIn))
        {
            Actions = new List<Master.Actions>() {  },
            Screencaption = "تسجيل الدخول",
        };
        public static ScreenAccessProfile LoginHistory = new ScreenAccessProfile(nameof(frmLoginHistory),MainSettings)
        {
            Actions = new List<Master.Actions>() { },
            Screencaption = "سجل تسجيل دخول المستخدمين",
        };
        public static ScreenAccessProfile VendorsAccountCheck = new ScreenAccessProfile("frmVendorsAccountCheck", Vendors)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Print },
            Screencaption = "كشف حساب مورد",
        };
        public static ScreenAccessProfile CusomerAccountCheck = new ScreenAccessProfile("frmCustomersAccountCheck", Customers)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Print },
            Screencaption = "كشف حساب عميل",
        };
        public static ScreenAccessProfile BackUp = new ScreenAccessProfile(nameof(frmBackUp), MainSettings)
        {
            Actions = new List<Master.Actions>() {Master.Actions.Show,Master.Actions.Open },
            Screencaption = "انشاء نسخه احتياطيه",
        };
        public static ScreenAccessProfile Restore = new ScreenAccessProfile(nameof(frmRestore), MainSettings)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open },
            Screencaption = "استعاده نسخه احتياطيه",
        };
        public static ScreenAccessProfile ExpencesItem = new ScreenAccessProfile(nameof(frmExpencesItem), Finance)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "بنود المصروفات",
        };
        public static ScreenAccessProfile Expences = new ScreenAccessProfile(nameof(frmExpencesItem), Finance)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add },
            Screencaption = "اضافه مصروف",
        };
        public static ScreenAccessProfile CashIn = new ScreenAccessProfile(nameof(frmCashNoteIn), Finance)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Add, Master.Actions.Print },
            Screencaption = "سند قبض",
        };
        public static ScreenAccessProfile CashOut = new ScreenAccessProfile(nameof(frmCashNoteOut), Finance)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open,  Master.Actions.Add ,Master.Actions.Print},
            Screencaption = "سند دفع",
        };
        public static ScreenAccessProfile DrawerActivity = new ScreenAccessProfile(nameof(frmDrawerActivity), Finance)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open,  Master.Actions.Print },
            Screencaption = "حركات الخزنه",
        };
        public static ScreenAccessProfile InvoicePurchaseReturn = new ScreenAccessProfile("frmInvoicePurchaseReturn", Invoices)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add, Master.Actions.Print },
            Screencaption = "فاتوره مرتجع شراء",
        };
        public static ScreenAccessProfile InvoiceSalesReturn = new ScreenAccessProfile("frmInvoiceSalesReturn", Invoices)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Add, Master.Actions.Print },
            Screencaption = "فاتوره مرتجع بيع",
        };
        public static ScreenAccessProfile InvoicePurchaseReturnList = new ScreenAccessProfile("frmInvoicePurchaseReturnList", Invoices)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Print },
            Screencaption = "قائمه فواتير مردود الشراء",
        };
        public static ScreenAccessProfile InvoiceSalesReturnList = new ScreenAccessProfile("frmInvoiceSalesReturnList", Invoices)
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show, Master.Actions.Open, Master.Actions.Edit, Master.Actions.Delete, Master.Actions.Print },
            Screencaption = "قائمه فواتير مردود البيع",
        };
        public static List<ScreenAccessProfile> GetScreensProp
        {
            get
            {
                Type type= typeof(Screens); 
                FieldInfo[] info=type.GetFields(BindingFlags.Public | BindingFlags.Static);
                var list = new List<ScreenAccessProfile>();
                foreach (var item in info)
                {
                    var obj = item.GetValue(null);
                    if (obj != null && obj.GetType() == typeof(ScreenAccessProfile)) list.Add((ScreenAccessProfile)obj);
                    
                }
                return list;
            }
        }

    }
}
