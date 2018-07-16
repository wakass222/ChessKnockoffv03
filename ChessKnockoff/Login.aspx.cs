using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChessKnockoff.Models;

namespace ChessKnockoff
{
    public partial class WebForm4 : ExtendedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //If user is already logged in
            if (User.Identity.IsAuthenticated)
            {
                //Redirect them to the play page
                if (this.Request.QueryString["ReturnUrl"] != null)
                {
                    this.Response.Redirect(Request.QueryString["ReturnUrl"].ToString());
                }
                else
                {
                    this.Response.Redirect("/Play");
                }
            }

            //Make the current link in the navbar active
            activateNav( "likLogin");

            //Hide the messages when first entering the page and for other request as the viewstate is not saved
            altAuthentication.Visible = false;
            altRegistered.Visible = false;
            altMustBeLoggedIn.Visible = false;
            altVerify.Visible = false;
            altEmailConfirm.Visible = false;
            altResetPassword.Visible = false;
            altLockout.Visible = false;

            //Get the confirmation code and ID
            string confirmationCode = IdentityHelper.GetCodeFromRequest(Request);
            string confirmationUserId = IdentityHelper.GetUserIdFromRequest(Request);

            //Check they both exist
            if (confirmationCode != null && confirmationUserId != null)
            {
                //Try to confirm them
                ApplicationUserManager manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                IdentityResult result = manager.ConfirmEmail(confirmationUserId, confirmationCode);
               
                //If it succeeded them confirm
                if (result.Succeeded)
                {
                    altEmailConfirm.Visible = true;
                }
            }

            //If the user was redirected from another page that required to be logged in
            if (Request.QueryString["ReturnUrl"] != null)
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

            if (Request.QueryString["ResetPassword"] == "1")
            {
                //Displat the message
                altResetPassword.Visible = true;
            }
        }

        protected void LoginClick(object sender, EventArgs e)
        {
            if (IsValid)
            {
                //Validate the user password
                ApplicationUserManager manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationSignInManager signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

                //Require the user to have a confirmed email before they can log on.
                ApplicationUser user = manager.FindByName(inpUsername.Value);

                //Check if a user by that name exists
                if (user != null)
                {
                    //Check if their email has been confirmed
                    if (!user.EmailConfirmed)
                    {
                        //Send email confirmation link
                        string code = manager.GenerateEmailConfirmationToken(user.Id);
                        string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
                        manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");

                        //If it has not been confirmed show an error message
                        altVerify.Visible = true;
                    }
                    else
                    {
                        //Try to log them in
                        SignInStatus result = signinManager.PasswordSignIn(inpUsername.Value, inpPassword.Value, boxRememberCheck.Checked, true);

                        //Check the result of sign in status to the possible errors
                        if (result == SignInStatus.Success)
                        {
                            //If there is return url then redirect them to it
                            if (Request.QueryString["ReturnUrl"] != null)
                            {
                                Response.Redirect(Request.QueryString["ReturnUrl"]);
                            }
                            else
                            {
                                //If it does not exist redirect them to the play page
                                Response.Redirect("~/Play");
                            }
                        }
                        else if(result == SignInStatus.RequiresVerification)
                        {
                            //Show that they need to verify their email
                            altVerify.Visible = true;
                        }
                        else if(result == SignInStatus.Failure)
                        {
                            //Show that the password/username combination was incorrect
                            altAuthentication.Visible = true;
                        }
                        else if(result == SignInStatus.LockedOut)
                        {
                            //Show that they have been locked out
                            altLockout.Visible = true;
                            altLockout.InnerText = string.Format("Your account has been locked for {0} minutes after {1} failed attempts. Please reset your password to prematurely end the lockout.",
                                manager.DefaultAccountLockoutTimeSpan.Minutes, manager.MaxFailedAccessAttemptsBeforeLockout);
                        }
                    }
                }
                else
                {
                    //Show error message even if the username does not exist
                    //Does not reveal whether that username exists
                    altAuthentication.Visible = true;
                }
            }
        }
    }
}