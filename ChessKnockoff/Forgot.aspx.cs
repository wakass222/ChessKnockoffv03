using ChessKnockoff.Models;
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
    public partial class WebForm7 : ValidationPage
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
            //Check if the inputs are valid
            if (IsValid)
            {
                //Create manager object
                ApplicationUserManager manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //Look for user by that email
                ApplicationUser user = manager.FindByEmail(inpEmail.Value);

                //Check if a user by that email exists and has their email confirmed
                if (user != null && user.EmailConfirmed)
                {
                    //Generate the reset code
                    string code = manager.GeneratePasswordResetToken(user.Id);

                    //Create the link with the reset code
                    string callbackUrl = IdentityHelper.GetResetPasswordRedirectUrl(user.Id, code, Request);
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
}