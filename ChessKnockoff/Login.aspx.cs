using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace ChessKnockoff
{
    /// <summary>
    /// Class for the login
    /// </summary>
    public partial class LoginForm : ExtendedPage
    {
        /// <summary>
        /// Checks whether the user has their email confirmed or not
        /// </summary>
        /// <param name="username">The name of the player</param>
        /// <returns>Returns true if their email is confirmed or false, returns null if user does not exist</returns>
        public bool? isEmailConfirmedFromUsername(string username)
        {
            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Stores the query string
                string queryString = "SELECT * FROM Player WHERE Username=@Username";

                //Create the query string in the sqlCommand format
                SqlCommand command = new SqlCommand(queryString, connection);

                //Add the parameters to the query
                command.Parameters.AddWithValue("@Username", username);

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
        /// Validates the user credentials
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="passwordPlaintext">The password is plaintext</param>
        /// <returns>Returns true if the crendtials are correct else false</returns>
        private bool checkUserCredentials(string username, string passwordPlaintext)
        {
            //Stores the query string
            string queryString = "SELECT * FROM Player WHERE Username=@Username AND EmailIsConfirmed = 1";

            //Create the reader to store results
            SqlDataReader reader;

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand command = new SqlCommand(queryString, connection);

                //Add the parameters into the query
                command.Parameters.AddWithValue("@Username", username);

                //Execute the SQL command and get the results
                reader = command.ExecuteReader();

                //Check if a user was found
                if (reader.HasRows)
                {
                    //Read the first row
                    reader.Read();

                    //Retrieve the salt password from the database and cast it to a byte array
                    byte[] salt = (byte[])reader["Salt"];

                    //Retrieve the salted hash from the database
                    byte[] saltedPasswordHash = (byte[])reader["Password"];

                    //Calculate user entered hash with the salt
                    byte[] saltedUserenteredHash = generateSaltedHash(passwordPlaintext, salt);

                    //Check if they match
                    if (saltedUserenteredHash.SequenceEqual(saltedPasswordHash))
                    {
                        //Return true that the credentials are valid
                        return true;
                    }
                }

                //The crendentials are not valid so return false
                return false;
            }
        }

        /// <summary>
        /// Called when the page loads. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //If user is already logged in
            if (isAuthenticated())
            {
                //Check if they had the return query
                if (this.Request.QueryString["ReturnUrl"] != null)
                {
                    //IF they did redirect them to that
                    this.Response.Redirect(Request.QueryString["ReturnUrl"].ToString());
                }
                else
                {
                    //Redirect them to the play page
                    this.Response.Redirect("/Play");
                }
            }

            //Make the current link in the navbar active
            activateNav( "likLogin");

            //Hide the messages when first entering the page and for other request as the viewstate is not saved
            altAuthentication.Visible = false;
            altRegistered.Visible = false;
            altMustBeLoggedIn.Visible = false;
            altVerify.Visible = false;
            altEmailConfirm.Visible = false;
            altResetPassword.Visible = false;
            altLockout.Visible = false;
            
            //If the user was redirected from another page that required to be logged in
            if (Request.QueryString["ReturnUrl"] != null)
            {
                //Display the message
                altMustBeLoggedIn.Visible = true;
            }

            //If the user has a confirmation code
            if (Request.QueryString["ConfirmationToken"] != null)
            {
                //Check the password confirmation token

                //Stores the query string
                string queryString = "UPDATE Player SET Player.EmailIsConfirmed = 1 FROM Player INNER JOIN Confirmation ON Player.Username = Confirmation.Username WHERE Confirmation.ConfirmationToken = @Token";

                //Create the database connection then dispose when done
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    //Open the database connection
                    connection.Open();

                    //Create the query string in the sqlCommand format
                    SqlCommand command = new SqlCommand(queryString, connection);

                    //Try to retrieve the token and convert to a byte array
                    try
                    {
                        //Add the parameters
                        command.Parameters.AddWithValue("@Token", Convert.FromBase64String(HttpUtility.UrlDecode(Request.QueryString["ConfirmationToken"])));

                        //Store the result in a reader
                        int rowsAffected = command.ExecuteNonQuery();

                        //Rows were affected therefore show the success message
                        if (rowsAffected > 0)
                        {
                            //Show that the email has been confirmed
                            altEmailConfirm.Visible = true;

                            //Also delete the row since it will never be needed again
                            queryString = "DELETE FROM Confirmation WHERE ConfirmationToken = @Token";

                            //Make a new query
                            command = new SqlCommand(queryString, connection);

                            //Add the parameters
                            command.Parameters.AddWithValue("@Token", decodeToBytes(Request.QueryString["ConfirmationToken"]));

                            //Execute the command
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {
                        //Do nothing with the exception
                    }
                }
            }

            //If the user just registered show them the success message and prompt them to login
            if (Request.QueryString["Registered"] == "1")
            {
                //Displat the message
                altRegistered.Visible = true;
            }

            //If the password was reset
            if (Request.QueryString["ResetPassword"] == "1")
            {
                //Displat the message
                altResetPassword.Visible = true;
            }
        }

        /// <summary>
        /// Called when the login button is clicked. Should not be called directly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LoginClick(object sender, EventArgs e)
        {
            if (IsValid)
            {
                //Store whether the email is confirmed
                bool? emailConfirmed = isEmailConfirmedFromUsername(inpUsername.Value);
                if (emailConfirmed == false)
                {
                    //Show the message to verify their email
                    altVerify.Visible = true;
                }

                //Check whether the credentials are correct
                bool credentialCorrect = checkUserCredentials(inpUsername.Value, inpPassword.Value);
                if (!credentialCorrect)
                {
                    //Show the alert
                    altAuthentication.Visible = true;
                }

                //Check whether their credentials are correct and their email has been confirmed
                if (credentialCorrect && emailConfirmed == true)
                {
                    //Store their information in a session variable
                    Session["Username"] = inpUsername.Value;

                    //Redirect them to the play page
                    Response.Redirect("/Play");
                }
            }
        }
    }
}