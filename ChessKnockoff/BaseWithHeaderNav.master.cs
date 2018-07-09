using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class BaseWithHeaderNav : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //If the user is authenticated
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Display a logout link
                likLogInner.InnerText = "Logout";
                likLogInner.Attributes.Add("OnClick", "Logout_Click");

                //Display the name of the logged in user
                txtName.InnerText = HttpContext.Current.User.Identity.Name;
            }
            else
            {
                //Display a login link
                likLogInner.InnerText = "Login";
                likLogInner.Attributes.Add("href", "~/Login");
            }
        }
    }
}