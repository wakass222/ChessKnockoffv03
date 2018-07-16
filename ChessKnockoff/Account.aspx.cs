using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class WebForm3 : LoginRequiredPage
    {
        protected void checkPassword(object source, ServerValidateEventArgs args)
        {
            //Pass on password validation to another function
            validatePassword(source, args, inpPassword.Value, inpRePassword.Value);
        }

        protected void ChangePassword(object sender, EventArgs e)
        {
            if (IsValid)
            {
                //Create the Owin object
                ApplicationUserManager manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //Create the user manager object
                ApplicationSignInManager signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
                IdentityResult result = manager.ChangePassword(User.Identity.GetUserId(), inpCurrentPassword.Value, inpPassword.Value);

                //Check if it was successful
                if (result.Succeeded)
                {
                    //Show that is was successful
                    altSuccess.Visible = true;
                }
                else
                {
                    //Show the error message
                    altError.Visible = true;
                    //Show the password error
                    altError.InnerText = result.Errors.FirstOrDefault<string>();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Activate the current link
            activateNav("likAccount");

            //Hide the error message
            altError.Visible = false;
            altSuccess.Visible = false;
        }
    }
}