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
        private bool checkUsername()
        {
            //Create username regex
            Regex regexUsername = new Regex(@"^[a-zA-Z0-9_]*$");
            bool regexUsernameResult = regexUsername.IsMatch(inpUsernameRegister.Value);

            //Create manager
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var resultUsername = manager.FindByName(inpUsernameRegister.Value);

            //Will only return true if the username is valid and has not beent taken
            if (regexUsernameResult)
            {
                if (resultUsername != null)
                {
                    //Only show the email is taken if the email passes the regex test since that is shown client side
                    altEmailTaken.Visible = true;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool checkEmail()
        {
            //Create manager
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var resultEmail = manager.FindByEmail(inpEmailRegister.Value);

            //Will only return true if the email has not been taken and is valid
            if (IsValidEmail(inpEmailRegister.Value))
            {
                //Only show the email is taken if the email passes the regex check since that is shown clientside
                if (resultEmail != null)
                {
                    altEmailTaken.Visible = true;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool checkPassword()
        {
            //Create manager
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //Check if password is valid
            var resultPassword = manager.PasswordValidator.ValidateAsync(inpPasswordRegister.Value).Result;

            //Check if passwords match
            bool matchResult = inpPasswordRegister.Value == inpRePasswordRegister.Value;

            //Only returns true if the password is valid against password rules and they both match
            if (matchResult)
            {
                if (!resultPassword.Succeeded)
                {
                    altPassword.Visible = true;
                    altPassword.InnerText = resultPassword.Errors.FirstOrDefault<string>();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
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
            altError.Visible = true;
            //Check they are all valid and show the appropiate messages
            if (checkUsername() & checkPassword() & checkEmail())
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
                var user = new ApplicationUser() { UserName = inpUsernameRegister.Value, Email = inpEmailRegister.Value };

                IdentityResult result = manager.Create(user, inpPasswordRegister.Value);

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
                    //Only one error is ever shown even if there is multiple errors
                    string tempHolder = result.Errors.FirstOrDefault<string>();

                    //Display that an error has occured
                    altError.Visible = true;
                }
            }
        }
    }
}