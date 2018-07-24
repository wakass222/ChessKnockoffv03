using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    /// <summary>
    /// Class for the account page
    /// </summary>
    public partial class AccountForm : LoginRequiredPage
    {
        /// <summary>
        /// Called to server validate by the custom validator. Should not be called directly.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void checkPassword(object source, ServerValidateEventArgs args)
        {
            //Pass on password validation to another function
            checkPasswordMarch(source, args, inpPassword.Value, inpRePassword.Value);
        }

        /// <summary>
        /// Called when the change password button is pressed. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ChangePassword(object sender, EventArgs e)
        {
            if (IsValid)
            {
                //Check the password against the rule
                string validPasswordResult = validatePassword(inpPassword.Value);

                //Show an error if it is not correct
                if (validPasswordResult != "")
                {
                    altError.Visible = true;
                    altError.InnerText = validPasswordResult;
                }

                //Create a byte array to store the salt
                byte[] newSalt = new byte[20];
                //Fill the array with the salt
                fillByteRandom(newSalt);

                //Hash the new password with the salt
                byte[] newSaltedHash = generateSaltedHash(inpPassword.Value, newSalt);

                //Hash the user's current password


                //Updates the user's password
                string queryString = "UPDATE Player SET Password=@Password, Salt=@Salt WHERE Username=@Username AND Password=@oldPassword";

                //Create the database connection and command then dispose when done
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    //Open the database connection
                    connection.Open();

                    //Add the parameters
                    command.Parameters.AddWithValue("@Salt", newSalt);
                    command.Parameters.AddWithValue("@Password", newSaltedHash);
                    command.Parameters.AddWithValue("@Username", Session["Username"]);
                    command.Parameters.AddWithValue("@oldPassword", oldSaltedHash);
                }

                //Check if it was successful
                if (result.Succeeded)
                {
                    //Show that is was successful
                    altSuccess.Visible = true;
                }
                else
                {
                    //Show the error message
                    altError.Visible = true;
                    //Show the password error
                    altError.InnerText = result.Errors.FirstOrDefault<string>();
                }
            }
        }

        /// <summary>
        /// Called when the page loads. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Activate the current link
            activateNav("likAccount");

            //Hide the error message
            altError.Visible = false;
            altSuccess.Visible = false;
        }
    }
}