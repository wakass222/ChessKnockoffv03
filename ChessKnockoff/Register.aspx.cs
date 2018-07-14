using ChessKnockoff.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static ChessKnockoff.Utilities;

namespace ChessKnockoff
{
    public partial class WebForm1 : ValidationPage
    {

        public void checkPassword(object source, ServerValidateEventArgs args)
        {
            //Pass on validation to the password validation function
            validatePassword(source, args, inpPassword.Value, inpRePassword.Value);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //If user is already logged in
            if (User.Identity.IsAuthenticated)
            {
                //Redirect them to the play page
                Response.Redirect("~/Play");
            }

            //Make the current link in the navbar active
            activateNav(this, "likRegister");

            //Hide errors as the viewstate is not saved
            altPassword.Visible = false;
            altEmailTaken.Visible = false;
            altUsernameTaken.Visible = false;
            altError.Visible = false;
        }

        protected async Task RegisterClick(object sender, EventArgs e)
        {
            //Check if controls in the group are all valid
            if (IsValid)
            {
                //Create manager
                ApplicationUserManager manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

                //Start the async task
                //Check if password is valid
                Task<IdentityResult> taskPassword = manager.PasswordValidator.ValidateAsync(inpPassword.Value);

                //Check if username is not taken
                Task<ApplicationUser> taskUsername = manager.FindByNameAsync(inpUsername.Value);

                //Check if email is valid
                Task<ApplicationUser> taskEmail = manager.FindByEmailAsync(inpEmail.Value);

                //Await each task before each check
                IdentityResult resultPassword = await taskPassword;

                //Check if the password can be used
                if (!resultPassword.Succeeded)
                {
                    //Show the error
                    altPassword.Visible = true;
                    //Also show the specific issue
                    altPassword.InnerText = resultPassword.Errors.FirstOrDefault<string>();
                }

                ApplicationUser resultUsername = await taskUsername;

                //Check if the user is not null
                if (resultUsername != null)
                {
                    //Show the error
                    altUsernameTaken.Visible = true;
                }

                ApplicationUser resultEmail = await taskEmail;

                if (resultEmail != null)
                {
                    //Show the error
                    altEmailTaken.Visible = true;
                }

                //If there are no errors
                if (resultEmail == null && resultPassword.Succeeded && resultUsername == null)
                {
                    //Create sign in manager
                    ApplicationSignInManager signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();

                    ApplicationUser user = new ApplicationUser() { UserName = inpUsername.Value, Email = inpEmail.Value };

                    IdentityResult result = manager.Create(user, inpPassword.Value);

                    //Check if it succeeded
                    if (result.Succeeded)
                    {
                        //Send email confirmation link
                        string code = manager.GenerateEmailConfirmationToken(user.Id);
                        string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);

                        try
                        {
                            //Try to send the email
                            manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");
                        }
                        catch (Exception)
                        {
                            //Email did not send so show error
                            altError.Visible = true;
                            //Delete the account since the email will never be able to get confirmed and play if the email never gets sent
                            manager.Delete(user);

                            //Don't redirect
                            return;
                        }

                        //Redirect them to the login page
                        Response.Redirect("~/Login?Registered=1");
                    }
                }
            }
        }
    }
}