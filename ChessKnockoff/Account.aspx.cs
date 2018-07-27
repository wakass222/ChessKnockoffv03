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
                    altPasswordFeedback.Visible = true;
                    altPasswordFeedback.InnerText = validPasswordResult;
                }
                else
                {
                    string queryString = "SELECT * FROM Player WHERE Username=@Username";

                    //Hash the user's current password

                    //Create the database connection and command then dispose when done
                    using (SqlConnection connectionSelect = new SqlConnection(dbConnectionString))
                    using (SqlCommand commandSelect = new SqlCommand(queryString, connectionSelect))
                    {
                        //Open the database connection
                        connectionSelect.Open();

                        //Add the parameters
                        commandSelect.Parameters.AddWithValue("@Username", Context.User.Identity.Name);

                        using (SqlDataReader reader = commandSelect.ExecuteReader())
                        {
                            //User should exist in order to be able to land on this page
                            reader.Read();

                            //Get the values from the query
                            byte[] oldHash = (byte[])reader["Password"];
                            byte[] oldSalt = (byte[])reader["Salt"];

                            //Create a byte array to store the salt
                            byte[] newSalt = new byte[20];
                            //Fill the array with the salt
                            fillByteRandom(newSalt);

                            //Hash the new password with the salt
                            byte[] newSaltedHash = generateSaltedHash(inpPassword.Value, newSalt);

                            //Check if they match
                            if (generateSaltedHash(inpCurrentPassword.Value, oldSalt).SequenceEqual(oldHash))
                            {
                                //Updates the user's password
                                queryString = "UPDATE Player SET Password=@Password, Salt=@Salt WHERE Username=@Username";

                                //Stores how many rows were affected
                                int rowsAffected;

                                //Create the database connection and command then dispose when done
                                using (SqlConnection connectionUpdate = new SqlConnection(dbConnectionString))
                                using (SqlCommand commandUpdate = new SqlCommand(queryString, connectionUpdate))
                                {
                                    //Open the database connection
                                    connectionUpdate.Open();

                                    //Add the parameters
                                    commandUpdate.Parameters.AddWithValue("@Salt", newSalt);
                                    commandUpdate.Parameters.AddWithValue("@Password", newSaltedHash);
                                    commandUpdate.Parameters.AddWithValue("@Username", Context.User.Identity.Name);

                                    ///Execute the command
                                    rowsAffected = commandUpdate.ExecuteNonQuery();
                                }
                                //Show that it succeeded
                                altSuccess.Visible = true;
                            }
                            else
                            {
                                //Show that it did not work
                                altCurrent.Visible = true;
                            }
                        }
                    }
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

            //Hide the messages
            altPasswordFeedback.Visible = false;
            altCurrent.Visible = false;
            altSuccess.Visible = false;
        }
    }
}