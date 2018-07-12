using ChessKnockoff.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static ChessKnockoff.Utilities;

namespace ChessKnockoff
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        /// <summary>
        /// Check if the username is valid and not taken
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void checkUsername(object source, ServerValidateEventArgs args)
        {
            //Create username regex
            Regex regexUsername = new Regex(@"^[a-zA-Z0-9_]*$");
            bool regexUsernameResult = regexUsername.IsMatch(inpUsernameRegister.Text);

            //Create manager
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var resultUsername = manager.FindByName(inpUsernameRegister.Text);

            //Will only return true if the username is valid and has not beent taken
            if (regexUsernameResult)
            {
                if (resultUsername != null)
                {
                    //Only show the email is taken if the email passes the regex test since that is shown client side
                    altUsernameTaken.Visible = true;
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = true;
                }
            }
            else
            {
                args.IsValid = false;
            }
        }

        /// <summary>
        /// Checks if the email is valid and not taken
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void checkEmail(object source, ServerValidateEventArgs args)
        {
            //Create manager
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var resultEmail = manager.FindByEmail(args.Value);

            //Will only return true if the email has not been taken and is valid
            if (IsValidEmail(inpEmailRegister.Text))
            {
                //Only show the email is taken if the email passes the regex check since that is shown clientside
                if (resultEmail != null)
                {
                    altEmailTaken.Visible = true;
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = true;
                }
            }
            else
            {
                args.IsValid = false;
            }
        }

        /// <summary>
        /// Checks if the password is valid
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void checkPassword(object source, ServerValidateEventArgs args)
        {
            //Create manager
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //Check if password is valid
            var resultPassword = manager.PasswordValidator.ValidateAsync(args.Value).Result;

            //Check if passwords match
            bool matchResult = inpPasswordRegister.Text == inpRePasswordRegister.Text;

            //Only returns true if the password is valid against password rules and they both match
            if (matchResult)
            {
                if (!resultPassword.Succeeded)
                {
                    altPassword.Visible = true;
                    altPassword.InnerText = resultPassword.Errors.FirstOrDefault<string>();
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = true;
                }
            }
            else
            {
                args.IsValid = false;
            }
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

        protected void RegisterNewUser(object sender, EventArgs e)
        {
            //Check if controls in the group are all valid
            if (IsValid)
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
                var user = new ApplicationUser() { UserName = inpUsernameRegister.Text, Email = inpEmailRegister.Text };

                IdentityResult result = manager.Create(user, inpPasswordRegister.Text);

                //Check if it succeeded
                if (result.Succeeded)
                {
                    //Send email confirmation link
                    string code = manager.GenerateEmailConfirmationToken(user.Id);
                    string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
                    manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");

                    //Redirect them to the login page
                    Response.Redirect("~/Login?Registered=1");
                }
                else
                {
                    //Write to the debug log something has occured
                    System.Diagnostics.Debug.WriteLine(result.Errors.FirstOrDefault<string>());
                }
            }
        }
    }
}