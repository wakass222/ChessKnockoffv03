using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChessKnockoff.Models;

namespace ChessKnockoff
{
    public partial class BaseWithHeaderNavNoLogo : System.Web.UI.MasterPage
    {
        protected void Logout_Click(object sender, EventArgs e)
        {
            //Remove the cookie
            Context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //Redirect to the login page
            Response.Redirect("~/Login");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //If the user is authenticated
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Display a logout link and hide the login links
                navLogin.Visible = false;
                navLogout.Visible = true;

                //Create manager
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

                //Return the current user along with their other information
                var user = manager.FindById(HttpContext.Current.User.Identity.GetUserId());

                //Display the name of the logged in user and escaping any tags
                txtInfo.InnerText = string.Format(HttpUtility.HtmlEncode(HttpContext.Current.User.Identity.Name));
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