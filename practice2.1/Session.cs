using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableDependency.SqlClient.Base.Abstracts;

namespace practice2._1
{
    public static class Session
    {
       
        public static class Defaults
        {
            public static int Drawer { get => 1; }
            public static int Vendor { get => 1; }
            public static int Customer { get => 2; }
            public static int Store { get => 11; }
            public static int RawStore { get => 11; }
            public static DateTime Date { get => DateTime.Now; }
            public static int DiscountAllowedAccountID { get => 20; }
            public static int DiscountReceivedAccountID { get => 19; }
            public static int SalesTax { get => 29; }
            public static int PurchaseTax { get => 30; }
            public static int PurchaseExpenses { get => 31; }
          
        }


        private static BindingList<Product> _products;
        public static BindingList<Product> Products
        {
            get
            {
                if (_products == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _products = new BindingList<Product>(db.Products.ToList());
                    }
                    //Set the DataBase service broker using "ALTER DATABASE DatabasName SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE; "
                    DataBaseChanges.Products = new TableDependency.SqlClient.SqlTableDependency<Product>(Properties.Settings.Default.UserConnectionString);
                    DataBaseChanges.Products.OnChanged += DataBaseChanges.Products_Changed;
                    DataBaseChanges.Products.Start();
                }
                return _products;
            }
        }
        private static CompanyInfo _CompanyInfo;
        public static CompanyInfo CompanyInfo
        {
            get
            {
                if (_CompanyInfo == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _CompanyInfo = db.CompanyInfos.First();
                    }
                }
                return _CompanyInfo;
            }
        }
        public static void ResetCompanyInfo()
        {
            _CompanyInfo=null;
        }
        private static BindingList<ProductViewClass> _ProductsView;
        public static BindingList<ProductViewClass> ProductsView
        {
            get
            {
                if (_ProductsView == null)
                {
                    using (var db = new dbDataContext())
                    {
                        //join tables together "Relations"
                        var Data = from pr in Session.Products
                                   join cg in db.ProductCategories on pr.CategoryID equals cg.ID
                                   join tp in db.ProductTypes on pr.ProductTypeID equals tp.ID
                                   select new ProductViewClass
                                   {
                                       ID = pr.ID,
                                       Code = pr.Code,
                                       Name = pr.Name,
                                       Description = pr.Description,
                                       CategoryName = cg.Name,
                                       Type = tp.Name,
                                       IsActive = pr.IsActive,
                                       // show ProductUnits table as list in the grid
                                       Units = (from u in db.ProductUnits
                                                where u.ProductID == pr.ID
                                                join un in db.UnitNames on u.UnitID equals un.ID
                                                select new ProductViewClass.ProductUOMview
                                                {
                                                    UnitID = u.UnitID,
                                                    UnitName = un.Name,
                                                    Factor = u.Factor,
                                                    SellPrice = u.SellPrice,
                                                    SellDisscount = u.SellDisscount,
                                                    BuyPrice = u.BuyPrice,
                                                    Barcode = u.Barcode
                                                }).ToList()
                                   };
                        _ProductsView = new BindingList<ProductViewClass>(Data.ToList());
                    }

                }
                return _ProductsView;
            }
        }

        public class ProductViewClass
        {
            public static ProductViewClass GetProduct(int id)
            {
                using (var db = new dbDataContext())
                {
                    //join tables together "Relations"
                    var Data = from pr in Session.Products
                               where pr.ID == id
                               join cg in db.ProductCategories on pr.CategoryID equals cg.ID
                               join tp in db.ProductTypes on pr.ProductTypeID equals tp.ID
                               select new ProductViewClass
                               {
                                   ID = pr.ID,
                                   Code = pr.Code,
                                   Name = pr.Name,
                                   Description = pr.Description,
                                   CategoryName = cg.Name,
                                   Type = tp.Name,
                                   IsActive = pr.IsActive,
                                   // show ProductUnits table as list in the grid
                                   Units = (from u in db.ProductUnits
                                            where u.ProductID == pr.ID
                                            join un in db.UnitNames on u.UnitID equals un.ID
                                            select new ProductViewClass.ProductUOMview
                                            {
                                                UnitID = u.UnitID,
                                                UnitName = un.Name,
                                                Factor = u.Factor,
                                                SellPrice = u.SellPrice,
                                                SellDisscount = u.SellDisscount,
                                                BuyPrice = u.BuyPrice,
                                                Barcode = u.Barcode
                                            }).ToList()
                               };
                    return Data.First();
                }
            }
            public int ID { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string CategoryName { get; set; }
            public string Description { get; set; }
            public Boolean IsActive { get; set; }
            public string Type { get; set; }
            public List<ProductUOMview> Units { get; set; }
            public class ProductUOMview
            {
                public int UnitID { get; set; }
                public string UnitName { get; set; }
                public string Barcode { get; set; }
                public double Factor { get; set; }
                public double SellPrice { get; set; }
                public double SellDisscount { get; set; }
                public double BuyPrice { get; set; }
            }
        }

        private static BindingList<CustomersAndVendor> _vendor;
        public static BindingList<CustomersAndVendor> Vendor
        {
            get
            {

                if (_vendor == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _vendor = new BindingList<CustomersAndVendor>(db.CustomersAndVendors.Where(x => x.IsCustomer == false).ToList());
                    }
                    DataBaseChanges.Vendors = new TableDependency.SqlClient.SqlTableDependency<DataBaseChanges.CustomersAndVendors>(Properties.Settings.Default.UserConnectionString, filter: new VendorsOnly());
                    DataBaseChanges.Vendors.OnChanged += DataBaseChanges.Vendors_Changed;
                    DataBaseChanges.Vendors.Start();
                }
                return _vendor;
            }
        }
        private static BindingList<Account> _account;
        public static BindingList<Account> Accounts
        {
            get
            {

                if (_account == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _account = new BindingList<Account>(db.Accounts.ToList());
                    }
                    DataBaseChanges.Account = new TableDependency.SqlClient.SqlTableDependency<DataBaseChanges.Accounts>(Properties.Settings.Default.UserConnectionString);
                    DataBaseChanges.Account.OnChanged += DataBaseChanges.Accounts_Changed;
                    DataBaseChanges.Account.Start();
                }
                return _account;
            }
        }
        //Filtering the changes just to vendors
        public class VendorsOnly : ITableDependencyFilter
        {
            public string Translate()
            {
                return "IsCustomer=0";
            }
        }
        private static BindingList<CustomersAndVendor> _customer;
        public static BindingList<CustomersAndVendor> Customer
        {
            get
            {

                if (_customer == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _customer = new BindingList<CustomersAndVendor>(db.CustomersAndVendors.Where(x => x.IsCustomer == true).ToList());
                    }
                    DataBaseChanges.Customers = new TableDependency.SqlClient.SqlTableDependency<DataBaseChanges.CustomersAndVendors>(Properties.Settings.Default.UserConnectionString, filter: new CustomersOnly());
                    DataBaseChanges.Customers.OnChanged += DataBaseChanges.Customers_Changed;
                    DataBaseChanges.Customers.Start();
                }

                return _customer;
            }
        }
        public class CustomersOnly : ITableDependencyFilter
        {
            public string Translate()
            {
                return "IsCustomer=1";
            }
        }
        private static BindingList<UnitName> _unitNames;
        public static BindingList<UnitName> UnitNames
        {
            get
            {
                if (_unitNames == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _unitNames = new BindingList<UnitName>(db.UnitNames.ToList());
                    }
                }
                return _unitNames;
            }
        }
        private static BindingList<Drawer> _drawer;
        public static BindingList<Drawer> Drawer
        {
            get
            {
                if (_drawer == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _drawer = new BindingList<Drawer>(db.Drawers.ToList());
                    }
                }
                return _drawer;
            }
        }
        private static BindingList<Store> _store;
        public static BindingList<Store> Store
        {
            get
            {
                if (_store == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _store = new BindingList<Store>(db.Stores.ToList());
                    }
                    DataBaseChanges._Stores = new TableDependency.SqlClient.SqlTableDependency<Store>(Properties.Settings.Default.UserConnectionString);
                    DataBaseChanges._Stores.OnChanged += DataBaseChanges._Store_Changed;
                    DataBaseChanges._Stores.Start();
                }
                return _store;
            }
        }
        private static BindingList<UserSettingsProfileProperty> _profileProperty;
        public static BindingList<UserSettingsProfileProperty> ProfileProperty
        {
            get
            {
                if (_profileProperty == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _profileProperty = new BindingList<UserSettingsProfileProperty>(db.UserSettingsProfileProperties.ToList());
                    }
                }
                return _profileProperty;
            }
        }
        public static class UserSettings
        {
            public static Master.PrintMode Invoiceprintmode { get => Master.PrintMode.ShowPreview; }
        }
        private static UserSettingsTemplate _userSettings;
        public static UserSettingsTemplate Settings
        {
            get
            {
                if (_userSettings == null)
                    _userSettings = new UserSettingsTemplate(User.SettingsProfileID);

                return _userSettings;
            }
        }
        // passing user and his permissions who logged into the session
        private static User _user;
        public static User User
        {
            get => _user;
        }
        public static void SetUser(User user)
        {
            _user = user;
            using (var db = new dbDataContext())
            {
                _screenAccessProfile =   (from s in Screens.GetScreensProp
                        from d in db.UserAccessProfileDetails
                        .Where(x => x.ProfileID == user.ScreenProfileID && x.ScreenID == s.ScreenID).DefaultIfEmpty()
                        select new ScreenAccessProfile(s.ScreenName)
                        {
                            CanAdd = (d == null) ? true : d.CanAdd,
                            CanOpen = (d == null) ? true : d.CanOpen,
                            CanShow = (d == null) ? true : d.CanShow,
                            CanEdit = (d == null) ? true : d.CanEdit,
                            CanDelete = (d == null) ? true : d.CanDelete,
                            CanPrint = (d == null) ? true : d.CanPrint,
                            Actions = s.Actions,
                            Screencaption = s.Screencaption,
                            ScreenName = s.ScreenName,
                            ScreenID = s.ScreenID,
                            ParentScreenID = s.ParentScreenID,


                        }).ToList();
            }
        }
        private static List<ScreenAccessProfile>  _screenAccessProfile;
        public static List<ScreenAccessProfile> ScreenAccessProfile
        {
            get
            {
               
                 return  _screenAccessProfile;
              
            } 
           
        }

    }
}
