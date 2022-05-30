using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practice2._1
{
    public class frmCashNoteIn:frmCashNote
    {
        public  frmCashNoteIn():base(true)
        {

            this.Text = "سند قبض";
        }
      

    }
    public class frmCashNoteOut : frmCashNote
    {
        public frmCashNoteOut() : base(false)
        {
            this.Text = "سند دفع";

        }


    }
}
