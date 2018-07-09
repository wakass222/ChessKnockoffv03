using System;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using static ChessKnockoff.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        private bool checkPasswordMatch()
        {
            //Then check if they match
            if (inpPasswordRegister.Text != inpRePasswordRegister.Text)
            {
                //Makes the input appear invalid
                inpPasswordRegister.Attributes["class"] += " is-invalid";
                inpRePasswordRegister.Attributes["class"] += " is-invalid";

                //Return false to signify they do not match
                return false;
            } else
            {
                //Return true to signify they do match
                return true;
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
            activateNav(this, "likLog");

            //Hide the error message
            fedPasswordHelpBlock.Visible = false;

            //Reset the class attribute to their default
            inpPasswordRegister.Attributes["class"] = "form-control";
            inpRePasswordRegister.Attributes["class"] = "form-control";

            inpUsernameRegister.Attributes["class"] = "form-control";

            //Check if any of the password fields have been edited
            if (Page.IsPostBack)
            {
                //Check the passwords match
                checkPasswordMatch();
            }
        }

        protected void RegisterNewUser(object sender, EventArgs e)
        {
            //If both passwords match then continue
            if(checkPasswordMatch())
            {
                //Creates database connection
                UserStore<IdentityUser> userStore = new UserStore<IdentityUser>();
                //Pass database connection to the UserManager
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);

                //Create the new user in the database and escape HTML tags
                IdentityUser user = new IdentityUser() { UserName = WebUtility.HtmlEncode(inpUsernameRegister.Text) }; 
                IdentityResult result = manager.Create(user, inpPasswordRegister.Text);

                //Check if it succeeded
                if (result.Succeeded)
                {
                    Response.Redirect("~/Login.aspx?Registered=1");
                }
                else //An error occured
                {
                    //Will hold the final html string
                    string tempHolder = "";
                    foreach (string error in result.Errors)
                    {
                        //Add a space in before a capital ignoring the first one
                        tempHolder += Regex.Replace(error, "([a-z])([A-Z])", "$1 $2");
                        tempHolder += "<br />";
                    }

                    //Display the error message without escaping HTML enocoding
                    fedPasswordHelpBlock.Visible = true;
                    fedPasswordHelpBlock.InnerHtml = tempHolder;
                }
            }
        }
    }
}