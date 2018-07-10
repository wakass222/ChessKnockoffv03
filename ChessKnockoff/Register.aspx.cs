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
            if (inpPasswordRegister.Text == inpRePasswordRegister.Text && IsValidEmail(inpEmailRegister.Text))
            {
                //Creates database connection
                UserStore<IdentityUser> userStore = new UserStore<IdentityUser>();
                //Pass database connection to the UserManager
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);

                //Create the new user in the database and escape HTML tags so users may have html tags in their names
                IdentityUser user = new IdentityUser() { UserName = WebUtility.HtmlEncode(inpUsernameRegister.Text) }; 
                IdentityResult result = manager.Create(user, inpPasswordRegister.Text);

                //Check if it succeeded
                if (result.Succeeded)
                {
                    Response.Redirect("~/Login?Registered=1");
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