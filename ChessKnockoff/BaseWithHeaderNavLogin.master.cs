using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class BaseWithScripts_MustBeLoggedIn : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            {
                //If the user is not authenticated
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    //then redirect to the login page
                    Response.Redirect("~/Login?MustBeLoggedIn=1");
                }
            }
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            //When the logout button is pressed clear cookies
            FormsAuthentication.SignOut();
            //redirect to the login page
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}