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
    /// Class for the forgot page
    /// </summary>
    public partial class ForgotForm : ExtendedPage
    {
        /// <summary>
        /// Checks whether the user has their email confirmed or not
        /// </summary>
        /// <param name="email">The email of the player</param>
        /// <returns>Returns true if their email is confirmed or false, returns null if user does not exist</returns>
        public bool? isEmailConfirmedFromEmail(string email)
        {
            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Stores the query string
                string queryString = "SELECT * FROM Player WHERE Email=@Email";

                //Create the query string in the sqlCommand format
                SqlCommand command = new SqlCommand(queryString, connection);

                //Add the parameters to the query
                command.Parameters.AddWithValue("@Email", email);

                //Execute the command and store the result
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    //If there were rows matching it
                    if (reader.HasRows)
                    {
                        //Read the first row
                        reader.Read();

                        //Return the result
                        return (bool)reader["EmailIsConfirmed"];
                    }

                    //Return null if no user was found
                    return null;
                }
            }
        }

        /// <summary>
        /// Called on page load. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the success message as the viewstate is not saved
            altEmailSent.Visible = false;
            altEmailFail.Visible = false;

            //If user is already logged in
            if (isAuthenticated())
            {
                //Redirect them to the play page
                Response.Redirect("~/Play");
            }
        }

        /// <summary>
        /// Called when the submit button is pressed. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EmailClick(object sender, EventArgs e)
        {
            //Check if the inputs are valid
            if (IsValid)
            {
                //If the email is confirmed
                if (isEmailConfirmedFromEmail(inpEmail.Value) == true)
                {
                    //Holds the random bytes
                    byte[] randomToken = new byte[32];
                    //FIll it with random bytes
                    fillByteRandom(randomToken);

                    //Create a URI builder
                    UriBuilder builder = new UriBuilder();
                    //Set the host
                    builder.Host = Request.Url.Host;
                    //Set the port
                    builder.Port = Request.Url.Port;
                    //Set the path
                    builder.Path = "/Reset/";
                    //Set the query parameter
                    builder.Query += "ResetToken=" + encodeToString(randomToken);

                    //Send the email
                    sendEmail(inpEmail.Value, "Reset password", "Please reset your password by clicking <a href=\"" + builder.ToString() + "\">here</a>.");

                    //Show that it was successful
                    altEmailSent.Visible = true;

                    //Create the database connection then dispose when done
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        //Open the database connection
                        connection.Open();

                        //Stores the query string
                        string queryString = "SELECT * FROM Player WHERE Email=@Email";

                        //Create a command object
                        SqlCommand command = new SqlCommand(queryString, connection);

                        //Add the parameters
                        command.Parameters.AddWithValue("@Email", inpEmail.Value);

                        //Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();

                            //Make sure to cast it to a string
                            string username = reader["Username"].ToString();

                            //Create a new query string
                            queryString = "INSERT INTO Reset (Username, ConfirmationToken, ExpirationDateUTC) VALUES (@Username, @ConfirmationToken, @ExpirationDateUTC)";

                            //Create the query string in the sqlCommand format
                            command = new SqlCommand(queryString, connection);

                            //Create the expiration date on the token
                            DateTime currentDate = DateTime.UtcNow;
                            TimeSpan expirationTime = new TimeSpan(2, 0, 0);

                            //Add the query parameters
                            command.Parameters.AddWithValue("@Username", username);
                            command.Parameters.AddWithValue("@ConfirmationToken", randomToken);
                            command.Parameters.AddWithValue("@ExpirationDateUTC", currentDate.Add(expirationTime));

                            //Execute the command
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    //Show that it was unsuccessful
                    altEmailFail.Visible = true;
                }
            }
        }
    }
}