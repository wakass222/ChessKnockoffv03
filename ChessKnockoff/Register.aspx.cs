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

            //Hide the error message
            fedPasswordHelpBlock.Visible = false;
        }

        protected void RegisterNewUser(object sender, EventArgs e)
        {
            //Server sided validation
            //Create Regex object for email validation
            Regex regexEmail = new Regex(@"^(([^<>()\[\]\\.,;:\s@]+(\.[^<>()\[\]\\.,;:\s@]+)*)|(.+))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");
            bool regexEmailResult = regexEmail.IsMatch(inpEmailRegister.Text);

            //Validate password
            bool matchResult = inpPasswordRegister.Text == inpRePasswordRegister.Text;

            //Check they are all valid
            if (matchResult && regexEmailResult)
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