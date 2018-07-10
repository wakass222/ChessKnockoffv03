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
                //Display a logout link and hide the login links
                navLogin.Visible = false;
                navLogout.Visible = true;

                //Display the name of the logged in user
                txtName.InnerText = HttpContext.Current.User.Identity.Name;
            }
            else
            {
                //Display the login link and hide the logout link
                navLogin.Visible = true;
                navLogout.Visible = false;
            }
        }
    }
}