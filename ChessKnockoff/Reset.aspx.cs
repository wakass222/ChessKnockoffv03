using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        private bool checkPassword()
        {
            //Create manager
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //Check if password is valid
            var resultPassword = manager.PasswordValidator.ValidateAsync(inpPasswordReset.Value).Result;

            //Check if passwords match
            bool matchResult = inpPasswordReset.Value == inpRePasswordReset.Value;

            //Only returns true if the password is valid against password rules and they both match
            if (matchResult)
            {
                if (!resultPassword.Succeeded)
                {
                    //Show the error
                    altError.Visible = true;
                    altError.InnerText = resultPassword.Errors.FirstOrDefault<string>();
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

        protected void ResetPassword(object sender, EventArgs e)
        {
            //Create manager object
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //Get the reset code
            string code = IdentityHelper.GetCodeFromRequest(Request);

            //Get the user ID
            string userID = IdentityHelper.GetUserIdFromRequest(Request);

            //Check if the user exists
            if (checkPassword())
            {
                //Change their password
                var result = manager.ResetPassword(userID, code, inpPasswordReset.Value);

                //Check if it was successful
                if (result.Succeeded)
                {
                    //Redirect to the login page and show the success message
                    Response.Redirect("~/Login?ResetPassword=1");
                }
                else
                {
                    //Write to the debug log something has occured
                    System.Diagnostics.Debug.WriteLine(result.Errors.FirstOrDefault<string>());
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the error message
            altError.Visible = false;

            //Get the reset code
            string code = IdentityHelper.GetCodeFromRequest(Request);

            //Get the user ID
            string userID = IdentityHelper.GetUserIdFromRequest(Request);

            //Check if the code and userID is sent
            if (code == null && userID != "")
            {
                //Redirect them to the play game but if they arent logged in, they are redirected to the login page
                Response.Redirect("~/Play");
            }
        }
    }
}