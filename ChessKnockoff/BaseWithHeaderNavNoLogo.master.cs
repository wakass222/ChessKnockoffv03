using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace ChessKnockoff
{
    /// <summary>
    /// Class for the BaseWithHeaderNavNoLogo master page
    /// </summary>
    public partial class BaseWithHeaderNavNoLogo : System.Web.UI.MasterPage
    {
        /// <summary>
        /// Called when the log out button is pressed. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Logout_Click(object sender, EventArgs e)
        {
            //Remove the authentication cookie
            FormsAuthentication.SignOut();

            //Redirect to the login page
            Response.Redirect("~/Login");
        }

        /// <summary>
        /// Called when the page loads. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //If the user is authenticated
            if (Context.User.Identity.IsAuthenticated)
            {
                //Display a logout link and hide the login links
                navLogin.Visible = false;
                navLogout.Visible = true;

                //Display the name of the logged in user and escaping any tags
                txtInfo.InnerText = string.Format(HttpUtility.HtmlEncode(Context.User.Identity.Name));
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