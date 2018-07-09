using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using static ChessKnockoff.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class WebForm1 : System.Web.UI.Page
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
            activateNav(this, "likLog");

            //Hide the messages when first entering the page
            altAuthentication.Visible = false;
            altRegistered.Visible = false;
            altMustBeLoggedIn.Visible = false;

            //If the user was redirected from another page that required to be logged in
            if (Request.QueryString["MustBeLoggedIn"] == "1")
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

        }
        protected void LoginClick(object sender, EventArgs e)
        {
            //Creates database connection
            var userStore = new UserStore<IdentityUser>();
            //Pass database connection to the UserManager
            var userManager = new UserManager<IdentityUser>(userStore);

            //Looks for credentials
            var user = userManager.Find(inpUsername.Value, inpPassword.Value);

            //Authentication was successful
            if (user != null)
            {
                //Creates an interface for OWIN
                var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                //Pass the OWIN interface to the UserManager
                var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                //Only allow persistent cookie if remember me was checked
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = boxMeCheck.Checked }, userIdentity);

                //Redirect to the Play page
                Response.Redirect("~/Play");
            } else //Authentication was no successful
            {
                //Show an error message
                altAuthentication.Visible = true;
            }
        }
    }
}