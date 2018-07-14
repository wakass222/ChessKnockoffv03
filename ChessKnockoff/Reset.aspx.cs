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
    public partial class WebForm2 :  ValidationPage
    {
        protected void checkPassword(object source, ServerValidateEventArgs args)
        {
            //Pass on password validation to another function
            validatePassword(source, args, inpPassword.Value, inpRePassword.Value);
        }

        protected void ResetPassword(object sender, EventArgs e)
        {
            //Check if the inputs are valid in this case if both passwords match
            if (IsValid)
            {
                //Create manager object
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

                //Check if the password is valid
                var resultPassword = manager.PasswordValidator.ValidateAsync(inpPassword.Value).Result;

                //Check if the password is valid
                if (!resultPassword.Succeeded)
                {
                    //Show the error message
                    altError.Visible = true;
                    //Show the specific error
                    altError.InnerText = resultPassword.Errors.FirstOrDefault<string>();
                }

                //Check if the password is valid
                if (resultPassword.Succeeded)
                {
                    //Get the reset code
                    string code = IdentityHelper.GetCodeFromRequest(Request);

                    //Get the user ID
                    string userID = IdentityHelper.GetUserIdFromRequest(Request);

                    //Change their password
                    var result = manager.ResetPassword(userID, code, inpPassword.Value);

                    //Check if it was successful
                    if (result.Succeeded)
                    {
                        //Redirect to the login page and show the success message
                        Response.Redirect("~/Login?ResetPassword=1");
                    }
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