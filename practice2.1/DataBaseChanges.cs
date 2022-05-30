using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using Sales;
namespace practice2._1
{
    public static class DataBaseChanges
    {
        public static SqlTableDependency<Product> Products;
        //the void name must be the same as the varaiable ex:  Products =>> Products_Changed
        public static void Products_Changed(object sender, RecordChangedEventArgs<Product> e)
        {
            //creating thread that allows eddit on main thread to avoid cross threading
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                if (e.ChangeType == ChangeType.Insert)
                {
                    Session.Products.Add(e.Entity);
                    Session.ProductsView.Add(Session.ProductViewClass.GetProduct(e.Entity.ID));
                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    var index = Session.Products.IndexOf(Session.Products.Single(x => x.ID == e.Entity.ID));
                    Session.Products.Remove(Session.Products.Single(x => x.ID == e.Entity.ID));
                    Session.Products.Insert(index, e.Entity);
                    var viewIndex = Session.ProductsView.IndexOf(Session.ProductsView.Single(x => x.ID == e.Entity.ID));
                    Session.ProductsView.Remove(Session.ProductsView.Single(x => x.ID == e.Entity.ID));
                    Session.ProductsView.Insert(viewIndex,Session.ProductViewClass.GetProduct(e.Entity.ID));
                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    Session.Products.Remove(Session.Products.Single(x => x.ID == e.Entity.ID));
                    Session.ProductsView.Remove(Session.ProductsView.Single(x => x.ID == e.Entity.ID));
                }
            }
            ));
        }
        public class ProductUnits : ProductUnit { }
        public static SqlTableDependency<ProductUnits> _ProductUnits;
        public static void _ProductUnits_Changed(object sender, RecordChangedEventArgs<ProductUnits> e)
        {
            var index = Session.ProductsView.IndexOf(Session.ProductsView.Single(x => x.ID == e.Entity.ProductID));
            Session.ProductsView[index] = Session.ProductViewClass.GetProduct(e.Entity.ProductID);
        }
        //this class just because the database name ends with" s "
        public class CustomersAndVendors : CustomersAndVendor { }
        public static SqlTableDependency<CustomersAndVendors> Vendors;

        public static void Vendors_Changed(object sender, RecordChangedEventArgs<CustomersAndVendors> e)
        {
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                switch (e.ChangeType)
                {
                    case ChangeType.None:
                        break;
                    case ChangeType.Insert:
                        Session.Vendor.Add(e.Entity);
                        break;
                    case ChangeType.Delete:

                        Session.Vendor.Remove(Session.Vendor.Single(x => x.ID == e.Entity.ID));

                        break;
                    case ChangeType.Update:

                        var index = Session.Vendor.IndexOf(Session.Vendor.Single(x => x.ID == e.Entity.ID));
                        Session.Vendor.Remove(Session.Vendor.Single(x => x.ID == e.Entity.ID));
                        Session.Vendor.Insert(index, e.Entity);
                        // Or this  Session.Vendor[index] = e.Entity;
                        break;
                }
            }));
        }
        public static SqlTableDependency<CustomersAndVendors> Customers;

        public static void Customers_Changed(object sender, RecordChangedEventArgs<CustomersAndVendors> e)
        {
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                switch (e.ChangeType)
                {
                    case ChangeType.None:
                        break;
                    case ChangeType.Insert:

                        Session.Customer.Add(e.Entity);
                        break;
                    case ChangeType.Delete:

                        Session.Customer.Remove(Session.Customer.Single(x => x.ID == e.Entity.ID));
                        break;
                    case ChangeType.Update:

                        var index = Session.Customer.IndexOf(Session.Customer.Single(x => x.ID == e.Entity.ID));
                        Session.Customer.Remove(Session.Customer.Single(x => x.ID == e.Entity.ID));
                        Session.Customer.Insert(index, e.Entity);
                        break;
                }

            }));
        }


         public class Accounts : Account { }
        public static SqlTableDependency<Accounts> Account;

        public static void Accounts_Changed(object sender, RecordChangedEventArgs<Accounts> e)
        {
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                switch (e.ChangeType)
                {
                    case ChangeType.None:
                        break;
                    case ChangeType.Insert:
                        Session.Accounts.Add(e.Entity);
                        break;
                    case ChangeType.Delete:

                        Session.Accounts.Remove(Session.Accounts.Single(x => x.ID == e.Entity.ID));

                        break;
                    case ChangeType.Update:

                        var index = Session.Accounts.IndexOf(Session.Accounts.Single(x => x.ID == e.Entity.ID));
                        Session.Accounts.Remove(Session.Accounts.Single(x => x.ID == e.Entity.ID));
                        Session.Accounts.Insert(index, e.Entity);
                        // Or this  Session.Vendor[index] = e.Entity;
                        break;
                }
            }));
        }
        
        public static  SqlTableDependency<Store> _Stores;
        public static void _Store_Changed(object sender , RecordChangedEventArgs<Store> e)
        {
            Application.OpenForms[0].Invoke(new Action(()=> 
            {
                switch (e.ChangeType)
                {
                    case ChangeType.None:
                        break;
                    case ChangeType.Delete:
                        Session.Store.Remove(Session.Store.SingleOrDefault(x => x.ID == e.Entity.ID));
                        break;
                    case ChangeType.Insert:
                        Session.Store.Add(e.Entity);
                        break;
                    case ChangeType.Update:
                        var index = Session.Store.IndexOf(Session.Store.Single(x => x.ID == e.Entity.ID));
                        Session.Store[index] = e.Entity;
                        break;
                   
                }

            }));
        }
       
    }
}