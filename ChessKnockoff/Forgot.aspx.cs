using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static ChessKnockoff.Utilities;

namespace ChessKnockoff
{
    public partial class WebForm7 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the success message as the viewstate is not saved
            altEmailSent.Visible = false;
            altEmailFail.Visible = false;

            //If user is already logged in
            if (User.Identity.IsAuthenticated)
            {
                //Redirect them to the play page
                Response.Redirect("~/Play");
            }
        }

        protected void EmailClick(object sender, EventArgs e)
        {
            //Create manager object
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //Look for user by that email
            var user = manager.FindByEmail(inpEmailReset.Value);

            //Check if a user by that email exists
            if (user != null)
            {
                //Send reset link
                string code = manager.GeneratePasswordResetToken(user.Id);
                string callbackUrl = IdentityHelper.GetResetPasswordRedirectUrl(user.UserName, code, Request);
                manager.SendEmail(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.");

                //Show that it was successful
                altEmailSent.Visible = true;
            }
            else
            {
                //Show that it was unsuccessful
                altEmailFail.Visible = true;
            }
        }
    }
}