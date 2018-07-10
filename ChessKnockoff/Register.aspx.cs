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
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var resultUsername = manager.UserValidator.ValidateAsync(new ApplicationUser() { UserName = inpUsernameRegister.Text, Email = "test@test.com"}).Result;

            if (resultUsername.Succeeded)
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
        }

        private bool checkEmail()
        {
            Regex regexEmail = new Regex(@"^(([^<>()\[\]\\.,;:\s@]+(\.[^<>()\[\]\\.,;:\s@]+)*)|(.+))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$",
                RegexOptions.IgnoreCase
                );
            bool regexEmailResult = regexEmail.IsMatch(inpEmailRegister.Text);

            if (regexEmailResult)
            {
                inpEmailRegister.Attributes["class"] = "form-control is-valid";
                return true;
            }
            else
            {
                inpEmailRegister.Attributes["class"] = "form-control is-invalid";
                return false;
            }
        }

        private bool checkPassword()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var resultPassword = manager.PasswordValidator.ValidateAsync(inpPasswordRegister.Text).Result;

            bool matchResult = inpPasswordRegister.Text == inpRePasswordRegister.Text;

            if (matchResult && resultPassword.Succeeded)
            {
                inpPasswordRegister.Attributes["class"] = "form-control is-valid";
                inpRePasswordRegister.Attributes["class"] = "form-control is-valid";
                return true;
            }
            else
            {
                inpPasswordRegister.Attributes["class"] = "form-control is-invalid";
                inpRePasswordRegister.Attributes["class"] = "form-control is-invalid";

                if (!matchResult)
                {
                    fedPasswordHelpBlock.Visible = true;
                }

                if (!resultPassword.Succeeded)
                {
                    fedPasswordHelpBlock.Visible = true;
                    fedPasswordHelpBlock.InnerText = resultPassword.Errors.FirstOrDefault<string>();
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

            //Set them to their defaults class
            /*
            inpEmailRegister.Attributes["class"] = "form-control";
            inpPasswordRegister.Attributes["class"] = "form-control";
            inpRePasswordRegister.Attributes["class"] = "form-control";
            inpUsernameRegister.Attributes["class"] = "form-control";
            */

            if (Page.IsPostBack)
            {
                switch (this.Request.Params.Get("__EVENTTARGET"))
                {
                    case "inpUsernameRegister":
                        checkUsername();
                        break;
                    case "inpEmailRegister":
                        checkEmail();
                        break;
                    case "inpPasswordRegister":
                        checkPassword();
                        break;
                }
            }
        }

        protected void RegisterNewUser(object sender, EventArgs e)
        {
            //Check they are all valid
            if (checkUsername() && checkPassword() && checkEmail())
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
                var user = new ApplicationUser() { UserName = inpUsernameRegister.Text, Email = inpEmailRegister.Text };
                
                IdentityResult result = manager.Create(user, inpPasswordRegister.Text);

                //Check if it succeeded
                if (result.Succeeded)
                {
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