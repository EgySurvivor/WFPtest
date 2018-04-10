using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WFPtest.ASPX
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected string ReturnText(object val)
        {
            if (val != null)
            {

                if (val.ToString().Equals("37410")) { return "IN"; }
                else if (val.ToString().Equals("37372")) { return "OUT"; }
                else if (val.ToString().Equals("538616732")) { return "IN"; }
                else if (val.ToString().Equals("538616742")) { return "OUT"; }
            }
            return "";
        }

        protected string ReturnTextOffice(object val)
        {
            if (val != null)
            {

                if (val.ToString().Equals("37410")) { return "Office1"; }
                else if (val.ToString().Equals("37372")) { return "Office1"; }
                else if (val.ToString().Equals("538616732")) { return "Office2"; }
                else if (val.ToString().Equals("538616742")) { return "Office2"; }
            }
            return "";
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}