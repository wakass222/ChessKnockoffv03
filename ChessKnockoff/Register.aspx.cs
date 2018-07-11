using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using ChessKnockoff.Models;
using static ChessKnockoff.Utilities;
using System.Text.RegularExpressions;

namespace ChessKnockoff
{
    public partial class WebForm2 : System.Web.UI.Page
    {

        private bool checkUsername()
        {
            Regex regexUsername = new Regex(@"^[a-zA-Z0-9_]*$");
            bool regexUsernameResult = regexUsername.IsMatch(inpUsernameRegister.Value);

            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var resultUsername = manager.FindByName(inpUsernameRegister.Value);

            //Check if the username is alphanumeric and the username does exists
            if (regexUsernameResult && resultUsername != null)
            {

            }
            else
            {
                return false;
            }

            /*
            if (resultUsername != null)
            {
                inpUsernameRegister.Attributes["class"] = "form-control is-valid";
                return true;
            }
            else
            {
                inpUsernameRegister.Attributes["class"] = "form-control is-invalid";

                fedUsername.Visible = true;
                fedUsername.InnerText = resultUsername.Errors.FirstOrDefault<string>();

                return false;
            }
            */
        }

        private bool checkEmail()
        {
            Regex regexEmail = new Regex(@"^(([^<>()\[\]\\.,;:\s@]+(\.[^<>()\[\]\\.,;:\s@]+)*)|(.+))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$",
               RegexOptions.IgnoreCase
               );
            bool regexEmailResult = regexEmail.IsMatch(inpEmailRegister.Value);

            if (regexEmailResult)
            {
                //inpEmailRegister.Attributes["class"] = "form-control is-valid";
                return true;
            }
            else
            {
                //inpEmailRegister.Attributes["class"] = "form-control is-invalid";
                return false;
            }
        }

        private bool checkPassword()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var resultPassword = manager.PasswordValidator.ValidateAsync(inpPasswordRegister.Value).Result;

            bool matchResult = inpPasswordRegister.Value == inpRePasswordRegister.Value;

            if (matchResult && resultPassword.Succeeded)
            {
                //inpPasswordRegister.Attributes["class"] = "form-control is-valid";
                //inpRePasswordRegister.Attributes["class"] = "form-control is-valid";
                return true;
            }
            else
            {
                //inpPasswordRegister.Attributes["class"] = "form-control is-invalid";
                //inpRePasswordRegister.Attributes["class"] = "form-control is-invalid";

                if (!matchResult)
                {
                    //fedPasswordHelpBlock.Visible = true;
                }

                if (!resultPassword.Succeeded)
                {
                    //fedPasswordHelpBlock.Visible = true;
                    //fedPasswordHelpBlock.InnerText = resultPassword.Errors.FirstOrDefault<string>();
                }
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

            //Hide the error messages
            fedPasswordHelpBlock.Visible = false;
        }

        protected void RegisterNewUser(object sender, EventArgs e)
        {
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

                    //Display the error message without escaping HTML enocoding
                    fedPasswordHelpBlock.Visible = true;
                    fedPasswordHelpBlock.InnerHtml = tempHolder;
                }
            }
        }
    }
}