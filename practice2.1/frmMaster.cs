using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static practice2._1.Master;

namespace practice2._1
{
    public partial class frmMaster : XtraForm
    {
       public bool isnew= false;
        public frmMaster()
        {
            InitializeComponent();
        }
        public  void InsertUserLog(Actions action, int partid, string partname)
        {
            InsertUserLog(action, partid, partname, this.Name);
        }
        public static void InsertUserLog(Actions action, int partid, string partname,string screenname)
        {
            int screenID = 0;
            var screen = Screens.GetScreensProp.SingleOrDefault(x => x.ScreenName ==screenname);
            if (screen != null)
            {
                screenID = screen.ScreenID;
                using (var db = new dbDataContext())
                {
                    db.UserLogs.InsertOnSubmit(new UserLog()
                    {
                        ActionDate = DateTime.Now,
                        ActionType = (byte)action,
                        ScreenID = screenID,
                        UserID = Session.User.ID,
                        PartID = partid,
                        PartName = partname
                    });
                    db.SubmitChanges();
                }
            }
        }
    }
}
