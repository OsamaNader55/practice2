using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace practice2._1
{
    public static class Master
    {
        public static string AppLayoutPath { get 
            {
                var mydocument = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var appname = "POS";
                var path = Path.Combine(mydocument, appname,"Layouts");
            Directory.CreateDirectory(path);
                return path;
            } 
        }
        public static void SaveGridLayout(this GridView view , string formname)
        {
            var filePath = $"{AppLayoutPath}\\{formname}_{view.Name}";
            view.SaveLayoutToXml(filePath);
        }
        public static void RestoreGridLayout(this GridView view, string formname)
        {
            try
            {
                var filePath = $"{AppLayoutPath}\\{formname}_{view.Name}";
                if (File.Exists(filePath))
                    view.RestoreLayoutFromXml(filePath);
            }
            catch 
            {

                return;
            }
        }
        public static void SaveLayoutControlLayout(this LayoutControl layout, string formname)
        {
            var filePath = $"{AppLayoutPath}\\{formname}_{layout.Name}";
            layout.SaveLayoutToXml(filePath);
        }
        public static void RestoreLayoutControlLayout(this LayoutControl layout, string formname)
        {
            try
            {
                var filePath = $"{AppLayoutPath}\\{formname}_{layout.Name}";
                if (File.Exists(filePath))
                    layout.RestoreLayoutFromXml(filePath);
            }
            catch
            {

                return;
            }
        }
        public static bool DeleteConfirmateionMsg()
        {
            return (MessageBox.Show(text: "هل تريد الحذف؟", caption: "تأكيد الحذف", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes);
        }
        public static bool validateText(this TextEdit txt)
        {
            if(txt.Text.Trim() == String.Empty.Trim())
            {
                txt.ErrorText = "هذا الحقل مطلوب";
                return false;
            }
            else return true;
        }
        public static int GetRowHandleByRowObject(this GridView view ,object row)
        {
            if (row != null)
            {
                for (int i = 0; i < view.DataRowCount; i++)
                {
                    if (row.Equals(view.GetRow(i)))
                        return i;
                }
            }
             return GridControl.InvalidRowHandle;
        }
        public static bool IsValuenotlessthanZero(this SpinEdit spn)
        {
            if (spn.Value<0)
            {
                spn.ErrorText = "لا يمكن ان تكون القيمه اصغر من صفر";
                return false;
            }
            else return true;
        }
        public static bool IsValuenotEqualORlessthanZero(this SpinEdit spn)
        {
            if (spn.Value <= 0)
            {
                spn.ErrorText = "لا يمكن ان تكون القيمه اصغر من او تساوي صفر";
                return false;
            }
            else return true;
        }
        public static bool IsEditValueValidAndNotZero(this LookUpEditBase lkp)
        {
            if (lkp.IsEditvalueNotNull() == false || Convert.ToInt32(lkp.EditValue) == 0)
            {
                lkp.ErrorText = "هذا الحقل مطلوب";
                return false;
            }
            else return true;
        }
        public static bool IsEditvalueNotNull(this LookUpEditBase edit)
        {
            if( edit.EditValue==null) return false; else return true;
        }
        public static bool IsdateValid(this DateEdit dt)
        {
            if (dt.DateTime.Year<2000)
            {
                dt.ErrorText = "هذا الحقل مطلوب و يجب ان يكون التاريخ مظبوط";
                return false;
            }
            else return true;
        }
        // Using Extinsions prop , OverLoad Methods (OOP).
        //To use Extinsions prop The class must be static .
        public class ValueAndID
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        public enum Parttype
        {
            Vendor,
            Customer,
            Account,
        }
        public enum PrintMode
        {
            Print,
            ShowPreview,
            PrintDialog,
        }
        public static List<ValueAndID> PayMthodList = new List<ValueAndID>()
        {
            new ValueAndID(){ID=(int)PayMethod.Cash, Name="كاش"},
            new ValueAndID(){ID=(int)PayMethod.Credit, Name="اجل"}
        };

        public enum PayMethod
        {
            Cash=1,
            Credit,
        }
        public static List<ValueAndID> JournalsOperationTypeList = new List<ValueAndID>()
        {
            new ValueAndID(){ID=(int)JournalsOperationType.VendorGeneral, Name="حساب عام لمورد"},
            new ValueAndID(){ID=(int)JournalsOperationType.VendorDisocunt, Name="حساب خصم لمورد"},
             new ValueAndID(){ID=(int)JournalsOperationType.VendorPaid, Name="حساب دفع لمورد"},
              new ValueAndID(){ID=(int)JournalsOperationType.CustomerGeneral, Name="حساب عام لعميل "},
               new ValueAndID(){ID=(int)JournalsOperationType.CustomerDisocunt, Name="حساب خصم لعميل"},
                new ValueAndID(){ID=(int)JournalsOperationType.CustomerPaid, Name="حساب دفع لعميل"},
              new ValueAndID(){ID=(int)JournalsOperationType.StoreInventory, Name="حساب عام لمخزن"},
              new ValueAndID(){ID=(int)JournalsOperationType.StoreTaxPurchase, Name="حساب ضريبه شراء لمخزن "},
              new ValueAndID(){ID=(int)JournalsOperationType.StoreTaxSell, Name="حساب ضريبه بيع لمخزن"},
               new ValueAndID(){ID=(int)JournalsOperationType.StoreDiscountPurchaseRecieved, Name="حساب خصم شراء متلقي لمخزن"},
                new ValueAndID(){ID=(int)JournalsOperationType.StoreDiscountSellAllowed, Name="حساب خصم بيع مسموح لمخزن"},
                 new ValueAndID(){ID=(int)JournalsOperationType.PurchaseExpences, Name="حساب ضرائب البيع"},
                  new ValueAndID(){ID=(int)JournalsOperationType.Drawer, Name="حساب خزنه"},
                   new ValueAndID(){ID=(int)JournalsOperationType.ExpenecesItems, Name="حساب بند مصروفات"},
                    new ValueAndID(){ID=(int)JournalsOperationType.CashnoteIn, Name="حساب سند قبض"},
                   new ValueAndID(){ID=(int)JournalsOperationType.CashNoteOut, Name="حساب سند دفع"},
        };
        public enum JournalsOperationType
        {
            VendorGeneral = 1,  
            VendorDisocunt,
            VendorPaid,
            CustomerGeneral ,  
            CustomerDisocunt,
            CustomerPaid,
            StoreInventory,
            StoreTaxPurchase,
            StoreTaxSell,
            StoreDiscountPurchaseRecieved,
            StoreDiscountSellAllowed,
            PurchaseExpences,
            Drawer,
            ExpenecesItems,
            CashnoteIn,
            CashNoteOut,

        }
        public static List<ValueAndID> UserTypeList = new List<ValueAndID>()
        {
            new ValueAndID(){ID=(int)UserType.Admin, Name="دخول كامل"},
            new ValueAndID(){ID=(int)UserType.User, Name="دخول مخصـص"}
        };

        public enum UserType
        {
            Admin = 1,
            User,
        }
        public enum Actions
        {
            Show =1,
            Open ,
            Add,
            Edit,
            Delete,
            Print,
            LogIn,
        }
        public static List<ValueAndID> WarningLevelsList = new List<ValueAndID>()
        {
            new ValueAndID(){ID=(int)WarningLevels.DonnotEnterupt, Name="عدم التداخل"},
            new ValueAndID(){ID=(int)WarningLevels.ShowWarning, Name="تحذير"},
             new ValueAndID(){ID=(int)WarningLevels.Prevent, Name="منع"},
        };

        public enum WarningLevels
        {
            DonnotEnterupt=1,
            ShowWarning,
            Prevent,
        }
        public static List<ValueAndID> PartTypeList = new List<ValueAndID>()
        {
            new ValueAndID(){ID = (int)Parttype.Vendor, Name ="مورد"},
            new ValueAndID(){ID = (int)Parttype.Customer, Name ="عميل"},
              new ValueAndID(){ID = (int)Parttype.Account, Name ="حساب"}
        };
        public enum InvoiceType
        {
            purchase=SourceType.purchase,
            sales= SourceType.sales,
            purchaseReturn= SourceType.purchaseReturn,
            salesReturn= SourceType.salesReturn,
            cashnotein=SourceType.CashNoteIN,
            cashnoteout=SourceType.CashNoteOut,
        }
        public enum SourceType
        {
            purchase,
            sales,
            purchaseReturn,
            salesReturn,
            ExpencesItems,
            CashNoteIN,
            CashNoteOut,
        }
        public static List<ValueAndID> InvoiceTypeList = new List<ValueAndID>()
        {
            new ValueAndID(){ID = (int)InvoiceType.purchase, Name ="مشتريات"},
            new ValueAndID(){ID = (int)InvoiceType.sales, Name ="مبيعات"},
             new ValueAndID(){ID = (int)InvoiceType.purchaseReturn, Name ="مردود مشتريات"},
              new ValueAndID(){ID = (int)InvoiceType.salesReturn, Name ="مردود مبيعات"},
               new ValueAndID(){ID = (int)SourceType.ExpencesItems, Name ="بند مصروفات"},
               new ValueAndID(){ID = (int)SourceType.CashNoteIN, Name ="سند قبض"},
               new ValueAndID(){ID = (int)SourceType.CashNoteOut, Name ="سند دفع"},
        };
        public static void InitializeData(this LookUpEdit lkp, object datasource)
        {
          lkp.InitializeData( datasource, "ID", "Name");
        }
        public static void InitializeData(this LookUpEdit lkp, object datasource, string valuemember, string displaymember)
        {
            lkp.Properties.DataSource = datasource;
            lkp.Properties.ValueMember = valuemember;
            lkp.Properties.DisplayMember = displaymember;
            lkp.Properties.Columns.Clear(); 
            lkp.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo()
            {
                FieldName=displaymember,
                
            });
            lkp.Properties.ShowHeader = false;   
            //lkp.Properties.PopulateColumns();
            //lkp.Properties.Columns[valuemember].Visible = false;
           
        }
        public static void InitializeData(this GridLookUpEdit lkp, object datasource)
        {
            lkp.InitializeData(datasource, "ID", "Name");
        }
        public static void InitializeData(this GridLookUpEdit lkp, object datasource, string valuemember, string displaymember)
        {
            lkp.Properties.DataSource = datasource;
            lkp.Properties.ValueMember = valuemember;
            lkp.Properties.DisplayMember = displaymember;
        }
        public static void InitializeData(this RepositoryItemLookUpEditBase repo, object datasource,GridColumn column,GridControl grid)
        {
            repo.InitializeData(datasource, column,grid, "ID","Name");
        }
        public static void InitializeData(this RepositoryItemLookUpEditBase repo, object datasource,GridColumn column, GridControl grid, string valuemember, string displaymember)
        {
            if(repo == null)
                repo = new RepositoryItemLookUpEdit();
            repo.DataSource = datasource;
            repo.ValueMember = valuemember;
            repo.DisplayMember = displaymember;
            
           
            column.ColumnEdit = repo;
            repo.NullText = "";
            if(grid != null)    
                grid.RepositoryItems.Add(repo);
        }
        public static string GetNextNumberInSTring(string number)
        {
            if (number == string.Empty || number == null)
                return "1";
            string str1 = "";
            foreach (char c in number)
                str1 = char.IsDigit(c) ? str1 + c.ToString() : "";
                if (str1 == string.Empty)
                    return number + "1";
            string str2 = str1.Insert(0, "1");
            str2 = (Convert.ToInt32(str2) + 1).ToString();
            string str3 = str2[0] == '1' ? str2.Remove(0, 1) : str2.Remove(0, 1).Insert(0, "1");
            int index=number.LastIndexOf(str1);  
            number=number.Remove(index);
            number=number.Insert(index,str3);   
            return number;
        }
        public static Byte[] GetbyteFromImage(Image image)
        {
            if (image == null)
            {
                return null;
            }
            //تخزين الصوره في الذاكره في صوره array لتخزينها في الداتا بيز

            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return stream.ToArray();
                }
                catch
                {
                    return stream.ToArray();
                }
            };
        }
        public static Image GetimageFromByteArray(Byte[] data)
        {
            Image image = null;
            try
            {
                
                    using (MemoryStream stream = new MemoryStream(data, false))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                    if(stream.Length > 0)   
                        image = (Image)formatter.Deserialize(stream);
                        //image=Image.FromStream(stream);
                    }
            }
            catch
            {
                image = null;
            }
            return image;
        }
        // T , <T> ==> Generic type
        public static T FromByteArray<T>(byte[] data)
        {
            if(data == null) return default(T);
            BinaryFormatter formatter = new BinaryFormatter();
            using(MemoryStream stream = new MemoryStream(data))
            {
                return (T)formatter.Deserialize(stream);    
            };
        }
        public static byte[] ToByteArray<T> (T obj)
        {
            if(obj == null) return default(byte[]);
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                 formatter.Serialize(stream,obj);
                return stream.ToArray();
            };
           
        }
        public static byte[] GetPropertyValue(string propertyName , int profileID)
        {
            using(var db = new dbDataContext())
            {
                var prop=db.UserSettingsProfileProperties.SingleOrDefault(p => p.PropertyName == propertyName && p.ProfileID==profileID);
                if(prop == null) return null;
                return prop.PropertyValue.ToArray();
                
            }
        }
        public static string GetCallerName([CallerMemberName]string callername="")
        {

            //CallerMemberName ==> returns methodName of a line
            return callername;
        }

        public static bool IsConnectionAvailable()
        {
            //Or this way
            //using (var db = new dbDataContext())
            //{
            //    if (db.DatabaseExists()) return true;
            //    else return false;
            //}
            using (var db = new dbDataContext())
            {
                try
                {
                    db.Connection.Open();
                    db.Connection.Close();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
                //or u can use easily if( db.DatabaseExists())

            }

        }


    }
}
