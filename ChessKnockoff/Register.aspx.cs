using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Net;
using System.Text.RegularExpressions;
using static ChessKnockoff.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChessKnockoff.Models;
using Microsoft.AspNet.Identity.Owin;

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

            //Validate username so that two error messages are not shown
            Regex regexUsername = new Regex(@"^[a-z0-9]+$", RegexOptions.IgnoreCase);
            bool regexUsernameResult = regexUsername.IsMatch(inpUsernameRegister.Text);

            //Validate password
            bool matchResult = inpPasswordRegister.Text == inpRePasswordRegister.Text;

            //Check they are all valid
            if (matchResult && regexEmailResult && regexUsernameResult)
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
                    //The identity framework can implement various password rules which passwords can break more than one of
                    //this allows various errors so be displayed without using a Jquery plugin to display each rule
                    string tempHolder = Regex.Replace(result.Errors.FirstOrDefault<string>(), "([a-z])([A-Z])", "$1 $2") + "<br>";

                    //Display the error message without escaping HTML enocoding
                    fedPasswordHelpBlock.Visible = true;
                    fedPasswordHelpBlock.InnerHtml = tempHolder;
                }
            }
        }
    }
}