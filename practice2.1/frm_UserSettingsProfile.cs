using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace practice2._1
{
    public partial class frm_UserSettingsProfile : frmMaster
    {
        
        UserSettingsProfile profile;
        List<BaseEdit> editors;
        public frm_UserSettingsProfile()
        {
            InitializeComponent();
            accordionControl1.ElementClick += AccordionControl1_ElementClick;
            New();
            Getdata();
        }
        public frm_UserSettingsProfile(int id)
        {
            InitializeComponent();
            accordionControl1.ElementClick += AccordionControl1_ElementClick;
            using (var db = new dbDataContext())
            {
                profile = db.UserSettingsProfiles.Single(x=>x.ID==id) ;
                
            }
            txtName.Text = profile.Name;    
            Getdata();
        }

        private void AccordionControl1_ElementClick(object sender, DevExpress.XtraBars.Navigation.ElementClickEventArgs e)
        {
          var index=accordionControl1.Elements.IndexOf(e.Element);
            xtraTabControl1.SelectedTabPageIndex=index; 
        }
        void New()
        {
           
            profile= new UserSettingsProfile(); 
            txtName.Text = profile.Name;    
            isnew = true;
        }
        void Getdata()
        {
            editors= new List<BaseEdit>();
            UserSettingsTemplate settingsTemplate = new UserSettingsTemplate(profile.ID);
            accordionControl1.Elements.Clear();
            xtraTabControl1.TabPages.Clear();
            accordionControl1.AllowItemSelection = true;

            var catalog = settingsTemplate.GetType().GetProperties(); // prop must be public
            foreach (var item in catalog)
            {
                accordionControl1.Elements.Add(new DevExpress.XtraBars.Navigation.AccordionControlElement()
                {
                    Name = item.Name,
                    Text = UserSettingsTemplate.GetpropCaption(item.Name),
                    Style = DevExpress.XtraBars.Navigation.ElementStyle.Item,

                });
                var page = new DevExpress.XtraTab.XtraTabPage()
                {
                    Name = item.Name,
                    Text = UserSettingsTemplate.GetpropCaption(item.Name),
                };
                xtraTabControl1.TabPages.Add(page);
                LayoutControl lc = new LayoutControl();

                var properties = item.GetValue(settingsTemplate).GetType().GetProperties();
                foreach (var prop in properties)
                {
                    BaseEdit edit = UserSettingsTemplate.GetPropControl(prop.Name, prop.GetValue(item.GetValue(settingsTemplate)));
                    if (edit != null)
                    {
                        var layout = lc.AddItem(" ", edit);
                        layout.TextVisible = true;
                        layout.Text = UserSettingsTemplate.GetpropCaption(prop.Name);
                        layout.AppearanceItemCaption.FontSizeDelta = 2;
                        editors.Add(edit);
                    }

                }
                lc.Dock = DockStyle.Fill;
                page.Controls.Add(lc);

            }
        }
       public bool IsDataValid()
        {
           
            int number = 0;
            number += txtName.validateText() ? 0 : 1;
            editors.ForEach(e =>
            {
                if (e.GetType() == typeof(LookUpEdit)&& ((LookUpEdit)e).Properties.DataSource.GetType()!=typeof(List<Master.ValueAndID>))
                    number += ((LookUpEdit)e).IsEditValueValidAndNotZero() ? 0 : 1;
            });

            return (number == 0);
        }
        void Save()
        {
            if (frmMain.CheckActionPermission(this.Name, isnew ? Master.Actions.Add : Master.Actions.Edit))
            {
                if (IsDataValid())
                {
                    var db = new dbDataContext();
                    if (profile.ID == 0)
                    {
                        db.UserSettingsProfiles.InsertOnSubmit(profile);
                    }
                    else
                    {
                        db.UserSettingsProfiles.Attach(profile);
                    }
                    profile.Name = txtName.Text;
                    db.SubmitChanges();
                    db.UserSettingsProfileProperties.DeleteAllOnSubmit(db.UserSettingsProfileProperties.Where(x => x.ProfileID == profile.ID));
                    db.SubmitChanges();
                    editors.ForEach(e =>
                    {
                        db.UserSettingsProfileProperties.InsertOnSubmit(new UserSettingsProfileProperty()
                        {
                            ProfileID = profile.ID,
                            PropertyName = e.Name,
                            PropertyValue = Master.ToByteArray<object>(e.EditValue),
                        });
                    });

                    db.SubmitChanges();
                    MessageBox.Show("تم الحفظ بنجاااح");
                    InsertUserLog(isnew ? Master.Actions.Add : Master.Actions.Edit, profile.ID, profile.Name + "اعدادات مستخدم", this.Name);
                    isnew = false;
                }
            }
        }
        private void frm_UserSettingsProfile_Load(object sender, EventArgs e)
        {
            
            xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            xtraTabControl1.Transition.AllowTransition = DevExpress.Utils.DefaultBoolean.True;
            xtraTabControl1.Transition.EasingMode = DevExpress.Data.Utils.EasingMode.EaseInOut;
            accordionControl1.OptionsMinimizing.AllowMinimizeMode = DevExpress.Utils.DefaultBoolean.False;
        }

        private void btnSavee_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        { 
                Save();
        }

        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            New();
        }
    }
}
