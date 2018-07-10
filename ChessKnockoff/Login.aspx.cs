using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using static ChessKnockoff.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity.Owin;

namespace ChessKnockoff
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //If user is already logged in
            if (User.Identity.IsAuthenticated)
            {
                //Redirect them to the play page
                Response.Redirect("~/Play");
            }

            //Make the current link in the navbar active
            activateNav(this, "likLogin");

            //Hide the messages when first entering the page
            altAuthentication.Visible = false;
            altRegistered.Visible = false;
            altMustBeLoggedIn.Visible = false;
            altVerify.Visible = false;

            //If the user was redirected from another page that required to be logged in
            if (Request.QueryString["MustBeLoggedIn"] == "1")
            {
                //Display the message
                altMustBeLoggedIn.Visible = true;
            }

            //If the user just registered show them the success message and prompt them to login
            if (Request.QueryString["Registered"] == "1")
            {
                //Displat the message
                altRegistered.Visible = true;
            }

        }

        protected void LoginClick(object sender, EventArgs e)
        {
            //Validate the user password
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

            //Require the user to have a confirmed email before they can log on.
            var user = manager.FindByName(inpUsername.Value);

            //Check if a user by that name exists
            if (user != null)
            {
                //Check if their email has been confirmed
                if (!user.EmailConfirmed)
                {
                    //If it has not been confirmed show an error message
                    altVerify.Visible = true;
                }
                else
                {
                    //Try to log them in
                    var result = signinManager.PasswordSignIn(inpUsername.Value, inpPassword.Value, boxRememberCheck.Checked, false);

                    switch (result)
                    {
                        case SignInStatus.Success:
                            Response.Redirect(Request.QueryString["ReturnUrl"]);
                            break;
                        case SignInStatus.RequiresVerification:
                            //Show error message to verify
                            altVerify.Visible = true;
                            break;
                        case SignInStatus.Failure:
                            //Show error message that password and username are not correct
                            altAuthentication.Visible = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}